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
using System.Diagnostics;
using MinecraftLaunch.Modules.Models.Install;

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
            var MCList = await GameCoreInstaller.GetGameCoresAsync();
            verBox.ItemsSource = MCList.Cores;
            opt_game_verBox.ItemsSource = MCList.Cores;
            fabric_game_verBox.ItemsSource = MCList.Cores;
            Forge_game_verBox.ItemsSource = MCList.Cores;

        }
        public GameCoreInstaller installer;
        public XiaZai()
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
                await Task.Run(async() =>
                {
                    var opt_installer = new OptiFineInstaller(v, OPT, javas[0], name);
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
                optInstallProgress.IsIndeterminate = false;
            }
        }


        private async void opt_game_verBox_DropDownClosed(object sender, EventArgs e)
        {
            if (opt_game_verBox.Text != "")
            {
                string selectedGameV = (opt_game_verBox.SelectedItem as MinecraftLaunch.Modules.Models.Install.GameCoreEmtity).Id;
                var OptList = await OptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(selectedGameV);
                opt_game_verBox_Copy.ItemsSource = OptList;
            }
        }

        private async void javaInstall_start_Click(object sender, RoutedEventArgs e)
        {
            if (java_verBox.Text != "")
            {
                javaInstallProgress.IsIndeterminate = true;
                var jv = (OpenJdkType)java_verBox.SelectedItem;
                JavaInstaller java = new(JdkDownloadSource.JdkJavaNet, jv, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                await Task.Run(async () =>
                {
                    var res = await java.InstallAsync((e) =>
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

        private async void fabricInstall_start_Click(object sender, RoutedEventArgs e)
        {
            fabricInstallProgress.IsIndeterminate = true;
            if (fabric_game_verBox.Text != "" && fabric_game_verBox_Copy.Text != "")
            {
                var FabricBuild = fabric_game_verBox_Copy.SelectedItem as FabricInstallBuild;
                var v = new GameCoreToolkit(gameFolder);
                var fi = new FabricInstaller(v,FabricBuild);
                await Task.Run(async() =>
                {
                    await fi.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            fabricDownLoadLog.AppendText($"[{DateTime.Now}]{e.Item2} 进度:{e.Item1.ToString("P")}\n");
                            fabricDownLoadLog.ScrollToEnd();
                        }));
                    });
                });
                fabricInstallProgress.IsIndeterminate = false;
                MessageBoxX.Show("下载完成！", "MCL启动器");
            }
            else
            {
                MessageBoxX.Show("尚有信息未输入！", "MCL启动器");
                fabricInstallProgress.IsIndeterminate = false;
            }
        }

        private async void fabric_game_verBox_DropDownClosed(object sender, EventArgs e)
        {
            string selectedGameV = (fabric_game_verBox.SelectedItem as MinecraftLaunch.Modules.Models.Install.GameCoreEmtity).Id;
            var FabricList = await FabricInstaller.GetFabricBuildsByVersionAsync(selectedGameV);
            fabric_game_verBox_Copy.ItemsSource = FabricList;
        }
        private async void Forge_game_verBox_DropDownClosed(object sender, EventArgs e)
        {
            if(Forge_game_verBox.Text != "")
            {
                string selectedGameV = (Forge_game_verBox.SelectedItem as MinecraftLaunch.Modules.Models.Install.GameCoreEmtity).Id;
                var ForgeList = await ForgeInstaller.GetForgeBuildsOfVersionAsync(selectedGameV);
                Forge_game_verBox_Copy.ItemsSource = ForgeList;
            }
            
        }

        private async void ForgeInstall_start_Click(object sender, RoutedEventArgs e)
        {
            if (Forge_game_verBox_Copy.Text != "" && Forge_game_verBox.Text != "")
            {
                ForgeInstallProgress.IsIndeterminate = true;
                string version = Forge_game_verBox.Text;
                string Forge_version = Forge_game_verBox_Copy.Text;
                var javaList = JavaToolkit.GetJavas();
                var v = new GameCoreToolkit(gameFolder);
                MinecraftLaunch.Modules.Models.Install.ForgeInstallEntity Forge = Forge_game_verBox_Copy.SelectedItem as MinecraftLaunch.Modules.Models.Install.ForgeInstallEntity;
                List<string> javas = new List<string>();
                foreach (var a in javaList)
                {
                    javas.Add(a.JavaPath);
                }
                await Task.Run(async () =>
                {
                    var Forge_installer = new ForgeInstaller(v, Forge, javas[0], name);
                    await Forge_installer.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            ForgeDownLoadLog.AppendText($"[{DateTime.Now}]{e.Item2} 进度:{e.Item1.ToString("P")}\n");
                            ForgeDownLoadLog.ScrollToEnd();
                        }));
                    });
                });


                ForgeInstallProgress.IsIndeterminate = false;
                MessageBoxX.Show("下载完成！", "MCL启动器");
                //
            }
            else
            {
                MessageBoxX.Show("尚有信息未输入！", "MCL启动器");
                ForgeInstallProgress.IsIndeterminate = false;
            }
        }

        private async void fabric_game_verBox_Copy_DropDownOpened(object sender, EventArgs e)
        {
            if(fabric_game_verBox.Text != "" && fabric_game_verBox_Copy.Text == "")
            {
                fabric_game_verBox_Copy.ItemsSource = await FabricInstaller.GetFabricBuildsByVersionAsync(fabric_game_verBox.Text);
            }
        }
    }
}
