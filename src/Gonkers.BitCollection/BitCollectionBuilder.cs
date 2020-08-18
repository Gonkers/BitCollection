using System;
using System.Collections.Generic;
using System.Text;

namespace Gonkers.Bits
{
    public sealed class BitCollectionBuilder
    {
        private List<(ulong bits, uint bitmask, byte bitCount)> _map = new List<(ulong, uint, byte)>();
        private ushort _totalBits = 0;

        public Range Add(int bits, int bitmask)
        {
            var bitCount = GetBitCount((uint)bitmask);
            var range = new Range(_totalBits, (_totalBits += bitCount) - 1);
            _map.Add((unchecked((ulong)bits), unchecked((uint)bitmask), bitCount));
            return range;
        }
        public Range Add(bool bit) => Add(bit ? 1 : 0, 1);
        public Range Add(byte @byte) => Add(@byte, byte.MaxValue);
        public Range Add(short @short) => Add(@short, ushort.MaxValue);

        public BitCollection Build()
        {
            var bytes = new List<byte>();
            ulong bitBuffer = 0;
            byte bitBufferLen = 0;
            for (int i = 0; i < _map.Count; i++)
            {
                bitBuffer ^= _map[i].bits << bitBufferLen;
                bitBufferLen += _map[i].bitCount;

                while (bitBufferLen >= BitCollection.BitsInAByte)
                {
                    bytes.Add(unchecked((byte)bitBuffer));
                    bitBuffer >>= BitCollection.BitsInAByte;
                    bitBufferLen -= BitCollection.BitsInAByte;
                }
            }

            if (bitBufferLen > 0) // if bits remain in the buffer, flush
                bytes.Add(unchecked((byte)(bitBuffer)));

            return new BitCollection(bytes.ToArray());
        }

        // The bitmask must be right aligned, any 1s after a 0 will be ignored. 00001111
        private static byte GetBitCount(uint bitmask)
        {
            byte count = 0;
            while ((bitmask & 0b1) == 0b1)
            {
                count++;
                bitmask >>= 1;
            }
            return count;
        }
    }
}
