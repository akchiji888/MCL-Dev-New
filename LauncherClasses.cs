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
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.VisualBasic;
using System.Windows.Media;

namespace MCL_Dev
{
    internal class LauncherClasses
    {        
        static BrushConverter brushConverter = new BrushConverter();
        public static Brush PanuonLightBlue = (Brush)brushConverter.ConvertFromString("#FF33B4F5");
        public const string LauncherVersion = "1.2.2";
        public const string APIKey_2018k = "4386F97F6C36488887EBA723C4C99C83";
        public static YggdrasilAccount yggdrasilAccount;
        public static MicrosoftAccount microsoftaccount = new();
        public static int waizhi_selectedplayer = 114514;
        public static List<Account> gameAccountsList = new();
        public class Mod
        {
            public string Description { get; set; }
            public BitmapImage image { set; get; }
            public string Name { set; get; }
            public string Version { set; get; }
            public Dictionary<string, List<CurseForgeModpackFileInfo>> Files { set; get; }
        }
        public class LauncherSetting
        {
            public int maxRam { get; set; }
            public int JavaSelectedItemIndex { get; set; }
            public int GameComboSelectedIndex { get; set; }
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
