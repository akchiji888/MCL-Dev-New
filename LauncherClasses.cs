using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Windows;

namespace MCL_Dev
{
    internal class LauncherClasses
    {
        public static bool IsRegeditItemExist()
        {
            string[] subkeyNames;
            RegistryKey hkml = Registry.LocalMachine;
            RegistryKey software = hkml.OpenSubKey("SOFTWARE");
            //RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);  
            subkeyNames = software.GetSubKeyNames();
            //取得该项下所有子项的名称的序列，并传递给预定的数组中  
            foreach (string keyName in subkeyNames)
            //遍历整个数组  
            {
                if (keyName == "ModernCraftLauncher")
                //判断子项的名称  
                {
                    hkml.Close();
                    return true;
                }
            }
            hkml.Close();
            return false;
        }
        public static long ConvertLocalDateTimeToUtcTimestamp(System.DateTime localTime)
        {
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(localTime);
            DateTime utcStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan ts = utcTime - utcStartTime;
            return Convert.ToInt64(ts.TotalMilliseconds);
        }
        public static void OuterVisit(string url)
        {
            dynamic? kstr;
            string s;
            try
            {
                // 从注册表中读取默认浏览器可执行文件路径
                RegistryKey? key2 = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
                RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice\");
                if (key != null)
                {
                    kstr = key.GetValue("ProgId");
                    if (kstr != null)
                    {
                        s = kstr.ToString();
                        // "ChromeHTML","MSEdgeHTM" etc.
                        if (Registry.GetValue(@"HKEY_CLASSES_ROOT\" + s + @"\shell\open\command", null, null) is string path)
                        {
                            var split = path.Split('"');
                            path = split.Length >= 2 ? split[1] : "";
                            if (path != "")
                            {
                                Process.Start(path, url);
                                return;
                            }
                        }
                    }
                }
                if (key2 != null)
                {
                    kstr = key2.GetValue("");
                    if (kstr != null)
                    {
                        s = kstr.ToString();
                        var lastIndex = s.IndexOf(".exe", StringComparison.Ordinal);
                        if (lastIndex == -1)
                        {
                            lastIndex = s.IndexOf(".EXE", StringComparison.Ordinal);
                        }
                        var path = s.Substring(1, lastIndex + 3);
                        var result1 = Process.Start(path, url);
                        if (result1 == null)
                        {
                            var result2 = Process.Start("explorer.exe", url);
                            if (result2 == null)
                            {
                                Process.Start(url);
                            }
                        }
                    }
                }
                else
                {
                    var result2 = Process.Start("explorer.exe", url);
                    if (result2 == null)
                    {
                        Process.Start(url);
                    }
                }
            }
            catch
            {
                MessageBox.Show("无法打开浏览器以进行微软登录，已复制链接到剪贴板，请自行打开", "微软登录错误");
                Clipboard.SetText(url);
            }
        }

        public class JavaVersion
        {
            public string Version { get; internal set; }
            public string Path { get; internal set; }
        }
        internal class SearchBase
        {
            int ra;
            public List<JavaVersion> vs = new List<JavaVersion>();
            public void addSubDirectory()
            {
                string[] path = { "Program Files\\Java", "Program Files (x86)\\Java", @"MCLDownload\ext\", @"MCLDownload\ext\" };
                foreach (var t in GetRemovableDeviceID())
                {
                    foreach (var i in path)
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(t + "\\" + i);
                        FileInfo[] j;
                        try
                        {
                            j = directoryInfo.GetFiles("javaw.exe", SearchOption.AllDirectories);
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                        foreach (var y in j)
                        {
                            bool s = false;
                            foreach (var p in vs) if (p.Path.Equals(y.FullName)) { s = true; break; }
                            if (s) continue;
                            addrelativeDocument(y.FullName);
                        }
                    }
                }

            }
            public void addrelativeDocument(string path)
            {
                JavaVersion javaVersion = new JavaVersion();
                javaVersion.Path = path;
                javaVersion.Version = GetProductVersion(path);
                vs.Add(javaVersion);
            }

            private string GetProductVersion(string path)
            {
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(path);
                return info.ProductName;
            }


            public List<string> GetRemovableDeviceID()
            {
                List<string> deviceIDs = new List<string>();
                ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT  *  From  Win32_LogicalDisk ");
                ManagementObjectCollection queryCollection = query.Get();
                foreach (ManagementObject mo in queryCollection)
                {
                    deviceIDs.Add(mo["DeviceID"].ToString());
                }
                return deviceIDs;
            }
        }
        public static List<JavaVersion> GetJavaPath()
        {
            SearchBase searchBase = new SearchBase();
            searchBase.addSubDirectory();
            return searchBase.vs;
        }
    }
}
