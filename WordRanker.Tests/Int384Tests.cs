namespace WordRanker.Tests
{
    public class Int384Tests
    {
        private const ulong OnlyLastBit = 0x8000000000000000ul;

        [Fact]
        public void CompareTwo()
        {
            var a = new Int384(523235, 1241452, 51351, 13413, 123, 13245);
            var b = new Int384(523235, 531241, 315325, 351235, 51123, 514123);
            Assert.True(a > b);
            Assert.True(b < a);
            Assert.True(a >= b);
            Assert.True(b <= a);
        }

        [Fact]
        public void TwoEquals()
        {
            var a = new Int384(523235, 531241, 315325, 351235, 51123, 514123);
            var b = new Int384(523235, 531241, 315325, 351235, 51123, 514123);
            Assert.Equal(a, b);
            Assert.True(a == b);
        }

        [Fact]
        public void TwoNotEqual()
        {
            var a = new Int384(523235, 531241, 315325, 351235, 51123, 514123);
            var b = new Int384(523235, 531241, 315326, 351235, 51123, 514123);
            Assert.NotEqual(a, b);
            Assert.True(a != b);
        }

        [Fact]
        public void BitShiftLeft()
        {
            var a = new Int384(352314123, OnlyLastBit >> 1, 0, 0, 0, 0b100000);
            var b = new Int384(0, 0, 0, 0, 0b10000000, 0);
            a <<= 66;
            Assert.Equal(b, a);
        }

        [Fact]
        public void BitShiftLeft1()
        {
            var a = new Int384(OnlyLastBit, 0, OnlyLastBit, 0, OnlyLastBit, 0);
            var b = new Int384(0, 1, 0, 1, 0, 0);
            a <<= 1;
            Assert.Equal(b, a);
        }

        [Fact]
        public void BitShiftRight()
        {
            var a = new Int384(0, 0, 0, 0, 0, 0b100);
            var b = new Int384(0, 0, 0, 0, 0b100000, 25141242);
            b >>= 67;
            Assert.Equal(a, b);
        }

        [Fact]
        public void BitShiftRight1()
        {
            var a = new Int384(0, 0, 0, 0, 0, OnlyLastBit);
            var b = new Int384(0, 0, 0, 0, 1, 0);
            b >>= 1;
            Assert.Equal(a, b);
        }

        [Fact]
        public void BitShiftRightShouldMakeZero()
        {
            var a = new Int384(0, 0, 0, 0, 0b111110, 5235690431);
            Assert.NotEqual(new Int384(), a >> 69);
            Assert.Equal(new Int384(), a >> 70);
        }

        [Fact]
        public void BitwiseOr()
        {
            var a = new Int384(0b110, 0b110, 0b110, 0b110, 0b110, 0b110);
            var b = new Int384(0b011, 0b011, 0b011, 0b011, 0b011, 0b011);
            Assert.Equal(a | b, new Int384(0b111, 0b111, 0b111, 0b111, 0b111, 0b111));
        }

        [Fact]
        public void BitwiseAnd()
        {
            var a = new Int384(0b110, 0b110, 0b110, 0b110, 0b110, 0b110);
            var b = new Int384(0b011, 0b011, 0b011, 0b011, 0b011, 0b011);
            Assert.Equal(a & b, new Int384(0b010, 0b010, 0b010, 0b010, 0b010, 0b010));
        }
    }
}