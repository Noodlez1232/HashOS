using System;
using System.Collections.Generic;
using System.Text;

using Sys = Cosmos.System;

namespace TestOS
{
    public class Kernel : Sys.Kernel
    {
        string[] commandArray = new string[]
        {
            "testprogram",
            "echo",
            "shutdown",
            "reboot"
        };
        byte[] commandTypeArray = new byte[]
        {
            0,
            1,
            1,
            1
        };
        TestOS.ShellWrapper shell = new ShellWrapper();
        protected override void BeforeRun()
        {
            Console.Clear();
            Console.WriteLine("Booting up...");
            Console.WriteLine("Welcome to HashOS!");
        }

        protected override void Run()
        {
            Console.Write("HOS> ");
            string input = Console.ReadLine();
            commandArray = input.Split(' ');
            Sys.Global.mDebugger.Send("Shell Wrapper called");
            int returnCode = shell.runCommand(input);
            if (returnCode == 3)
            {
                if (input.Split(' ')[0].ToLower() == "echo")
                {
                    for (int i = 1; i < input.Split(' ').GetLength(0); i++)
                    {
                        Console.Write(input.Split(' ')[i]);
                        Console.Write(' ');
                    }
                    Console.Write('\n');
                }
                if (input.Substring(0, "shutdown".Length).ToLower() == "shutdown")
                {
                    Console.WriteLine("Shutting Down...");
                    Stop();
                }
                if (input.Substring(0,"reboot".Length).ToLower() == "reboot")
                {
                    Console.WriteLine("Rebooting...");
                    Sys.Power.Reboot();
                }
            }
            if (returnCode == 4)
            {
                Console.WriteLine("Command not found!");
            }

        }
    }
}
