using System;
using System.IO;

namespace Nuclex.Support.Collections {

  /// <summary>Specialized memory stream for ring buffers</summary>
  public class RingMemoryStream : Stream {

    /// <summary>Initializes a new ring memory stream</summary>
    /// <param name="capacity">Maximum capacity of the stream</param>
    public RingMemoryStream(int capacity) {
      this.ringBuffer = new MemoryStream(capacity);
      this.ringBuffer.SetLength(capacity);
      this.empty = true;
    }

    /// <summary>Maximum amount of data that will fit into the ring memory stream</summary>
    public long Capacity {
      get { return this.ringBuffer.Length; }
      set {
        int length = (int)Length;
        if(value < length)
          throw new ArgumentOutOfRangeException(
            "New capacity is less than the stream's length"
          );

        // This could be done in a more efficient manner than just replacing
        // the stream, but this operation will probably be called only once
        // during the lifetime of the application -- if at all...
        MemoryStream newBuffer = new MemoryStream((int)value);

        newBuffer.SetLength(value);
        if(length > 0)
          Read(newBuffer.GetBuffer(), 0, length);

        this.ringBuffer.Close();
        this.ringBuffer = newBuffer;
        this.startIndex = 0;
        this.endIndex = length;
      }

    }

    /// <summary>Whether it's possible to read from this stream</summary>
    public override bool CanRead { get { return true; } }
    /// <summary>Whether this stream supports random access</summary>
    public override bool CanSeek { get { return false; } }
    /// <summary>Whether it's possible to write into this stream</summary>
    public override bool CanWrite { get { return true; } }
    /// <summary>Flushes the buffers and writes down unsaved data</summary>
    public override void Flush() { }

    /// <summary>Current length of the stream</summary>
    public override long Length {
      get {
        if((this.endIndex > this.startIndex) || this.empty) {
          return this.endIndex - this.startIndex;
        } else {
          return this.ringBuffer.Length - this.startIndex + this.endIndex;
        }
      }
    }

    /// <summary>Current cursor position within the stream</summary>
    public override long Position {
      get { throw new NotSupportedException("The ring buffer does not support seeking"); }
      set { throw new NotSupportedException("The ring buffer does not support seeking"); }
    }

    /// <summary>Reads data from the beginning of the stream</summary>
    /// <param name="buffer">Buffer in which to store the data</param>
    /// <param name="offset">Starting index at which to begin writing the buffer</param>
    /// <param name="count">Number of bytes to read from the stream</param>
    /// <returns>Die Number of bytes actually read</returns>
    public override int Read(byte[] buffer, int offset, int count) {

      // The end index lies behind the start index (usual case), so the
      // ring memory is not fragmented. Example: |-----<#######>-----|
      if((this.startIndex < this.endIndex) || this.empty) {

        // The Stream interface requires us to return less than the requested
        // number of bytes if we don't have enough data
        count = Math.Min(count, this.endIndex - this.startIndex);
        if(count > 0) {
          this.ringBuffer.Position = this.startIndex;
          this.ringBuffer.Read(buffer, offset, count);
          this.startIndex += count;

          if(this.startIndex == this.endIndex)
            setEmpty();
        }

        // If the end index lies before the start index, the data in the
        // ring memory stream is fragmented. Example: |#####>-------<#####|
      } else {
        int linearAvailable = (int)(this.ringBuffer.Length - this.startIndex);

        // Will this read process cross the end of the ring buffer, requiring us to
        // read the data in 2 steps?
        if(count > linearAvailable) {

          // The Stream interface requires us to return less than the requested
          // number of bytes if we don't have enough data
          count = Math.Min(count, linearAvailable + this.endIndex);

          this.ringBuffer.Position = this.startIndex;
          this.ringBuffer.Read(buffer, offset, linearAvailable);
          this.ringBuffer.Position = 0;
          this.startIndex = count - linearAvailable;
          this.ringBuffer.Read(buffer, offset + linearAvailable, this.startIndex);

          // Nope, the amount of requested data can be read in one piece without
          // crossing the end of the ring buffer
        } else {
          this.ringBuffer.Position = this.startIndex;
          this.ringBuffer.Read(buffer, offset, count);
          this.startIndex += count;

        }

        if(this.startIndex == this.endIndex)
          setEmpty();
      }

      return count;
    }

    /// <summary>Appends data to the end of the stream</summary>
    /// <param name="buffer">Buffer containing the data to append</param>
    /// <param name="offset">Starting index of the data in the buffer</param>
    /// <param name="count">Number of bytes to write to the stream</param>
    public override void Write(byte[] buffer, int offset, int count) {

      // The end index lies behind the start index (usual case), so the
      // ring memory is not fragmented. Example: |-----<#######>-----|
      if((this.startIndex < this.endIndex) || this.empty) {
        int linearAvailable = (int)(this.ringBuffer.Length - this.endIndex);

        // If the data to be written would cross the ring memory stream's end,
        // we have to check that there's enough space at the beginning of the
        // stream to contain the remainder of the data.
        if(count > linearAvailable) {
          if(count > (linearAvailable + this.startIndex))
            throw new OverflowException("Data does not fit in Ringbuffer");

          this.ringBuffer.Position = this.endIndex;
          this.ringBuffer.Write(buffer, offset, linearAvailable);
          this.ringBuffer.Position = 0;
          this.endIndex = count - linearAvailable;
          this.ringBuffer.Write(buffer, offset + linearAvailable, this.endIndex);

          // All data can be appended at the current stream position without
          // crossing the ring memory stream's end
        } else {
          this.ringBuffer.Position = this.endIndex;
          this.ringBuffer.Write(buffer, offset, count);
          this.endIndex += count;
        }

        this.empty = false;

        // If the end index lies before the start index, the ring memory stream
        // has been fragmented. Hence, this means the gap into which we are about
        // to write cannot be fragmented. Example: |#####>-------<#####|
      } else {
        if(count > (this.startIndex - this.endIndex))
          throw new OverflowException("Data does not fit in Ringbuffer");

        // Because the gap isn't fragmented, we can be sure that a single
        // write call will suffice.
        this.ringBuffer.Position = this.endIndex;
        this.ringBuffer.Write(buffer, offset, count);
        this.endIndex += count;
      }

    }

    /// <summary>Jumps to the specified location within the stream</summary>
    /// <param name="offset">Position to jump to</param>
    /// <param name="origin">Origin towards which to interpret the offset</param>
    /// <returns>The new offset within the stream</returns>
    public override long Seek(long offset, SeekOrigin origin) {
      throw new NotSupportedException("The ring buffer does not support seeking");
    }

    /// <summary>Changes the length of the stream</summary>
    /// <param name="value">New length to resize the stream to</param>
    public override void SetLength(long value) {
      throw new NotSupportedException("This operation is not supported");
    }

    /// <summary>Resets the stream to its empty state</summary>
    private void setEmpty() {
      this.empty = true;
      this.startIndex = 0;
      this.endIndex = 0;
    }

    /// <summary>Internal stream containing the ring buffer data</summary>
    private MemoryStream ringBuffer;
    /// <summary>Start index of the data within the ring buffer</summary>
    private int startIndex;
    /// <summary>End index of the data within the ring buffer</summary>
    private int endIndex;
    /// <summary>Whether the ring buffer is empty</summary>
    /// <remarks>
    ///   This field is required to differentiate between the ring buffer being
    ///   filled to the limit and being totally empty in the case that
    ///    the start index and the end index are the same. 
    /// </remarks>
    bool empty;

  }

} // namespace Nuclex.Support.Collections
