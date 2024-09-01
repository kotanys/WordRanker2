using System.Runtime.InteropServices;

namespace WordRanker;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Int384 : IEquatable<Int384>, IComparable<Int384>
{
    // _b0 _b1 _b2 _b3 _b4 _b5
    private readonly ulong _b0;
    private readonly ulong _b1;
    private readonly ulong _b2;
    private readonly ulong _b3;
    private readonly ulong _b4;
    private readonly ulong _b5;

    /// <summary>
    /// Creates new instance of <see cref="Int384"/>.
    /// </summary>
    /// <param name="b0">MSB</param>
    /// <param name="b5">LSB</param>
    public Int384(ulong b0, ulong b1, ulong b2, ulong b3, ulong b4, ulong b5)
    {
        _b0 = b0;
        _b1 = b1;
        _b2 = b2;
        _b3 = b3;
        _b4 = b4;
        _b5 = b5;
    }

    public int CompareTo(Int384 other)
    {
        if (_b0 != other._b0)
            return _b0.CompareTo(other._b0);
        else if (_b1 != other._b1)
            return _b1.CompareTo(other._b1);
        else if (_b2 != other._b2)
            return _b2.CompareTo(other._b2);
        else if (_b3 != other._b3)
            return _b3.CompareTo(other._b3);
        else if (_b4 != other._b4)
            return _b4.CompareTo(other._b4);
        else if (_b5 != other._b5)
            return _b5.CompareTo(other._b5);
        else return 0;
    }

    public bool Equals(Int384 other)
    {
        return _b0 == other._b0 && _b1 == other._b1 && _b2 == other._b2 && _b3 == other._b3 && _b4 == other._b4 && _b5 == other._b5;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Int384 i)
            return Equals(i);
        else
            return false;
    }

    public override string ToString()
    {
        Span<char> span = stackalloc char[96];
        span.Fill('0');
        Append(span, 15, _b0);
        Append(span, 31, _b1);
        Append(span, 47, _b2);
        Append(span, 63, _b3);
        Append(span, 79, _b4);
        Append(span, 95, _b5);
        return new string(span);

        static void Append(Span<char> span, int pos, ulong val)
        {
            string s = Convert.ToString((long)val, 16);
            for (int i = s.Length - 1; i >= 0; i--)
            {
                span[pos--] = s[i];
            }
        }
    }

    public override int GetHashCode()
    {
        return _b0.GetHashCode() ^ _b1.GetHashCode() ^ _b2.GetHashCode() ^ _b3.GetHashCode() ^ _b4.GetHashCode() ^ _b5.GetHashCode();
    }

    public static bool operator ==(Int384 left, Int384 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Int384 left, Int384 right)
    {
        return !(left == right);
    }

    public static bool operator <(Int384 left, Int384 right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Int384 left, Int384 right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Int384 left, Int384 right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Int384 left, Int384 right)
    {
        return left.CompareTo(right) >= 0;
    }

    public static Int384 operator <<(Int384 left, int right)
    {
        Span<ulong> longs = [left._b0, left._b1, left._b2, left._b3, left._b4, left._b5];
        while (right > 0)
        {
            int shift = Math.Min(right, 64);

            ulong lastBits = shift == 64 ? ulong.MaxValue : ~(ulong)Math.Pow(2, 63 - shift); 
            ulong scarry = 0;
            for (int i = 5; i >= 0; i--)
            {
                ulong carry = longs[i] & lastBits;
                longs[i] = shift == 64 ? 0 : longs[i] << shift;
                longs[i] |= scarry >> (64 - shift);
                scarry = carry;
            }
            right -= 64;
        }
        return new Int384(longs[0], longs[1], longs[2], longs[3], longs[4], longs[5]);
    }

    public static Int384 operator >>(Int384 left, int right)
    {
        Span<ulong> longs = [left._b0, left._b1, left._b2, left._b3, left._b4, left._b5];
        while (right > 0)
        {
            int shift = Math.Min(right, 64);

            ulong firstBits = shift == 64 ? ulong.MaxValue : (ulong)Math.Pow(2, shift) - 1;
            ulong scarry = 0;
            for (int i = 0; i < 6; i++)
            {
                ulong carry = longs[i] & firstBits;
                longs[i] = shift == 64 ? 0 : longs[i] >> shift;
                longs[i] |= scarry << (64 - shift);
                scarry = carry;
            }
            right -= 64;
        }
        
        return new Int384(longs[0], longs[1], longs[2], longs[3], longs[4], longs[5]);
    }

    public static Int384 operator |(Int384 left, Int384 right)
    {
        var b0 = left._b0 | right._b0;
        var b1 = left._b1 | right._b1;
        var b2 = left._b2 | right._b2;
        var b3 = left._b3 | right._b3;
        var b4 = left._b4 | right._b4;
        var b5 = left._b5 | right._b5;
        return new Int384(b0, b1, b2, b3, b4, b5);
    }

    public static Int384 operator &(Int384 left, Int384 right)
    {
        var b0 = left._b0 & right._b0;
        var b1 = left._b1 & right._b1;
        var b2 = left._b2 & right._b2;
        var b3 = left._b3 & right._b3;
        var b4 = left._b4 & right._b4;
        var b5 = left._b5 & right._b5;
        return new Int384(b0, b1, b2, b3, b4, b5);
    }
}