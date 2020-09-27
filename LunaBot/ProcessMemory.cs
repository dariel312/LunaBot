using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LunaBot
{
    public class ProcessMemory
    {
        private IntPtr _Handle;
        public IntPtr Handle { get { return _Handle; } set { _Handle = value; } }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        public ProcessMemory(Process Process)
        {
            Handle = Process.Handle;
        }

        #region Read Methods
        public Int32 ReadInt32(IntPtr Pointer)
        {
            byte[] buffer = new byte[4];
            IntPtr ptrBytesRead;
            ReadProcessMemory(_Handle, Pointer, buffer, 4, out ptrBytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }
        public UInt32 ReadUInt32(IntPtr Pointer)
        {
            byte[] buffer = new byte[4];
            IntPtr ptrBytesRead;
            ReadProcessMemory(_Handle, Pointer, buffer, 4, out ptrBytesRead);
            return BitConverter.ToUInt32(buffer, 0);
        }
        public Int64 ReadInt64(IntPtr Pointer)
        {
            byte[] buffer = new byte[8];
            IntPtr ptrBytesRead;
            ReadProcessMemory(_Handle, Pointer, buffer, 8, out ptrBytesRead);
            return BitConverter.ToInt64(buffer, 0);
        }
        public UInt64 ReadUInt64(IntPtr Pointer)
        {
            byte[] buffer = new byte[8];
            IntPtr ptrBytesRead;
            ReadProcessMemory(_Handle, Pointer, buffer, 8, out ptrBytesRead);
            return BitConverter.ToUInt64(buffer, 0);
        }
        public float ReadFloat(IntPtr Pointer)
        {
            byte[] buffer = new byte[4];
            IntPtr ptrBytesRead;
            ReadProcessMemory(_Handle, Pointer, buffer, 4, out ptrBytesRead);
            return BitConverter.ToSingle(buffer, 0);
        }
        public Double ReadDouble(IntPtr Pointer)
        {
            byte[] buffer = new byte[8];
            IntPtr ptrBytesRead;
            ReadProcessMemory(_Handle, Pointer, buffer, 8, out ptrBytesRead);
            return BitConverter.ToDouble(buffer, 0);
        }
        public string ReadString(IntPtr Pointer, uint Length)
        {
            byte[] buffer = new byte[Length];
            IntPtr ptrBytesRead;
            ReadProcessMemory(_Handle, Pointer, buffer, Length, out ptrBytesRead);
            return Encoding.ASCII.GetString(buffer);

        }
        public byte ReadByte(IntPtr Pointer)
        {
            byte[] buffer = new byte[1];
            IntPtr ptrBytesRead;
            ReadProcessMemory(_Handle, Pointer, buffer, 8, out ptrBytesRead);
            return buffer[0];
        }
        #endregion

        #region Write Methods
        public  void WriteInt32(int Pointer, Int32 Value)
        {
            int bytesWritten = 0;
            byte[] write = BitConverter.GetBytes(Convert.ToInt32(Value));
            ProcessMemory.WriteProcessMemory((int)Handle, Pointer, write, write.Length, ref bytesWritten);
        }
        public  void WriteUInt32(int Pointer, UInt32 Value)
        {
            int bytesWritten = 0;
            byte[] write = BitConverter.GetBytes(Convert.ToUInt32(Value));
            ProcessMemory.WriteProcessMemory((int)Handle, Pointer, write, write.Length, ref bytesWritten);
        }
        public  void WriteInt64(int Pointer, Int64 Value)
        {
            int bytesWritten = 0;
            byte[] write = BitConverter.GetBytes(Convert.ToInt64(Value));
            ProcessMemory.WriteProcessMemory((int)Handle, Pointer, write, write.Length, ref bytesWritten);
        }
        public  void WriteUInt64(int Pointer, UInt64 Value)
        {
            int bytesWritten = 0;
            byte[] write = BitConverter.GetBytes(Convert.ToUInt64(Value));
            ProcessMemory.WriteProcessMemory((int)Handle, Pointer, write, write.Length, ref bytesWritten);
        }
        public  void WriteFloat(int Pointer, Single Value)
        {
            int bytesWritten = 0;
            byte[] write = BitConverter.GetBytes(Convert.ToSingle(Value));
            ProcessMemory.WriteProcessMemory((int)Handle, Pointer, write, write.Length, ref bytesWritten);
        }
        public  void WriteDouble(int Pointer, Double Value)
        {
            int bytesWritten = 0;
            byte[] write = BitConverter.GetBytes(Convert.ToDouble(Value));
            ProcessMemory.WriteProcessMemory((int)Handle, Pointer, write, write.Length, ref bytesWritten);
        }
        #endregion

    }
}
