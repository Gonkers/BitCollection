namespace Gonkers.Bits;

public class BitCollection : ReadOnlyBitCollection
{
    public BitCollection(string base64) : base(base64)
    {
    }

    public BitCollection(ReadOnlySpan<byte> bytes) : base(bytes)
    {
    }


}
