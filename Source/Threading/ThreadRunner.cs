#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2019 Nuclex Development Labs

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

#if !NO_CONCURRENT_COLLECTIONS

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Nuclex.Support.Threading {

	/// <summary>Executes actions in a threads</summary>
	internal abstract class ThreadRunner : IDisposable {

		#region interface IRunner

		/// <summary>Interface for a background task runner</summary>
		private interface IRunner {

			/// <summary>Runs the background task</summary>
			void Run();

			/// <summary>The runner's cancellation token source</summary>
			CancellationTokenSource CancellationTokenSource { get; }

		}

		#endregion // interface IRunner

		#region struct Runner

		/// <summary>Runs a background task with no parameters</summary>
		private struct Runner : IRunner {

			/// <summary>Initializes a new runner</summary>
			/// <param name="action">Action the runner will execute</param>
			public Runner(Action action) {
				this.action = action;
			}

			/// <summary>Executes the runner's action</summary>
			public void Run() {
				this.action();
			}

			/// <summary>The runner's cancellation token source</summary>
			public CancellationTokenSource CancellationTokenSource {
				get { return null; }
			}

			/// <summary>Action the runner will execute</summary>
			private Action action;

		}

		#endregion // struct Runner

		#region struct CancellableRunner

		/// <summary>Runs a background task with no parameters</summary>
		private struct CancellableRunner : IRunner {

			/// <summary>Initializes a new runner</summary>
			/// <param name="action">Action the runner will execute</param>
			public CancellableRunner(CancellableAction action) {
				this.action = action;
				this.cancellationTokenSource = new CancellationTokenSource();
			}

			/// <summary>Executes the runner's action</summary>
			public void Run() {
				this.action(this.cancellationTokenSource.Token);
			}

			/// <summary>The runner's cancellation token source</summary>
			public CancellationTokenSource CancellationTokenSource {
				get { return this.cancellationTokenSource; }
			}

			/// <summary>The runner's cancellation token source</summary>
			private CancellationTokenSource cancellationTokenSource;
			/// <summary>Action the runner will execute</summary>
			private CancellableAction action;

		}

		#endregion // struct CancellableRunner

		#region struct Runner<P1>

		/// <summary>Runs a background task with one parameter</summary>
		private struct Runner<P1> : IRunner {

			/// <summary>Initializes a new runner</summary>
			/// <param name="action">Action the runner will execute</param>
			/// <param name="parameter1">Parameter that will be passed to the action</param>
			public Runner(Action<P1> action, P1 parameter1) {
				this.action = action;
				this.parameter1 = parameter1;
			}

			/// <summary>Executes the runner's action</summary>
			public void Run() {
				this.action(this.parameter1);
			}

			/// <summary>The runner's cancellation token source</summary>
			public CancellationTokenSource CancellationTokenSource {
				get { return null; }
			}

			/// <summary>Action the runner will execute</summary>
			private Action<P1> action;
			/// <summary>Parameter that will be passed to the action</summary>
			private P1 parameter1;

		}

		#endregion // struct Runner<P1>

		#region struct CancellableRunner<P1>

		/// <summary>Runs a background task with one parameter</summary>
		private struct CancellableRunner<P1> : IRunner {

			/// <summary>Initializes a new runner</summary>
			/// <param name="action">Action the runner will execute</param>
			/// <param name="parameter1">Parameter that will be passed to the action</param>
			public CancellableRunner(CancellableAction<P1> action, P1 parameter1) {
				this.action = action;
				this.parameter1 = parameter1;
				this.cancellationTokenSource = new CancellationTokenSource();
			}

			/// <summary>Executes the runner's action</summary>
			public void Run() {
				this.action(this.parameter1, this.cancellationTokenSource.Token);
			}

			/// <summary>The runner's cancellation token source</summary>
			public CancellationTokenSource CancellationTokenSource {
				get { return this.cancellationTokenSource; }
			}

			/// <summary>The runner's cancellation token source</summary>
			private CancellationTokenSource cancellationTokenSource;
			/// <summary>Action the runner will execute</summary>
			private CancellableAction<P1> action;
			/// <summary>Parameter that will be passed to the action</summary>
			private P1 parameter1;

		}

		#endregion // struct CancellableRunner<P1>

		#region struct Runner<P1, P2>

		/// <summary>Runs a background task with one parameter</summary>
		private struct Runner<P1, P2> : IRunner {

			/// <summary>Initializes a new runner</summary>
			/// <param name="action">Action the runner will execute</param>
			/// <param name="parameter1">First parameter that will be passed to the action</param>
			/// <param name="parameter2">Second parameter that will be passed to the action</param>
			public Runner(Action<P1, P2> action, P1 parameter1, P2 parameter2) {
				this.action = action;
				this.parameter1 = parameter1;
				this.parameter2 = parameter2;
			}

			/// <summary>Executes the runner's action</summary>
			public void Run() {
				this.action(this.parameter1, this.parameter2);
			}

			/// <summary>The runner's cancellation token source</summary>
			public CancellationTokenSource CancellationTokenSource {
				get { return null; }
			}

			/// <summary>Action the runner will execute</summary>
			private Action<P1, P2> action;
			/// <summary>First parameter that will be passed to the action</summary>
			private P1 parameter1;
			/// <summary>Second parameter that will be passed to the action</summary>
			private P2 parameter2;

		}

		#endregion // struct Runner<P1, P2>

		#region struct CancellableRunner<P1, P2>

		/// <summary>Runs a background task with one parameter</summary>
		private struct CancellableRunner<P1, P2> : IRunner {

			/// <summary>Initializes a new runner</summary>
			/// <param name="action">Action the runner will execute</param>
			/// <param name="parameter1">First parameter that will be passed to the action</param>
			/// <param name="parameter2">Second parameter that will be passed to the action</param>
			public CancellableRunner(CancellableAction<P1, P2> action, P1 parameter1, P2 parameter2) {
				this.action = action;
				this.parameter1 = parameter1;
				this.parameter2 = parameter2;
				this.cancellationTokenSource = new CancellationTokenSource();
			}

			/// <summary>Executes the runner's action</summary>
			public void Run() {
				this.action(this.parameter1, this.parameter2, this.cancellationTokenSource.Token);
			}

			/// <summary>The runner's cancellation token source</summary>
			public CancellationTokenSource CancellationTokenSource {
				get { return this.cancellationTokenSource; }
			}

			/// <summary>The runner's cancellation token source</summary>
			private CancellationTokenSource cancellationTokenSource;
			/// <summary>Action the runner will execute</summary>
			private CancellableAction<P1, P2> action;
			/// <summary>First parameter that will be passed to the action</summary>
			private P1 parameter1;
			/// <summary>Second parameter that will be passed to the action</summary>
			private P2 parameter2;

		}

		#endregion // struct CancellableRunner<P1, P2>

		/// <summary>Initializes a new background processing handler</summary>
		public ThreadRunner() {
			this.executeQueuedRunnersInThreadDelegate = new Action(executeQueuedRunnersInThread);
			this.queuedRunners = new ConcurrentQueue<IRunner>();
		}

		/// <summary>
		///   Immediately cancels all operations and releases the resources used by the instance
		/// </summary>
		public void Dispose() {
			Dispose(timeoutMilliseconds: 2500);
		}

		/// <summary>
		///   Immediately cancels all operations and releases the resources used by the instance
		/// </summary>
		/// <param name="timeoutMilliseconds">
		///   Time to wait for the background tasks before dropping the tasks unfinished
		/// </param>
		public void Dispose(int timeoutMilliseconds) {
			CancelAllBackgroundOperations();

			Task currentTask;
			lock(this) {
				currentTask = this.currentTask;
			}

			if(currentTask != null) {
				if(!currentTask.Wait(timeoutMilliseconds)) {
					Debug.Assert(false, "Task does not support cancellation or did not cancel in time");
				}
				lock(this) {
					this.currentTask = null;
					IsBusy = false;
				}
			}
		}

		/// <summary>Whether the view model is currently busy executing a task</summary>
		public bool IsBusy {
			get { return this.isBusy; }
			private set {
				if(value != this.isBusy) {
					this.isBusy = value;
					BusyChanged();
				}
			}
		}

		/// <summary>Reports an error</summary>
		/// <param name="exception">Error that will be reported</param>
		protected abstract void ReportError(Exception exception);

		/// <summary>Called when the status of the busy flag changes</summary>
		protected abstract void BusyChanged();

		/// <summary>Executes the specified operation in the background</summary>
		/// <param name="action">Action that will be executed in the background</param>
		public void RunInBackground(Action action) {
			this.queuedRunners.Enqueue(new Runner(action));
			startBackgroundProcessingIfNecessary();
		}

		/// <summary>Executes the specified operation in the background</summary>
		/// <param name="action">Action that will be executed in the background</param>
		public void RunInBackground(CancellableAction action) {
			this.queuedRunners.Enqueue(new CancellableRunner(action));
			startBackgroundProcessingIfNecessary();
		}

		/// <summary>Executes the specified operation in the background</summary>
		/// <param name="action">Action that will be executed in the background</param>
		/// <param name="parameter1">Parameter that will be passed to the action</param>
		public void RunInBackground<P1>(Action<P1> action, P1 parameter1) {
			this.queuedRunners.Enqueue(new Runner<P1>(action, parameter1));
			startBackgroundProcessingIfNecessary();
		}

		/// <summary>Executes the specified operation in the background</summary>
		/// <param name="action">Action that will be executed in the background</param>
		/// <param name="parameter1">Parameter that will be passed to the action</param>
		public void RunInBackground<P1>(CancellableAction<P1> action, P1 parameter1) {
			this.queuedRunners.Enqueue(new CancellableRunner<P1>(action, parameter1));
			startBackgroundProcessingIfNecessary();
		}

		/// <summary>Executes the specified operation in the background</summary>
		/// <param name="action">Action that will be executed in the background</param>
		/// <param name="parameter1">First parameter that will be passed to the action</param>
		/// <param name="parameter2">Second parameter that will be passed to the action</param>
		public void RunInBackground<P1, P2>(Action<P1, P2> action, P1 parameter1, P2 parameter2) {
			this.queuedRunners.Enqueue(new Runner<P1, P2>(action, parameter1, parameter2));
			startBackgroundProcessingIfNecessary();
		}

		/// <summary>Executes the specified operation in the background</summary>
		/// <param name="action">Action that will be executed in the background</param>
		/// <param name="parameter1">First parameter that will be passed to the action</param>
		/// <param name="parameter2">Second parameter that will be passed to the action</param>
		public void RunInBackground<P1, P2>(
			CancellableAction<P1, P2> action, P1 parameter1, P2 parameter2
		) {
			this.queuedRunners.Enqueue(new CancellableRunner<P1, P2>(action, parameter1, parameter2));
			startBackgroundProcessingIfNecessary();
		}

		/// <summary>Cancels the currently running background operation</summary>
		public void CancelBackgroundOperation() {
			IRunner currentRunner = this.currentRunner;
			if(currentRunner != null) {
				CancellationTokenSource cancellationTokenSource = currentRunner.CancellationTokenSource;
				if(cancellationTokenSource != null) {
					cancellationTokenSource.Cancel();
				}
			}
		}

		/// <summary>Cancels all queued and the currently running background operation</summary>
		public void CancelAllBackgroundOperations() {
			IRunner runner;
			while(this.queuedRunners.TryDequeue(out runner)) {
				CancellationTokenSource cancellationTokenSource = runner.CancellationTokenSource;
				if(cancellationTokenSource != null) {
					cancellationTokenSource.Cancel();
				}
			}

			CancelBackgroundOperation();
		}

		/// <summary>Whether the background operation has been cancelled</summary>
		//[Obsolete("Please use a method accepting a cancellation token instead of using this")]
		public bool IsBackgroundOperationCancelled {
			get {
				IRunner currentRunner = this.currentRunner;
				if(currentRunner != null) {
					return currentRunner.CancellationTokenSource.IsCancellationRequested;
				} else {
					return false;
				}
			}
		}

		/// <summary>Throws an exception if the background operation was cancelled</summary>
		//[Obsolete("Please use a method accepting a cancellation token instead of using this")]
		public void ThrowIfBackgroundOperationCancelled() {
			IRunner currentRunner = this.currentRunner;
			if(currentRunner != null) {
				currentRunner.CancellationTokenSource.Token.ThrowIfCancellationRequested();
			}
		}

		/// <summary>Executes the queued runners in the background</summary>
		private void executeQueuedRunnersInThread() {
			IsBusy = true;

			IRunner runner;
			while(this.queuedRunners.TryDequeue(out runner)) {
				try {
					this.currentRunner = runner;
					runner.Run();
				}
				catch(OperationCanceledException) {
					// Ignore
				}
				catch(Exception exception) {
					this.currentRunner = null; // When the error is reported this should already be null
					ReportError(exception);
				}
				this.currentRunner = null;
			}

			lock(this) {
				this.currentTask = null;
				IsBusy = false;
			}
		}

		/// <summary>Starts the background processing thread, if needed</summary>
		private void startBackgroundProcessingIfNecessary() {
			Task currentTask;

			lock(this) {
				if(this.currentTask == null) {
					currentTask = new Task(this.executeQueuedRunnersInThreadDelegate);
					this.currentTask = currentTask;
				} else {
					return; // Task is already running
				}
			}

			// Start the task outside of the lock statement so that when the thread starts to run,
			// it is guaranteed to read the currentTask variable as the task we just created.
			currentTask.Start();
		}

		/// <summary>Whether the view model is currently busy executing a task</summary>
		private volatile bool isBusy;
		/// <summary>Delegate for the executedQueuedRunnersInThread() method</summary>
		private Action executeQueuedRunnersInThreadDelegate;
		/// <summary>Queued background operations</summary>
		private ConcurrentQueue<IRunner> queuedRunners;
		/// <summary>Runner currently executing in the background</summary>
		private volatile IRunner currentRunner;
		/// <summary>Task that is currently executing the runners</summary>
		private Task currentTask;

	}

} // namespace Nuclex.Support.Threading

#endif // !NO_CONCURRENT_COLLECTIONS
