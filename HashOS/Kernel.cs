using System;
using System.Collections.Generic;
using System.Text;
using HashOS;

using Sys = Cosmos.System;

namespace HashOS
{
    public class Kernel : Sys.Kernel
    {
        #region Variables
        Drivers.Time time = new Drivers.Time();
        #region Command Variables
        string[] commandArray = new string[]
        {
            "argumenttest",
            "echo",
            "echo.",
            "shutdown",
            "reboot",
            "cls",
            "easteregg",
            "help",
            "time",
            "dir",
            "hscript",
            "exechbc",
            "hbcbasm",
        };
        byte[] commandTypeArray = new byte[]
        {
            0, //argumenttest = userland
            1, //echo = kernel
            1, //echo. = kernel
            1, //shutdown = kernel
            1, //reboot = kernel
            1, //cls = kernel
            0, //easteregg = userland
            1, //help = kernel
            1, //time = kernel
            0, //dir = userland
            1, //hscript = kernel
            1, //exechbc = kernel
            0, //hsasm = userland
        };
        #endregion
        HashOS.ShellWrapper shell = new ShellWrapper();
        bool hscriptRunning = false;
        bool hbcRunning = false;
        #endregion
        
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
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            //YAY! We've successfully booted up
            Console.Write("Welcome to HashOS ");
            Console.Write(GlobalVars.Version);
            Console.WriteLine("!");
            Console.ForegroundColor = ConsoleColor.White;
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
                    Console.Write(FullCommand.Substring("echo ".Length, FullCommand.Length - "echo ".Length));
                    Console.WriteLine();
                    return;
                }
                if (FullCommand.Substring(0, "help".Length).ToLower() == "help")
                {
                    if (FullCommand.Split(' ').Length>1)
                    {
                        getHelp(FullCommand.Split(' ')[1]);
                        return;
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("HashOS Version ");
                    Console.WriteLine(GlobalVars.Version);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("help Version 0.1");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Commands:");
                    for (int i = 0; i < commandArray.Length; i++)
                    {
                        if (
                        //Command exclusions to hide from help (Used for duplicate commands that have suffixes and the easteregg)
                        commandArray[i] != "easteregg" && 
                        commandArray[i] != "echo."
                        )
                            Console.WriteLine(commandArray[i]);
                    }
                    return;
                }
                if (FullCommand.Substring(0, "time".Length).ToLower() == "time")
                {
                    Console.Write("Time: ");
                    Console.WriteLine(time.getTime12(true, true, true));
                    return;
                }
                if (FullCommand.Substring(0, "echo.".Length).ToLower() == "echo.")
                {
                    Console.WriteLine();
                    return;
                }
                if (FullCommand.Substring(0, "reboot".Length).ToLower() == "reboot")
                {
                    Console.WriteLine("Rebooting...");
                    
                    Sys.Power.Reboot();
                    return;
                }
                if (FullCommand.Substring(0, "hscript".Length).ToLower() == "hscript")
                {
                    string[] args = FullCommand.Split(' ');
                    if (args.Length==1)
                    {
                        getHelp("hscript");
                    }
                    for (int i = 0; i<args.Length; i++)
                    {

                    }
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

        void getHelp(string command)
        {
            switch (command)
            {
                case "argumenttest":
                    Console.WriteLine("argumenttest: Test of the arguments system");
                    Console.WriteLine("Syntax: argumenttest [arg1] [arg2]...");
                    break;
                case "echo":
                    Console.WriteLine("echo: Prints a string on screen");
                    Console.WriteLine("Syntax: echo[.] [string]");
                    break;
                case "shutdown":
                    Console.WriteLine("shutdown: Shuts down the machine");
                    Console.WriteLine("Syntax: shutdown");
                    break;
                case "reboot":
                    Console.WriteLine("reboot: Reboots the machine");
                    Console.WriteLine("Syntax: reboot");
                    break;
                case "cls":
                    Console.WriteLine("cls: Clears the screen");
                    Console.WriteLine("Syntax: cls");
                    break;
                case "help":
                    Console.WriteLine("help: Gives help");
                    Console.WriteLine("Syntax: help [command]");
                    Console.WriteLine();
                    Console.WriteLine("command: The command to display help on");
                    Console.WriteLine("\tIf this is missing it prints a list of possible commands");
                    break;
                case "time":
                    Console.WriteLine("time: Shows the current time");
                    Console.WriteLine("Syntax: time");
                    break;
                case "dir":
                    Console.WriteLine("dir: Displays the contents of a directory");
                    Console.WriteLine("Syntax: dir <path>");
                    Console.WriteLine();
                    Console.WriteLine("path: Path to show");
                    break;
                case "hscript":
                    Console.WriteLine("hscript: HashScript runner");
                    Console.WriteLine("Syntax: hscript [/h] [/a;(0-5);argument1,argument2...] [filename]");
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                    Console.WriteLine("/h: Display this help screen");
                    Console.WriteLine("/a;(1-5): Specify arguments for the script to use");
                    Console.WriteLine("\tExample: hscript /h;2;argtest1,argtest2 [filename]");
                    Console.WriteLine("filename: File to run");
                    break;
                case "exechbc":
                    Console.WriteLine("exechbc: Hash Bytecode executor");
                    Console.WriteLine("Syntax: Not Implemented");
                    break;
                case "hbcasm":
                    Console.WriteLine("hbcasm: Hash Bytecode assembler");
                    Console.WriteLine("Syntax: Not Implemented");
                    break;
                default:
                    Console.WriteLine("Command does not exist!");
                    break;
                
            }

        }       
    }
}
