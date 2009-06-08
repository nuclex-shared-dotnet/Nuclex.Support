#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using Nuclex.Support.Collections;

#if false

namespace Nuclex.Support.Scheduling {

  /// <summary>Schedules actions for execution at a future point in time</summary>
  public class Scheduler : IDisposable {

    /// <summary>One tick is 100 ns, meaning 10000 ticks equal 1 ms</summary>
    private const long TicksPerMillisecond = 10000;

#region class TimeSourceSingleton

    /// <summary>
    ///   Manages the singleton instance of the scheduler's default time source
    /// </summary>
    private class TimeSourceSingleton {

      /// <summary>
      ///   Explicit static constructor to guarantee the singleton is initialized only
      ///   when a static member of this class is accessed.
      /// </summary>
      static TimeSourceSingleton() { } // Do not remove!

      /// <summary>The singleton instance of the default time source</summary>
      internal static readonly ITimeSource Instance = Scheduler.CreateDefaultTimeSource();

    }

    #endregion // class TimeSourceSingleton

#region class Notification

    /// <summary>Scheduled notification</summary>
    private class Notification {

      /// <summary>Initializes a new notification</summary>
      /// <param name="intervalTicks">
      ///   Interval in which the notification will re-executed
      /// </param>
      /// <param name="nextDueTicks">
      ///   Time source ticks the notification is next due at
      /// </param>
      /// <param name="absoluteUtcTime">
      ///   Absolute time in UTC at which the notification is due
      /// </param>
      /// <param name="callback">
      ///   Callback to be invoked when the notification is due
      /// </param>
      public Notification(
        long intervalTicks,
        long nextDueTicks,
        DateTime absoluteUtcTime,
        Delegate callback
      ) {
        this.IntervalTicks = intervalTicks;
        this.NextDueTicks = nextDueTicks;
        this.AbsoluteUtcTime = absoluteUtcTime;
        this.Callback = callback;
        this.Cancelled = false;
      }

      /// <summary>
      ///   Ticks specifying the interval in which the notification will be re-executed
      /// </summary>
      public long IntervalTicks;

      /// <summary>Next due time for this notification</summary>
      public long NextDueTicks;
      /// <summary>Absolute time in UTC at which the notification is due</summary>
      /// <remarks>
      ///   Only stored for notifications scheduled in absolute time, meaning they
      ///   have to be adjusted if the system date/time changes
      /// </remarks>
      public DateTime AbsoluteUtcTime;
      /// <summary>Callback that will be invoked when the notification is due</summary>
      public Delegate Callback;
      /// <summary>Whether the notification has been cancelled</summary>
      public bool Cancelled;

    }

    #endregion // class Notification

    /// <summary>Initializes a new scheduler using the default time source</summary>
    public Scheduler() : this(DefaultTimeSource) { }

    /// <summary>Initializes a new scheduler using the specified time source</summary>
    /// <param name="timeSource">Source source the scheduler will use</param>
    public Scheduler(ITimeSource timeSource) {
      this.timeSource = timeSource;
      this.notifications = new PriorityQueue<Notification>();
      this.notificationWaitEvent = new AutoResetEvent(false);

      this.timerThread = new Thread(new ThreadStart(runTimerThread));
      this.timerThread.Name = "Nuclex.Support.Scheduling.Scheduler";
      this.timerThread.Priority = ThreadPriority.Highest;
      this.timerThread.IsBackground = true;
      this.timerThread.Start();
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      if(this.timerThread != null) {
        this.endRequested = true;
        this.notificationWaitEvent.Set();

        // Wait for the timer thread to exit. If it doesn't exit in 10 seconds (which is
        // a lot of time given that it doesn't do any real work), forcefully abort
        // the thread. This may risk some leaks, but it's the only thing we can do.
        Trace.Assert(
          this.timerThread.Join(2500), "Scheduler timer thread did not exit in time"
        );

        // Get rid of the notification wait event now that we've made sure that
        // the timer thread is down.
        this.notificationWaitEvent.Close();

        // Help the GC a bit
        this.notificationWaitEvent = null;
        this.notifications = null;
        this.timeSource = null;

        // Set to null so we don't attempt to end the thread again if Dispose() is
        // called multiple times.
        this.timerThread = null;
      }
    }

    /// <summary>Schedules a notification at the specified absolute time</summary>
    /// <param name="notificationTime">
    ///   Absolute time at which the notification will occur
    /// </param>
    /// <param name="callback">
    ///   Callback that will be invoked when the notification is due
    /// </param>
    /// <returns>A handle that can be used to cancel the notification</returns>
    /// <remarks>
    ///   The notification is scheduled for the indicated absolute time. If the system
    ///   enters/leaves daylight saving time or the date/time is changed (for example
    ///   when the system synchronizes with an NTP server), this will affect
    ///   the notification. So if you need to be notified after a fixed time, use
    ///   the NotifyIn() method instead.
    /// </remarks>
    public object NotifyAt(DateTime notificationTime, Delegate callback) {
      if(notificationTime.Kind == DateTimeKind.Unspecified) {
        throw new ArgumentException(
          "Notification time is neither UTC or local", "notificationTime"
        );
      }

      DateTime notificationTimeUtc = notificationTime.ToUniversalTime();
      long remainingTicks = notificationTimeUtc.Ticks - DateTime.UtcNow.Ticks;
      long nextDueTicks = this.timeSource.Ticks + remainingTicks;

      return scheduleNotification(
        new Notification(
          0,
          nextDueTicks,
          notificationTimeUtc,
          callback
        )
      );
    }

