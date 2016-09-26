using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Guid_Changer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool waitingforhotkey = false;
        string hotkey;
        public MainWindow()
        {
            InitializeComponent();
          
            Run();
        }
        void Run()
        {
            string filename = "hotkey.txt";
            if (!Directory.Exists("bin"))
                Directory.CreateDirectory("bin");

            if (!File.Exists(filename))
                File.WriteAllText(filename, "NumPad0");

            hotkey = File.ReadAllText(filename);
            sethotkey.Content = hotkey;
            Thread status = new Thread(WatchGameStatus);
            status.Start();
            Thread waitforkey = new Thread(WatchForHotkey);
            waitforkey.Start();
        }
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void sethotkey_Click(object sender, RoutedEventArgs e)
        {
            hotkeystatus.Content = "- Press Any Key";
            waitingforhotkey = true;
        }

        private void sethotkey_KeyDown(object sender, KeyEventArgs e)
        {
            if (!waitingforhotkey)
                return;

            sethotkey.Content = e.Key;
            hotkey = e.Key.ToString();
            File.WriteAllText("hotkey.txt", hotkey);
            hotkeystatus.Content = "- Click to Change";
            waitingforhotkey = false;
        }

        private void titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void exit_MouseEnter(object sender, MouseEventArgs e)
        {
            //BrushConverter b = new BrushConverter();
            ((Label)sender).Foreground = Brushes.Gold;
            ((Label)sender).FontSize += 1;
        }

        private void exit_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Label)sender).Foreground = Brushes.White;
            ((Label)sender).FontSize -= 1;
        }
        public void WatchGameStatus()
        {
            BrushConverter b = new BrushConverter();
            try
            {
                while (true)
                {
                    if (MemoryReader.isRunnung("iw3mp"))
                        Dispatcher.Invoke(new Action(() => {
                            gamestatuscolour.Fill = (Brush)b.ConvertFromString("#FF307893");
                            gamestatus.Content = "Connected";
                            guid.Content = "Guid: " + CdKey.CdkeyToGuid(CdKey.GetCod4Key());
                            guid.ToolTip = "Key: "+CdKey.GetCod4Key();
                        }));
                    else
                        Dispatcher.Invoke(new Action(() => { gamestatuscolour.Fill = Brushes.Transparent; gamestatus.Content = "Waiting for iw3mp.exe"; }));

                    Thread.Sleep(10);
                }
            }
            catch (Exception) { }
        }
        public void WatchForHotkey()
        {
            try
            {
                while (true)
                {
                    Key key;
                    Enum.TryParse(hotkey,out key);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (Keyboard.IsKeyDown(key))
                        {
                            KeyGenWatcher watcher = new KeyGenWatcher();
                            watcher.OnGeneratorKeyChanged += (args) => { CdKey.SetCod4Key(args.Newkey); Dispatcher.Invoke(new Action(() => { guid.Content = "Guid: " + CdKey.CdkeyToGuid(args.Newkey); })); };
                        }
                    }));
                    Thread.Sleep(100);
                }
            }
            catch(Exception){}
        }
    }
}
