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

        public static string offlineName;
        public static bool IsRegeditItemExist()
        {
            string[] subkeyNames;
            RegistryKey key = Registry.LocalMachine;
            RegistryKey soft = key.OpenSubKey("software");
            subkeyNames = soft.GetSubKeyNames();
            foreach (string keyName in subkeyNames)
            //遍历整个数组  
            {
                if (keyName == "ModernCraftLauncher")
                //判断子项的名称  
                {
                    RegistryKey mcl = key.OpenSubKey("software\\ModernCraftLauncher");
                    subkeyNames = mcl.GetSubKeyNames();
                    //取得该项下所有子项的名称的序列，并传递给预定的数组中  
                    foreach (string keyName_1 in subkeyNames)
                    //遍历整个数组  
                    {
                        if (keyName_1 == "firstTime")
                        //判断子项的名称  
                        {
                            key.Close();
                            return true;
                        }
                    }
                    key.Close();
                    return false;
                }
            }
            key.Close();
            return false;
            
        }
        public static long ConvertLocalDateTimeToUtcTimestamp(System.DateTime localTime)
        {
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(localTime);
            DateTime utcStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan ts = utcTime - utcStartTime;
            return Convert.ToInt64(ts.TotalMilliseconds);
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
