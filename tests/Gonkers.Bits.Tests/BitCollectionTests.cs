namespace Gonkers.Bits.Tests;

public class BitCollectionTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void WhenAllBitsAreOn_ThenAllItemsAreTrue()
    {
        var collection1 = new BitCollection([255]);
        var collection2 = new BitCollection([255, 255]);
        var collection3 = new BitCollection([255, 255, 255]);

        var collection = collection1.Concat(collection2).Concat(collection3);

        Assert.Multiple(() =>
        {
            foreach (var bit in collection)
            {
                Assert.That(bit, Is.True);
            }
        });
    }

    [Test]
    public void WhenAllBitsAreOff_ThenAllItemsAreFalse()
    {
        var collection1 = new BitCollection([0]);
        var collection2 = new BitCollection([0, 0]);
        var collection3 = new BitCollection([0, 0, 0]);

        Assert.Multiple(() =>
        {
            Assert.That(collection1, Is.All.False);
            Assert.That(collection2, Is.All.False);
            Assert.That(collection3, Is.All.False);
        });
    }

    [Test]
    public void WhenCreatedWithBase64String_ThenConvertToBits()
    {
        var base64 = Convert.ToBase64String([255, 255, 255]);
        var collection = new BitCollection(base64);

        Assert.That(collection, Is.All.True);
    }

    [Test]
    public void WhenGettingASliceOnASmallArray_ThenReturnAnInt()
    {
        var collection = new BitCollection([255, 255, 0, 0]);

        Assert.Multiple(() =>
        {
            Assert.That(collection.SliceInt32(0, 16), Is.EqualTo(65535));
            Assert.That(collection.SliceInt32(16, 16), Is.EqualTo(0));
            Assert.That(collection.SliceInt32(8, 32), Is.EqualTo(255));
        });
    }

    [Test]
    public void WhenGettingASliceOnALargeArray_ThenReturnAnInt()
    {
        var collection = new BitCollection([
            0b1111_1111, // 255 0
            0b1111_1111, // 255 8
            0b1111_1111, // 255 16
            0b1111_1111, // 255 24
            0b0000_1111, // 240 32
            0b1111_0000, //  15 40
            0b1111_1111, // 255 48
            0b1010_1010]); // 170 56

        Assert.Multiple(() =>
        {
            Assert.That(collection.SliceInt32(32, 8), Is.EqualTo(15));
            Assert.That(collection.SliceInt32(40, 8), Is.EqualTo(240));
            Assert.That(collection.SliceInt32(56, 8), Is.EqualTo(170));
            Assert.That(collection.SliceInt32(36, 8), Is.EqualTo(0));
            Assert.That(collection.SliceInt32(32, 16), Is.EqualTo(61455));
        });
    }

    [Test]
    public void WhenGettingASliceOnAnOversizedArray_ThenReturnAnInt()
    {
        var collection = new BitCollection([
            156, 96, 217, 246, 167, 187, 102, 225, 58, 161, 245, 162,
            161, 148, 114, 76, 43, 9, 165, 58, 167, 130, 59, 132, 215,
            0b1111_1111, // 255 200
            0b1111_1111, // 255 208
            0b1111_1111, // 255 216
            0b1111_1111, // 255 224
            0b0000_1111, // 240 232
            0b1111_0000, //  15 240
            0b1111_1111, // 255 248
            0b1010_1010, // 170 256
            38, 187, 183, 204, 56, 244, 125, 173, 76, 114, 207, 222, 241,
            198, 233, 158, 1, 177, 115, 107, 171, 95, 10, 227, 226, 233,
            68, 118, 6, 216, 178, 192, 250, 1, 10, 156, 149, 236, 231,
            0b1111_1111, // 255 576
            0b1111_1111, // 255 584
        ]);

        Assert.Multiple(() =>
        {
            Assert.That(collection.SliceInt32(200, 32), Is.EqualTo(-1));
            Assert.That(collection.SliceInt32(232, 8), Is.EqualTo(15));
            Assert.That(collection.SliceInt32(240, 8), Is.EqualTo(240));
            Assert.That(collection.SliceInt32(256, 8), Is.EqualTo(170));
            Assert.That(collection.SliceInt32(236, 8), Is.EqualTo(0));
            Assert.That(collection.SliceInt32(232, 16), Is.EqualTo(61455));
            Assert.That(collection.SliceInt32(576, 16), Is.EqualTo(65535));
        });
    }

    [Test]
    public void WhenReferencingBitsByIndex_ThenReturnABoolean()
    {
        var collection = new BitCollection([
            0b_1010_1010,
            0b_1010_1010,
            0b_1010_1010,
        ]);

        Assert.Multiple(() =>
        {
            for (var i = 0; i < collection.Count; i++)
            {
                Assert.That(collection[i], Is.EqualTo(i % 2 != 0));
            }
        });
    }
}
