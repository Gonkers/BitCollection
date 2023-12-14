using System.Collections;

namespace Gonkers.Bits;

public class BitCollectionEnumerator : IEnumerator<bool>
{
    public BitCollectionEnumerator(ReadOnlyBitCollection bitCollection)
    {
        _bitCollection = bitCollection;
        _index = -1;
    }

    private readonly ReadOnlyBitCollection _bitCollection;
    private int _index;

    public bool Current => _bitCollection[_index];

    object IEnumerator.Current => Current;

    public void Dispose() { }

    public bool MoveNext() => ++_index < _bitCollection.Count;

    public void Reset() => _index = -1;
}