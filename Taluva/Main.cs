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
        }
    }

}
