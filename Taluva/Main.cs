using System;
using System.Drawing;
using Taluva.Utils;

namespace Taluva
{
    
    class MainClass
    {
        static void Main(string[] args)
        {
            DynamicMatrix<String> matrix = new DynamicMatrix<String>();

            matrix.Add("A", new Point(0, 0));
            matrix.Add("B", new Point(0, 2));
            matrix.Add("C", new Point(1, 0));
            matrix.Add("K", new Point(4, 1));
            matrix.Add("E", new Point(2, 2));
            matrix.Add("L", new Point(0, 0));

            Console.WriteLine(matrix);
            
            //interpret commands

            string s = Console.ReadLine();
            if(s!=null) InterpretActions(s);

        }
        public static void InterpretActions(string s)
        {
            switch (s)
            {
                case "Redo":
                    Console.WriteLine("Redo");
                    break;
                case "Undo":
                    Console.WriteLine("Undo");
                    break;
                case "Play":
                    PlayParser(s);
                    Console.WriteLine("Redo");
                    break;
                case "Print":
                    Console.WriteLine("Print");
                    //PrintMap();
                    break;
                case "Quit":
                    Console.WriteLine("Quit");
                    break;
                default:
                    Console.WriteLine("Command is not valid");
                    break;
            }
        }

        public static void PlayParser(string s)
        {
            
        }
    }

}
