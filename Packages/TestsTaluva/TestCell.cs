using NUnit.Framework;
using Taluva.Model;

namespace TestsTaluva
{
    [TestFixture]
    public class TestCell
    {

        private Cell[] _cell;

        [SetUp]
        public void Init()
        {
            _cell = new Cell[7];

            for (int i = 0; i < _cell.Length; i++) {
                _cell[i] = new((Biomes)i);
            }
        }

        [Test]
        public void TestInit()
        {
            for (int i = 0; i < _cell.Length; i++) {
                Assert.AreEqual((Biomes)i, _cell[i].CurrentBiome);
                Assert.IsFalse(_cell[i].ContainsBuilding());
                if (_cell[i].CurrentBiome == Biomes.Volcano || _cell[i].CurrentBiome == Biomes.None) {
                    Assert.IsFalse(_cell[i].IsBuildable);
                } else {
                    Assert.IsTrue(_cell[i].IsBuildable);
                }
            }
        }

        [Test]
        public void TestToString()
        {
            for (int i = 0; i < _cell.Length; i++) {
                switch (_cell[i].CurrentBiome) {
                    case Biomes.Desert:
                        Assert.AreEqual("D", _cell[i].ToString());
                        break;
                    case Biomes.Forest:
                        Assert.AreEqual("F", _cell[i].ToString());
                        break;
                    case Biomes.Lake:
                        Assert.AreEqual("L", _cell[i].ToString());
                        break;
                    case Biomes.Mountain:
                        Assert.AreEqual("M", _cell[i].ToString());
                        break;
                    case Biomes.Plain:
                        Assert.AreEqual("P", _cell[i].ToString());
                        break;
                    case Biomes.Volcano:
                        Assert.AreEqual("V", _cell[i].ToString());
                        break;
                    default:
                        Assert.AreEqual("\0", _cell[i].ToString());
                        break;
                }
            }
        }

        [Test]
        public void TestBuildBarrack()
        {
            foreach (Cell c in _cell)
                if (c.IsBuildable)
                    c.Build(Building.Barrack);

            foreach (Cell c in _cell) {
                if (c.CurrentBiome == Biomes.Volcano || c.CurrentBiome == Biomes.None) {
                    Assert.AreEqual(c.CurrentBuildings, Building.None);
                    Assert.IsFalse(c.ContainsBuilding());
                }  
                else {
                    Assert.AreEqual(c.CurrentBuildings, Building.Barrack);
                    Assert.IsTrue(c.ContainsBuilding());
                }   
                Assert.IsFalse(c.IsBuildable);
            }
        }

        [Test]
        public void TestBuildTower()
        {
            //For the test, we only check if we can put a tower on a cell
            //The game rules are not applied

            foreach (Cell c in _cell)
                if (c.IsBuildable)
                    c.Build(Building.Tower);

            foreach (Cell c in _cell) {
                if (c.CurrentBiome == Biomes.Volcano || c.CurrentBiome == Biomes.None) {
                    Assert.AreEqual(c.CurrentBuildings, Building.None);
                    Assert.IsFalse(c.ContainsBuilding());
                } else {
                    Assert.AreEqual(c.CurrentBuildings, Building.Tower);
                    Assert.IsTrue(c.ContainsBuilding());
                }
                Assert.IsFalse(c.IsBuildable);
            }
        }

        [Test]
        public void TestBuildTemple()
        {
            //For the test, we only check if we can put a temple on a cell
            //The game rules are not applied

            foreach (Cell c in _cell)
                if (c.IsBuildable)
                    c.Build(Building.Temple);

            foreach (Cell c in _cell) {
                if (c.CurrentBiome == Biomes.Volcano || c.CurrentBiome == Biomes.None) {
                    Assert.AreEqual(c.CurrentBuildings, Building.None);
                    Assert.IsFalse(c.ContainsBuilding());
                } else {
                    Assert.AreEqual(c.CurrentBuildings, Building.Temple);
                    Assert.IsTrue(c.ContainsBuilding());
                }
                Assert.IsFalse(c.IsBuildable);
            }
        }
    }
}
