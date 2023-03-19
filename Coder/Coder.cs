namespace ArithmeticCoder;

public class Coder
{
    public static int[] Decode(ulong codedField, int[] coeffs, int fieldLength)
    {
        var result = new int[fieldLength];
        var start = ulong.MinValue;
        var end = ulong.MaxValue;
        var coeffsTotal = (ulong)coeffs.Sum();

        for (int position = 0; position < result.Length; position++)
        {
            var currentLength = end - start;
            for (int figure = 0; figure < coeffs.Length; figure++)
            {
                if (currentLength < 10)
                {
                    start = start % 10 * 111111111;
                    end = end % 10 * 111111111;
                    codedField = codedField % 10 * 111111111;
                    currentLength = end - start;
                }

                var buffer = currentLength / coeffsTotal * (ulong)coeffs[..(figure + 1)].Sum() + start;
                if (codedField >= buffer)
                    continue;
                result[position] = figure;
                if (figure != coeffs.Length - 1)
                    end -= currentLength / coeffsTotal * (ulong)coeffs[(figure + 1)..].Sum();
                if (figure != 0)
                    start += currentLength / coeffsTotal * (ulong)coeffs[..figure].Sum();
                break;
            }
        }

        return result;
    }

    public static int[] Decode(ulong[] codedField, int[] coeffs, int fieldLength = 32)
    {
        var result = Array.Empty<int>();

        return codedField.Aggregate(result,
            (current, tst) => current.Concat(tst.Decode(coeffs, 32 / codedField.Length)).ToArray());
    }

    public static Result Encode(int[] field, int[] coeffs)
    {
        if (coeffs.Any(x => x <= 0))
            throw new ArgumentException("Some of coeffs is zero or negative");

        var start = ulong.MinValue;
        var end = ulong.MaxValue;
        var coeffsTotal = (ulong)coeffs.Sum();

        foreach (var figure in field)
        {
            var currentLength = end - start;
            if (figure != coeffs.Length - 1)
                end -= currentLength / coeffsTotal * (ulong)coeffs[(figure + 1)..].Sum();

            if (figure != 0)
                start += currentLength / coeffsTotal * (ulong)coeffs[..figure].Sum();
        }

        var codedField = start + 1;
        var isCoded = codedField.Decode(coeffs, field.Length).SequenceEqual(field);
        if (isCoded)
            return new Result { IsSplitted = false, CodedField = new[] { codedField } };

        if (!isCoded && field.Length != 32)
            throw new Exception("Attemption to split on more than two longs");

        var firstHalf = field[..(field.Length / 2)].Encode(coeffs);
        var secondHalf = field[(field.Length / 2)..].Encode(coeffs);

        return new Result
            { IsSplitted = true, CodedField = firstHalf.CodedField.Concat(secondHalf.CodedField).ToArray() };
    }
}