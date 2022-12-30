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
using System.Threading;
using MinecraftLaunch.Modules.Enum;

namespace MCL_Dev
{
    /// <summary>
    /// XiaZai.xaml 的交互逻辑
    /// </summary>
    public partial class XiaZai : Page
    {
        int DoNotNeedToGet = 0;
        private async void GetMcVersionList()
        {
            var v = new GameCoreToolkit(gameFolder);
            var MCList = await GameCoreInstaller.GetGameCoresAsync();
            verBox.ItemsSource = MCList.Cores;
            opt_game_verBox.ItemsSource = MCList.Cores;
        }
        public GameCoreInstaller installer;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public XiaZai()
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        {
            InitializeComponent();
            GetMcVersionList();
            java_verBox.Items.Add(OpenJdkType.OpenJdk8);
            java_verBox.Items.Add(OpenJdkType.OpenJdk11);
            java_verBox.Items.Add(OpenJdkType.OpenJdk17);
            java_verBox.Items.Add(OpenJdkType.OpenJdk18);
        }       
        private async void start_Click(object sender, RoutedEventArgs e)
        {
            if (verBox.Text != "")
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
                MessageBoxX.Show("下载完成！", "MCL启动器");
            }
            else
            {
                MessageBoxX.Show("下载版本未输入！", "MCL启动器");
                progress.IsIndeterminate = false;
            }
        }

        private void opt_game_verBox_Copy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private async void optInstall_start_Click(object sender, RoutedEventArgs e)
        {
            if (opt_game_verBox_Copy.Text != "" && opt_game_verBox.Text != "")
            {
                optInstallProgress.IsIndeterminate = true;
                string version = opt_game_verBox.Text;
                string opt_version = opt_game_verBox_Copy.Text;
                var javaList = JavaToolkit.GetJavas();
                var v = new GameCoreToolkit(gameFolder);
                MinecraftLaunch.Modules.Models.Install.OptiFineInstallEntity OPT = opt_game_verBox_Copy.SelectedItem as MinecraftLaunch.Modules.Models.Install.OptiFineInstallEntity;
                List<string> javas = new List<string>();
                foreach (var a in javaList)
                {
                    javas.Add(a.JavaPath);
                }
                MinecraftLaunch.Modules.Installer.OptiFineInstaller opt_installer = new();
                await Task.Run(() =>
                {
                    opt_installer = new OptiFineInstaller(v, OPT, javas[0], name);
                });
                await Task.Run(async () =>
                {
                    await opt_installer.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            optDownLoadLog.AppendText($"[{DateTime.Now}]{e.Item2} 进度:{e.Item1.ToString("P")}\n");
                            optDownLoadLog.ScrollToEnd();
                        }));
                    });
                });

                optInstallProgress.IsIndeterminate = false;
                MessageBoxX.Show("下载完成！", "MCL启动器");
                //
            }
            else
            {
                MessageBoxX.Show("尚有信息未输入！", "MCL启动器");
                progress.IsIndeterminate = false;
            }
        }


        private async void opt_game_verBox_DropDownClosed(object sender, EventArgs e)
        {
            if (opt_game_verBox.Text != "")
            {
                DoNotNeedToGet = 1;
                string selectedGameV = (opt_game_verBox.SelectedItem as MinecraftLaunch.Modules.Models.Install.GameCoreEmtity).Id;
                var OptList = await OptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(selectedGameV);
                opt_game_verBox_Copy.ItemsSource = OptList;
            }
        }

        private async void javaInstall_start_Click(object sender, RoutedEventArgs e)
        {
            if(java_verBox.Text != "")
            {
                var jv = (OpenJdkType)java_verBox.SelectedItem;
                JavaInstaller java = new(JdkDownloadSource.JdkJavaNet,jv, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                await Task.Run(async () =>
                {
                    await java.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            javaDownLoadLog.AppendText($"[{DateTime.Now}]{e.Item2} 进度:{e.Item1.ToString("P")}\n");
                            javaDownLoadLog.ScrollToEnd();
                        }));
                    });
                });
                MessageBoxX.Show("安装完成！", "MCL启动器");
            }
        }
    }
}
