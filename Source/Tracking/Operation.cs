using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclex.Support.Tracking {

  /// <summary>Base class for observable operations running in the background</summary>
  public abstract class Operation : Progression {

    /// <summary>Possible outcomes of an operation</summary>
    public enum Outcomes {
      /// <summary>The operation has not ended yet</summary>
      Pending,
      /// <summary>The operation has succeeded</summary>
      Success,
      /// <summary>The operation has failed</summary>
      Failure,
    }
/*
    /// <summary>Begins executing the operation</summary>
    public abstract void BeginExecute();

    public virtual void Execute() {
      try {
        BeginExecute();
        this.outcome = EndExecute();
      }
      catch(Exception exception) {
         this.outcome = Outcomes.Failure;
      }
    }

    /// <summary>Ends the </summary>
    public virtual Outcomes EndExecute() {
      WaitHandle.WaitOne();

      return this.outcome;
    }

    /// <summary>The Outcome of the operation when it has finished</summary>
    public Outcomes Outcome {
      get { return this.outcome; }
    }

    /// <summary>Outcome of the operation</summary>
    protected Outcomes outcome;
*/
  }

} // namespace Nuclex.Support.Tracking
