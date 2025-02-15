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
using System.Runtime.InteropServices;

namespace Nuclex.Support {

  /// <summary>View into a section of a string without copying said string</summary>
  /// <remarks>
  ///   <para>
  ///     The design of this class pretty much mirrors that of the
  ///     <see cref="T:System.ArraySegment" /> class found in the .NET framework, but is
  ///     specialized to be used for strings, which can not be expressed as arrays but
  ///     share a lot of the characteristics of an array.
  ///   </para>
  ///   <para>
  ///     In certain situations, passing a StringSegment instead of the actual copied
  ///     substring is useful. For example, the caller might want to know from which
  ///     index of the original string the substring was taken. Used internally in parsers,
  ///     it can also prevent needless string copying and garbage generation.
  ///   </para>
  /// </remarks>
#if !NO_SERIALIZATION
  [Serializable, StructLayout(LayoutKind.Sequential)]
#endif
  public struct StringSegment {

    /// <summary>
    ///   Initializes a new instance of the <see cref="StringSegment" /> class that delimits
    ///   all the elements in the specified string
    /// </summary>
    /// <param name="text">String that will be wrapped</param>
    /// <exception cref="System.ArgumentNullException">String is null</exception>
    public StringSegment(string text) {
      if(text == null) { // questionable, but matches behavior of ArraySegment class
        throw new ArgumentNullException("text", "Text must not be null");
      }

      this.text = text;
      this.offset = 0;
      this.count = text.Length;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="StringSegment" /> class that delimits
    ///   the specified range of the elements in the specified string
    /// </summary>
    /// <param name="text">The string containing the range of elements to delimit</param>
    /// <param name="offset">The zero-based index of the first element in the range</param>
    /// <param name="count">The number of elements in the range</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   Offset or count is less than 0
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///   Offset and count do not specify a valid range in array
    /// </exception>
    /// <exception cref="System.ArgumentNullException">String is null</exception>
    public StringSegment(string text, int offset, int count) {
      if(text == null) { // questionable, but matches behavior of ArraySegment class
        throw new ArgumentNullException("text");
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
      if(count > (text.Length - offset)) {
        throw new ArgumentException(
          "Invalid argument, specified offset and count exceed string length"
        );
      }

      this.text = text;
      this.offset = offset;
      this.count = count;
    }

    /// <summary>
    ///   Gets the original string containing the range of elements that the string
    ///   segment delimits
    /// </summary>
    /// <returns>
    ///   The original string that was passed to the constructor, and that contains the range
    ///   delimited by the <see cref="StringSegment" />
    /// </returns>
    public string Text {
      get { return this.text; }
    }

    /// <summary>
    ///   Gets the position of the first element in the range delimited by the string segment,
    ///   relative to the start of the original string
    /// </summary>
    /// <returns>
    ///   The position of the first element in the range delimited by the
    ///   <see cref="StringSegment" />, relative to the start of the original string
    /// </returns>
    public int Offset {
      get { return this.offset; }
    }

    /// <summary>
    ///   Gets the number of elements in the range delimited by the string segment
    /// </summary>
    /// <returns>
    ///   The number of elements in the range delimited by the <see cref="StringSegment" />
    /// </returns>
    public int Count {
      get { return this.count; }
    }

    /// <summary>Returns the hash code for the current instance</summary>
    /// <returns>A 32-bit signed integer hash code</returns>
    public override int GetHashCode() {
      return this.text.GetHashCode() ^ this.offset ^ this.count;
    }

    /// <summary>
    ///   Determines whether the specified object is equal to the current instance
    /// </summary>
    /// <returns>
    ///   True if the specified object is a <see cref="StringSegment" /> structure and is
    ///   equal to the current instance; otherwise, false
    /// </returns>
    /// <param name="other">The object to be compared with the current instance</param>
    public override bool Equals(object other) {
      return
        (other is StringSegment) &&
        this.Equals((StringSegment)other);
    }

    /// <summary>
    ///   Determines whether the specified <see cref="StringSegment" /> structure is equal
    ///   to the current instance
    /// </summary>
    /// <returns>
    ///   True if the specified <see cref="StringSegment" /> structure is equal to the
    ///   current instance; otherwise, false
    /// </returns>
    /// <param name="other">
    ///   The <see cref="StringSegment" /> structure to be compared with the current instance
    /// </param>
    public bool Equals(StringSegment other) {
      return
        (other.text == this.text) &&
        (other.offset == this.offset) &&
        (other.count == this.count);
    }

    /// <summary>
    ///   Indicates whether two <see cref="StringSegment" /> structures are equal
    /// </summary>
    /// <returns>True if a is equal to b; otherwise, false</returns>
    /// <param name="left">
    ///   The <see cref="StringSegment" /> structure on the left side of the
    ///   equality operator
    /// </param>
    /// <param name="right">
    ///   The <see cref="StringSegment" /> structure on the right side of the
    ///   equality operator
    /// </param>
    public static bool operator ==(StringSegment left, StringSegment right) {
      return left.Equals(right);
    }

    /// <summary>
    ///   Indicates whether two <see cref="StringSegment" /> structures are unequal
    /// </summary>
    /// <returns>True if a is not equal to b; otherwise, false</returns>
    /// <param name="left">
    ///   The <see cref="StringSegment" /> structure on the left side of the
    ///   inequality operator
    /// </param>
    /// <param name="right">
    ///   The <see cref="StringSegment" /> structure on the right side of the
    ///   inequality operator
    /// </param>
    public static bool operator !=(StringSegment left, StringSegment right) {
      return !(left == right);
    }

    /// <summary>Returns a string representation of the string segment</summary>
    /// <returns>The string representation of the string segment</returns>
    public override string ToString() {
      return this.text.Substring(this.offset, this.count);
    }

    /// <summary>String wrapped by the string segment</summary>
    private string text;
    /// <summary>Offset in the original string the segment begins at</summary>
    private int offset;
    /// <summary>Number of characters in the segment</summary>
    private int count;

  }

} // namespace Nuclex.Support
