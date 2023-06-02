using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ExternalDisplaySwitching
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                // OS名とビルド番号をレジストリから取得
                RegistryKey rKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                var product = rKey.GetValue("ProductName").ToString();
                rKey.Close();

                rKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                OSBuild = int.Parse(rKey.GetValue("CurrentBuild").ToString());
                rKey.Close();

                TB_OSVersion.Text = string.Format("{0}, Build:{1}", product, OSBuild);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Failure to open registry");
            }
        }

        private int OSBuild = 0;

        private void ProcessStart(string para)
        {
            ProcessStartInfo processStartInfo;
            string parameter = " /" + para;
            
            if (OSBuild >= 22621) // upper than or equal to Windows11 22H2 ?
            {
                // Windows11 22H2以降は DisplaySwitch.exe が引数を受け付けなくなっている
                // そこで、22H1以前の古いバイナリを同じフォルダに置いてそれを参照することにする
                if (File.Exists("DisplaySwitch.exe"))
                {
                    processStartInfo = new ProcessStartInfo(@"DisplaySwitch.exe", parameter);
                }
                else
                {
                    MessageBox.Show("DisplaySwitch.exe: Not Found. Abort...");
                    return;
                }
            }
            else if (!Environment.Is64BitProcess)
            {
                processStartInfo = new ProcessStartInfo(@"C:\Windows\sysnative\DisplaySwitch.exe", parameter);
            }
            else
            {
                processStartInfo = new ProcessStartInfo(@"C:\Windows\system32\DisplaySwitch.exe", parameter);
            }

            // コマンド実行
            var process = Process.Start(processStartInfo);

            process.WaitForExit();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProcessStart("extend");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ProcessStart("clone");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ProcessStart("internal");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ProcessStart("external");
        }
    }
}
