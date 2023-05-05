using System;
using System.Text.RegularExpressions;
using Taluva.Model;
using Taluva.Utils;
using UnityEngine;


DynamicMatrix<String> matrix = new();
Pile<Chunk> Pile = ListeChunk.Pile;
Board b = new Board();

Console.WriteLine(matrix);

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
            
    Player p = new Player(PlayerColor.Red);
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

