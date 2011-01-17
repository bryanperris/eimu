package eimu.core;

public class Tools {

    public static short MakeShort(byte a, byte b)
    {
        return (short)((short)a << 8 | b);
    }
}
