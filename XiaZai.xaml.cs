using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using static MCL_Dev.LauncherClasses;
using static MCL_Dev.ZhuYe;
using System.Collections;

namespace MCL_Dev
{
    /// <summary>
    /// XiaZai.xaml 的交互逻辑
    /// </summary>
    public partial class XiaZai : Page
    {
        public GameCoreInstaller installer;        
        public XiaZai()
        {
            InitializeComponent();
        }
        

        private async void start_Click(object sender, RoutedEventArgs e)
        {
            
            progress.IsIndeterminate = true;
            string version = verBox.Text;
            var v = new GameCoreToolkit(gameFolder);
            await Task.Run(async() =>
            {
                installer = new(v, version);
            });
            await installer.InstallAsync((e) =>
            {
                downloadLog.AppendText($"[{DateTime.Now}]{e.Item2} 进度:{e.Item1.ToString("P")}\n");
                downloadLog.ScrollToEnd();
            });
            progress.IsIndeterminate = false;
        }
    }
}
