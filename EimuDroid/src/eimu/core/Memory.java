package eimu.core;

public class Memory {
	
	private byte[] m_Memory;
	
	
	public Memory(int size)
	{
		this.m_Memory = new byte[size];
	}
	
    public byte GetByte(int address)
    {
        return m_Memory[address];
    }
    
    public void SetByte(int address, byte value)
    {
        m_Memory[address] = value;
    }

    public void SetBytes(byte[] buffer, int offset, int size)
    {
        for (int i = 0; i < size; i++)
        {
            m_Memory[i + offset] = buffer[i];
        }
    }

    public void GetBytes(byte[] buffer, int offset, int size)
    {
        for (int i = 0; i < size; i++)
        {
            buffer[i] = m_Memory[i + offset];
        }
    }

    public int getSize()
    {
        return m_Memory.length;
    }
}
