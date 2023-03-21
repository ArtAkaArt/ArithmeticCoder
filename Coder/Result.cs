namespace ArithmeticCoder;

//public record Result
//{
//    public bool IsSplitted { get; set; }
//    public ulong[] CodedField { get; set; }
//}

public record Result
{
    public Result(byte[] encoded, int meaningfulBits)
    {
        MeaningfulBits = meaningfulBits;
        Encoded = encoded;
    }

    public int MeaningfulBits { get; set; }
    public byte[] Encoded { get; set; }
}