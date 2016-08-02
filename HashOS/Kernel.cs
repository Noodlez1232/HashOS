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
            "argumenttest",
            "echo",
            "shutdown",
            "reboot",
            "cls",
            "easteregg",
            "help"
        };
        byte[] commandTypeArray = new byte[]
        {
            0,
            1,
            1,
            1,
            1,
            0,
            1
        };
        TestOS.ShellWrapper shell = new ShellWrapper();
        protected override void BeforeRun()
        {
            //Clear it and write the inital messages
            Console.Clear();
            Console.WriteLine("Booting up...");
            //Give the commands to the shell wrapper
            shell.listOfCommands = commandArray;
            shell.listOfCommandTypes = commandTypeArray;

            //YAY! We've successfully booted up
            Console.WriteLine("Welcome to HashOS!");
        }

        protected override void Run()
        {
            //Get the shell all good and running
            Console.Write("HOS> ");
            string input = Console.ReadLine();
            Sys.Global.mDebugger.Send("Shell Wrapper called");
            int returnCode = shell.runCommand(input);
            if (returnCode == 3)
            {
                //Have to be sorted by length
                if (input.Substring(0, "cls".Length).ToLower() == "cls")
                {
                    Console.Clear();
                    return;
                }
                if (input.Split(' ')[0].ToLower() == "echo")
                {
                    for (int i = 1; i < input.Split(' ').GetLength(0); i++)
                    {
                        Console.Write(input.Split(' ')[i]);
                        Console.Write(' ');
                    }
                    Console.WriteLine();
                    return;
                }
                if (input.Substring(0,"help".Length).ToLower() == "help")
                {
                    Console.WriteLine("HashOS Version 0.1");
                    Console.WriteLine("help Version 0.01");
                    Console.WriteLine("Commands:");
                    for (int i = 0; i < commandArray.Length; i++)
                    {
                        if (commandArray[i]!="easteregg") Console.WriteLine(commandArray[i]);
                    }
                    return;
                }
                if (input.Substring(0, "reboot".Length).ToLower() == "reboot")
                {
                    Console.WriteLine("Rebooting...");
                    Sys.Power.Reboot();
                    return;
                }
                if (input.Substring(0, "shutdown".Length).ToLower() == "shutdown")
                {
                    Console.WriteLine("Shutting Down...");
                    Stop();
                    return;
                }
                Console.WriteLine("Kernel Command not found!!");
            }
            if (returnCode == 4)
            {
                Console.WriteLine("Command not found!");
            }
            if (returnCode==2)
            {
                Console.WriteLine("APPLICATION SUFFFERED A FATAL ERROR!");
            }

        }
        protected override void AfterRun()
        {

        }
       
    }
}
