using MinecaftOAuth.Module.Models;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using Natsurainko.Toolkits.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MCL_Dev
{
    internal class LauncherClasses
    {
        public const string LauncherVersion = "1.2.2";
        public const string APIKey_2018k = "4386F97F6C36488887EBA723C4C99C83";
        public static YggdrasilAccount yggdrasilAccount;
        public static MicrosoftAccount microsoftaccount = new();
        public static int waizhi_selectedplayer = 114514;
        public class Mod
        {
            public string Description { get; set; }
            public BitmapImage image { set; get; }
            public string Name { set; get; }
            public string Version { set; get; }
            public Dictionary<string, List<CurseForgeModpackFileInfo>> Files { set; get; }
        }
        public static string GetTotalSize(GameCore id)
        {
            double total = 0;
            foreach (var library in id.LibraryResources)
            {
                if (library.Size != 0)
                    total += library.Size;
                else if (library.Size == 0 && library.ToFileInfo().Exists)
                    total += library.ToFileInfo().Length;
            }

            try
            {
                var assets = new ResourceInstaller(new()).GetAssetResourcesAsync().Result;

                foreach (var asset in assets)
                {
                    if (asset.Size != 0)
                        total += asset.Size;
                    else if (asset.Size == 0 && asset.ToFileInfo().Exists)
                        total += asset.ToFileInfo().Length;
                }
            }
            catch { }

            return $"{double.Parse(((double)total / (1024 * 1024)).ToString("0.00"))}";
        }
        public class MinecraftVersion
        {
            public BitmapImage bitmapImage { get; set; }
            public string Description { get; set; }
            public string Id { get; set; }
        }
        /*
        public static float GetRAM()//分配逻辑是直接从PCL2那儿拿来的
        {
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            var RamAvailable = (ramCounter.NextValue())/1024;
            var minRam = 0.75;//最低内存
            float RamT1 = 1.5f;
            float RamT2 = 2.5f;
            float RamT3 = 4;//T1代表勉强能跑起来的RAM，T2代表能流畅跑起来的RAM,T3的用处我不到啊（均为原版）（带Mod的懒得弄了）
            if (RamAvailable <= RamT1)
            {
                return RamAvailable * 1024;
            }
            else if (RamAvailable < RamT2)
            {
                return (RamAvailable + 1.25f) / 2 * 1024;
            }
            else if ((RamAvailable - RamT2) < 0.5f)
            {
                return RamT2 * 1024;
            }
            else if (RamAvailable <= RamT3)
            {
                return (RamAvailable + 2) / 2;
            }
            else if ((RamAvailable - RamT3) < 0.8)
            {
                return RamT3 * 1024;
            }
            else if (RamAvailable <= 8&&RamAvailable >= RamT3+0.8f)
            {
                return (RamT3 + (RamAvailable - RamT3) / 3) * 1024;
            }
            else
            {
                return 8096;
            }
        }
        */
        public enum viewStyle
        {
            Day = 1,
            Night = 2
        }
        public static string waizhi_selectedUsr;
        public class YggdrasilAuthenticator
        {
            public string? Uri { get; set; }

            public string? Email { get; set; }

            public string? Password { get; set; }

            public string ClientToken { get; set; } = Guid.NewGuid().ToString("N");


            public string AccessToken { get; set; } = string.Empty;


            public IEnumerable<YggdrasilAccount> Auth()
            {
                return AuthAsync(null).Result;
            }

            public async ValueTask<IEnumerable<YggdrasilAccount>> AuthAsync(Action<string> func)
            {
                Action<string> func2 = func;
                IProgress<string> progress = new Progress<string>();
                ((Progress<string>)progress).ProgressChanged += new EventHandler<string>(ProgressChanged);
                _ = Uri;
                _ = string.Empty;
                Report("开始第三方（Yggdrasil）登录");
                string content = await (await HttpWrapper.HttpPostAsync(content: MinecraftLaunch.Modules.Toolkits.ExtendToolkit.ToJson((object)new
                {
                    clientToken = Guid.NewGuid().ToString("N"),
                    username = Email,
                    password = Password,
                    requestUser = false,
                    agent = new
                    {
                        name = "Minecraft",
                        version = 1
                    }
                }), url: Uri + (string.IsNullOrEmpty(Uri) ? "https://authserver.mojang.com" : "/authserver") + "/authenticate")).Content.ReadAsStringAsync();
                List<YggdrasilAccount> accounts = new List<YggdrasilAccount>();
                foreach (AvailableProfiles i in content.ToJsonEntity<YggdrasilResponse>().UserAccounts)
                {
                    accounts.Add(new YggdrasilAccount
                    {
                        AccessToken = content.ToJsonEntity<YggdrasilResponse>().AccessToken,
                        ClientToken = content.ToJsonEntity<YggdrasilResponse>().ClientToken,
                        Name = i.Name,
                        Uuid = Guid.Parse(i.Uuid),
                        YggdrasilServerUrl = Uri,
                        Email = Email,
                        Password = Password
                    });
                }

                return accounts;
                void ProgressChanged(object _, string e)
                {
                    func2(e);
                }

                void Report(string value)
                {
                    if (func2 != null)
                    {
                        progress.Report(value);
                    }
                }
            }

            public async ValueTask<bool> ValidateAsync(string accesstoken)
            {
                using HttpResponseMessage res = await HttpWrapper.HttpPostAsync(content: MinecraftLaunch.Modules.Toolkits.ExtendToolkit.ToJson((object)new
                {
                    clientToken = ClientToken,
                    accesstoken = accesstoken
                }), url: Uri + "/authserver/validate");
                return res.IsSuccessStatusCode;
            }

            public YggdrasilAuthenticator()
            {
            }

            public YggdrasilAuthenticator(string uri, string email, string password)
            {
                Uri = uri;
                Email = email;
                Password = password;
            }

            public YggdrasilAuthenticator(bool IsLittleSkin, string email, string password)
            {
                if (IsLittleSkin)
                {
                    Uri = "https://littleskin.cn/api/yggdrasil";
                }

                Email = email;
                Password = password;
            }

            [Obsolete]
            public YggdrasilAuthenticator(string email, string password)
            {
            }
        }

    }
}
