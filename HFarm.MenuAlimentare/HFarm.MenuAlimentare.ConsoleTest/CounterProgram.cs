using HFarm.MenuAlimentare.Domain.Contracts;
using HFarm.MenuAlimentare.FileSystemStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFarm.MenuAlimentare.ConsoleTest
{
    public static class CounterProgram
    {
        public static void Start(string menuFile)
        {
            try
            {
                if (menuFile != null)
                {
                    menuFile.MenuFileExists();
                }
                else
                {
                    menuFile = CookBookFileSystem.DEFAULT_STORAGE_FILE_NAME;
                }

                ICookBook cookbook = new CookBookFileSystem();
                ICollection<string> validMenus = new LinkedList<string>();

                Console.WriteLine("COUNTER PROGRAM");
                using (StreamReader reader = new StreamReader(menuFile))
                {
                    string currentLine;
                    while ((currentLine = reader.ReadLine()) != null)
                    {
                        var dishes = currentLine.Split(',').Select(dish => dish.Trim()).ToArray<string>();

                        bool isValidMenu = true;
                        for (int dishPosition = 0; dishPosition < 5 && isValidMenu; dishPosition++)
                        {
                            try
                            {
                                var dish = dishes[dishPosition].ToLower();
                                int dishCorrectPosition;
                                string cookbookPosition = cookbook.Get(dish);

                                if (cookbookPosition == null)
                                {
                                    dishCorrectPosition = RequestUserKnowledge(dish);

                                    if (dishCorrectPosition == -1) //exit program
                                        throw new EndProgramException();

                                    cookbook.Set(dish, dishCorrectPosition.ToString());
                                }
                                else if (!int.TryParse(cookbookPosition, out dishCorrectPosition))
                                {
                                    isValidMenu = false; // something wrong in the cookbook
                                    break;
                                }

                                if (dishCorrectPosition != dishPosition)
                                {
                                    isValidMenu = false;
                                    break;
                                }
                            }
                            catch (IndexOutOfRangeException iOOREx)
                            { }
                        }

                        if (isValidMenu)
                        {
                            validMenus.Add(currentLine);
                        }
                    }
                }

                var menus = "- " + string.Join("\n- ", validMenus);
                Console.WriteLine(menus);
                Console.WriteLine("\nTotal valid menus: {0}", validMenus.Count);
                Console.WriteLine("---");
            }
            catch (EndProgramException ePEx)
            {
                Console.WriteLine("---");
            }
        }

        private static int RequestUserKnowledge(string unknownDish)
        {
            int dishCorrectPosition;

            var userDishPosition = AskUserAbout(unknownDish).ToUpper();

            if (userDishPosition == "QUIT")
                return -1; //exit program

            if (!int.TryParse(userDishPosition, out dishCorrectPosition))
            {
                Console.WriteLine("- Insert the correct dish position in optimal menu!");
                Console.WriteLine();
                do
                {
                    userDishPosition = AskUserAbout(unknownDish);
                }
                while (!int.TryParse(userDishPosition, out dishCorrectPosition) && dishCorrectPosition > 4 && dishCorrectPosition >= 0);
            }

            Console.WriteLine();
            return dishCorrectPosition;
        }

        private static void MenuFileExists(this string menuFile)
        {
            if (!File.Exists(menuFile))
                throw new ArgumentNullException("File don't exists");
        }

        private static string AskUserAbout(string unknownDish)
        {
            Console.WriteLine("Help! What is \"{0}\"?\t", unknownDish);
            Console.WriteLine("0 - MAIN COURSE, 1 - SECOND COURSE, 2 - SIDE DISH, 3 - DESSERT, 4 - COFFE");
            var userDishPosition = Console.ReadLine();
            return userDishPosition;
        }
    }

    class EndProgramException : Exception
    {
    }
}
