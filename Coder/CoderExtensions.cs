namespace ArithmeticCoder;

public static class UlongCoderExtensions
{
    public static Result Encode(this int[] field, int[] coeffs) => UlongCoder.Encode(field, coeffs);

    public static int[] Decode(this ulong[] codedField, int[] coeffs) =>
        UlongCoder.Decode(codedField, coeffs);

    public static int[] Decode(this ulong codedField, int[] coeffs, int fieldLength = 32) =>
        UlongCoder.Decode(codedField, coeffs, fieldLength);

    public static int[] Decode(this Result codedField, int[] coeffs) => UlongCoder.Decode(codedField.CodedField, coeffs);
}

public static class ByteCoderExtensions
{
    public static byte[] ByteEncode(this int[] field, int[] coeffs) => ByteCoder.Encode(field, coeffs);

    public static int[] ByteDecode(this byte[] codedField, int[] coeffs) =>
        ByteCoder.Decode(codedField, coeffs);

    //public static int[] Decode(this Result codedField, int[] coeffs) => ByteCoder.Decode(codedField.Encoded, coeffs);
}

