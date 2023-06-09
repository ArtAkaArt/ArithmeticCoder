﻿namespace ArithmeticCoder;

public static class CoderExtensions
{
    public static Result Encode(this int[] field, int[] coeffs) => Coder.Encode(field, coeffs);

    public static int[] Decode(this ulong codedField, int[] coeffs) => Coder.Decode(codedField, coeffs);

    public static int[] Decode(this Result codedField, int[] coeffs) => Coder.Decode(codedField.CodedField, coeffs);
}