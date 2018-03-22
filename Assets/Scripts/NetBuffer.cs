using System;

//Utility class to read and write from a byte array
public class NetBuffer
{
    private byte[] data;
    private int position;

    public byte[] Data
    {
        get { return data; }
        set { data = value; }
    }

    public int Position
    {
        get { return position; }
        set { position = value; }
    }

    public void Skip(int numBytes)
    {
        position += numBytes;
    }

    #region Read
    public byte ReadByte()
    {
        byte retval = data[position++];
        return retval;
    }

    public sbyte ReadSByte()
    {
        byte retval = ReadByte();
        return (sbyte)retval;
    }

    public ushort ReadUInt16()
    {
        ushort retval = data[position++];
        retval |= (ushort)(data[position++] << 8);
        return retval;
    }

    public short ReadInt16()
    {
        ushort retval = ReadUInt16();
        return (short)retval;
    }

    public uint ReadUInt32()
    {
        uint retval = data[position++];
        retval |= (uint)(data[position++] << 8);
        retval |= (uint)(data[position++] << 16);
        retval |= (uint)(data[position++] << 24);
        return retval;
    }

    public int ReadInt32()
    {
        uint retval = ReadUInt32();
        return (int)retval;
    }
    #endregion

    #region Write
    public void Write(byte source)
    {
        data[position++] = source;
    }

    public void Write(sbyte source)
    {
        Write((byte)source);
    }

    public void Write(ushort source)
    {
        data[position++] = (byte)source;
        data[position++] = (byte)(source >> 8);
    }

    public void Write(short source)
    {
        Write((ushort)source);
    }

    public void Write(uint source)
    {
        data[position++] = (byte)source;
        data[position++] = (byte)(source >> 8);
        data[position++] = (byte)(source >> 16);
        data[position++] = (byte)(source >> 24);
    }

    public void Write(int source)
    {
        Write((uint)source);
    }

    public void Write(byte[] source)
    {
        int sourceLength = source.Length;
        Buffer.BlockCopy(source, 0, data, position, sourceLength);
        position += sourceLength;
    }
    #endregion
}