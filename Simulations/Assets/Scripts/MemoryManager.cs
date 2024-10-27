using System;
using System.Runtime.InteropServices;

public class MemoryManager
{
    // "private" imports have a wrapper method to handle unconventional parameters
    [DllImport("EMSimDLL", EntryPoint = "Open", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Open();

    [DllImport("EMSimDLL", EntryPoint = "Close", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Close();

    [DllImport("EMSimDLL", EntryPoint = "Write", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private unsafe static extern bool Write(
        byte *inputBuf,
        int length);

    [DllImport("EMSimDLL", EntryPoint = "Read", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private unsafe static extern bool Read(
        byte *outputBuf,
        int bufSize,
        int offset = 0);

    [DllImport("EMSimDLL", EntryPoint = "Valid", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool Valid();

    [DllImport("EMSimDLL", EntryPoint = "BufferSize", CallingConvention = CallingConvention.Cdecl)]
    public static extern int BufferSize();

    // the following are the wrapper methods mentioned above
    public static bool Write(
        byte[] inputBuf)
    {
        bool success = false;

        unsafe {
            fixed (byte *bufPtr = inputBuf) {
                success = Write(bufPtr, inputBuf.Length);
            }
        }

        return success;
    }

    public static bool Read(
        out byte[] outputBuf,
        int bufSize,
        int offset = 0)
    {
        bool success = false;
        outputBuf = new byte[bufSize];

        unsafe {
            fixed (byte *bufPtr = outputBuf) {
                success = Read(bufPtr, bufSize, offset);
            }
        }

        return success;
    }
}
