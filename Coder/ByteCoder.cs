namespace ArithmeticCoder;

public class ByteCoder
{
    public static byte[] Encode(int[] field, int[] coeffs)
    {
        var result = new NoULong();
        if (coeffs.Any(x => x <= 0))
            throw new ArgumentException("Some of coeffs is zero or negative");
        var start = ulong.MinValue;
        var end = ulong.MaxValue;
        var coeffsTotal = (ulong)coeffs.Sum();
        foreach (var figure in field)
        {
            var deltaLength = end - start;
            if (deltaLength < 25)
            {
                result.AddByte(end);
                start <<= 8;
                end <<= 8;
                deltaLength = end - start;
            }

            if (figure != coeffs.Length - 1)
                end -= deltaLength / coeffsTotal * (ulong)coeffs[(figure + 1)..].Sum();
            if (figure != 0)
                start += deltaLength / coeffsTotal * (ulong)coeffs[..figure].Sum();
        }

        result.AddLong(start + 1);
        return result.Bytes;
    }

    public static int[] Decode(byte[] codedFieldBytes, int[] coeffs)
    {
        var result = new int[32];
        var start = ulong.MinValue;
        var end = ulong.MaxValue;
        var myLong = new NoULong(codedFieldBytes);
        var codedField = myLong.ToLong();
        var coeffsTotal = (ulong)coeffs.Sum();
        for (int position = 0; position < result.Length; position++)
        {
            var deltaLength = end - start;
            if (deltaLength < 25)
            {
                start <<= 8;
                end <<= 8;
                codedField = myLong.ToLong();
                deltaLength = end - start;
            }

            for (int figure = 0; figure < coeffs.Length; figure++)
            {
                var buffer = deltaLength / coeffsTotal * (ulong)coeffs[..(figure + 1)].Sum() + start;
                if (codedField >= buffer)
                    continue;
                result[position] = figure;
                if (figure != coeffs.Length - 1)
                    end -= deltaLength / coeffsTotal * (ulong)coeffs[(figure + 1)..].Sum();
                if (figure != 0)
                    start += deltaLength / coeffsTotal * (ulong)coeffs[..figure].Sum();
                break;
            }
        }

        return result;
    }
}