using Microsoft.WindowsAPICodePack.Dialogs;
using MinecaftOAuth;
using MinecraftLaunch.Launch;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using Panuon.WPF.UI;
using Panuon.WPF.UI.Configurations;
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
using static MCL_Dev.MCL_MainSetting;
using static MCL_Dev.LauncherClasses;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Windows.Documents;
using System.Net.WebSockets;

namespace MCL_Dev
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowX
    {
        bool IsRamSliderInited = false;
        bool VersionComboNeedToBeUpdated = false;
        CurseForgeToolkit ModToolkit_Forge = new("前面的区域，以后再来探索吧");
        #region 公共变量与void声明
        List<string> MCReleaseList;
        List<string> MCSnapshotList;
        List<string> MCOldList;
        private async void GetMcVersionList()
        {
            try
            {
                if (IsMCListInitialize == false)
                {
                    var ModToolkit = ModToolkit_Forge;
                    ModSpin.IsSpinning = true;
                    ModLoadingText.Visibility = Visibility.Visible;
                    ModSpin_Mod.IsSpinning = true;
                    ModLoadingText_Mod.Visibility = Visibility.Visible;
                    var v = new GameCoreToolkit(gameFolder);
                    var MCList = await GameCoreInstaller.GetGameCoresAsync();
                    MCReleaseList = MCList.Cores.Where(x => x.Type == "release").Select(x => x.Id).ToList();
                    MCSnapshotList = MCList.Cores.Where(x => x.Type == "snapshot").Select(x => x.Id).ToList();
                    MCOldList = MCList.Cores.Where(x => x.Type == "old_alpha" || x.Type == "old_beta").Select(x => x.Id).ToList();
                    verBox.ItemsSource = MCReleaseList;
                    opt_game_verBox.ItemsSource = MCReleaseList;
                    fabric_game_verBox.ItemsSource = MCReleaseList;
                    Forge_game_verBox.ItemsSource = MCReleaseList;
                    Quilt_game_verBox.ItemsSource = MCReleaseList;
                    List<OpenJdkType> jdkTypes = new()
                    {
                        OpenJdkType.OpenJdk8,OpenJdkType.OpenJdk11,OpenJdkType.OpenJdk17,OpenJdkType.OpenJdk18
                    };
                    java_verBox.ItemsSource = jdkTypes;
                    var res = await ModToolkit.GetFeaturedModpacksAsync();
                    var modrinthRes = await ModrinthToolkit.GetFeaturedModpacksAsync();
                    IsMCListInitialize = true;
                    List<Mod> mods = new();
                    List<ModrinthMod> modrinthMods = new();
                    res.ForEach(x =>
                    {
                        Mod modItem = new();
                        modItem.Description = x.Description;
                        modItem.image = new(new System.Uri(x.IconUrl));
                        modItem.Name = x.Name;
                        modItem.Version = $"{x.SupportedVersions.Last()}-{x.SupportedVersions[0]}";
                        modItem.Files = x.Files;
                        mods.Add(modItem);
                    });
                    modrinthRes.Hits.ForEach(async x =>
                    {
                        var res = await ModrinthToolkit.GetProjectInfos(x.ProjectId);
                        ModrinthMod modItem = new();
                        modItem.Files = new();
                        modItem.Description = x.Description;
                        modItem.image = new(new Uri(x.IconUrl));
                        modItem.Name = x.Title;
                        modItem.Version = $"{x.Versions.Last()}-{x.Versions[0]}";
                        res.ForEach(x =>
                        {
                            modItem.Versions = x.GameVersion;
                            modItem.Loaders = x.Loaders;
                            x.Files.ForEach(x =>
                            {
                                modItem.Files.Add(x);
                            });
                        });
                        modrinthMods.Add(modItem);
                    });
                    ModGrid.ItemsSource = mods;
                    ModSpin.IsSpinning = false;
                    ModLoadingText.Visibility = Visibility.Hidden;
                    ModGrid_Mod.ItemsSource = modrinthMods;
                    ModSpin_Mod.IsSpinning = false;
                    ModLoadingText_Mod.Visibility = Visibility.Hidden;
                }
            }
            catch
            {
                IsMCListInitialize = false;
                MessageBox.Show("无法获取下载信息！请检查电脑是否连接了互联网！", "MCL启动器", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        bool IsMCListInitialize = false;
        #endregion        
        private GameCoreInstaller installer;
        #region 全局变量声明
        private string modName;
        string APIKey_2018k;
        private string appFolder = AppDomain.CurrentDomain.BaseDirectory;
        private static string gameFolder = System.AppDomain.CurrentDomain.BaseDirectory + ".minecraft";
        public static int mode = 114514;//homo特有的变量（喜）
        #endregion
        public MainWindow()
        {
            InitializeComponent();
        }
        #region 启动游戏(void)
        private async void startGame()
        {
            progressBar.Value = 0;
            if (AccountCombo.Text != "" && versionCombo.Text != "")
            {
                var stop = false;
                var account = AccountCombo.SelectedItem as Account;
                switch (account.Type)
                {
                    case AccountType.Microsoft:
                        progressBar.Value = 0.1f;
                        #region 登录
                        var MSaccount = account as MicrosoftAccount;
                        var auth = new MicrosoftAuthenticator();
                        auth.ClientId = "dd09ec86-031b-4429-adb8-7fec6bc1fd79";
                        auth.AuthType = MinecaftOAuth.Module.Enum.AuthType.Refresh;
                        auth.RefreshToken = MSaccount.RefreshToken;
                        try
                        {
                            var result = await auth.AuthAsync(x => { });
                            var NeedToChange = gameAccountsList.Where(x => x == account).FirstOrDefault();
                            NeedToChange = result;
                            account = result;
                        }
                        catch
                        {
                            mode = 114514;
                            MessageBoxX.Show(this, "微软登录失败！\n错误原因可能有:1.微软的验证服务器炸了\n2.你没联网", "MCL启动器", MessageBoxIcon.Error);
                            stop = true;
                        }
                        #endregion
                        break;
                }
                if (javaCombo.SelectedIndex != 0 && stop == false)
                {
                    var lc = new LaunchConfig()
                    {
                        Account = AccountCombo.SelectedItem as Account,
                        GameWindowConfig = new GameWindowConfig()
                        {
                            Width = 854,
                            Height = 480,
                            IsFullscreen = false
                        },
                        JvmConfig = new JvmConfig(javaCombo.Text)
                        {
                            MaxMemory = Convert.ToInt32(maxMem.Value),
                        },
                        NativesFolder = null,//一般可以无视这个选项
                        WorkingFolder = new("q"),
                        LauncherName = "ModernCraftLauncher",

                    };
                    JavaClientLauncher clientLauncher = new(lc, new(gameFolder), true);
                    launchLog.Text = "";
                    using var res = await clientLauncher.LaunchTaskAsync(versionCombo.Text, x =>
                    {
                        launchLog.AppendText($"[{DateTime.Now}]{x.Item2} 进度:{x.Item1.ToString("P")}\n");
                        launchLog.ScrollToEnd();
                        progressBar.Value = x.Item1;
                    });
                    if (res.State is LaunchState.Succeess)
                    {
                        //启动成功的情况下会执行的代码块
                        launchLog.AppendText($"[{DateTime.Now}]启动成功");
                        progressBar.Value = 1;
                    }
                    else
                    {
                        //启动失败的情况下会执行的代码块
                        launchLog.AppendText("启动失败\n");
                        launchLog.AppendText("详细异常信息：" + res.Exception);
                        progressBar.Value = 0;
                    }
                }
                else if (stop == true)
                {
                    NoticeBox.Show("已停止启动", "MCL启动器", MessageBoxIcon.Info);
                }
                else
                {
                    var javaList = JavaToolkit.GetJavas();
                    var gCore = GameCoreToolkit.GetGameCore(gameFolder, versionCombo.Text);
                    var gToolkit = new GameCoreToolkit(gameFolder);
                    var java = JavaToolkit.GetCorrectOfGameJava(javaList, gCore);
                    if (java != null)
                    {
                        var lc = new LaunchConfig()
                        {
                            Account = AccountCombo.SelectedItem as Account,
                            GameWindowConfig = new GameWindowConfig()
                            {
                                Width = 854,
                                Height = 480,
                                IsFullscreen = false
                            },
                            JvmConfig = new JvmConfig(java.JavaPath)
                            {
                                MaxMemory = Convert.ToInt32(maxMem.Value),
                            },
                            WorkingFolder = new("q"),
                            NativesFolder = null,//一般可以无视这个选项
                            LauncherName = "ModernCraftLauncher"
                        };
                        JavaClientLauncher clientLauncher = new(lc, gToolkit, true);
                        launchLog.Text = "";
                        using var res = await clientLauncher.LaunchTaskAsync(versionCombo.Text, x =>
                        {
                            launchLog.AppendText($"[{DateTime.Now}]{x.Item2} 进度:{x.Item1.ToString("P")}\n");
                            launchLog.ScrollToEnd();
                            progressBar.Value = x.Item1;
                        });
                        if (res.State is LaunchState.Succeess)
                        {
                            //启动成功的情况下会执行的代码块
                            launchLog.AppendText($"[{DateTime.Now}]启动成功");
                            progressBar.Value = 1;
                        }
                        else
                        {
                            //启动失败的情况下会执行的代码块
                            launchLog.AppendText("启动失败");
                            launchLog.AppendText("详细异常信息：" + res.Exception);
                            progressBar.Value = 0;
                        }
                    }
                    else
                    {
                        MessageBoxX.Show(this, "启动失败！\n错误原因：未能找到合适的Java", "MCL启动器", MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBoxX.Show(this, "无法启动游戏！\n错误原因：参数缺失\n请检查Java、最大内存、账号选择等选项是否为空\nJava、最大内存可前往[设置-游戏设置]中进行设置\n若无可用账号请去[设置-账户管理]中设置", "MCL启动器", MessageBoxIcon.Error);
            }
        }
        #endregion
        #region 启动游戏（按钮事件）
        private void start_Click(object sender, RoutedEventArgs e)
        {
            startGame();
        }
        #endregion
        #region 下载事件
        private async void VanilaStart_Click(object sender, RoutedEventArgs e)
        {
            progress.Value = 0;
            if (verBox.Text != "")
            {
                string version = verBox.Text;
                var v = new GameCoreToolkit(gameFolder);
                await Task.Run(() =>
                {
                    installer = new(v, version);
                });
                downloadLog.Text = "";
                await Task.Run(async () =>
                {
                    await installer.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            downloadLog.AppendText($"[{DateTime.Now}]{e.Item2}\n");
                            downloadLog.ScrollToEnd();
                            progress.Value = e.Item1;
                        }));
                    });
                });

                var handler = NoticeBox.Show("下载完成！", "MCL启动器", MessageBoxIcon.Success, true);
                VersionComboNeedToBeUpdated = true;
                await Task.Run(() =>
                {
                    Thread.Sleep(1500);
                });
                NoticeBox.DestroyInstance();
            }
            else
            {
                MessageBoxX.Show(this, "下载版本未输入！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }
        private async void optInstall_start_Click(object sender, RoutedEventArgs e)
        {
            optInstallProgress.Value = 0;
            if (opt_game_verBox_Copy.Text != "" && opt_game_verBox.Text != "")
            {
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
                optDownLoadLog.Text = "";
                await Task.Run(async () =>
                {
                    var opt_installer = new OptiFineInstaller(v, OPT, javas[0]);
                    await opt_installer.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            optDownLoadLog.AppendText($"[{DateTime.Now}]{e.Item2}\n");
                            optDownLoadLog.ScrollToEnd();
                            optInstallProgress.Value = e.Item1;
                        }));
                    });
                });
                VersionComboNeedToBeUpdated = true;
                var handler = NoticeBox.Show("下载完成！", "MCL启动器", MessageBoxIcon.Success, true);
                await Task.Run(() =>
                {
                    Thread.Sleep(1500);
                });
                NoticeBox.DestroyInstance();
                //
            }
            else
            {
                MessageBoxX.Show(this, "尚有信息未输入！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
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
            javaInstallProgress.Value = 0;
            if (java_verBox.Text != "")
            {
                var jv = (OpenJdkType)java_verBox.SelectedItem;
                javaDownLoadLog.Text = "";
                JavaInstaller java = new(JdkDownloadSource.JdkJavaNet, jv, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                await Task.Run(async () =>
                {
                    var res = await java.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            javaDownLoadLog.AppendText($"[{DateTime.Now}]{e.Item2}\n");
                            javaDownLoadLog.ScrollToEnd();
                            javaInstallProgress.Value = e.Item1;
                        }));
                    });
                });
                VersionComboNeedToBeUpdated = true;
                var handler = NoticeBox.Show("安装完成！", "MCL启动器", MessageBoxIcon.Success, true);
                await Task.Run(() =>
                {
                    Thread.Sleep(1500);
                });
                NoticeBox.DestroyInstance();
            }
            else
            {
                MessageBoxX.Show(this, "尚有信息未输入！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }
        private async void fabricInstall_start_Click(object sender, RoutedEventArgs e)
        {
            fabricInstallProgress.Value = 0;
            if (fabric_game_verBox.Text != "" && fabric_game_verBox_Copy.Text != "")
            {
                fabricDownLoadLog.Text = "";
                var FabricBuild = fabric_game_verBox_Copy.SelectedItem as FabricInstallBuild;
                var v = new GameCoreToolkit(gameFolder);
                var fi = new FabricInstaller(v, FabricBuild);
                await Task.Run(async () =>
                {
                    await fi.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            fabricDownLoadLog.AppendText($"[{DateTime.Now}]{e.Item2}\n");
                            fabricDownLoadLog.ScrollToEnd();
                            fabricInstallProgress.Value = e.Item1;
                        }));
                    });
                    VersionComboNeedToBeUpdated = true;
                    var handler = NoticeBox.Show("下载完成！\nFabric API请至[Mod下载]页面中搜索\"Fabric API\"进行获取", "MCL启动器", MessageBoxIcon.Success, true);
                    await Task.Run(() =>
                    {
                        Thread.Sleep(1500);
                    });
                    NoticeBox.DestroyInstance();
                });
            }
            else
            {
                MessageBoxX.Show(this, "尚有信息未输入！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }
        private async void fabric_game_verBox_DropDownClosed(object sender, EventArgs e)
        {
            if (fabric_game_verBox.Text != "")
            {
                string selectedGameV = (fabric_game_verBox.SelectedItem as MinecraftLaunch.Modules.Models.Install.GameCoreEmtity).Id;
                var FabricList = await FabricInstaller.GetFabricBuildsByVersionAsync(selectedGameV);
                fabric_game_verBox_Copy.ItemsSource = FabricList;
            }
        }
        private async void Forge_game_verBox_DropDownClosed(object sender, EventArgs e)
        {
            if (Forge_game_verBox.Text != "")
            {
                string selectedGameV = (Forge_game_verBox.SelectedItem as MinecraftLaunch.Modules.Models.Install.GameCoreEmtity).Id;
                var ForgeList = await ForgeInstaller.GetForgeBuildsOfVersionAsync(selectedGameV);
                Forge_game_verBox_Copy.ItemsSource = ForgeList;
            }

        }
        private async void ForgeInstall_start_Click(object sender, RoutedEventArgs e)
        {
            ForgeInstallProgress.Value = 0;
            if (Forge_game_verBox_Copy.Text != "" && Forge_game_verBox.Text != "")
            {
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
                ForgeDownLoadLog.Text = "";
                await Task.Run(async () =>
                {
                    var Forge_installer = new ForgeInstaller(v, Forge, javas[0]);
                    await Forge_installer.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            ForgeDownLoadLog.AppendText($"[{DateTime.Now}]{e.Item2}\n");
                            ForgeDownLoadLog.ScrollToEnd();
                            ForgeInstallProgress.Value = e.Item1;
                        }));
                    });
                });
                VersionComboNeedToBeUpdated = true;
                var handler = NoticeBox.Show("下载完成！", "MCL启动器", MessageBoxIcon.Success, true);
                await Task.Run(() =>
                {
                    Thread.Sleep(1500);
                });
                NoticeBox.DestroyInstance();

            }
            else
            {
                MessageBoxX.Show(this, "尚有信息未输入！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }
        private async void fabric_game_verBox_Copy_DropDownOpened(object sender, EventArgs e)
        {
            if (fabric_game_verBox.Text != "" && fabric_game_verBox_Copy.Text == "")
            {
                fabric_game_verBox_Copy.ItemsSource = await FabricInstaller.GetFabricBuildsByVersionAsync(fabric_game_verBox.Text);
            }
        }
        private async void Quilt_game_verBox_Copy_DropDownOpened(object sender, EventArgs e)
        {
            if (Quilt_game_verBox.Text != "" && Quilt_game_verBox_Copy.Text == "")
            {
                Quilt_game_verBox_Copy.ItemsSource = await QuiltInstaller.GetQuiltBuildsByVersionAsync(Quilt_game_verBox.Text);
            }
        }
        private async void Quilt_game_verBox_DropDownClosed(object sender, EventArgs e)
        {
            if (Quilt_game_verBox.Text != "")
            {
                string selectedGameV = (Quilt_game_verBox.SelectedItem as MinecraftLaunch.Modules.Models.Install.GameCoreEmtity).Id;
                var QuiltList = await QuiltInstaller.GetQuiltBuildsByVersionAsync(selectedGameV);
                Quilt_game_verBox_Copy.ItemsSource = QuiltList;
            }
        }
        private async void QuiltInstall_start_Click(object sender, RoutedEventArgs e)
        {
            if (Quilt_game_verBox.Text != "" && Quilt_game_verBox_Copy.Text != "")
            {
                QuiltDownLoadLog.Text = "";
                QuiltInstallProgress.Value = 0;
                var QuiltBuild = Quilt_game_verBox_Copy.SelectedItem as QuiltInstallBuild;
                var v = new GameCoreToolkit(gameFolder);
                var fi = new QuiltInstaller(v, QuiltBuild);
                await Task.Run(async () =>
                {
                    await fi.InstallAsync((e) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            QuiltDownLoadLog.AppendText($"[{DateTime.Now}]{e.Item2}\n");
                            QuiltDownLoadLog.ScrollToEnd();
                            QuiltInstallProgress.Value = e.Item1;
                        }));
                    });
                });
                VersionComboNeedToBeUpdated = true;
                var handler = NoticeBox.Show("下载完成！", "MCL启动器", MessageBoxIcon.Success, true);
                await Task.Run(() =>
                {
                    Thread.Sleep(1500);
                });
                NoticeBox.DestroyInstance();
            }
            else
            {
                MessageBoxX.Show(this, "尚有信息未输入！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        #region 一堆void
        private async void ModSearch_Click(object sender, RoutedEventArgs e)
        {
            ModGrid.ItemsSource = null;
            ModSpin.IsSpinning = true;
            ModLoadingText.Visibility = Visibility.Visible;
            if (ModInput.Text != "")
            {
                var ModToolkit = ModToolkit_Forge;
                var res = await ModToolkit.SearchModpacksAsync(ModInput.Text);
                List<Mod> mods = new();
                res.ForEach(x =>
                {
                    Mod modItem = new();
                    modItem.Description = x.Description;
                    modItem.image = new(new System.Uri(x.IconUrl));
                    modItem.Name = x.Name;
                    modItem.Version = $"{x.SupportedVersions.Last()}-{x.SupportedVersions[0]}";
                    modItem.Files = x.Files;
                    mods.Add(modItem);
                });
                ModGrid.ItemsSource = mods;

            }
            else
            {
                var res = await ModToolkit_Forge.GetFeaturedModpacksAsync();
                List<Mod> mods = new();
                res.ForEach(x =>
                {
                    Mod modItem = new();
                    modItem.Description = x.Description;
                    modItem.image = new(new System.Uri(x.IconUrl));
                    modItem.Name = x.Name;
                    modItem.Version = $"{x.SupportedVersions.Last()}-{x.SupportedVersions[0]}";
                    modItem.Files = x.Files;
                    mods.Add(modItem);
                });
                ModGrid.ItemsSource = mods;
            }
            ModSpin.IsSpinning = false;
            ModLoadingText.Visibility = Visibility.Hidden;
        }
        private void PageChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Page.SelectedIndex)
            {
                case 0:
                    if (VersionComboNeedToBeUpdated != false)
                    {
                        var core = new GameCoreToolkit(gameFolder);
                        versionCombo.ItemsSource = core.GetGameCores();
                    }
                    VersionComboNeedToBeUpdated = false;
                    break;
                case 1:
                    GameVersionData.Items.Clear();
                    GetMcVersions();
                    GetMcVersionList();
                    break;
                case 2:
                    break;
            }
        }
        private void ModGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ModGrid.SelectedItem as Mod;
            modName = item.Name;
            ModGrid.Visibility = Visibility.Hidden;
            ModPage.Visibility = Visibility.Visible;
            ModDownLoad_Back.Visibility = Visibility.Visible;
            ModInfoImage.Source = item.image;
            ModDescribe.Text = item.Description;
            ModName.Text = item.Name;
            var items = item.Files;
            foreach (var file in items)
            {
                if (file.Value[0].SupportedVersion == null)
                {
                    items.Remove(file.Key);
                }
            }
            ModDownloadFile.Items.Clear();
            foreach (var file in items)
            {
                int item_count = file.Value.Count;
                for (int temp = 0; temp < item_count; temp++)
                {
                    CurseForgeModpackFileInfo modinfo = new();
                    modinfo.FileId = file.Value[temp].FileId;
                    modinfo.DownloadUrl = file.Value[temp].DownloadUrl;
                    modinfo.SupportedVersion = file.Value[temp].SupportedVersion;
                    modinfo.FileName = file.Value[temp].FileName;
                    modinfo.ModLoaderType = file.Value[temp].ModLoaderType;
                    ModDownloadFile.Items.Add(modinfo);
                }
            }
        }
        private void ModDownLoad_Back_Click(object sender, RoutedEventArgs e)
        {
            ModGrid.Visibility = Visibility.Visible;
            ModPage.Visibility = Visibility.Hidden;
            ModDownLoad_Back.Visibility = Visibility.Hidden;
            ModGrid_Mod.Visibility = Visibility.Visible;
            ModPage_Mod.Visibility = Visibility.Hidden;
            ModDownLoad_Back_Mod.Visibility = Visibility.Hidden;
        }
        private void Mod_ChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            string sPath = AppDomain.CurrentDomain.BaseDirectory + "MCL\\DownLoads\\Mods";
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }
            dlg.InitialDirectory = sPath;
            string mod_saveFolder;
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                mod_saveFolder = dlg.FileName;
                Mod_SaveFolderText.Text = mod_saveFolder;
                Mod_SaveFolderText_Mod.Text = mod_saveFolder;
            }
        }
        private async void ModDownloadFile_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mod = ModDownloadFile.SelectedItem as CurseForgeModpackFileInfo;
            if (Mod_SaveFolderText.Text != "")
            {
                ModDownloadFile.Visibility = Visibility.Hidden;
                await HttpToolkit.HttpDownloadAsync(mod.DownloadUrl, Mod_SaveFolderText.Text);
                var handler = NoticeBox.Show($"{modName}下载完成！", "MCL启动器", MessageBoxIcon.Success, true);
                ModDownloadFile.Visibility = Visibility.Visible;
                await Task.Run(() =>
                {
                    Thread.Sleep(1500);
                });
                NoticeBox.DestroyInstance();
            }
            else
            {
                MessageBoxX.Show(this, "未设置保存目录！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }

        }
        private void GetMcVersions()
        {
            GameVersionData.Items.Clear();
            var core = new GameCoreToolkit(gameFolder);
            var mcVersions = core.GetGameCores();
            foreach (var mcVersion in mcVersions)
            {
                MinecraftVersion minecraftVersion = new MinecraftVersion();
                minecraftVersion.Id = mcVersion.Id;
                var ModLoaderInfomation = mcVersion.ModLoaderInfos.ToList();
                switch (mcVersion.HasModLoader)
                {
                    case true:
                        minecraftVersion.Description = $"继承自{mcVersion.InheritsFrom}，{ModLoaderInfomation[0].ModLoaderType}客户端";
                        switch (ModLoaderInfomation[0].ModLoaderType)
                        {
                            case ModLoaderType.OptiFine:
                                minecraftVersion.bitmapImage = new(new Uri("/Resources/images/optfine.png", UriKind.Relative));
                                break;
                            case ModLoaderType.Fabric:
                                minecraftVersion.bitmapImage = new(new Uri("/Resources/images/fabric.png", UriKind.Relative));
                                break;
                            case ModLoaderType.Forge:
                                minecraftVersion.bitmapImage = new(new Uri("/Resources/images/forge.png", UriKind.Relative));
                                break;
                            case ModLoaderType.Quilt:
                                minecraftVersion.bitmapImage = new(new Uri("/Resources/images/quilt.png", UriKind.Relative));
                                break;
                        }
                        break;
                    case false:
                        switch (mcVersion.Type)
                        {
                            case "release":
                                minecraftVersion.Description = $"正式版 {mcVersion.Id}";
                                break;
                            case "snapshot":
                                minecraftVersion.Description = $"快照版 {mcVersion.Id}";
                                break;
                            case "old_alpha":
                                minecraftVersion.Description = $"远古Alpha测试版 {mcVersion.Id}";
                                break;
                            case "old_beta":
                                minecraftVersion.Description = $"远古Beta测试版 {mcVersion.Id}";
                                break;
                        }
                        minecraftVersion.bitmapImage = new(new Uri("/Resources/images/normal.png", UriKind.Relative));
                        break;
                }
                GameVersionData.Items.Add(minecraftVersion);
            }
        }
        private async void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateD.Update updated = new UpdateD.Update();
                var HaveHigherVersion = updated.GetUpdate(APIKey_2018k, LauncherVersion);
                if (HaveHigherVersion == true)
                {
                    var SavePath = $"{appFolder}MCL\\Updates";
                    var exeFilePath = Process.GetCurrentProcess().MainModule.FileName;
                    var DownLoadLink = updated.GetUpdateFile(APIKey_2018k);
                    var GengXinNeiRong = updated.GetUpdateRem(APIKey_2018k);
                    var setting = Application.Current.FindResource("UpdateMessage") as MessageBoxXSetting;
                    var WantToUpdate = MessageBoxX.Show(this, $"有新版本可更新！\n更新内容:\n{GengXinNeiRong}", "MCL启动器", MessageBoxButton.OKCancel, MessageBoxIcon.Info, setting);
                    if (WantToUpdate == MessageBoxResult.OK)
                    {
                        var PendingSetting = Application.Current.FindResource("pendingSetting") as PendingBoxSetting;
                        var handle = PendingBox.Show(this, "正在下载更新文件中……", "MCL启动器", true, PendingSetting);
                        var updateFile = await HttpToolkit.HttpDownloadAsync(DownLoadLink, SavePath);
                        var filePath = updateFile.FileInfo.FullName;
                        handle.Close();
                        MessageBoxX.Show(this, "最新版文件已经下载完成！请立即重启启动器以应用更新！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Success);
                        Process cmdProcess = new Process();
                        cmdProcess.StartInfo.CreateNoWindow = true;
                        cmdProcess.StartInfo.FileName = "powershell.exe";
                        cmdProcess.StartInfo.Arguments = $"-noexit sleep(1) ; rm {exeFilePath} ; move {filePath} {exeFilePath} ; {exeFilePath}";
                        cmdProcess.Start();
                        Environment.Exit(0);
                    }
                }
                else
                {
                    var handler = NoticeBox.Show("启动器已是最新版本", "MCL启动器", MessageBoxIcon.Success, true);
                    await Task.Run(() =>
                    {
                        Thread.Sleep(1500);
                    });
                    NoticeBox.DestroyInstance();
                }
            }
            catch
            {
                MessageBoxX.Show(this, "获取版本更新失败！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }
        private void XieYiButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://docs.qq.com/doc/DUklrU3VJTm92SnFG")
            {
                UseShellExecute = true,
                CreateNoWindow = true
            });
        }
        private async void GameVersionData_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (GameVersionData.SelectedItem != null)
            {
                var GridItem = GameVersionData.SelectedItem;
                var SelectedGame = GridItem as MinecraftVersion;
                GameVersionData.Visibility = Visibility.Hidden;
                GameVersion.Visibility = Visibility.Visible;
                var toolkit = new GameCoreToolkit(gameFolder);
                GameVersionImage.Source = SelectedGame.bitmapImage;
                GamePath.Text = $"路径：{gameFolder}";
                GameID.Content = SelectedGame.Id;
                GameDescription.Content = SelectedGame.Description;
                var core = toolkit.GetGameCore(SelectedGame.Id);
                ResourceInstaller resourceInstaller = new(core);
                var res = await resourceInstaller.GetAssetResourcesAsync();
                GameAssets.Text = $"依赖文件：共计{res.Count}个";
                GameSize.Text = $"大小：{GetTotalSize(core)}MB";
                GameLibrary.Text = $"依赖库：共计{core.LibraryResources.Count}个";
            }
        }
        private void GameVersion_back_Click(object sender, RoutedEventArgs e)
        {
            GameVersion.Visibility = Visibility.Hidden;
            GameVersionData.Visibility = Visibility.Visible;
        }
        private void openVersionFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(gameFolder)
            {
                UseShellExecute = true,
                CreateNoWindow = true
            });
        }
        private async void deleteVersion_Click(object sender, RoutedEventArgs e)
        {
            var gameName = GameID.Content.ToString();
            var res = MessageBoxX.Show(this, $"确定要删除{gameName}吗？", "MCL启动器", MessageBoxButton.OKCancel, MessageBoxIcon.Question);
            if (res == MessageBoxResult.OK)
            {
                var toolkit = new GameCoreToolkit(gameFolder);
                toolkit.Delete(gameName);
                GameVersion.Visibility = Visibility.Hidden;
                GameVersionData.Visibility = Visibility.Visible;
                GetMcVersions();
                await Task.Run(() =>
                {
                    NoticeBox.Show($"已删除核心{gameName}", "MCL启动器", MessageBoxIcon.Success);
                    Thread.Sleep(3000);
                    NoticeBox.DestroyInstance();
                });
            }
        }
        private void ZhengShiButton_Checked(object sender, RoutedEventArgs e)
        {
            verBox.ItemsSource = null;
            verBox.ItemsSource = MCReleaseList;
        }
        private void KuaiZhaoButton_Checked(object sender, RoutedEventArgs e)
        {
            verBox.ItemsSource = null;
            verBox.ItemsSource = MCSnapshotList;
        }
        private void YuanGuButton_Checked(object sender, RoutedEventArgs e)
        {
            verBox.ItemsSource = null;
            verBox.ItemsSource = MCOldList;
        }
        private void maxMem_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsRamSliderInited != false)
            {
                RamSliderText.Text = $"已设置内存{maxMem.Value}MB";
            }
        }
        private void ZanZhu_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://afdian.net/a/mcl888")
            {
                UseShellExecute = true,
                CreateNoWindow = true
            });
        }
        #endregion
        private void Window_closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Default.MaxMemory = Convert.ToInt32(maxMem.Value);
            Default.JavaSelectedItemIndex = javaCombo.SelectedIndex;
            if (MingXieMingDan.Text != "" || MingXieMingDan.Text != "获取鸣谢名单失败！请检查您是否连接上了互联网！")
            {
                Default.ZanZhuMingDan = MingXieMingDan.Text;
            }
            if (gameAccountsList.Count() > 0)
            {
                Default.gameAccountsList = JsonConvert.SerializeObject(gameAccountsList);
            }
            Default.Save();
        }
        private void AddAccountsButton_Click(object sender, RoutedEventArgs e)
        {
            Resources.AddAccount addAccount = new();
            addAccount.ShowDialog();
            accountsGrid.Children.Clear();
            JieXiZhangHu();
        }
        private void JieXiZhangHu()
        {
            if (gameAccountsList.Count > 0)
            {
                AccountCombo.ItemsSource = gameAccountsList;
                gameAccountsList.ForEach(x =>
                {
                    BrushConverter brushConverter = new BrushConverter();
                    Brush brush_gray = (Brush)brushConverter.ConvertFromString("#87CEFA");
                    StackPanel stackPanel = new()
                    {
                        Width = 128,
                        Height = 256
                    };
                    Image image = new()
                    {
                        Height = 128,
                        Width = 128,
                        Source = new BitmapImage(new Uri($"https://crafatar.com/avatars/{x.Uuid}", UriKind.Absolute))
                    };
                    TextBlock userName = new()
                    {
                        Text = x.Name,
                        TextAlignment = TextAlignment.Left,
                        Foreground = PanuonLightBlue,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 20
                    };
                    TextBlock AccountTypeText = new()
                    {
                        TextAlignment = TextAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 12,
                        Foreground = brush_gray
                    };
                    switch (x.Type)
                    {
                        case AccountType.Offline:
                            AccountTypeText.Text = "离线登录账户";
                            break;
                        case AccountType.Microsoft:
                            AccountTypeText.Text = "微软正版账户";
                            break;
                        case AccountType.Yggdrasil:
                            AccountTypeText.Text = "外置登录账户";
                            break;
                    }
                    stackPanel.Children.Add(image);
                    stackPanel.Children.Add(userName);
                    stackPanel.Children.Add(AccountTypeText);
                    Border border = new()
                    {
                        BorderBrush = brush_gray,
                        BorderThickness = new(3),
                        CornerRadius = new(5),
                        Height = 185,
                        Width = 135
                    };
                    StackPanel TouMing = new()
                    {
                        Height = 185,
                        Width = 175
                    };
                    border.Child = stackPanel;
                    TouMing.Children.Add(border);
                    accountsGrid.Children.Add(TouMing);
                });
            }
        }
        private void settingPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (settingPage.SelectedIndex)
            {
                case 0:
                    accountsGrid.Children.Clear();
                    JieXiZhangHu();
                    break;
            }

        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 控件初始化
            var core = new GameCoreToolkit(gameFolder);
            versionCombo.ItemsSource = core.GetGameCores();
            javaCombo.Items.Add("自动选择Java");
            javaCombo.SelectedItem = 0;
            var java = JavaToolkit.GetJavas();
            switch (MCLType)
            {
                case LauncherType.Release:
                    APIKey_2018k = APIKey_2018k_release;
                    launcherVersionText.Text = $"当前版本：Release{LauncherVersion}";
                    break;
                case LauncherType.Beta:
                    APIKey_2018k = APIKey_2018k_beta;
                    launcherVersionText.Text = $"当前版本：Beta{LauncherVersion}";
                    break;
            }
            foreach (var j in java)
            {
                javaCombo.Items.Add(j.JavaPath);
            }
            javaCombo.SelectedIndex = 0;
            #endregion
            GetMcVersions();
            if (Default.JavaSelectedItemIndex != -1)
            {
                javaCombo.SelectedIndex = Default.JavaSelectedItemIndex;
            }
            if (Default.GameComboSelectedIndex != -1)
            {
                versionCombo.SelectedIndex = Default.GameComboSelectedIndex;
            }
            maxMem.Value = Default.MaxMemory;
            RamSliderText.Text = $"已设置内存{maxMem.Value}MB";
            if (Default.gameAccountsList != "")
            {
                gameAccountsList = JsonConvert.DeserializeObject<List<Account>>(Default.gameAccountsList);
                JieXiZhangHu();
            }
            IsRamSliderInited = true;
            GameVersion.Visibility = Visibility.Hidden;
            ModGrid.Visibility = Visibility.Visible;
            ModPage.Visibility = Visibility.Hidden;
            ModGrid_Mod.Visibility = Visibility.Visible;
            ModPage_Mod.Visibility = Visibility.Hidden;
            var FirstTime = !Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "MCL");
            Task.Run(() =>
            {
                Thread.Sleep(500);
                if (FirstTime == true)
                {
                    var setting = Application.Current.FindResource("YongHuXieYi") as MessageBoxXSetting;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        var XieYiMessage = MessageBoxX.Show(this, "使用MCL启动器就代表您同意《用户协议与免责声明》", "MCL启动器", MessageBoxButton.OKCancel, MessageBoxIcon.Info, setting);
                        if (XieYiMessage == MessageBoxResult.OK)
                        {
                            Process.Start(new ProcessStartInfo("https://docs.qq.com/doc/DUklrU3VJTm92SnFG")
                            {
                                UseShellExecute = true,
                                CreateNoWindow = true
                            });
                        }
                    }));
                    DirectoryInfo directoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "MCL");
                    directoryInfo.Create();
                }
            });
            Task.Run(new Action(() =>
            {
                try
                {
                    UpdateD.Update updated = new UpdateD.Update();
                    var HaveHigherVersion = updated.GetUpdate(APIKey_2018k, LauncherVersion);
                    if (HaveHigherVersion == true)
                    {
                        NoticeBox.Show("启动器有新版本可供更新，可前往【设置】进行更新！", "MCL启动器", MessageBoxIcon.Info);
                        Thread.Sleep(3000);
                        NoticeBox.DestroyInstance();
                    }
                    else
                    {
                        NoticeBox.Show("启动器已是最新版本！", "MCL启动器", MessageBoxIcon.Success);
                        Thread.Sleep(3000);
                        NoticeBox.DestroyInstance();
                    }
                }
                catch
                {
                    NoticeBox.Show("无法检测是否有更新！", "MCL启动器", MessageBoxIcon.Error);
                    Thread.Sleep(3000);
                    NoticeBox.DestroyInstance();
                }
            }));
            try
            {
                UpdateD.Update updated = new UpdateD.Update();
                MingXieMingDan.Text = updated.GetUpdateNotice(APIKey_2018k_release);
            }
            catch
            {
                if (Default.ZanZhuMingDan != "")
                {
                    MingXieMingDan.Text = Default.ZanZhuMingDan;
                }
                else
                {
                    MingXieMingDan.Text = "获取鸣谢名单失败！请检查您是否连接上了互联网！";
                }
            }
        }

        private async void ModSearch_Mod_Click(object sender, RoutedEventArgs e)
        {
            ModGrid.ItemsSource = null;
            ModSpin.IsSpinning = true;
            ModLoadingText.Visibility = Visibility.Visible;
            if (ModInput.Text != "")
            {
                var ModToolkit = ModToolkit_Forge;
                var res = await ModrinthToolkit.SearchModpacksAsync(ModInput.Text);
                List<ModrinthMod> mods = new();
                res.Hits.ForEach(async x =>
                {
                    var res = await ModrinthToolkit.GetProjectInfos(x.ProjectId);
                    ModrinthMod modItem = new();
                    modItem.Description = x.Description;
                    modItem.image = new(new Uri(x.IconUrl));
                    modItem.Name = x.Title;
                    modItem.Version = $"{x.Versions.Last()}-{x.Versions[0]}";
                    res.ForEach(x =>
                    {
                        modItem.Versions = x.GameVersion;
                        modItem.Loaders = x.Loaders;
                        x.Files.ForEach(x =>
                        {
                            modItem.Files.Add(x);
                        });
                    });                    
                    mods.Add(modItem);
                });
                ModGrid_Mod.ItemsSource = mods;

            }
            else
            {
                var res = await ModrinthToolkit.GetFeaturedModpacksAsync();
                List<ModrinthMod> mods = new();
                res.Hits.ForEach(async x =>
                {
                    var res = await ModrinthToolkit.GetProjectInfos(x.ProjectId);
                    ModrinthMod modItem = new();
                    modItem.Files = new();
                    modItem.Description = x.Description;
                    modItem.image = new(new Uri(x.IconUrl));
                    modItem.Name = x.Title;
                    modItem.Version = $"{x.Versions.Last()}-{x.Versions[0]}";
                    res.ForEach(x =>
                    {
                        modItem.Versions = x.GameVersion;
                        modItem.Loaders = x.Loaders;
                        x.Files.ForEach(x =>
                        {
                            modItem.Files.Add(x);
                        });
                    });
                    mods.Add(modItem);
                });
                ModGrid.ItemsSource = mods;
            }
            ModSpin.IsSpinning = false;
            ModLoadingText.Visibility = Visibility.Hidden;
        }

        private void ModGrid_Mod_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ModGrid_Mod.SelectedItem as ModrinthMod;
            modName = item.Name;
            ModGrid_Mod.Visibility = Visibility.Hidden;
            ModPage_Mod.Visibility = Visibility.Visible;
            ModDownLoad_Back_Mod.Visibility = Visibility.Visible;
            ModInfoImage_Mod.Source = item.image;
            ModDescribe_Mod.Text = item.Description;
            ModName_Mod.Text = item.Name;
            var items = item.Files;
            ModDownloadFile_Mod.Items.Clear();
            ModDownloadFile_Mod.ItemsSource = items;            
        }

        private async void ModDownloadFile_Mod_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mod = ModDownloadFile_Mod.SelectedItem as ModrinthFileInfo;
            if (Mod_SaveFolderText.Text != "")
            {
                ModDownloadFile_Mod.Visibility = Visibility.Hidden;
                await HttpToolkit.HttpDownloadAsync(mod.Url, Mod_SaveFolderText_Mod.Text);
                var handler = NoticeBox.Show($"{modName}下载完成！", "MCL启动器", MessageBoxIcon.Success, true);
                ModDownloadFile_Mod.Visibility = Visibility.Visible;
                await Task.Run(() =>
                {
                    Thread.Sleep(1500);
                });
                NoticeBox.DestroyInstance();
            }
            else
            {
                MessageBoxX.Show(this, "未设置保存目录！", "MCL启动器", MessageBoxButton.OK, MessageBoxIcon.Error);
            }
        }
    }
}
