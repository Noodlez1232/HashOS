using System;
using System.Collections.Generic;
using System.Text;
//using HashOS;

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
                runKernelCommand(FullCommand);
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

        void runKernelCommand(string FullCommand)
        {
            if (FullCommand.ToLower().StartsWith("cd"))
            {
                if (FullCommand.Split(' ').Length == 1) ; //TODO: Finish this (cd)
            }
            if (FullCommand.ToLower().StartsWith("cls"))
            {
                Console.Clear();
                return;
            }
            if (FullCommand.ToLower().StartsWith("echo"))
            {
                Console.Write(FullCommand.Substring("echo ".Length, FullCommand.Length - "echo ".Length));
                Console.WriteLine();
                return;
            }
            if (FullCommand.ToLower().StartsWith("help"))
            {
                if (FullCommand.Split(' ').Length > 1)
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
            if (FullCommand.ToLower().StartsWith("time"))
            {
                Console.Write("Time: ");
                Console.WriteLine(time.getTime12(true, true, true));
                return;
            }
            if (FullCommand.ToLower().StartsWith("echo."))
            {
                Console.WriteLine();
                return;
            }
            if (FullCommand.ToLower().StartsWith("reboot"))
            {
                Console.WriteLine("Rebooting...");

                Sys.Power.Reboot();
                return;
            }
            if (FullCommand.ToLower().StartsWith("hscript"))
            {
                string[] args = FullCommand.Split(' ');
                if (args.Length == 1)
                {
                    getHelp("hscript");
                }
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "/h")
                    {
                        getHelp("hscript");
                        return;
                    }
                }
                return;
            }
            if (FullCommand.ToLower().StartsWith("shutdown"))
            {
                Console.WriteLine("Shutting Down...");
                Drivers.Power.ShutDown();
                return;
            }
            Console.WriteLine("Kernel Command not found!!");
        }

        void hscript(string path, string[] args)
        {
            int returnCode = 0;
            string[] lines = null;
            string[] tmpStringArray = null;
            //List of variables
            List<Variable> variables = new List<Variable>();
            Variable tmpVariable = new Variable();
            //Add all argument variables
            for (int i = 1; i <= 5; i++)
            {
                tmpVariable.Name = i.ToString();
                tmpVariable.Value = args[i - 1];
                variables.Add(tmpVariable);
            }

            //Get the lines
            try
            {
                lines = System.IO.File.ReadAllLines(path);
            }
            catch (Exception e)
            {
                //Oh noes!! Something happened!
                Console.WriteLine(e.Message);
            }
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i].Trim();
                //Checking and replacing variables
                #region Checking Variables
                if (lines[i].Contains("%%"))
                {
                    tmpStringArray = lines[i].Split(' ');
                    for (int j = 0; j < tmpStringArray.Length; j++)
                    {
                        if (tmpStringArray[j].StartsWith("%%"))
                        {
                            for (int k = 0; k<variables.Count; k++)
                            {
                                
                            }
                        }
                    }
                }
                #endregion

            }


        }

        protected override void AfterRun()
        {

        }

        void getHelp(string command)
        {
            switch (command)
            {
                #region argumenttest
                case "argumenttest":
                    Console.WriteLine("argumenttest: Test of the arguments system");
                    Console.WriteLine("Syntax: argumenttest [arg1] [arg2]...");
                    break;
                #endregion
                #region echo
                case "echo":
                    Console.WriteLine("echo: Prints a string on screen");
                    Console.WriteLine("Syntax: echo[.] [string]");
                    break;
                #endregion
                #region shutdown
                case "shutdown":
                    Console.WriteLine("shutdown: Shuts down the machine");
                    Console.WriteLine("Syntax: shutdown");
                    break;
                #endregion
                #region reboot
                case "reboot":
                    Console.WriteLine("reboot: Reboots the machine");
                    Console.WriteLine("Syntax: reboot");
                    break;
                #endregion
                #region cls
                case "cls":
                    Console.WriteLine("cls: Clears the screen");
                    Console.WriteLine("Syntax: cls");
                    break;
                #endregion
                #region help
                case "help":
                    Console.WriteLine("help: Gives help");
                    Console.WriteLine("Syntax: help [command]");
                    Console.WriteLine();
                    Console.WriteLine("command: The command to display help on");
                    Console.WriteLine("\tIf this is missing it prints a list of possible commands");
                    break;
                #endregion
                #region time
                case "time":
                    Console.WriteLine("time: Shows the current time");
                    Console.WriteLine("Syntax: time");
                    break;
                #endregion
                #region dir
                case "dir":
                    Console.WriteLine("dir: Displays the contents of a directory");
                    Console.WriteLine("Syntax: dir <path>");
                    Console.WriteLine();
                    Console.WriteLine("path: Path to show");
                    break;
                #endregion
                #region hscript
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
                #endregion
                #region exehbc
                case "exechbc":
                    Console.WriteLine("exechbc: Hash Bytecode executor");
                    Console.WriteLine("Syntax: Not Implemented");
                    break;
                #endregion
                #region hbcasm
                case "hbcasm":
                    Console.WriteLine("hbcasm: Hash Bytecode assembler");
                    Console.WriteLine("Syntax: Not Implemented");
                    break;
                #endregion
                default:
                    Console.WriteLine("Command does not exist!");
                    break;

            }

        }

    }

    class Variable
    {
        private string name = "";
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        private string val = "";
        public string Value
        {
            get
            {
                return val;
            }
            set
            {
                val = value;
            }
        }
    }
}