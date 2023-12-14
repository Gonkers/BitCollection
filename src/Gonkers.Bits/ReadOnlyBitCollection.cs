using System.Collections;

namespace Gonkers.Bits;

public class ReadOnlyBitCollection : IReadOnlyList<bool>, IEquatable<ReadOnlyBitCollection>
{
    internal const int
        BitsInAByte = 8,
        BitsInAnInt32 = 32,
        BytesInAnInt64 = 8;

    public ReadOnlyBitCollection(string base64) : this(Convert.FromBase64String(base64)) { }

    public ReadOnlyBitCollection(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0)
            throw new ArgumentException("The source collection must not be empty.", nameof(bytes));

        _bytes = bytes.Length <= BytesInAnInt64 ? (new byte[BytesInAnInt64]) : (new byte[bytes.Length]);
        bytes.CopyTo(_bytes);
        Count = bytes.Length * BitsInAByte;
        TotalBytes = bytes.Length;
    }

    public ReadOnlyBitCollection(ReadOnlyBitCollection other) : this(other._bytes) { }
    public ReadOnlyBitCollection(Guid value) : this(value.ToByteArray()) { }
    public ReadOnlyBitCollection(ulong value) : this(BitConverter.GetBytes(value)) { }
    public ReadOnlyBitCollection(long value) : this(BitConverter.GetBytes(value)) { }
    public ReadOnlyBitCollection(uint value) : this(BitConverter.GetBytes(value)) { }
    public ReadOnlyBitCollection(int value) : this(BitConverter.GetBytes(value)) { }
    public ReadOnlyBitCollection(ushort value) : this(BitConverter.GetBytes(value)) { }
    public ReadOnlyBitCollection(short value) : this(BitConverter.GetBytes(value)) { }
    public ReadOnlyBitCollection(byte value) : this([value]) { }

    protected readonly byte[] _bytes;

    public int Count { get; protected set; }
    public int TotalBytes { get; protected set; }

    public int SliceInt32(int start, int length)
    {
        if (start < 0 || start > MaxIndex)
            throw new IndexOutOfRangeException($"The {nameof(start)} must be from 0 to {MaxIndex}.");

        if (length is < 1 or > BitsInAnInt32)
            throw new IndexOutOfRangeException($"The {nameof(length)} must be from 1 to {BitsInAnInt32}.");

        if (length == 1) return this[start] ? 1 : 0;

        ulong bitHolder = BitConverter.ToUInt64(_bytes, 0);
        if (_bytes.Length > BytesInAnInt64)
        {
            var startByteIndex = start / BitsInAByte;
            if (startByteIndex + BytesInAnInt64 > _bytes.Length)
                startByteIndex = _bytes.Length - BytesInAnInt64;

            start -= startByteIndex * BitsInAByte;
            var byteView = _bytes.AsSpan(startByteIndex, BytesInAnInt64);
            bitHolder = BitConverter.ToUInt64(byteView);
        }

        var endBitOffset = (BytesInAnInt64 * BitsInAByte) - (start + length);
        bitHolder = bitHolder << endBitOffset >> (endBitOffset + start);
        return unchecked((int)bitHolder);
    }

    public ReadOnlySpan<byte> AsReadOnlySpan() => new(_bytes);

    protected int MaxIndex => Count - 1;

    protected bool Get(int index)
    {
        if (index < 0 || index > MaxIndex)
            throw new IndexOutOfRangeException($"The {nameof(index)} must be from 0 to {MaxIndex}");

        // Determine the byte in the array which contains the indexed bit
        // Shift the bits in the byte so the requested bit index is in the first bit position
        // Check if the first bit is 1 or 0 and return true or false respectively
        return ((byte)(_bytes[index / BitsInAByte] >> (index % BitsInAByte)) & 0b0000_0001) == 0b0000_0001;
    }

    public bool this[int index] => Get(index);

    public string ToBase64String() => Convert.ToBase64String(new ReadOnlySpan<byte>(_bytes, 0, TotalBytes));
    public override string ToString() => ToBase64String();
    public IEnumerator<bool> GetEnumerator() => new BitCollectionEnumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => new BitCollectionEnumerator(this);

    public bool Equals(ReadOnlyBitCollection? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return _bytes.SequenceEqual(other._bytes);
    }

    public override int GetHashCode() => _bytes.GetHashCode();

    public static bool operator ==(ReadOnlyBitCollection left, ReadOnlyBitCollection right) =>
        left is not null && right is not null && left.Equals(right);

    public static bool operator !=(ReadOnlyBitCollection left, ReadOnlyBitCollection right) => !(left == right);

    public override bool Equals(object? obj) => Equals(obj as ReadOnlyBitCollection);
}
