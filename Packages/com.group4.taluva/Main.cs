using System;
using System.Text.RegularExpressions;
using Taluva.Model;
using Taluva.Utils;

namespace Taluva
{
    class MainClass
    {
        static void Main(string[] args)
        {
            DynamicMatrix<String> matrix = new();

            // matrix.Add("A", new(0, 0));
            // matrix.Add("B", new(0, 2));
            // matrix.Add("C", new(1, 0));
            // matrix.Add("K", new(4, 1));
            // matrix.Add("E", new(2, 2));
            // matrix.Add("L", new(0, 0));

            Console.WriteLine(matrix);
            
            //interpret commands
            bool a = true;
            string s;
            while (a)
            {
                s = Console.ReadLine();
                if (s != null) InterpretActions(s);
                else a = false;
            }
            

        }
        public static void InterpretActions(string s)
        {
            string[] splitted = s.Split(" ");
            
            switch (splitted[0])
            {
                case "Redo":
                    Console.WriteLine("Redo");
                    break;
                case "Undo":
                    Console.WriteLine("Undo");
                    break;
                case "Play":
                    PlayParser(splitted);
                    Console.WriteLine("Play");
                    break;
                case "Quit":
                    Console.WriteLine("Quit");
                    break;
                default:
                    Console.WriteLine("Command is not valid");
                    break;
            }
        }

        public static void PlayParser(string[] s)
        {
            Regex rg = new("[0-9].*");
            if (rg.IsMatch(s[1]) && rg.IsMatch(s[2]) && rg.IsMatch(s[3]))     //Coordinate conversion
            {
                int x = int.Parse(s[1]);
                int y = int.Parse(s[2]);
                int r = int.Parse(s[3]);        //verify in Rotations enum
            }

            ListeChunk lc = new ListeChunk();
            lc.CreateCellSelection();
            

            //Play mov
        }
    }

}
