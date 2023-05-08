using System;
using System.Drawing;
using System.Text.RegularExpressions;
using Taluva.Model;
using Taluva.Utils;
using UnityEngine;

Pile<Chunk> Pile = ListeChunk.Pile;
Board b = new Board();
Player p1 = new Player(PlayerColor.Red);
Player p2 = new Player(PlayerColor.Blue);
Player p3 = new Player(PlayerColor.Green);

//interpret commands
Console.WriteLine("Enter a command : ");
Console.WriteLine("Redo : Not implemented yet\n" +
                  "Undo : Not implemented yet\n" +
                  "Play : Exemple Play 0 0 1 (0 0 the coord of the volcano and 1 is the rotation) \n" +
                  "Barracks : Return the available barracks\n" +
                  "Temples : Exemple Temples 1 (1 c'est le joueur)\n" +
                  "Towers : Exemple Towers 1 (1 c'est le joueur)\n" +
                  "Place : Exemple Place 0 0 3 2 (0 0 sont les coordonnées de cellule 3 le type de building 2 le joueur\n"
                      );
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
    int z = 0;
    if (rg.IsMatch(s[1]) && rg.IsMatch(s[2]) && rg.IsMatch(s[3]) && rg.IsMatch(s[4]))     //Coordinate conversion
    {
        int x = int.Parse(s[1]);
        int y = int.Parse(s[2]); 
        z = int.Parse(s[4]);
        r = (Rotation)int.Parse(s[3]); //verify in Rotations enum
        pr = new PointRotation(new Vector2Int(x, y), r);
    }

    if (z == 1)
    {
        b.AddChunk(Pile.Draw(), p1, pr);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(b.worldMap);
        Console.ResetColor();
    }
    else if (z == 2)
    {
        b.AddChunk(Pile.Draw(), p2, pr);
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(b.worldMap);
        Console.ResetColor();
    }
    else if (z == 3)
    {
        b.AddChunk(Pile.Draw(), p3, pr);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(b.worldMap);
        Console.ResetColor();
    }
    
}
void Barracks()
{
    Vector2Int[] Barracks = b.GetBarrackSlots();
        foreach (Vector2Int tmp in Barracks)
        {
            Console.WriteLine("Point : x : "+ tmp.x + " y : " + tmp.y );
        }
}
void Temples(string[] s)
{
    Regex rg = new("[0-9].*");
    int x = 0;
    Vector2Int[] temples = null;
    if (rg.IsMatch(s[1]))     //Coordinate conversion
    {
        x = int.Parse(s[1]);
        if(x==1)
        {
            temples = b.GetTempleSlot(p1);
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (Vector2Int tmp in temples)
            {
                Console.WriteLine("Point : x : "+ tmp.x + " y : " + tmp.y );
            }
            Console.ResetColor();
        }
        else if(x==2)
        {
            temples = b.GetTempleSlot(p2);
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (Vector2Int tmp in temples)
            {
                Console.WriteLine("Point : x : "+ tmp.x + " y : " + tmp.y );
            }
            Console.ResetColor();
        }
        else if(x==3)
        {
            temples = b.GetTempleSlot(p3);
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (Vector2Int tmp in temples)
            {
                Console.WriteLine("Point : x : "+ tmp.x + " y : " + tmp.y );
            }
            Console.ResetColor();
        }
        
    }
}

void Towers(string[] s)
{
    Regex rg = new("[0-9].*");
    int x = 0;
    Vector2Int[] towers = null;
    if (rg.IsMatch(s[1]))     //Coordinate conversion
    {
        x = int.Parse(s[1]);
        if(x==1)
        {
            towers = b.GetTowerSlots(p1);
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (Vector2Int tmp in towers)
            {
                Console.WriteLine("Point : x : "+ tmp.x + " y : " + tmp.y );
            }
            Console.ResetColor();
        }
        else if(x==2)
        {
            towers = b.GetTowerSlots(p2);
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (Vector2Int tmp in towers)
            {
                Console.WriteLine("Point : x : "+ tmp.x + " y : " + tmp.y );
            }
            Console.ResetColor();
        }
        else if(x==3)
        {
            towers = b.GetTowerSlots(p3);
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (Vector2Int tmp in towers
)
            {
                Console.WriteLine("Point : x : "+ tmp.x + " y : " + tmp.y );
            }
            Console.ResetColor();
        }
        
    }
}
void Place(string[] s)
{
    Regex rg = new("[0-9].*");
    int x,y,z,k = 0;
    Vector2Int[] temples = null;
    Building build = Building.None;
    Player player = p1;
    if (rg.IsMatch(s[1]) && rg.IsMatch(s[2]) && rg.IsMatch(s[3]) && rg.IsMatch(s[4]))     //Coordinate conversion
    {
        x = int.Parse(s[1]);
        y = int.Parse(s[2]);
        z = int.Parse(s[3]);
        k = int.Parse(s[4]);
        Vector2Int p = new Vector2Int(x, y);
        Cell c = b.worldMap.GetValue(p);
        if (z == 0)
        {
            build = Building.None;
        }
        else if (z == 1)
            build = Building.Temple;
        else if (z == 2)
            build = Building.Tower;
        else if (z == 3)
            build = Building.Barrack;
        if (k == 1)
        {
            player = p1;
        }
        else if (k == 2)
            player = p2;
        else if (k == 3)
            player = p3;
        
        b.PlaceBuilding(c,build,player);


    }
}
void InterpretActions(string s)
{
    string[] splitted = s.Split(' ');

    switch (splitted[0])
    {
        case "Redo":
            Console.WriteLine("Redo");
            Console.WriteLine("Done");
            Console.WriteLine("Enter a command");
            break;
        case "Undo":
            Console.WriteLine("Undo");
            Console.WriteLine("Done");
            Console.WriteLine("Enter a command");
            break;
        case "Play":
            PlayParser(splitted);
            Console.WriteLine("Done");
            Console.WriteLine("Enter a command");
            break;
        case "Barracks":
            Barracks();
            Console.WriteLine("Done");
            Console.WriteLine("Enter a command");
            break;
        case "Temples":
            Temples(splitted);
            Console.WriteLine("Done");
            Console.WriteLine("Enter a command");
            break;
        case "Towers":
            Towers(splitted);
            Console.WriteLine("Done");
            Console.WriteLine("Enter a command");
            break;
        case "Place":
            Place(splitted);
            Console.WriteLine("Done");
            Console.WriteLine("Enter a command");
            break;
        case "Quit":
            Console.WriteLine("Quit");
            Environment.Exit(0);
            break;
        default:
            Console.WriteLine("Command is not valid");
            Console.WriteLine("Enter a command");
            break;
    }
}

