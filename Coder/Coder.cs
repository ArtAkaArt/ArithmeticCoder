namespace ArithmeticCoder;

public class Coder
{
    public static int[] Decode(ulong codedField, params int[] coeffs)
    {
        var result = new int[32];
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

    public static Result Encode(int[] field, params int[] coeffs)
    {
        if (coeffs.Any(x => x <= 0))
            throw new ArgumentException("Some of coeffs is zero or negative");

        if (field.Max() > coeffs.Length - 1)
            throw new ArgumentException("Some figure outside of coeffs");

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

        return Enumerable.SequenceEqual(codedField.Decode(coeffs), field)
            ? new Result { IsCoded = true, CodedField = codedField }
            : new Result { IsCoded = false };
    }
}