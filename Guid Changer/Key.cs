using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Guid_Changer
{
    static class CdKey
    {
        public static string GetCod4Key(string processname = "")
        {
            processname = processname == "" ? "iw3mp" : processname;
            return MemoryReader.ReadAddress(processname, Addresses.GameCdKeypart1, 16) + MemoryReader.ReadAddress(processname, Addresses.GameCdKeypart2, 4);
        }

        public static void SetCod4Key(string cdkey,string processname = "")
        {
                processname = processname == "" ? "iw3mp" : processname;
                MemoryReader.WriteAddress(processname, Addresses.GameCdKeypart1, 16, cdkey.Substring(0, 16));
                MemoryReader.WriteAddress(processname, Addresses.GameCdKeypart2, 4, cdkey.Substring(16));
        }

        public static string CdkeyToGuid(string key)
        {
                string filename = "bin\\keytoguid.exe";
                if (!File.Exists(filename))
                    File.WriteAllBytes(filename, Properties.Resources.keytoguid);

                Process converter = new Process();
                converter.StartInfo.FileName = filename;
                converter.StartInfo.Arguments = key.Substring(0, 16);
                converter.StartInfo.UseShellExecute = false;
                converter.StartInfo.RedirectStandardOutput = true;
                converter.StartInfo.CreateNoWindow = true;
                converter.Start();

                string guid = converter.StandardOutput.ReadToEnd();

                converter.WaitForExit();

                return guid;
        }
    }
}
