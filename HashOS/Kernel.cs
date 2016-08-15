using System;
using System.Collections.Generic;
using System.IO;
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
            "cd",
            "type",
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
            1, //hscript = kernel
            1, //exechbc = kernel
            0, //hsasm = userland
            1, //cd = kernel
            1, //type = kernel
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
            Console.Write(GlobalVars.CurrentDir);
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
            /*
             * All commands should end with return to keep from displaying the "Kernel command not found! message"
             */
            #region cd
            if (FullCommand.ToLower().StartsWith("cd"))
            {
                //Check if there were no args passed and if so, just display the current directory
                if (FullCommand.Split(' ').Length == 1)
                {
                    Console.Write("Current Directory: ");
                    Console.WriteLine(GlobalVars.CurrentDir);
                }
                return;
            }
            #endregion
            #region cls
            if (FullCommand.ToLower().StartsWith("cls"))
            {
                //Clear the screen
                Console.Clear();
                return;
            }
            #endregion
            #region echo
            if (FullCommand.ToLower().StartsWith("echo"))
            {
                //Check if echo is blank or is "echo."
                if (FullCommand.Length<="echo ".Length)
                {
                    //Just write a blank line
                    Console.WriteLine();
                    return;
                }
                //Write all the following characters after echo
                Console.Write(FullCommand.Substring("echo ".Length, FullCommand.Length - "echo ".Length));
                //End the line
                Console.WriteLine();
                return;
            }
            #endregion
            #region help
            if (FullCommand.ToLower().StartsWith("help"))
            {
                //Check if there was a command argument
                if (FullCommand.Split(' ').Length > 1)
                {
                    //Get the help for that command
                    getHelp(FullCommand.Split(' ')[1]);
                    return;
                }
                //No arguments, just resume the search

                //Display the version for HashOS
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("HashOS Version ");
                Console.WriteLine(GlobalVars.Version);
                Console.ForegroundColor = ConsoleColor.Cyan;
                //Display the version for help
                Console.WriteLine("help Version 0.1");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Commands:");

                //Get every command that exists in the command array
                for (int i = 0; i < commandArray.Length; i++)
                {
                    if (
                    //Command exclusions to hide from help (Used for duplicate commands that have suffixes and the easteregg)
                    commandArray[i] != "easteregg"
                    )
                        //Display such command
                        Console.WriteLine(commandArray[i]);
                }
                return;
            }
            #endregion
            #region time
            if (FullCommand.ToLower().StartsWith("time"))
            {
                //Write that time
                Console.Write("Time: ");
                //Check if we want it to be in 12 or 24 hour mode
                if (GlobalVars.HourMode)
                {
                    //Display the time in 12 hours with everything included
                    Console.WriteLine(time.getTime12(true, true, true));
                }
                else //We want 24 hour time
                {
                    //DISPLAY ALL THE TIME STUFF
                    Console.WriteLine(time.getTime24(true, true, true));
                }
                return;
            }
            #endregion
            #region reboot
            if (FullCommand.ToLower().StartsWith("reboot"))
            {
                //Show that the machine will be rebooting
                Console.WriteLine("Rebooting...");
                //Actually reboot
                Sys.Power.Reboot();
                //Display error message if it fails
                throw new Exception("Reboot failed!");
                //No return needed, it's unreachable
            }
            #endregion
            #region hscript
            if (FullCommand.ToLower().StartsWith("hscript"))
            {
                //Get the arguments
                string[] args = FullCommand.Split(' ');
                string[] argsPassed = { };
                //If there were no arguments
                if (args.Length == 1)
                {
                    //Just get help on it
                    getHelp("hscript");
                }
                //Else, just go through the arguments and parse them
                for (int i = 0; i < args.Length; i++)
                {
                    // "/h" gets help
                    if (args[i] == "/h")
                    {
                        //Get the help
                        getHelp("hscript");
                        return;
                    }
                    if (!args[i].StartsWith("/"))
                    {
                        string tmp = getFilePath(args[i].Trim());
                        //File does not exist
                        if (tmp == "N")
                        {
                            Console.WriteLine("File does not exist!");
                            return;
                        }
                        //Open the script and run it
                        hscript("0:\\Kudzu.txt", argsPassed);
                    }
                    //TODO: Finish parsing on hscript
                }
                return;
            }
            #endregion
            #region shutdown
            if (FullCommand.ToLower().StartsWith("shutdown"))
            {
                //Display message
                Console.WriteLine("Shutting Down...");
                //Shutdown through ACPI
                Drivers.Power.ShutDown();
                //This computer is not able to use ACPI, so inform the user of such, and halt the computer.
                Console.WriteLine("Unable to shutdown through ACPI, halting your computer instead");
                Console.WriteLine("You can now safely shutdown your computer");
                Stop();
                //No return needed, as it's unreachable
            }
            #endregion
            #region type
            if (FullCommand.StartsWith("type"))
            {
                //TODO: Finish this
            }
            //No command found, this means I made a mistake if it hits this
            Console.WriteLine("Kernel Command not found!!");
        }

        //Runs a HashScript file. Needs to be in kernel so that it can run kernel commands
        void hscript(string path, string[] args)
        {
            int returnCode = 0;
            string[] lines = null;
            string[] tmpStringArray = null;
            const string variableTag = "%%";
            #region Variable system initalization
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
            #endregion
            //Get the lines
            try
            {
                lines = System.IO.File.ReadAllLines(path);
            }
            catch (Exception e)
            {
                //Oh noes!! Something happened!
                Console.WriteLine(e.Message);
                return;
            }

            //Parse the lines
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i].Trim();
                //Checking and replacing variables
                #region Checking Variables
                //Check if it has the variable tag
                if (lines[i].Contains(variableTag))
                {
                    //Split the command into arguments
                    tmpStringArray = lines[i].Split(' ');
                    //Go through all the arguments
                    for (int j = 0; j < tmpStringArray.Length; j++)
                    {
                        //If the argument starts with the variable tag then....
                        if (tmpStringArray[j].StartsWith(variableTag))
                        {
                            //Go through all the variables
                            for (int k = 0; k<variables.Count; k++)
                            {
                                //Check if the variable is valid
                                if (tmpStringArray[j].Substring(variableTag.Length) == variables[k].Name)
                                {
                                    //Variable is valid, replace it with the value
                                    lines[i].Replace(tmpStringArray[j], variables[k].Value);
                                    break;
                                }
                            }
                        }
                    }
                }
                #endregion

            }


        }

        //Opens a file and displays it's contents
        //TODO: Just check this
        void type (string path)
        {
            string[] lines = null;
            //Get the lines
            try
            {
                lines = System.IO.File.ReadAllLines(path);
            }
            catch (Exception e)
            {
                //Oh noes!! Something happened!
                Console.WriteLine(e.Message);
                return;
            }
            for (int i=0; i<lines.Length; i++)
            {
                Console.WriteLine(lines[i]);
            }
        }

        public string getFilePath(string path)
        {
            //Check if it is a full path
            //if (System.IO.File.Exists(path))
            //{
            //    return path;
            //}
            //Check if file exists in the path also
            if (File.Exists(GlobalVars.CurrentDir + path.Trim()))
            {
                return GlobalVars.CurrentDir + path;
            }
            //Doesn't exist in any of them
            return "N";
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