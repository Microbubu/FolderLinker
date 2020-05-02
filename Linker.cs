using System.Management.Automation;

namespace FolderLinker
{
    public static class Linker
    {
        public static void Link(string dir, string dstDir)
        {
            string script = "new-item -itemtype Junction -path " + $"\"{dir}\"" +
                            " -target " + $"\"{dstDir}\"";
            PowerShell ps = PowerShell.Create().AddScript(script);
            ps.Invoke();
            Service.Log.WriteLine($"{dstDir} => {dir}", LogType.Link);
        }
    }
}