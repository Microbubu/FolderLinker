using System.Management.Automation;

namespace FolderLinker
{
    public class Linker
    {
        private string srcDir;
        private string dstDir;
        public Linker(string src, string dst)
        {
            this.srcDir = src;
            this.dstDir = dst;
        }

        public void Link()
        {
            string script = "new-item -itemtype Junction -path " + $"\"{srcDir}\"" +
                            " -target " + $"\"{dstDir}\"";
            PowerShell ps = PowerShell.Create().AddScript(script);
            ps.Invoke();
            Service.Log.WriteLine($"{dstDir} ⋙ {srcDir}", LogType.Link);
        }
    }
}