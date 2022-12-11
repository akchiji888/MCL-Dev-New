using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.Http;
using System.Threading.Tasks;

namespace MCL_Dev
{
    internal class LauncherClasses
    {
        public static string waizhi_email;
        public static string waizhi_password;
        public static string offlineName;
        public static int waizhi_selectedplayer = 114514;
        public static string waizhi_selectedUsr;
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
#pragma warning disable CS0168 // 声明了变量，但从未使用过
                        try
                        {
                            j = directoryInfo.GetFiles("javaw.exe", SearchOption.AllDirectories);
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
#pragma warning restore CS0168 // 声明了变量，但从未使用过
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
#pragma warning disable CS8603 // 可能返回 null 引用。
                return info.ProductName;
#pragma warning restore CS8603 // 可能返回 null 引用。
            }


            public List<string> GetRemovableDeviceID()
            {
                List<string> deviceIDs = new List<string>();
                ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT  *  From  Win32_LogicalDisk ");
                ManagementObjectCollection queryCollection = query.Get();
                foreach (ManagementObject mo in queryCollection)
                {
#pragma warning disable CS8604 // 引用类型参数可能为 null。
                    deviceIDs.Add(mo["DeviceID"].ToString());
#pragma warning restore CS8604 // 引用类型参数可能为 null。
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
