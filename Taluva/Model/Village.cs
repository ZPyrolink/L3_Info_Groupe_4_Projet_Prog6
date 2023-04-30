namespace Taluva.Model;

public class Village
{
    private Cell?[] neighbors;

    public Village()
    {
        neighbors = new Cell[5];
    }

    public void AddNeighbor(Cell c)
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] == null)
            {
                neighbors[i] = c;
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
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null)
            {
                count++;
            }
        }
        return count >= 2;
    }

    public bool CheckTempleExist()
    {
        return neighbors.Any(c => c?.ActualBuildings == Building.Temple);
    }

    public bool CheckTowerExist()
    {
        return neighbors.Any(c => c?.ActualBuildings == Building.Tower);
    }
}