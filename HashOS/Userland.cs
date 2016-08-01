using System;
using System.Collections.Generic;
using System.Text;
using Sys=Cosmos.System;


namespace TestOS
{
    /// <summary>
    /// Create your programs here
    /// </summary>
    class Userland
    { 
        /// <summary>
        /// Runs a userland program
        /// </summary>
        /// <param name="command">Command to run</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int runCommand(string command, string[] args, string fullCommand)
        {
            Sys.Global.mDebugger.Send("Userland called!");
            if (command == "testprogram")
            {
                runTestProgram(args, fullCommand);
                return 0;
            }
            if (command == "easteregg")
            {
                runEasterEgg();
            }
            return 2;
        }

        public void runTestProgram(string[] args, string fullCommand)
        {
            Console.WriteLine("Test Program");
            Console.WriteLine();
            Console.Write("Full command: ");
            Console.WriteLine(fullCommand);
                
            if (args.Length!=0)
            {
                Console.Write("Argument count: ");
                Console.WriteLine(args.Length);
                for (int i=0; i<args.Length;i++)
                {
                    Console.Write(i);
                    Console.Write(": ");
                    Console.WriteLine(args[i]);
                }
            }
        }
        public void runEasterEgg()
        {
            Console.WriteLine("Quit snooping around, please!");
        }
       
       


    }
}
