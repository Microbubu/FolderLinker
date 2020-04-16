using System;
using System.Management.Automation;
using System.Linq;

public class Linker{
    private string srcDir;
    private string dstDir;
    public Linker(string src, string dst)
    {
        this.srcDir=src;
        this.dstDir=dst;
    }

    public void Link()
    {
        Console.WriteLine($"Linking {dstDir} to {srcDir}...");
        string script = "new-item -itemtype Junction -path " + $"\"{srcDir}\"" +
                        " -target " + $"\"{dstDir}\"";
        PowerShell ps = PowerShell.Create().AddScript(script);
        ps.Invoke();
        Console.WriteLine("Done!");
    }
}