    /// <summary>Schedules a notification after the specified time span</summary>
    /// <param name="delay">Delay after which the notification will occur</param>
    /// <param name="callback">
    ///   Callback that will be invoked when the notification is due
    /// </param>
    /// <returns>A handle that can be used to cancel the notification</returns>
    public object NotifyIn(TimeSpan delay, Delegate callback) {
      return scheduleNotification(
        new Notification(
          0,
          delay.Ticks,
          DateTime.MinValue,
          callback
        )
      );
    }

    /// <summary>
    ///   Schedules a notification after the specified amount of milliseconds
    /// </summary>
    /// <param name="delayMilliseconds">
    ///   Number of milliseconds after which the notification will occur
    /// </param>
    /// <param name="callback">
    ///   Callback that will be invoked when the notification is due
    /// </param>
    /// <returns>A handle that can be used to cancel the notification</returns>
    public object NotifyIn(int delayMilliseconds, Delegate callback) {
      return scheduleNotification(
        new Notification(
          0,
          (long)delayMilliseconds * TicksPerMillisecond,
          DateTime.MinValue,
          callback
        )
      );
    }

    /// <summary>
    ///   Schedules a recurring notification after the specified time span
    /// </summary>
    /// <param name="delay">Delay after which the first notification will occur</param>
    /// <param name="interval">Interval at which the notification will be repeated</param>
    /// <param name="callback">
    ///   Callback that will be invoked when the notification is due
    /// </param>
    /// <returns>A handle that can be used to cancel the notification</returns>
    public object NotifyEach(TimeSpan delay, TimeSpan interval, Delegate callback) {
      return scheduleNotification(
        new Notification(
          interval.Ticks,
          delay.Ticks,
          DateTime.MinValue,
          callback
        )
      );
    }

    /// <summary>
    ///   Schedules a recurring notification after the specified amount of milliseconds
    /// </summary>
    /// <param name="delayMilliseconds">
    ///   Milliseconds after which the first notification will occur
    /// </param>
    /// <param name="intervalMilliseconds">
    ///   Interval in milliseconds at which the notification will be repeated
    /// </param>
    /// <param name="callback">
    ///   Callback that will be invoked when the notification is due
    /// </param>
    /// <returns>A handle that can be used to cancel the notification</returns>
    public object NotifyEach(
      int delayMilliseconds, int intervalMilliseconds, Delegate callback
    ) {
      return scheduleNotification(
        new Notification(
          (long)intervalMilliseconds * TicksPerMillisecond,
          (long)delayMilliseconds * TicksPerMillisecond,
          DateTime.MinValue,
          callback
        )
      );
    }

    /// <summary>Returns the default time source for the scheduler</summary>
    public static ITimeSource DefaultTimeSource {
      get { return TimeSourceSingleton.Instance; }
    }

    /// <summary>Creates a new default time source for the scheduler</summary>
    /// <param name="useWindowsTimeSource">
    ///   Whether the specialized windows time source should be used
    /// </param>
    /// <returns>The newly created time source</returns>
    internal static ITimeSource CreateTimeSource(bool useWindowsTimeSource) {
      if(useWindowsTimeSource) {
        return new WindowsTimeSource();
      } else {
        return new GenericTimeSource();
      }
    }

    /// <summary>Creates a new default time source for the scheduler</summary>
    /// <returns>The newly created time source</returns>
    internal static ITimeSource CreateDefaultTimeSource() {
      return CreateTimeSource(WindowsTimeSource.Available);
    }

    /// <summary>Schedules a notification for processing by the timer thread</summary>
    /// <param name="notification">Notification that will be scheduled</param>
    /// <returns>The scheduled notification</returns>
    private object scheduleNotification(Notification notification) {
      lock(this.notifications) {
        this.notifications.Enqueue(notification);

        // If this notification has become that next due notification, wake up
        // the timer thread so it can adjust its sleep period.
        if(ReferenceEquals(this.notifications.Peek(), notification)) {
          this.notificationWaitEvent.Set();
        }
      }

      return notification;
    }

    /// <summary>Executes the timer thread</summary>
    private void runTimerThread() {

      for(; ; ) {

        // Get the notification that is due next and wait for it. When no notifications
        // are queued, wait indefinitely until we're signalled
        Notification nextDueNotification = getNextDueNotification();
        if(nextDueNotification == null) {
          this.notificationWaitEvent.WaitOne();
        } else {
          long remainingTicks = nextDueNotification.NextDueTicks - this.timeSource.Ticks;
          this.timeSource.WaitOne(this.notificationWaitEvent, remainingTicks);
        }

        // Have we been woken up because the Scheduler is being disposed?
        if(this.endRequested) {
          break;
        }


        //if(nextDueNotification.AbsoluteUtcTime !=         

      }

    }

    /// <summary>Retrieves the notification that is due next</summary>
    /// <returns>The notification that is due next</returns>
    private Notification getNextDueNotification() {
      lock(this.notifications) {
        if(this.notifications.Count == 0) {
          return null;
        } else {
          Notification nextDueNotification = this.notifications.Peek();
          while(nextDueNotification.Cancelled) {
            this.notifications.Dequeue();
            nextDueNotification = this.notifications.Peek();
          }
          return nextDueNotification;
        }
      }
    }

    /// <summary>Time source used by the scheduler</summary>
    private ITimeSource timeSource;
    /// <summary>Thread that will wait for the next scheduled event</summary>
    private Thread timerThread;
    /// <summary>Notifications in the scheduler's queue</summary>
    private PriorityQueue<Notification> notifications;

    /// <summary>Event used by the timer thread to wait for the next notification</summary>
    private AutoResetEvent notificationWaitEvent;
    /// <summary>Whether the timer thread should end</summary>
    private volatile bool endRequested;

  }

} // namespace Nuclex.Support.Scheduling

#endif
