#region CPL License
/*
Nuclex Native Framework
Copyright (C) 2002-2013 Nuclex Development Labs

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
#endregion // CPL License

using System;
using System.Collections.Generic;

namespace Nuclex.Support.Collections {

  // ------------------------------------------------------------------------------------------- //

  /// <summary>Randomly selects between different options, trying to avoid repetition</summary>
  /// <typeparam name="TKey">Type of keys through which values can be looked up</typeparam>
  /// <typeparam name="TValue">Type of values provided by the variegator</typeparam>
  /// <remarks>
  ///   <para>
  ///     This class is useful wherever randomness is involved in a game: picking random
  ///     actions for an NPC to execute, selecting different songs to play, displaying
  ///     different dialogue and more.
  ///   </para>
  ///   <para>
  ///     In principle, it works like a multimap, associating keys with a number of values
  ///     and allowing you to look up a values by their keys. Unlike a multimap, it will
  ///     avoid handing out a previously provided value again.
  ///   </para>
  ///   <para>
  ///     A typical usage would be to set up a mapping between situations and dialogue lines.
  ///     Upon calling <see cref="Get(TKey)" /> with the situation 'detected-player-stealing',
  ///     the variegator would return a random (but not recently used) value which in this case
  ///     might contain a commentary an NPC might make upon encountering that situation.
  ///     Other NPCs requesting dialogue lines for the same situation would receive different
  ///     random commentary for as long as long as available data allows.
  ///   </para>
  /// </remarks>
  public class Variegator<TKey, TValue> {

    /// <summary>Initializes a new variegator</summary>
    /// <param name="historyLength">
    ///   How far into the past the variegator will look to avoid repetition
    /// </param>
    public Variegator(int historyLength = 64) {
      this.historyLength = historyLength;
      this.history = new TValue[historyLength];
      this.values = new MultiDictionary<TKey, TValue>();

      this.randomNumberGenerator = new Random();
    }

    /// <summary>Removes all entries from the variegator</summary>
    /// <remarks>
    ///   This is mainly useful if you are storing smart pointers to values of substantial
    ///   size (eg. audio clips instead of just resource proxies or paths) and need to
    ///   reclaim memory.
    /// </remarks>
    public void Clear() {
      freeHistory();
      this.historyFull = false;
      this.historyTailIndex = 0;
    }

    /// <summary>Checks whether the variegator is empty</summary>
    /// <returns>True if there are no entries in the variegator</returns>
    public bool IsEmpty {
      get { return (Count == 0); }
    }

    /// <summary>Returns the number of values in the variegator</summary>
    /// <returns>The number of values stored in the variegator</returns>
    /// <remarks>
    ///   If the same value is added with different keys (a situation that doesn't make
    ///   sense because such reuse should be covered by specifying multiple keys in
    ///   a query), it will be counted multiple times.
    /// </remarks>
    public int Count {
      get { return ((System.Collections.ICollection)this.values).Count; }
    }

    /// <summary>
    ///   Insert a new value that can be returned when requesting the specified key
    /// </summary>
    /// <param name="key">Key of the value that will be inserted</param>
    /// <param name="value">Value that will be inserted under the provided key</param>
    public void Add(TKey key, TValue value) {
      this.values.Add(key, value);
    }

    /// <summary>Retrieves a random value associated with the specified key</summary>
    /// <param name="key">For for which a value will be looked up</param>
    /// <returns>A random value associated with the specified key</returns>
    public TValue Get(TKey key) {
      ISet<TValue> candidates = new HashSet<TValue>();
      {
        ICollection<TValue> valueRange = this.values[key];

        // If possible access the values by index because it's faster and produces less
        // garbage, otherwise fall back to using an enumerator
        var indexableValueRange = valueRange as IList<TValue>;
        if(indexableValueRange == null) {
          foreach(TValue value in valueRange) {
            candidates.Add(value);
          }
        } else {
          for(int valueIndex = 0; valueIndex < indexableValueRange.Count; ++valueIndex) {
            candidates.Add(indexableValueRange[valueIndex]);
          }
        }
      }

      TValue result = destructivePickCandidateValue(candidates);
      addRecentlyUsedValue(result);
      return result;
    }

    /// <summary>Retrieves a random value associated with one of the specified keys</summary>
    /// <param name="keys">Keys that will be considered</param>
    /// <remarks>
    ///   In many cases, you have generic situations (such as 'detected-player-stealing',
    ///   'observed-hostile-action') and specified situations (such as
    ///   'detected-player-stealing-from-beggar', 'observed-hostile-action-on-cop')
    ///   where a values from both pools should be considered. This method allows you
    ///   to specify any number of keys, creating a greater set of values the variegator
    ///   can pick between.
    /// </remarks>
    public TValue Get(params TKey[] keys) {
      ISet<TValue> candidates = new HashSet<TValue>();

      for(int index = 0; index < keys.Length; ++index) {
        ICollection<TValue> valueRange = this.values[keys[index]];

        // If possible access the values by index because it's faster and produces less
        // garbage, otherwise fall back to using an enumerator
        var indexableValueRange = valueRange as IList<TValue>;
        if(indexableValueRange == null) {
          foreach(TValue value in valueRange) {
            candidates.Add(value);
          }
        } else {
          for(int valueIndex = 0; valueIndex < indexableValueRange.Count; ++valueIndex) {
            candidates.Add(indexableValueRange[valueIndex]);
          }
        }
      }

      TValue result = destructivePickCandidateValue(candidates);
      addRecentlyUsedValue(result);
      return result;
    }

    /// <summary>Picks amongst the values in a set</summary>
    /// <param name="candidates">
    ///   Set containing the candidats values to consider. Will be destroyed.
    /// </param>
    /// <returns>The least recently used candidate value or a random one</returns>
    private TValue destructivePickCandidateValue(ISet<TValue> candidates) {
      removeRecentlyUsedValues(candidates);

      switch(candidates.Count) {
        case 0: {
          throw new InvalidOperationException("No values mapped to this key");
        }
        case 1: {
          using(IEnumerator<TValue> enumerator = candidates.GetEnumerator()) {
            enumerator.MoveNext(); // We can be sure this one returns true
            return enumerator.Current;
          }
        }
        default: {
          int index = this.randomNumberGenerator.Next(candidates.Count);
          using(IEnumerator<TValue> enumerator = candidates.GetEnumerator()) {
            do {
              --index;
              enumerator.MoveNext(); // We can be sure this one returns true
            } while(index >= 0);

            return enumerator.Current;
          }

          throw new InvalidOperationException(
            "ISet.Count was off or random number generator malfunctioned"
          );
        }
      }
    }

    /// <summary>Adds a recently used value to the history</summary>
    /// <param name="value">Value that will be added to the history</param>
    private void addRecentlyUsedValue(TValue value) {
      if(this.historyTailIndex == this.historyLength) {
        this.historyFull = true;
        this.history[0] = value;
        this.historyTailIndex = 1;
      } else {
        this.history[this.historyTailIndex] = value;
        ++this.historyTailIndex;
      }
    }

    /// <summary>Removes all values that are in the recent use list from a set</summary>
    /// <param name="candidates">Set from which recently used values are removed</param>
    /// <remarks>
    ///   Stops removing values when there's only 1 value left in the set
    /// </remarks>
    private void removeRecentlyUsedValues(ISet<TValue> candidates) {
      if(candidates.Count <= 1) {
        return;
      }

      if(this.historyFull) { // History buffer has wrapped around
        int index = this.historyTailIndex;
        while(index > 0) {
          --index;
          if(candidates.Remove(this.history[index])) {
            if(candidates.Count <= 1) {
              return;
            }
          }
        }
        index = this.historyLength;
        while(index > this.historyTailIndex) {
          --index;
          if(candidates.Remove(this.history[index])) {
            if(candidates.Count <= 1) {
              return;
            }
          }
        }
      } else { // History buffer was not full yet
        int index = this.historyTailIndex;
        while(index > 0) {
          --index;
          if(candidates.Remove(this.history[index])) {
            if(candidates.Count <= 1) {
              return;
            }
          }
        }
      }
    }

    /// <summary>Frees all memory used by the individual history entries</summary>
    /// <remarks>
    ///   The history array itself is kept alive and the tail index + full flag will
    ///   not be reset.
    /// </remarks>
    private void freeHistory() {
      Array.Clear(this.history, 0, this.historyLength);
    }

    /// <summary>Stores the entries the variegator can select from by their keys</summary>
    private IMultiDictionary<TKey, TValue> values;

    /// <summary>Random number generator that will be used to pick random values</summary>
    private Random randomNumberGenerator;
    /// <summary>Number of entries in the recently used list</summary>
    private int historyLength;

    /// <summary>Array containing the most recently provided values</summary>
    private TValue[] history;
    /// <summary>Index of the tail in the recently used value array</summary>
    private int historyTailIndex;
    /// <summary>Whether the recently used value history is at capacity</summary>
    private bool historyFull;

  }

} // namespace Nuclex { namespace Support { namespace Collections
