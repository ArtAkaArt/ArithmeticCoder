using System.ComponentModel;

namespace ArithmeticCoder;

public class NoULong
{
    private const ulong lastByte = 18374686479671623680;
    private int pointer = 0;
    private List<byte> list = new(8);

    public byte[] Bytes => list.ToArray();

    public NoULong() { }

    public NoULong(byte[] initial)
    {
        list = initial.ToList();
    }

    public ulong ToLong()
    {
        var result = BitConverter.ToUInt64(Bytes[pointer..(pointer + 8)]);
        pointer++;
        return result;
    }

    public void AddByte(ulong current)
    {
        //list.Insert(pointer, (byte)((current & lastByte) >> 56));
        list.Insert(pointer, BitConverter.GetBytes(current)[0]);
        pointer++;
    }

    public void AddLong(ulong current)
    {
        var ads = BitConverter.GetBytes(current);

        list.AddRange(ads);
    }
}