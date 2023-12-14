namespace Gonkers.Bits;

public class BitCollection : ReadOnlyBitCollection
{
    public BitCollection(string base64) : base(base64) { }

    public BitCollection(ReadOnlySpan<byte> bytes) : base(bytes) { }

    public BitCollection(BitCollection other) : base(other) { }

    new public bool this[int index]
    {
        get => Get(index);
        set => Set(index, value);
    }

    protected void Set(int index, bool value)
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
