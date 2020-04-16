using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Threading.Tasks;

namespace FolderLinker
{
    class Program
    {
        static string configFileName = "Config.xml";
        static Dictionary<string, string> dirMap;
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to link your seperated folders together.");
            dirMap = new Dictionary<string, string>();
            string configFile = Path.Combine(Environment.CurrentDirectory, configFileName);
            if (!MapConfigFromFile(configFile))
            {
                Console.WriteLine($"Error occurred in config file: {configFile}, please check.");
                return;
            }
            foreach(var map in dirMap)
            {
                Run(map.Key, map.Value);
                DirectoryWatcher watcher = new DirectoryWatcher(map.Key, map.Value);
            }
            Console.WriteLine("Process Finished!");
            Console.Read();
        }

        static void Run(string src, string dst)
        {
            string[] srcDirs = Directory.GetDirectories(src);
            foreach(var folder in srcDirs)
            {
                string folderName = folder.Replace(Directory.GetParent(folder).FullName+@"\",string.Empty);
                string dstFolder = Path.Combine(dst,folderName);
                if(!Directory.Exists(dstFolder))
                {
                    Linker linker = new Linker(dstFolder,folder);
                    linker.Link();
                }
            }
        }

        static bool MapConfigFromFile(string fileName)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(fileName);
                XmlNode configNode = document.ChildNodes[0];
                if (configNode.Name!= "Config")
                    return false;
                foreach(XmlNode node in configNode.ChildNodes)
                {
                    if (node.Name == "Linker")
                    {
                        string src = node.Attributes["SourceDirectory"].Value;
                        string dst = node.Attributes["DestinationDirectory"].Value;
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
    }
}
