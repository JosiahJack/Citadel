//#define QUEUE_BASED

#if QUEUE_BASED
using System.Collections.Concurrent;
using System.Threading;
#else
using System.Collections;
#endif
using System;
using System.Collections.Generic;

namespace CSM.Runtime.Utilities {
  #if QUEUE_BASED
    /// <summary>
    /// Sized circular buffer data structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class CircularBuffer<T>
    {
        readonly ConcurrentQueue<T> _data;
        readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        readonly int _size;

        public CircularBuffer(int size)
        {
            if (size < 1)
            {
                throw new ArgumentException($"{nameof(size)} cannot be negative or zero");
            }

            this._data = new ConcurrentQueue<T>();
            this._size = size;
        }

        public IEnumerable<T> ToArray()
        {
            return this._data.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._data.GetEnumerator();
        }

        public void Prepend(T t)
        {
            this._lock.EnterWriteLock();
            try
            {
                if (this._data.Count >= this._size)
                {
                    this._data.TryDequeue(out var _);
                }

                this._data.Enqueue(t);
            }
            finally
            {
                this._lock.ExitWriteLock();
            }
        }
    }
  #else

  /// <inheritdoc/>
  /// <summary>
  /// Circular buffer.
  ///
  /// When writing to a full buffer:
  /// PushBack -> removes this[0] / Front()
  /// PushFront -> removes this[Size-1] / Back()
  /// </summary>
  public class CircularBuffer<T> : IEnumerable<T> {
    readonly T[] _buffer;

    /// <summary>
    /// The _start. Index of the first element in buffer.
    /// </summary>
    int _start;

    /// <summary>
    /// The _end. Index after the last element in the buffer.
    /// </summary>
    int _end;

    /// <summary>
    /// The _size. Buffer size.
    /// </summary>
    int _size;

    public CircularBuffer(int capacity) : this(capacity : capacity, items : new T[] { }) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CircularBuffer{T}"/> class.
    ///
    /// </summary>
    /// <param name='capacity'>
    /// Buffer capacity. Must be positive.
    /// </param>
    /// <param name='items'>
    /// Items to fill buffer with. Items length must be less than capacity.
    /// Suggestion: use Skip(x).Take(y).ToArray() to build this argument from
    /// any enumerable.
    /// </param>
    public CircularBuffer(int capacity, T[] items) {
      if (capacity < 1) {
        throw new ArgumentException("Circular buffer cannot have negative or zero capacity.",
                                    paramName : nameof(capacity));
      }

      if (items == null) {
        throw new ArgumentNullException(paramName : nameof(items));
      }

      if (items.Length > capacity) {
        throw new ArgumentException("Too many items to fit circular buffer", paramName : nameof(items));
      }

      this._buffer = new T[capacity];

      Array.Copy(sourceArray : items, destinationArray : this._buffer, length : items.Length);
      this._size = items.Length;

      this._start = 0;
      this._end = this._size == capacity ? 0 : this._size;
    }

    /// <summary>
    /// Maximum capacity of the buffer. Elements pushed into the buffer after
    /// maximum capacity is reached (IsFull = true), will remove an element.
    /// </summary>
    public int Capacity { get { return this._buffer.Length; } }

    /// <summary>
    ///
    /// </summary>
    public bool IsFull { get { return this.Size == this.Capacity; } }

    /// <summary>
    ///
    /// </summary>
    public bool IsEmpty { get { return this.Size == 0; } }

    /// <summary>
    /// Current buffer size (the number of elements that the buffer has).
    /// </summary>
    public int Size { get { return this._size; } }

    /// <summary>
    /// Element at the front of the buffer - this[0].
    /// </summary>
    /// <returns>The value of the element of type T at the front of the buffer.</returns>
    public T Front() {
      this.ThrowIfEmpty();
      return this._buffer[this._start];
    }

