#region Apache License 2.0
/*
Nuclex .NET Framework
Copyright (C) 2002-2024 Markus Ewald / Nuclex Development Labs

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion // Apache License 2.0

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Nuclex.Support.Collections {

  /// <summary>View into a section of an IList&lt;T&gt; without copying said string</summary>
  /// <typeparam name="TElement">
  ///   Type of elements that are stored in the list the segment references
  /// </typeparam>
  /// <remarks>
  ///   <para>
  ///     The design of this class pretty much mirrors that of the
  ///     <see cref="T:System.ArraySegment" /> class found in the .NET framework, but is
  ///     specialized to be used for IList&lt;T&gt;, which can not be cast to arrays
  ///     directly (and <see cref="M:System.ArrayList.Adapter" /> loses type safety).
  ///   </para>
  ///   <para>
  ///     In certain situations, passing a ListSegment instead of storing the selected
  ///     elements in a new list is useful. For example, the caller might want to know
  ///     from which index of the original list the section was taken. When the original
  ///     list needs to be modified, for example in a sorting algorithm, the list segment
  ///     can be used to specify a region for the algorithm to work on while still accessing
  ///     the original list.
  ///   </para>
  /// </remarks>
#if !NO_SERIALIZATION
  [Serializable, StructLayout(LayoutKind.Sequential)]
#endif
  public struct ListSegment<TElement> {

    /// <summary>
    ///   Initializes a new instance of the <see cref="ListSegment&lt;TElement&gt;" /> class
    ///   that delimits all the elements in the specified string
    /// </summary>
    /// <param name="list">List that will be wrapped</param>
    /// <exception cref="System.ArgumentNullException">String is null</exception>
    public ListSegment(IList<TElement> list) {
      if(list == null) { // questionable, but matches behavior of ArraySegment class
        throw new ArgumentNullException("text", "Text must not be null");
      }

      this.list = list;
      this.offset = 0;
      this.count = list.Count;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ListSegment&lt;TElement&gt;" /> class
    ///   that delimits the specified range of the elements in the specified string
    /// </summary>
    /// <param name="list">The list containing the range of elements to delimit</param>
    /// <param name="offset">The zero-based index of the first element in the range</param>
    /// <param name="count">The number of elements in the range</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   Offset or count is less than 0
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///   Offset and count do not specify a valid range in array
    /// </exception>
    /// <exception cref="System.ArgumentNullException">String is null</exception>
    public ListSegment(IList<TElement> list, int offset, int count) {
      if(list == null) { // questionable, but matches behavior of ArraySegment class
        throw new ArgumentNullException("list");
      }
      if(offset < 0) {
        throw new ArgumentOutOfRangeException(
          "offset", "Argument out of range, non-negative number required"
        );
      }
      if(count < 0) {
        throw new ArgumentOutOfRangeException(
          "count", "Argument out of range, non-negative number required"
        );
      }
      if(count > (list.Count - offset)) {
        throw new ArgumentException(
          "Invalid argument, specified offset and count exceed list size"
        );
      }

      this.list = list;
      this.offset = offset;
      this.count = count;
    }

    /// <summary>
    ///   Gets the original list containing the range of elements that the list
    ///   segment delimits
    /// </summary>
    /// <returns>
    ///   The original list that was passed to the constructor, and that contains the range
    ///   delimited by the <see cref="ListSegment&lt;TElement&gt;" />
    /// </returns>
    public IList<TElement> List {
      get { return this.list; }
    }

    /// <summary>
    ///   Gets the position of the first element in the range delimited by the list segment,
    ///   relative to the start of the original list
    /// </summary>
    /// <returns>
    ///   The position of the first element in the range delimited by the
    ///   <see cref="ListSegment&lt;TElement&gt;" />, relative to the start of the original list
    /// </returns>
    public int Offset {
      get { return this.offset; }
    }

    /// <summary>
    ///   Gets the number of elements in the range delimited by the list segment
    /// </summary>
    /// <returns>
    ///   The number of elements in the range delimited by
    ///   the <see cref="ListSegment&lt;TElement&gt;" />
    /// </returns>
    public int Count {
      get { return this.count; }
    }

    /// <summary>Returns the hash code for the current instance</summary>
    /// <returns>A 32-bit signed integer hash code</returns>
    public override int GetHashCode() {
      int hashCode = this.offset ^ this.count;
      for(int index = 0; index < this.count; ++index) {
        hashCode ^= this.list[index + this.offset].GetHashCode();
      }
      return hashCode;
    }

    /// <summary>
    ///   Determines whether the specified object is equal to the current instance
    /// </summary>
    /// <returns>
    ///   True if the specified object is a <see cref="ListSegment&lt;TElement&gt;" /> structure
    ///   and is equal to the current instance; otherwise, false
    /// </returns>
    /// <param name="other">The object to be compared with the current instance</param>
    public override bool Equals(object other) {
      return
        (other is ListSegment<TElement>) &&
        this.Equals((ListSegment<TElement>)other);
    }

    /// <summary>
    ///   Determines whether the specified <see cref="ListSegment&lt;TElement&gt;" />
    ///   structure is equal to the current instance
    /// </summary>
    /// <returns>
    ///   True if the specified <see cref="ListSegment&lt;TElement&gt;" /> structure is equal
    ///   to the current instance; otherwise, false
    /// </returns>
    /// <param name="other">
    ///   The <see cref="ListSegment&lt;TElement&gt;" /> structure to be compared with
    ///   the current instance
    /// </param>
    public bool Equals(ListSegment<TElement> other) {
      if(other.count != this.count) {
        return false;
      }

      if(ReferenceEquals(other.list, this.list)) {
        return (other.offset == this.offset);
      } else {
        var comparer = Comparer<TElement>.Default;
        for(int index = 0; index < this.count; ++index) {
          int difference = comparer.Compare(
            other.list[index + other.offset], this.list[index + this.offset]
          );
          if(difference != 0) {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    ///   Indicates whether two <see cref="ListSegment&lt;TElement&gt;" /> structures are equal
    /// </summary>
    /// <returns>True if a is equal to b; otherwise, false</returns>
    /// <param name="left">
    ///   The <see cref="ListSegment&lt;TElement&gt;" /> structure on the left side of
    ///   the equality operator
    /// </param>
    /// <param name="right">
    ///   The <see cref="ListSegment&lt;TElement&gt;" /> structure on the right side of
    ///   the equality operator
    /// </param>
    public static bool operator ==(ListSegment<TElement> left, ListSegment<TElement> right) {
      return left.Equals(right);
    }

    /// <summary>
    ///   Indicates whether two <see cref="ListSegment&lt;TElement&gt;" /> structures are unequal
    /// </summary>
    /// <returns>True if a is not equal to b; otherwise, false</returns>
    /// <param name="left">
    ///   The <see cref="ListSegment&lt;TElement&gt;" /> structure on the left side of
    ///   the inequality operator
    /// </param>
    /// <param name="right">
    ///   The <see cref="ListSegment&lt;TElement&gt;" /> structure on the right side of
    ///   the inequality operator
    /// </param>
    public static bool operator !=(ListSegment<TElement> left, ListSegment<TElement> right) {
      return !(left == right);
    }

    /// <summary>Returns a string representation of the list segment</summary>
    /// <returns>The string representation of the list segment</returns>
    public override string ToString() {
      var builder = new System.Text.StringBuilder();
      builder.Append("ListSegment {");
      for(int index = 0; index < Math.Min(this.count, 10); ++index) {
        if(index == 0) {
          builder.Append(" ");
        } else {
          builder.Append(", ");
        }
        builder.Append(this.list[index + this.offset].ToString());
      }

      if(this.count >= 11) {
        builder.Append(", ... }");
      } else {
        builder.Append(" }");
      }

      return builder.ToString();
    }

    /// <summary>Returns a new list containing only the elements in the list segment</summary>
    /// <returns>A new list containing only the elements in the list segment</returns>
    public List<TElement> ToList() {
      if(this.count == 0) {
        return new List<TElement>(capacity: 0);
      } else {
        var newList = new List<TElement>(capacity: this.count);
        {
          int endIndex = this.offset + this.count;
          for(int index = this.offset; index < endIndex; ++index) {
            newList.Add(this.list[index]);
          }
        }

        return newList;
      }
    }

    /// <summary>List wrapped by the list segment</summary>
    private IList<TElement> list;
    /// <summary>Offset in the original list the segment begins at</summary>
    private int offset;
    /// <summary>Number of elements in the segment</summary>
    private int count;

  }

} // namespace Nuclex.Support.Collections
