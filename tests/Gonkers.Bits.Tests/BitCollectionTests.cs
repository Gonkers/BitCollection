namespace Gonkers.Bits.Tests;

[TestFixture]
public class BitCollectionTests
{

    [Test]
    public void test()
    {
        var collection = new BitCollection([0b_0000_0000, 0b_0000_0000]);

        Assert.Multiple(() =>
        {
            Assert.That(collection.SliceInt32(0, 16), Is.EqualTo(0));

            for (int i = 0; i < collection.Count; i++)
            {
                collection[i] = true;
            }

            Assert.That(collection.SliceInt32(0, 16), Is.EqualTo(65535));
        });
    }
}
