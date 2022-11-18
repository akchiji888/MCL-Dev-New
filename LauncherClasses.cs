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
        public static string? offlineName;
        public async Task<string> getHtmlAsync(string html)//传入网址
        {
            string apiUrl = html;

            HttpClient client = new HttpClient();
            string jsonResponse = await client.GetStringAsync(apiUrl);
            return jsonResponse;
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
