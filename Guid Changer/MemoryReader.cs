using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Guid_Changer
{
    static class MemoryReader
    {
        const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess,
          int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress,
          byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        public static bool isRunnung(string Processname)
        {
            return Process.GetProcessesByName(Processname).Length != 0;

        }
        public static string ReadAddress(string ProcessName, int address, int length)
        {
            if (!isRunnung(ProcessName))
                return null;

            Process process = Process.GetProcessesByName(ProcessName)[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);

            int bytesRead = 0;
            byte[] buffer = new byte[length];
            ReadProcessMemory((int)processHandle, address, buffer, buffer.Length, ref bytesRead);
            return Encoding.Default.GetString(buffer, 0, bytesRead);
        }
        public static void WriteAddress(string ProcessName, int address, int length,string data)
        {
            if (!isRunnung(ProcessName))
                return;

            Process process = Process.GetProcessesByName(ProcessName)[0];
            IntPtr processHandle = OpenProcess(0x1F0FFF, false, process.Id);

            int bytesWritten = 0;
            byte[] buffer = Encoding.Default.GetBytes(data);
            WriteProcessMemory((int)processHandle, address, buffer, buffer.Length, ref bytesWritten);
        }

    }

    static class Addresses
    {
        public static int KeyGenCdKey { get { return 0x00403230; } }
        public static int GameCdKeypart1 { get { return 0x00724B84; } }
        public static int GameCdKeypart2 { get { return 0x00724BA8; } }
    }
}
