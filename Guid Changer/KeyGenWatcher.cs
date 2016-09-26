using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace Guid_Changer
{
    public class OnGeneratorKeyChangedEventArgs : EventArgs
    {
        public string Newkey { get; set; }
    }
    class KeyGenWatcher
    {

        public delegate void KeyChanged(OnGeneratorKeyChangedEventArgs e);
        public event KeyChanged OnGeneratorKeyChanged;

        private string ProcessName;
        private string filename = "bin\\rzr-cod4.exe";

        public KeyGenWatcher()
        {
            if (!File.Exists(filename))
                File.WriteAllBytes(filename, Properties.Resources.rzr_cod4);

            ProcessName = Path.GetFileNameWithoutExtension(filename);
            Process generator = new Process();
            generator.StartInfo.FileName = filename;
            generator.StartInfo.UseShellExecute = true;
            generator.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            generator.StartInfo.CreateNoWindow = true;
            generator.Start();

            //generator.WaitForExit();
            Thread watch = new Thread(WatchForChange);
            watch.Start();
        }
        private void WatchForChange()
        {
            if (!MemoryReader.isRunnung(ProcessName))
                return;
            Process process = Process.GetProcessesByName(ProcessName)[0];
            string newkey = string.Empty;
            while (!process.HasExited)
            {
                newkey = MemoryReader.ReadAddress(ProcessName, Addresses.KeyGenCdKey, 24);
                if (newkey.Contains('\0'))
                    continue;

                OnGeneratorKeyChanged(new OnGeneratorKeyChangedEventArgs { Newkey = newkey.Replace(" ","") });
                break;
            }
            process.Kill();
        }
    }
}
