using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Nuclex.Support.Collections;

namespace Nuclex.Support.Tracking {

  /// <summary>Collection of progressions with a weighting value</summary>
  /// <typeparam name="ProgressionType">Type of progressions to manage</typeparam>
  /// <remarks>
  ///   <para>
  ///     This collection is exposed as a read-only collection to the user that
  ///     stores WeightedProgressions. Internally, it merely wraps a collection of
  ///     an internal type used to keep track of the individual progression's
  ///     progress in the SetProgression and QueueOperation classes.
  ///   </para>
  ///   <para>
  ///     It is read-only because the design requires a progression to only ever
  ///     finish once. If it was possible eg. to add items after a SetProgression
  ///     had signalled itself as being finished, it would be moved into an
  ///     unfinished state again. Also, an empty SetProgression is, by definition,
  ///     finished (simply because there is no work to do) - unless the contents
  ///     of set are passed to the SetProgression's constructor and never modified
  ///     at all, the design would be violated as soon as ab instance of the
  ///     SetProgression or QueueOperation classes was created.
  ///   </para>
  /// </remarks>
  internal class WeightedProgressionCollection<ProgressionType> :
    TransformingReadOnlyCollection<
      ObservedProgression<ProgressionType>, WeightedProgression<ProgressionType>
    >
    where ProgressionType : Progression {

    /// <summary>Initializes a new weighted progression collection wrapper</summary>
    /// <param name="items">Items to be exposed as weighted progressions</param>
    internal WeightedProgressionCollection(
      IList<ObservedProgression<ProgressionType>> items
    )
      : base(items) { }

    /// <summary>Transforms an item into the exposed type</summary>
    /// <param name="item">Item to be transformed</param>
    /// <returns>The transformed item</returns>
    /// <remarks>
    ///   This method is used to transform an item in the wrapped collection into
    ///   the exposed item type whenever the user accesses an item. Expect it to
    ///   be called frequently, because the TransformingReadOnlyCollection does
    ///   not cache otherwise store the transformed items.
    /// </remarks>
    protected override WeightedProgression<ProgressionType> Transform(
      ObservedProgression<ProgressionType> item
    ) {
      return item.WeightedProgression;
    }

  }

} // namespace Nuclex.Support.Tracking
