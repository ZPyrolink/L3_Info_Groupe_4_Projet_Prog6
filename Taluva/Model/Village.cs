namespace Taluva.Model;

public class Village
{
    private Cell?[] neighbors;

    public Village()
    {
        neighbors = new Cell[5];
    }

    public void AddNeighbor(Cell c, int index)
    {
        neighbors[index] = c;
    }

    public void SelfRemove()
    {
        //Ici, je supprime ma cell dans les voisins de mes voisins
        //Puis, verifie si le village doit etre séparé en 2
    }

    public bool CheckVillageIntegrity()
    {
        return neighbors.All(c => c == null);
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