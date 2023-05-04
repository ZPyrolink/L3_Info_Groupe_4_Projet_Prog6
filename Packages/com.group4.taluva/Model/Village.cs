using System.Collections.Generic;
using System.Linq;

namespace Taluva.Model
{
    public class Village
    {
        public Cell?[] neighbors;
        public Cell currentCell;

        public Village(Cell c)
        {
            neighbors = new Cell[6];
            currentCell = c;
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

        public int VillageSize()
        {
            int villageSize = 1;

            List<Cell> cells = new List<Cell>();
            List<Cell> visited = new List<Cell>();
            foreach (Cell? c in neighbors)
            {
                if (c == null || !c.HaveBuilding())
                    continue;
                cells.Add(c);
            }

            visited.Add(currentCell);

            while (cells.Count != 0)
            {
                Cell cell = cells[0];
                cells.Remove(cell);
                villageSize++;
                visited.Add(cell);
                foreach (Cell? c in cell.actualVillage.neighbors)
                {
                    if (c == null || !c.HaveBuilding() || visited.Contains(c))
                        continue;
                    cells.Add(c);
                }
            }

            return villageSize;
        }

        public bool CheckVillageIntegrity()
        {
            return neighbors.Count(t => t != null) >= 2;
        }

        public bool CheckTempleExist()
        {
            return neighbors.Any(c => c?.ActualBuildings == Building.Temple);
        }

        public bool CheckTowerExist()
        {
            return neighbors.Any(c => c?.ActualBuildings == Building.Tower);
        }

        public bool CheckBarrackExist()
        {
            return neighbors.Any(c => c?.ActualBuildings == Building.Barrack);
        }
    }
}