    /// <summary>
    /// Element at the back of the buffer - this[Size - 1].
    /// </summary>
    /// <returns>The value of the element of type T at the back of the buffer.</returns>
    public T Back() {
      this.ThrowIfEmpty();
      return this._buffer[(this._end != 0 ? this._end : this.Capacity) - 1];
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="index"></param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public T this[int index] {
      get {
        if (this.IsEmpty) {
          throw new IndexOutOfRangeException(message : $"Cannot access index {index}. Buffer is empty");
        }

        if (index >= this._size) {
          throw new IndexOutOfRangeException(message : $"Cannot access index {index}. Buffer size is {this._size}");
        }

        var actual_index = this.InternalIndex(index : index);
        return this._buffer[actual_index];
      }
      set {
        if (this.IsEmpty) {
          throw new IndexOutOfRangeException(message : $"Cannot access index {index}. Buffer is empty");
        }

        if (index >= this._size) {
          throw new IndexOutOfRangeException(message : $"Cannot access index {index}. Buffer size is {this._size}");
        }

        var actual_index = this.InternalIndex(index : index);
        this._buffer[actual_index] = value;
      }
    }

    /// <summary>
    /// Pushes a new element to the back of the buffer. Back()/this[Size-1]
    /// will now return this element.
    ///
    /// When the buffer is full, the element at Front()/this[0] will be
    /// popped to allow for this new element to fit.
    /// </summary>
    /// <param name="item">Item to push to the back of the buffer</param>
    public void PushBack(T item) {
      if (this.IsFull) {
        this._buffer[this._end] = item;
        this.Increment(index : ref this._end);
        this._start = this._end;
      } else {
        this._buffer[this._end] = item;
        this.Increment(index : ref this._end);
        ++this._size;
      }
    }

    /// <summary>
    /// PushBack
    /// </summary>
    /// <param name="item"></param>
    public void Append(T item) { this.PushBack(item : item); }

    /// <summary>
    /// Pushes a new element to the front of the buffer. Front()/this[0]
    /// will now return this element.
    ///
    /// When the buffer is full, the element at Back()/this[Size-1] will be
    /// popped to allow for this new element to fit.
    /// </summary>
    /// <param name="item">Item to push to the front of the buffer</param>
    public void PushFront(T item) {
      if (this.IsFull) {
        this.Decrement(index : ref this._start);
        this._end = this._start;
        this._buffer[this._start] = item;
      } else {
        this.Decrement(index : ref this._start);
        this._buffer[this._start] = item;
        ++this._size;
      }
    }

    /// <summary>
    /// PushFront
    /// </summary>
    /// <param name="item"></param>
    public void Prepend(T item) { this.PushFront(item : item); }

    /// <summary>
    /// Removes the element at the back of the buffer. Decreasing the
    /// Buffer size by 1.
    /// </summary>
    public void PopBack() {
      this.ThrowIfEmpty("Cannot take elements from an empty buffer.");
      this.Decrement(index : ref this._end);
      this._buffer[this._end] = default;
      --this._size;
    }

    /// <summary>
    /// Removes the element at the front of the buffer. Decreasing the
    /// Buffer size by 1.
    /// </summary>
    public void PopFront() {
      this.ThrowIfEmpty("Cannot take elements from an empty buffer.");
      this._buffer[this._start] = default;
      this.Increment(index : ref this._start);
      --this._size;
    }

    /// <summary>
    /// Copies the buffer contents to an array, according to the logical
    /// contents of the buffer (i.e. independent of the internal
    /// order/contents)
    /// </summary>
    /// <returns>A new array with a copy of the buffer contents.</returns>
    public T[] ToArray() {
      var new_array = new T[this.Size];
      var new_array_offset = 0;
      var segments = new ArraySegment<T>[2] {this.ArrayOne(), this.ArrayTwo()};
      for (var index = 0; index < segments.Length; index++) {
        var segment = segments[index];
        Array.Copy(sourceArray : segment.Array,
                   sourceIndex : segment.Offset,
                   destinationArray : new_array,
                   destinationIndex : new_array_offset,
                   length : segment.Count);
        new_array_offset += segment.Count;
      }

      return new_array;
    }

    #region IEnumerable<T> implementation

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator() {
      var segments = new ArraySegment<T>[2] {this.ArrayOne(), this.ArrayTwo()};
      for (var index = 0; index < segments.Length; index++) {
        var segment = segments[index];
        for (var i = 0; i < segment.Count; i++) {
          yield return segment.Array[segment.Offset + i];
        }
      }
    }

    #endregion

    #region IEnumerable implementation

    IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

    #endregion

    void ThrowIfEmpty(string message = "Cannot access an empty buffer.") {
      if (this.IsEmpty) {
        throw new InvalidOperationException(message : message);
      }
    }

    /// <summary>
    /// Increments the provided index variable by one, wrapping
    /// around if necessary.
    /// </summary>
    /// <param name="index"></param>
    void Increment(ref int index) {
      if (++index == this.Capacity) {
        index = 0;
      }
    }

    /// <summary>
    /// Decrements the provided index variable by one, wrapping
    /// around if necessary.
    /// </summary>
    /// <param name="index"></param>
    void Decrement(ref int index) {
      if (index == 0) {
        index = this.Capacity;
      }

      index--;
    }

    /// <summary>
    /// Converts the index in the argument to an index in <code>_buffer</code>
    /// </summary>
    /// <returns>
    /// The transformed index.
    /// </returns>
    /// <param name='index'>
    /// External index.
    /// </param>
    int InternalIndex(int index) {
      return this._start + (index < (this.Capacity - this._start) ? index : index - this.Capacity);
    }

    // doing ArrayOne and ArrayTwo methods returning ArraySegment<T> as seen here:
    // http://www.boost.org/doc/libs/1_37_0/libs/circular_buffer/doc/circular_buffer.html#classboost_1_1circular__buffer_1957cccdcb0c4ef7d80a34a990065818d
    // http://www.boost.org/doc/libs/1_37_0/libs/circular_buffer/doc/circular_buffer.html#classboost_1_1circular__buffer_1f5081a54afbc2dfc1a7fb20329df7d5b
    // should help a lot with the code.

    #region Array items easy access.

    // The array is composed by at most two non-contiguous segments,
    // the next two methods allow easy access to those.

    ArraySegment<T> ArrayOne() {
      if (this.IsEmpty) {
        return new ArraySegment<T>(array : new T[0]);
      }

      if (this._start < this._end) {
        return new ArraySegment<T>(array : this._buffer, offset : this._start, count : this._end - this._start);
      }

      return new ArraySegment<T>(array : this._buffer, offset : this._start, count : this._buffer.Length - this._start);
    }

    ArraySegment<T> ArrayTwo() {
      if (this.IsEmpty) {
        return new ArraySegment<T>(array : new T[0]);
      }

      if (this._start < this._end) {
        return new ArraySegment<T>(array : this._buffer, offset : this._end, 0);
      }

      return new ArraySegment<T>(array : this._buffer, 0, count : this._end);
    }

    #endregion
  }

  #endif
}