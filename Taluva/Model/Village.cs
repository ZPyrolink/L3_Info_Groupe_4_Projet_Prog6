using System;


public class Village
{
    Cell[] Neighbors;

    public Village()
    {
        Neighbors = new Cell[5];
    }

    public void AddNeighbor(Cell c)
    {
        for (int i = 0; i < Neighbors.Length; i++)
        {
            if (Neighbors[i] == null)
            {
                Neighbors[i] = c;
                break;
            }
        }
    }

    public void SelfRemove()
    {
        //Ici, je supprime ma cell dans les voisins de mes voisins
        //Puis, verifie si le village doit etre séparé en 2
    }

    public bool CheckVillageIntegrity()
    {
        int count = 0;
        for (int i = 0; i < Neighbors.Length; i++)
        {
            if (Neighbors[i] != null)
            {
                count++;
            }
        }
        return count >= 2;
    }

    public bool CheckTempleExist()
    {
        foreach (Cell c in Neighbors) {
            if(c.ActualBuildings == Building.Temple)
                return true;
        }
        return false;
    }

    public bool CheckTowerExist()
    {
        foreach (Cell c in Neighbors) {
            if(c.ActualBuildings == Building.Tower)
                return true;
        }
        return false;
    }
}