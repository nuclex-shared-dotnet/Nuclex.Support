﻿Request Framework
=================

These are just some ponderings on the design. Most of what I was trying to accomplish
via my request framework has long since been covered by System.Threading.Tasks and
most of what I designed is not in the separate and forgotten Nuclex.Support.Transactions
library.


Ponderings
----------

The request framework should not require that .NET multithreading is used for
asynchronous requests.

Otherwise, it would prvent overlapped operations, 3rd party APIs (eg. used
via P/Invoke) from being able to use the request framework and possibly even
spawn duplicate implementations.


Design using interfaces
-----------------------

    interface IWaitable {

      /// <summary>Fired when the background process has finished</summary>
      /// <remarks>
      ///   If the process is already finished when a client registers to this event,
      ///   the registered callback will be invoked synchronously right when the
      ///   registration takes place.
      /// </remarks>
      event EventHandler Finished;

      /// <summary>Waits until the background process finishes</summary>
      void Wait();

      /// <summary>Waits until the background process finishes or a timeout occurs</summary>
      /// <param name="timeoutMilliseconds">
      ///   Number of milliseconds after which to stop waiting and return immediately
      /// </param>
      /// <returns>
      ///   True if the background process completed, false if the timeout was reached
      /// </returns>
      bool Wait(int timeoutMilliseconds);

      /// <summary>Whether the background process has finished</summary>
      bool Finished { get; }

  // ?
      /// <summary>Wait handle that can be used to wait for multiple waitables</summary>
      /// <remarks>
      ///   Only use the WaitHandle to wait if you're running in a different thread than
      ///   the request, or you may deadlock. Libraries based on single-threaded
      ///   multitasking may employ concepts such as window message processing to achieve
      ///   parallelism which could be stalled by improper waiting using the WaitHandle
      ///   whereas the Wait() method typically has a suitable implementation compatible
      ///   with the request's requirements.
      /// </remarks>
      WaitHandle WaitHandle { get; }

    }

    interface IThreadedWaitable : IWaitable {

      WaitHandle WaitHandle { get; }

    }

    interface IRequest : IWaitable {

      /// <summary>
      ///   Waits for the background process to complete and re-throws the exception to
      ///   the caller when an error has occured
      /// </summary>
      void Join();

    }

    interface IRequest<ResultType> : IRequest {

      /// <summary>
      ///   Waits for the background process to complete and re-throws the exception to
      ///   the caller when an error has occured
      /// </summary>
      /// <returns>The result of the background processing</returns>
      new ResultType Join();

    }

    interface IThreadedRequest : IRequest, IThreadedWaitable { }

    interface IThreadedRequest<ResultType> : IRequest<ResultType>, IThreadedRequest { }


Impossible implementation
-------------------------

    class Request : IRequest {

      event EventHandler Finished;
      void Wait();
      bool Wait(int timeoutMilliseconds);
      bool Finished { get; }
      void Join();
      protected virtual void ReraiseExceptions() { }

    }

    class Request<ResultType> : Request, IRequest<ResultType> {

      new ResultType Join();
      protected abstract ResultType GatherResults();

    }

Do not provide: (see conflict in second version)

    class ThreadedRequest : Request, IThreadedRequest {
    
      WaitHandle WaitHandle { get; }

    }

    class ThreadedRequest<ResultType> : ThreadedRequest, Request<ResultType> { }

    // However, not providing these, the user would have to rewrite
    // the complex threading routines everywhere he uses then. Bad.


Inelegant implementation
------------------------

    class Void {}

    class Request<ResultType> : IRequest<ResultType> {

      new ResultType Join();
      protected abstract ResultType GatherResults();

    }

    class ThreadedRequest<ResultType> : Request<ResultType> { }

    // However, not providing these, the user would have to rewrite
    // the complex threading routines everywhere he uses then. Bad.

Maybe keeping threaded and non-threaded requests apart is a good thing?

    IWaitable (without Finished event)
      Waitable (Finished event)
        Request
          Request<Bla>

    IWaitable (without Finished event)
      ThreadedWaitable (AsyncFinished event)
        ThreadedRequest
          ThreadedRequest<Bla>

Or just dump the WaitHandle schmonder

    Waitable (with virtual protected SyncRoot { get { return this; } })
      Request
        Request<Bla>

    LazyWaitHandle
      WaitHandle Get(Waitable waitable)

Or use policy classes (waithandle trouble)

    Waitable
      Request
        Request<Bla>

    RequestImpl<ThreadPolicy>
      RequestImpl<Bla, ThreadPolicy>

    LazyWaitHandle
      WaitHandle Get(Waitable waitable)
      WaitHandle Get(Waitable waitable, object syncRoot)

    ThreadPolicy {
      virtual void lock() {}
      virtual void unlock() {}
    }   
