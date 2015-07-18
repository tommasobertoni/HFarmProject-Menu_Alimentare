using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace HFarm.MenuAlimentare.ConsoleTest
{
    static class Program
    {

        static void Main(string[] args)
        {
            string userInput;
            int programIndex;
            do
            {
                Console.WriteLine();
                Console.WriteLine("PROGRAM INDEX\n" + 
                              "0 - CommandLineCookbookManager\n" + 
                              "1 - CounterProgram\n" + 
                              "or QUIT" + 
                              "\n\n");

                do
                {
                    programIndex = -1;
                    Console.Write("START PROGRAM: ");
                    userInput = Console.ReadLine();
                } while (userInput.ToUpper() != "QUIT" && !int.TryParse(userInput, out programIndex));

                Console.Write("---\n\n");
                switch(programIndex)
                {
                    case 0:
                        CommandLineCookbookManager.Start();
                        break;

                    case 1:
                        CounterProgram.Start(@"..\..\..\..\menu-archive.txt");
                        break;

                    default: //quit program
                        break;
                }

            } while (userInput.ToUpper() != "QUIT");
        }
    }
}
