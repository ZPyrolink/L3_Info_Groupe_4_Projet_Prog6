using System;
using System.Text.RegularExpressions;
using Taluva.Model;
using Taluva.Utils;
using UnityEngine;


DynamicMatrix<String> matrix = new();
Pile<Chunk> Pile = ListeChunk.Pile;
Board b = new Board();
Player p = new Player(PlayerColor.Red);
//interpret commands
Console.WriteLine("Enter a command : ");
bool t = true;
while (t)
{
    string s = Console.ReadLine();
    if (s != null) InterpretActions(s);
    else t = false;
}

void PlayParser(string[] s)
{
    Regex rg = new("[0-9].*");
    PointRotation pr = null;
    Rotation r = 0;
    if (rg.IsMatch(s[1]) && rg.IsMatch(s[2]) && rg.IsMatch(s[3]))     //Coordinate conversion
    {
        int x = int.Parse(s[1]);
        int y = int.Parse(s[2]);
        r = (Rotation)int.Parse(s[3]); //verify in Rotations enum
        pr = new PointRotation(new Vector2Int(x, y), r);
    }
            
    
    b.AddChunk(Pile.Draw(), p, pr);
    Console.WriteLine(b.worldMap);
            
    //Play mov
}

void InterpretActions(string s)
{
    string[] splitted = s.Split(' ');

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
            /*
            PointRotation[] chunks = b.GetChunkSlots();
            foreach (PointRotation p in chunks)
            {
                Console.WriteLine("x: " + p.point.x + ", y: " + p.point.y);
                Console.WriteLine("Rotations possible pour ce point:");
                for (int i = 0; i < p.rotations.Length; i++)
                {
                    if(p.rotations[i])
                        Console.Write((Rotation)i + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Play");
            */
            foreach (Cell c in b.worldMap)
            {
                Vector2Int tmp = b.GetCellCoord(c);
                Console.WriteLine("Point : ");
                Console.WriteLine(" x : " + tmp.x + " y : " + tmp.y);
            }
            Console.WriteLine("play");
            break;
        case "Quit":
            Console.WriteLine("Quit");
            break;
        default:
            Console.WriteLine("Command is not valid");
            break;
    }
}

