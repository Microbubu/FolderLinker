using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Topshelf;

namespace FolderLinker
{
    public class Service : ServiceControl
    {
        public static Log Log;
        private string configFileName = "config.xml";
        private Dictionary<string, string> dirMap;
        public bool Start(HostControl hostControl)
        {
            Service.Log = new Log();
            Service.Log.WriteLine("Welcome, enjoy linking your folders.");
            dirMap = new Dictionary<string, string>();
            if (!MapConfigFromFile())
            {
                Service.Log.WriteLine($"Reading config file failed.");
                return false;
            }
            FirstRun();
            StartWatchService();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            Service.Log = null;
            dirMap = null;
            hostControl.Stop();
            return true;
        }

        private void FirstRun()
        {
            foreach (var map in dirMap)
            {
                string[] srcDirs = Directory.GetDirectories(map.Key);
                foreach (var folder in srcDirs)
                {
                    string folderName = folder.Replace(Directory.GetParent(folder).FullName + @"\", string.Empty);
                    string dstFolder = Path.Combine(map.Value, folderName);
                    if (!Directory.Exists(dstFolder))
                    {
                        Linker linker = new Linker(dstFolder, folder);
                        linker.Link();
                    }
                }
            }
        }

        private void StartWatchService()
        {
            foreach(var map in dirMap)
            {
                new DirectoryWatcher(map.Key, map.Value).StartWatch();
            }
        }

        private bool MapConfigFromFile()
        {
            try
            {
                string config = Path.Combine(AppContext.BaseDirectory, configFileName);
                if (!File.Exists(config))
                {
                    BuildConfigFile();
                    return false;
                }
                XmlDocument document = new XmlDocument();
                document.Load(config);
                XmlNode configNode = document.ChildNodes[0];
                if (configNode.Name != "Config")
                    return false;
                foreach (XmlNode node in configNode.ChildNodes)
                {
                    if (node.Name == "Linker")
                    {
                        string src = node.Attributes["SourceDirectory"].Value;
                        string dst = node.Attributes["DestinationDirectory"].Value;
                        if (!Directory.Exists(src) || !Directory.Exists(dst))
                            return false;
                        dirMap.Add(src, dst);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void BuildConfigFile()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<Config>\n")
                .Append("  <!-- Add your own Linker nodes here -->\n")
                .Append("  <Linker SourceDirectory=\"\" DestinationDirectory=\"\"/>\n")
                .Append("</Config>");

            string config = Path.Combine(AppContext.BaseDirectory, configFileName);
            using (FileStream fs = File.Create(config))
            {
                byte[] configBytes = Encoding.UTF8.GetBytes(builder.ToString());
                fs.Write(configBytes, 0, configBytes.Length);
                fs.Close();
            }
            Service.Log.WriteLine("Config file not exists, created a sample.");
        }
    }
}
