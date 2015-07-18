using HFarm.MenuAlimentare.Domain.Contracts;
using HFarm.MenuAlimentare.FileSystemStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFarm.MenuAlimentare.ConsoleTest
{
    public static class CommandLineCookbookManager
    {
        public static void Start()
        {
            Console.WriteLine("COMMAND LINE COOKBOOK MANAGER\n\n");

            ICookBook cookbook = new CookBookFileSystem();

            string userInput = null;
            string[] commands =
            {
                "GET\t(requires dish:string)",
                "GETBT\t(requires dishType:int)",
                "LIKE\t(requires dish:string)",
                "SET\t(requires dish:string dishType:int)",
                "DEL\t(requires dish:string)",
                "CLEAR\t(clear all the cookbook)",
                "SAVE\t(save the cookbook in the storage)",
                "PRINT\t(print all the dishes in the cookbook)",
                "HELP\t(display this message)",
                "QUIT\t(exit the program)"
            };
            commands.DisplayHelp();
            Console.WriteLine("\n");

            do
            {
                Console.Write("Operation:\t");
                userInput = Console.ReadLine().ToUpper();
                Console.WriteLine("Result:");

                try
                {
                    string[] inputTokens = userInput.Split(' ');
                    string operation = inputTokens[0];
                    string value;

                    switch (operation)
                    {
                        case "GET":
                            if (operation.Length < 2)
                            {
                                WrongArguments();
                                break;
                            }

                            value = cookbook.Get(inputTokens[1]);
                            Console.WriteLine("\t\t" + inputTokens[1] + ", " + value);
                            break;

                        case "GETBT":
                            if (operation.Length < 2)
                            {
                                WrongArguments();
                                break;
                            }

                            var dishType = inputTokens[1];
                            var dishes = cookbook.Where(dish => dish.Value == dishType)
                                                 .OrderBy(dishPair => dishPair.Key)
                                                 .Select(dishPair => dishPair.Key)
                                                 .ToList<string>();
                            Console.WriteLine("\tFound {0} dishes:\n", dishes.Count);
                            Console.WriteLine("\t" + string.Join(",\n\t", dishes));
                            break;

                        case "LIKE":
                            if (operation.Length < 2)
                            {
                                WrongArguments();
                                break;
                            }

                            var partialValue = inputTokens[1];
                            var dishesContaining = cookbook.Where(dish => dish.Key.ToUpper().Contains(partialValue))
                                                           .OrderBy(dishPair => dishPair.Key)
                                                           .ThenBy(dishPair => dishPair.Value)
                                                           .Select(dishPair => String.Format("{0} - {1}", dishPair.Key, dishPair.Value))
                                                           .ToList<string>();
                            Console.WriteLine("\tFound {0} dishes matching \"{1}\":\n", dishesContaining.Count, partialValue);
                            Console.WriteLine("\t" + string.Join(",\n\t", dishesContaining));
                            break;

                        case "SET":
                            if (operation.Length < 3)
                            {
                                WrongArguments();
                                break;
                            }

                            cookbook.Set(inputTokens[1], inputTokens[2]);
                            value = cookbook.Get(inputTokens[1]);
                            Console.WriteLine("\t\tInserted pair: " + inputTokens[1] + ", " + inputTokens[2]);
                            break;

                        case "DEL":
                            if (operation.Length < 2)
                            {
                                WrongArguments();
                                break;
                            }

                            value = cookbook.Get(inputTokens[1]);
                            cookbook.Remove(inputTokens[1]);
                            Console.WriteLine("\t\tDeleted pair: " + inputTokens[1] + ", " + value);
                            break;

                        case "CLEAR":
                            cookbook.Clear();
                            Console.WriteLine("\t\tCookbook cleared");
                            break;

                        case "SAVE":
                            Console.WriteLine("\t\tBegin saving cookbook in storage: " + ((CookBookFileSystem)cookbook).STORAGE_PATH);
                            Console.WriteLine("\t\tPlease wait...");
                            cookbook.CleanAndSaveCookBook();
                            Console.WriteLine("\t\tCleaning and saving completed");
                            break;

                        case "PRINT":
                            cookbook.PrintAll();
                            break;

                        case "?":
                        case "HELP":
                            commands.DisplayHelp();
                            break;

                        case "QUIT":
                            Console.WriteLine("\tbye!");
                            break;

                        default:
                            Console.WriteLine("No command found: insert HELP to see all the available commands");
                            break;
                    }
                    Console.WriteLine("---");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("error: " + ex.Message);
                }
            }
            while (!userInput.Equals("QUIT"));
        }

        public static void WrongArguments()
        {
            Console.WriteLine("Wrong arguments");
        }

        private static void DisplayHelp(this string[] commands)
        {
            Console.WriteLine("Use the following commands to interact with the cookbook");
            Console.WriteLine("\t" + string.Join(",\n\t", commands));
            Console.WriteLine("Leave a space between the command and the operation parameters");
        }

        private static void PrintAll(this ICookBook cookbook)
        {
            var cookbookStamp = string.Join("\n",
                cookbook.OrderBy(dishPair => dishPair.Key)
                        .ThenBy(dishPair => dishPair.Value)
                        .Select(dishPair => String.Format("\t\t{0} - {1}", dishPair.Key, dishPair.Value)));

            Console.WriteLine(cookbookStamp);
            Console.WriteLine("\n\t\tCookbook length: {0}", cookbook.Count());
        }
    }
}
