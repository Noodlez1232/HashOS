using System;
using System.Collections.Generic;
using System.Text;
using HashOS;

using Sys = Cosmos.System;

namespace HashOS
{
    public class Kernel : Sys.Kernel
    {
        Drivers.Time time = new Drivers.Time();
        string[] commandArray = new string[]
        {
            "argumenttest",
            "echo",
            "shutdown",
            "reboot",
            "cls",
            "easteregg",
            "help",
            "time",
            "dir",
            "hscript",
            "exechbc",
            "hsasm",
        };
        byte[] commandTypeArray = new byte[]
        {
            0, //argumenttest = userland
            1, //echo = kernel
            1, //shutdown = kernel
            1, //reboot = kernel
            1, //cls = kernel
            0, //easteregg = userland
            1, //help = kernel
            1, //time = kernel
            0, //dir = userland
            0, //hscript = userland
            1, //exechbc = kernel
            0, //hsasm = userland
        };
        HashOS.ShellWrapper shell = new ShellWrapper();
        bool hscriptRunning = false;
        bool hbcRunning = false;
        protected override void BeforeRun()
        {
            //Clear it and write the inital messages
            Console.Clear();
            Console.WriteLine("Booting up...");
            //Give the commands to the shell wrapper
            shell.listOfCommands = commandArray;
            shell.listOfCommandTypes = commandTypeArray;
            //Init the shell and userland
            Console.WriteLine("Setting up the shell and userland");
            shell.init();
            //YAY! We've successfully booted up
            Console.WriteLine("Welcome to HashOS!");
        }

        protected override void Run()
        {
            //Get the shell all good and running
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("HOS");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("> ");
            Console.ForegroundColor = ConsoleColor.White;
            runCommand(Console.ReadLine());
            Sys.Global.mDebugger.Send("Shell Wrapper called");
        }
        public void runCommand(string FullCommand)
        {
            int returnCode = shell.runCommand(FullCommand);
            if (returnCode == 3)
            {
                //Have to be sorted by length
                if (FullCommand.Substring(0, "cls".Length).ToLower() == "cls")
                {
                    Console.Clear();
                    return;
                }
                if (FullCommand.Split(' ')[0].ToLower() == "echo")
                {
                    for (int i = 1; i < FullCommand.Split(' ').GetLength(0); i++)
                    {
                        Console.Write(FullCommand.Split(' ')[i]);
                        Console.Write(' ');
                    }
                    Console.WriteLine();
                    return;
                }
                if (FullCommand.Substring(0, "help".Length).ToLower() == "help")
                {
                    Console.WriteLine("HashOS Version 0.2");
                    Console.WriteLine("help Version 0.03");
                    Console.WriteLine("Commands:");
                    for (int i = 0; i < commandArray.Length; i++)
                    {
                        if (commandArray[i] != "easteregg") Console.WriteLine(commandArray[i]);
                    }
                    return;
                }
                if (FullCommand.Substring(0, "time".Length).ToLower() == "time")
                {
                    Console.Write("Time: ");
                    Console.WriteLine(time.getTime12(true, true, true));
                }
                if (FullCommand.Substring(0, "reboot".Length).ToLower() == "reboot")
                {
                    Console.WriteLine("Rebooting...");
                    Sys.Power.Reboot();
                    return;
                }
                if (FullCommand.Substring(0, "shutdown".Length).ToLower() == "shutdown")
                {
                    Console.WriteLine("Shutting Down...");
                    Drivers.Power.ShutDown();
                    return;
                }
                Console.WriteLine("Kernel Command not found!!");
            }
            if (returnCode == 4)
            {
                Console.WriteLine("Command not found!");
            }
            if (returnCode == 2)
            {
                Console.WriteLine("APPLICATION SUFFFERED A FATAL ERROR!");
            }
        }
        protected override void AfterRun()
        {

        }


       
    }
}
