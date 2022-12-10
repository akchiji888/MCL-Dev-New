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
using System.Net;
using System.IO;
using System.Text;
using Panuon.UI.Silver;

namespace MCL_Dev
{
    /// <summary>
    /// XiaZai.xaml 的交互逻辑
    /// </summary>
    public partial class XiaZai : Page
    {
        private async void GetMcVersionList()
        {
            var v = new GameCoreToolkit(gameFolder);
            await Task.Run(() =>
            {
                installer = new(v,"1.19.2");//其实这个1.19.2改成114514都行
            });
            var MCList = await installer.GetGameCoresAsync();
            verBox.ItemsSource = MCList.Cores;
        }
        public GameCoreInstaller installer;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public XiaZai()
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        {
            InitializeComponent();
            GetMcVersionList();
        }


        private async void start_Click(object sender, RoutedEventArgs e)
        {
            if(verBox.Text != null)
            {
                progress.IsIndeterminate = true;
                string version = verBox.Text;
                var v = new GameCoreToolkit(gameFolder);
                await Task.Run(() =>
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
            else
            {
                MessageBoxX.Show("下载版本未输入！", "MCL启动器");
            }
        }
    }
}
