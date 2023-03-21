namespace ArithmeticCoder;

public class Coder
{
    public static Result Encode(int[] field, int[] coeffs)
    {
        if (coeffs.Any(x => x <= 0))
            throw new ArgumentException("Some of coeffs is zero or negative");

        var start = ulong.MinValue;
        var end = ulong.MaxValue;
        var coeffsTotal = (ulong)coeffs.Sum();
        var coeffsCumulative = Enumerable.Range(0, coeffs.Length + 1).Select(x => (ulong)coeffs.Take(x).Sum()).ToArray();

        var output = new List<byte> { 0 };
        var counter = 0;

        foreach (var figure in field)
        {
            var currentLength = end - start;

            (start, end) = (start + currentLength / coeffsTotal * coeffsCumulative[figure],
                start - 1 + currentLength / coeffsTotal * coeffsCumulative[figure + 1]);

            var comparison = ~(start ^ end);
            var b = 1ul << 63;
            while ((b & comparison) != 0)
            {
                output[^1] = (byte)((output[^1] << 1) | (byte)((b & start) >> 63));
                counter++;
                if (counter == 8)
                {
                    counter = 0;
                    output.Add(0);
                }

                comparison <<= 1;
                start <<= 1;
                end <<= 1;
            }
        }
        output[^1] = (byte)((output[^1] << 1) | 1);
        counter++;
        output[^1] = (byte)(output[^1] << ((-counter) & 0b111));
        return new Result(output.ToArray(), counter + output.Count * 8);
    }

    public static int[] Decode(byte[] encoded, int[] coeffs, int fieldLength = 32)
    {
        var result = new int[fieldLength];
        var start = ulong.MinValue;
        var end = ulong.MaxValue;
        var coeffsTotal = (ulong)coeffs.Sum();
        var coeffsCumulative = Enumerable.Range(0, coeffs.Length + 1).Select(x => (ulong)coeffs.Take(x).Sum()).ToArray();

        var buffer = Enumerable.Range(0, 8).Select(x => x >= encoded.Length ? 0 : encoded[x])
            .Select((v, i) => (ulong)v << (56 - 8 * i)).Aggregate(0ul, (acc, v) => acc | v);
        var moved = 4 * 8;
        for (var i = 0; i < fieldLength; i++)
        {
            var currentLength = end - start;
            foreach (var figure in Enumerable.Range(0, coeffs.Length))
            {
                var (low, high) = (start + currentLength / coeffsTotal * coeffsCumulative[figure],
                    start - 1 + currentLength / coeffsTotal * coeffsCumulative[figure + 1]);

                if (buffer >= low && buffer <= high)
                {
                    result[i] = figure;
                    var comparison = ~(start ^ end);
                    var b = 1ul << 63;
                    start = low;
                    end = high;
                    while ((b & comparison) != 0)
                    {
                        comparison <<= 1;
                        start <<= 1;
                        end <<= 1;
                        buffer <<= 1;
                        moved++;
                        var byteNumber = moved / 8;
                        var bitNumber = moved % 8;
                        if (byteNumber < encoded.Length)
                        {
                            var sourceByte = encoded[byteNumber] >> (8 - bitNumber - 1);
                            buffer |= (uint)(sourceByte & 1);
                        }
                    }
                    break;
                }
            }
        }

        return result;
    }

    public static int[] Decode(byte[] codedField, int[] coeffs)
    {
        var result = Array.Empty<int>();
        return codedField.Decode(coeffs, 32);
    }
}

//public class Coder
//{
//    public static int[] Decode(ulong codedField, int[] coeffs, int fieldLength = 32)
//    {
//        var result = new int[fieldLength];
//        var start = ulong.MinValue;
//        var end = ulong.MaxValue;
//        var coeffsTotal = (ulong)coeffs.Sum();

//        for (int position = 0; position < result.Length; position++)
//        {
//            var currentLength = end - start;
//            for (int figure = 0; figure < coeffs.Length; figure++)
//            {
//                if (currentLength < 10)
//                {
//                    start = start % 10 * 111111111;
//                    end = end % 10 * 111111111;
//                    codedField = codedField % 10 * 111111111;
//                    currentLength = end - start;
//                }

//                var buffer = currentLength / coeffsTotal * (ulong)coeffs[..(figure + 1)].Sum() + start;
//                if (codedField >= buffer)
//                    continue;
//                result[position] = figure;
//                if (figure != coeffs.Length - 1)
//                    end -= currentLength / coeffsTotal * (ulong)coeffs[(figure + 1)..].Sum();
//                if (figure != 0)
//                    start += currentLength / coeffsTotal * (ulong)coeffs[..figure].Sum();
//                break;
//            }
//        }

//        return result;
//    }

//    public static int[] Decode(ulong[] codedField, int[] coeffs)
//    {
//        var result = Array.Empty<int>();

//        return codedField.Aggregate(result,
//            (current, tst) => current.Concat(tst.Decode(coeffs, 32 / codedField.Length)).ToArray());
//    }

//    public static Result Encode(int[] field, int[] coeffs)
//    {
//        if (coeffs.Any(x => x <= 0))
//            throw new ArgumentException("Some of coeffs is zero or negative");

//        var start = ulong.MinValue;
//        var end = ulong.MaxValue;
//        var coeffsTotal = (ulong)coeffs.Sum();

//        foreach (var figure in field)
//        {
//            var currentLength = end - start;
//            if (figure != coeffs.Length - 1)
//                end -= currentLength / coeffsTotal * (ulong)coeffs[(figure + 1)..].Sum();

//            if (figure != 0)
//                start += currentLength / coeffsTotal * (ulong)coeffs[..figure].Sum();
//        }

//        var codedField = start + 1;
//        var isCoded = codedField.Decode(coeffs, field.Length).SequenceEqual(field);
//        if (isCoded)
//            return new Result { IsSplitted = false, CodedField = new[] { codedField } };

//        if (!isCoded && field.Length != 32)
//            throw new Exception("Unable to code field in two longs");

//        var firstHalf = field[..(field.Length / 2)].Encode(coeffs);
//        var secondHalf = field[(field.Length / 2)..].Encode(coeffs);

//        return new Result
//            { IsSplitted = true, CodedField = firstHalf.CodedField.Concat(secondHalf.CodedField).ToArray() };
//    }
//}