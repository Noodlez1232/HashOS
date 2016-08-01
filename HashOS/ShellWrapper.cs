using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

namespace TestOS
{
    public class ShellWrapper
    {
        Userland userland = new Userland();
        private string[] commandList;
        private byte[] commandListType;
        /// <summary>
        /// Set the list of commands used
        /// [x]=Command name
        /// </summary>
        public string[]listOfCommands
        {
            set
            {
                Sys.Kernel.PrintDebug("listOfCommands set");
                commandList = value;
            }
        }
        /// <summary>
        /// Set the list of types of commands
        /// 0=Userland
        /// 1=Kernel
        /// </summary>
        public byte[] listOfCommandTypes
        {
            set
            {
                Sys.Kernel.PrintDebug("listOfCommandTypes set");
                commandListType = value;
            }
        }
        
        /// <summary>
        /// Runs a command
        /// </summary>
        /// <param name="input">The command to parse and run commands</param>
        /// <returns>Return code
        /// 0=No error
        /// 1=Minor error
        /// 2=Fatal error
        /// 3=Kernel command (like echo and shutdown)
        /// 4=Nonexiestant command
        /// </returns>
        public int runCommand(string input)
        {
            Sys.Kernel.PrintDebug("runCommand is running");
            string[] runCommandTmp = input.Split(' ');
            Sys.Kernel.PrintDebug("input has been split");
            for (int i=0;i<commandList.Length;i++)
            {
                //Check if command exists
                Sys.Kernel.PrintDebug("Check if command exists");
                if (runCommandTmp[0].ToLower() == commandList[i].ToLower())
                {
                    Sys.Kernel.PrintDebug("Command does exist");
                    Sys.Kernel.PrintDebug("Checking command type");
                    //Userland Program
                    if (commandListType[i]==0)
                    {
                        Sys.Kernel.PrintDebug("Command is userland");
                        string[] ArgList = new string[runCommandTmp.GetLength(0) - 1];

                        //Get a sub-array to take the command itself out and leave the arguments there
                        for (int j=1; j<runCommandTmp.GetLength(0);j++)
                        {
                            ArgList[j - 1] = runCommandTmp[j];
                        }
                        //Run the command and return the return code
                        Sys.Kernel.PrintDebug("Running userland program");
                        return userland.runCommand(runCommandTmp[0], ArgList, input);
                    }
                    //Kernel program
                    if (commandListType[i]==1)
                    {
                        Sys.Kernel.PrintDebug("Kernel command found");
                        return 3;
                    }

                }
            }

            Sys.Kernel.PrintDebug("Nonexiestant command");
            //Command does not exist
            return 4;
        }

    }
}
