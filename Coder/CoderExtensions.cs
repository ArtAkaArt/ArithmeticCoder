namespace ArithmeticCoder;

public static class CoderExtensions
{
    public static ulong Encode(this int[] field, int[] coeffs) => Coder.Encode(field, coeffs);

    public static int[] Decode(this ulong codedField, int[] coeffs) => Coder.Decode(codedField, coeffs);
}