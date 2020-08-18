using System;
using System.Collections;
using System.Collections.Generic;

namespace Gonkers.Bits
{
    public readonly struct BitCollection : IReadOnlyList<bool>
    {
        internal const byte BitsInAByte = 8;
        public BitCollection(string base64) : this(Convert.FromBase64String(base64)) { }

        public BitCollection(ReadOnlySpan<byte> bytes) : this(bytes.ToArray()) { }

        public BitCollection(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            _bytes = new byte[bytes.Length];
            bytes.CopyTo(_bytes, 0);
            Count = bytes.Length * BitsInAByte;
        }

        private readonly byte[] _bytes;

        public int Count { get; }

        public int Slice(int start, int length)
        {
            if (start < 0 || start > MaxIndex)
                throw new IndexOutOfRangeException($"The {nameof(start)} must be from 0 to {MaxIndex}.");

            if (length > 32)
                throw new IndexOutOfRangeException($"The {nameof(length)} must be from 1 to 32.");

            if (length == 0) return this[start] ? 1 : 0;

            var longBytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            var startByte = start / BitsInAByte;
            var startOffset = start - startByte * BitsInAByte;
            var endByte = (start + length - 1) / BitsInAByte;
            var endOffset = 64 - (startOffset + length);

            for (int i = 0; startByte + i <= endByte; i++)
            {
                longBytes[i] = _bytes[startByte + i];
            }

            // convert the bytes to a long, then trim extra bits on both ends
            var value = BitConverter.ToUInt64(longBytes) << endOffset >> (endOffset + startOffset);
            return unchecked((int)value);
        }

        public ReadOnlySpan<byte> AsReadOnlySpan() => new ReadOnlySpan<byte>(_bytes);

        private int MaxIndex => Count - 1;

        public bool this[int index]
        {
            get
            {
                if (index < 0 || index > MaxIndex)
                    throw new IndexOutOfRangeException($"The {nameof(index)} must be from 0 to {MaxIndex}");

                // Determine the byte in the array which contains the indexed bit
                // Shift the bits in the byte so the requested bit index is in the first bit position
                // Check if the first bit is 1 or 0 and return true or false respectively
                return ((byte)(_bytes[index / BitsInAByte] >> (index % BitsInAByte)) & 0b0000_0001) == 0b0000_0001;
            }
            set // considering making this read only... Write once, read many ?
            {
                if (index < 0 || index > MaxIndex)
                    throw new IndexOutOfRangeException($"The {nameof(index)} must be from 0 to {MaxIndex}");

                // Locate the bit we need to change and create a bitmask
                var bitmask = (byte)(0b0000_0001 << (index % BitsInAByte));

                if (value)
                {
                    // Using the bitmask set the indexed bit value to true
                    _bytes[index / BitsInAByte] |= bitmask;
                }
                else
                {
                    // Using the inverted bitmask set the indexed bit value to false
                    _bytes[index / BitsInAByte] &= (byte)~bitmask;
                }
            }
        }

        public string ToBase64String() => Convert.ToBase64String(_bytes);
        public override string ToString() => ToBase64String();
        public IEnumerator<bool> GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }
}
