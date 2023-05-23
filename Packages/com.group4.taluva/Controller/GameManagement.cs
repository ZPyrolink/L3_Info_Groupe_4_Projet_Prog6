using System;
using Taluva.Model;
using Taluva.Utils;
using Taluva.Model.AI;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Taluva.Controller
{
    public class GameManagment
    {
        public Board gameBoard { get; private set; }
        public Historic<Coup> historic;

        #region Players

        [Obsolete("Use the Players property instead")]
        public Player[] players
        {
            get => Players;
            set => Players = value;
        }

        public Player[] Players { get; set; }

        [Obsolete("use ActualPlayer instead")] public Player actualPlayer => ActualPlayer;
        public Player ActualPlayer => Players[ActualPlayerIndex];
        public Player PreviousPlayer => players[Math.Abs((ActualPlayerIndex - 1) % NbPlayers)];
        private AI ActualAi => (AI)actualPlayer;
        public int NbPlayers { get; private set; }
        public int ActualPlayerIndex { get; private set; }

        #endregion

        [Obsolete("Use SavePath instead")] public string savePath => SavePath;
        public string SavePath { get; } = Directory.GetCurrentDirectory() + "/Save/";

        [Obsolete("Use Pile instead")]
        public Pile<Chunk> pile
        {
            get => Pile;
            set => Pile = value;
        }

        public Pile<Chunk> Pile = ListeChunk.Pile;

        public TurnPhase actualPhase { get; private set; } = TurnPhase.NextPlayer;

        [Obsolete("Use MaxTurn instead")]
        public int maxTurn
        {
            get => MaxTurn;
            private set => MaxTurn = value;
        }

        public int MaxTurn { get; private set; }

        [Obsolete("Use ActualChunk Instead")]
        public Chunk actualChunk
        {
            get => ActualChunk;
            set => ActualChunk = value;
        }

        public Chunk ActualChunk { get; set; }

        #region Events

        //Actions
        //Notify phase change
        public Action<TurnPhase> ChangePhase { get; set; }
        private void OnChangePhase(TurnPhase phase) => ChangePhase?.Invoke(phase);

        public Action<Vector2Int, Rotation> NotifyReputTile { get; set; }
        private void OnReputTile(Vector2Int pos, Rotation r) => NotifyReputTile?.Invoke(pos, r);

        public Action<Vector2Int, Building> NotifyReputBuild { get; set; }
        private void OnReputBuild(Vector2Int pos, Building b) => NotifyReputBuild?.Invoke(pos, b);

        //Notify end of game
        public Action<Player, GameEnd> NotifyEndGame { get; set; }
        private void OnEndGame(Player winner, GameEnd victory) => NotifyEndGame?.Invoke(winner, victory);

        //Notify where the AI places the chunk
        public Action<PointRotation> NotifyAIChunkPlacement { get; set; }

        private void OnAIChunkPlacement(PointRotation pr) => NotifyAIChunkPlacement?.Invoke(pr);

        //Notify where the AI places the building
        public Action<Building, Vector2Int[]> NotifyAIBuildingPlacement { get; set; }
        private void OnAIBuildingPlacement(Building b, Vector2Int[] pos) => NotifyAIBuildingPlacement?.Invoke(b, pos);

        //Notify player eliminated
        public Action<Player> NotifyPlayerEliminated { get; set; }
        private void OnPlayerElimination(Player p) => NotifyPlayerEliminated?.Invoke(p);

        private bool checkIa = true;

        #endregion

        #region Ctors

        public GameManagment(GameManagment original)
        {
            this.NbPlayers = original.NbPlayers;
            this.gameBoard = new Board(original.gameBoard);
            this.historic = new Historic<Coup>(original.historic);
            this.pile = new Pile<Chunk>(original.pile);
            this.Players = new Player[NbPlayers];
            this.ActualPlayerIndex = original.ActualPlayerIndex;
            this.maxTurn = original.MaxTurn;
            this.ActualChunk = original.ActualChunk;
            for (int i = 0; i < NbPlayers; i++)
            {
                Players[i] = original.Players[i].Clone();
            }
        }
        public GameManagment(int nbPlayers, Type[] typeAI)
        {
            historic = new();
            pile = ListeChunk.Pile;
            ListeChunk.ResetChunk(pile);
            this.Players = new Player[nbPlayers];
            this.ActualPlayerIndex = -1;
            this.gameBoard = new();
            this.NbPlayers = nbPlayers;
            this.maxTurn = 12 * nbPlayers;

            PlayerColor[] pc = (PlayerColor[])Enum.GetValues(typeof(PlayerColor));

            for (int i = 0; i < nbPlayers - typeAI.Length; i++)
                Players[i] = new(pc[i]);

            int nbHumanPlayer = nbPlayers - typeAI.Length;

            for (int i = 0; i < typeAI.Length; i++)
            {
                Index index = (i + nbHumanPlayer);
                ref Player ptr = ref Players[index];

                if (typeAI[i] == typeof(AIRandom))
                    ptr = new AIRandom(pc[index], this);
                else if (typeAI[i] == typeof(AITree))
                    ptr = new AITree(pc[index], this);
                else if (typeAI[i] == typeof(AIMonteCarlo))
                    ptr = new AIMonteCarlo(pc[index], this);
            }
        }

        public GameManagment(int nbPlayers) : this(nbPlayers, Array.Empty<Type>())
        {
        }

        #endregion

        public class Coup : ICloneable
        {
            //New Chunk or cells
            public Vector2Int[] positions;
            public Rotation? rotation;

            //Last Chunk or last cells
            public Cell[] cells;
            public Chunk chunk;
            public int playerIndex;
            public Building[] building;

            private Coup(Vector2Int[] positions, Rotation? rotation, int playerIndex)
            {
                this.positions = positions;
                this.rotation = rotation;
                this.playerIndex = playerIndex;
            }

            public Coup(Vector2Int[] positions, Rotation rotation, int playerIndex, Chunk chunk) : this(positions,
                rotation, playerIndex)
            {
                this.chunk = chunk;
                this.cells = new Cell[1];
                cells[0] = null;
            }

            public Coup(Vector2Int[] positions, Rotation rotation, int playerIndex, Chunk chunk, Cell[] cells,
                Building[] b) : this(positions, rotation, playerIndex, cells, b)
            {
                this.chunk = chunk;
            }

            public Coup(Vector2Int[] positions, Rotation? rotation, int playerIndex, Cell[] cells, Building[] b) :
                this(positions, rotation, playerIndex)
            {
                this.cells = cells;
                building = b;
            }

            public Coup(Coup original)
            {
                positions = original.positions.ToArray();
                rotation = original.rotation;
                cells = original.cells.ToArray();
                if(original.chunk != null)
                    chunk = new Chunk(original.chunk);
                playerIndex = original.playerIndex;
                if(original.building != null)
                    building = original.building.ToArray();
                
            }    
            
            public System.Object Clone()
            {
                return new Coup(this);
            }
        }

        /// <summary>
        /// Check if we can undo
        /// </summary>
        public bool CanUndo => historic.CanUndo && actualPhase != TurnPhase.IAPlays &&
                               actualPhase != TurnPhase.NextPlayer;

        /// <summary>
        /// Check if we can redo
        /// </summary>
        public bool CanRedo => historic.CanRedo && actualPhase != TurnPhase.IAPlays &&
                               actualPhase != TurnPhase.NextPlayer;

        public void Save(string path)
        {
            PlayerColor[] pc = (PlayerColor[])Enum.GetValues(typeof(PlayerColor));
            Debug.Log(savePath + path);
            Directory.CreateDirectory(savePath);
            using (FileStream file = File.Open(savePath + path, FileMode.OpenOrCreate, FileAccess.Write))
            using (BinaryWriter writer = new(file))
            {
                writer.Write(historic.Index);
                writer.Write(NbPlayers);
                for (int i = 0; i < NbPlayers; i++)
                {
                    writer.Write((uint)Players[i].ID);
                    writer.Write(Players[i] is AI);
                    if (Players[i] is AI)
                        writer.Write((int)((AI)Players[i]).Difficulty);
                }

                Chunk[] stackArray = pile._stack.ToArray();
                writer.Write(stackArray.Length + pile._played.Count);

                for (int i = stackArray.Length - 1; i >= 0; i--)
                {
                    writer.Write((int)stackArray[i].Coords[1].ActualBiome);
                    writer.Write((int)stackArray[i].Coords[2].ActualBiome);
                }

                for (int i = pile._played.Count - 1; i >= 0; i--)
                {
                    writer.Write((int)pile._played[i].Coords[1].ActualBiome);
                    writer.Write((int)pile._played[i].Coords[2].ActualBiome);
                }

                writer.Write(historic.Count);
                for (int i = 0; i < historic.Count; i++)
                {
                    writer.Write(i <= historic.Index);
                    if (i % 2 == 0)
                    {
                        writer.Write(historic[i].positions.Length);
                        for (int j = 0; j < historic[i].positions.Length; j++)
                        {
                            writer.Write(historic[i].positions[j].x);
                            writer.Write(historic[i].positions[j].y);
                        }

                        writer.Write((int)historic[i].rotation);
                        writer.Write(historic[i].playerIndex);
                        for (int j = 1; j < historic[i].chunk.Coords.Length; j++)
                        {
                            writer.Write((int)historic[i].chunk.Coords[j].ActualBiome);
                            writer.Write((int)historic[i].chunk.Coords[j].ActualBuildings);
                            if (historic[i].chunk.Coords[j].ActualBuildings != Building.None)
                                writer.Write((int)historic[i].chunk.Coords[j].Owner);
                        }

                        writer.Write((int)historic[i].chunk.rotation);
                        writer.Write((int)historic[i].chunk.Level);
                        writer.Write(historic[i].cells[0] != null);
                        if (historic[i].cells[0] != null)
                        {
                            for (int j = 0; j < historic[i].cells.Length; j++)
                            {
                                writer.Write((int)historic[i].cells[j].ActualBiome);
                                writer.Write((int)historic[i].cells[j].ActualBuildings);
                                if (historic[i].cells[j].ActualBuildings != Building.None)
                                    writer.Write((int)historic[i].cells[j].Owner);
                                writer.Write((int)historic[i].building[j]);
                            }
                        }
                    }
                    else
                    {
                        writer.Write(historic[i].positions.Length);
                        for (int j = 0; j < historic[i].positions.Length; j++)
                        {
                            writer.Write(historic[i].positions[j].x);
                            writer.Write(historic[i].positions[j].y);
                        }

                        writer.Write(historic[i].playerIndex);
                        for (int j = 0; j < historic[i].cells.Length; j++)
                        {
                            writer.Write((int)historic[i].cells[j].ActualBiome);
                            writer.Write((int)historic[i].cells[j].ActualBuildings);
                            if (historic[i].cells[j].ActualBuildings != Building.None)
                                writer.Write((int)historic[i].cells[j].Owner);
                            writer.Write((int)historic[i].building[j]);
                        }
                    }
                }
            }
        }

        public void LoadGame(string path)
        {
            historic = new();
            using (FileStream file = File.Open(SavePath + path, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new(file))
            {
                int intIndex = reader.ReadInt32();
                this.NbPlayers = reader.ReadInt32();
                this.maxTurn = 12 * NbPlayers;
                this.ActualPlayerIndex = -1;
                this.gameBoard = new();
                this.Players = new Player[this.NbPlayers];
                for (int i = 0; i < NbPlayers; i++)
                {
                    PlayerColor id = (PlayerColor)reader.ReadInt32();
                    bool ia = reader.ReadBoolean();
                    if (ia)
                    {
                        Difficulty d = (Difficulty)reader.ReadInt32();
                        switch (d)
                        {
                            //a completer en declarant les nouvelles classes d'ia
                            case Difficulty.BadPlayer:
                                Players[i] = new AIRandom(id, this);
                                break;
                            case Difficulty.Normal:
                                throw new NotImplementedException();
                                break;
                            case Difficulty.SkillIssue:
                                throw new NotImplementedException();
                                break;
                        }
                    }
                    else
                    {
                        Players[i] = new(id);
                    }
                }

                List<Chunk> chunks = new();
                int nbChunk = reader.ReadInt32();

                for (int i = 0; i < nbChunk; i++)
                {
                    Cell left = new((Biomes)reader.ReadInt32());
                    Cell right = new((Biomes)reader.ReadInt32());
                    Chunk c = new(1, left, right);
                    chunks.Add(c);
                }

                Pile = new(chunks);
                actualChunk = pile.Draw();

                int historicCount = reader.ReadInt32();
                for (int i = 0; i < historicCount; i++)
                {
                    Vector2Int[] positions;
                    Rotation r = Rotation.N;
                    Chunk chunk = null;
                    bool boolean = false;
                    Cell[] newCells;
                    Building[] buildings;
                    int actualIndex;

                    bool index = reader.ReadBoolean();

                    if (i % 2 == 0)
                    {
                        int nbTiles = reader.ReadInt32();
                        positions = new Vector2Int[nbTiles];
                        for (int j = 0; j < nbTiles; j++)
                        {
                            positions[j] = new(reader.ReadInt32(), reader.ReadInt32());
                        }

                        r = (Rotation)reader.ReadInt32();
                        actualIndex = reader.ReadInt32();
                        Cell[] cells = new Cell[2];
                        for (int j = 1; j < 3; j++)
                        {
                            cells[j - 1] = new((Biomes)reader.ReadInt32());
                            cells[j - 1].ActualBuildings = (Building)reader.ReadInt32();
                            if (cells[j - 1].ActualBuildings != Building.None)
                            {
                                cells[j - 1].Owner = (PlayerColor)reader.ReadInt32();
                            }
                        }

                        Rotation chunkRotation = (Rotation)reader.ReadInt32();
                        int chunkLevel = reader.ReadInt32();
                        chunk = new(chunkLevel, cells[0], cells[1]);
                        chunk.rotation = chunkRotation;

                        boolean = reader.ReadBoolean();
                        buildings = new Building[3];
                        newCells = new Cell[chunk.Coords.Length];
                        if (boolean)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                newCells[j] = new((Biomes)reader.ReadInt32());
                                newCells[j].ActualBuildings = (Building)reader.ReadInt32();
                                if (newCells[j].ActualBuildings != Building.None)
                                    newCells[j].Owner = (PlayerColor)reader.ReadInt32();
                                buildings[j] = (Building)reader.ReadInt32();
                            }
                        }
                    }
                    else
                    {
                        int nbTiles = reader.ReadInt32();
                        positions = new Vector2Int[nbTiles];
                        for (int j = 0; j < nbTiles; j++)
                        {
                            positions[j] = new(reader.ReadInt32(), reader.ReadInt32());
                        }

                        actualIndex = reader.ReadInt32();
                        buildings = new Building[nbTiles];
                        newCells = new Cell[nbTiles];
                        for (int j = 0; j < nbTiles; j++)
                        {
                            newCells[j] = new((Biomes)reader.ReadInt32());
                            newCells[j].ActualBuildings = (Building)reader.ReadInt32();
                            if (newCells[j].ActualBuildings != Building.None)
                                newCells[j].Owner = (PlayerColor)reader.ReadInt32();
                            buildings[j] = (Building)reader.ReadInt32();
                        }
                    }

                    if (index)
                    {
                        ActualPlayerIndex = actualIndex;
                        checkIa = false;
                        if (i % 2 == 0)
                        {
                            Phase1(new(positions[0], r), r);
                            OnReputTile(positions[0], r);
                        }
                        else
                        {
                            bool init = i < intIndex;
                            Phase2(positions[0], buildings[0], init);
                            ActualPlayerIndex = actualIndex;
                            OnReputBuild(positions[0], buildings[0]);
                            NextPlayer();
                        }
                    }
                    else
                    {
                        if (i % 2 == 0)
                        {
                            if (boolean)
                            {
                                historic.Add(new(positions, r, ActualPlayerIndex, new(chunk), newCells, buildings));
                            }
                            else
                            {
                                historic.Add(new(positions, r, ActualPlayerIndex, new(chunk)));
                            }
                        }
                        else
                        {
                            historic.Add(new(positions, null, ActualPlayerIndex, newCells, buildings));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Use this when you're in phase 1 during the placement of the chunk.
        /// It will also store the possible cells under the chunk.
        /// </summary>
        /// <param name="position">Position of the chunk</param>
        /// <param name="rotation">Rotation of the chunk</param>
        /// <param name="chunk">The chunk played at the round</param>
        public void AddHistoric(Vector2Int position, Rotation rotation, Chunk chunk)
        {
            if (!gameBoard.WorldMap.IsVoid(position))
            {
                Vector2Int[] positions = gameBoard.GetChunksCoords(position, rotation);
                Cell[] newCells = new Cell[gameBoard.WorldMap[position].ParentChunk.Coords.Length];
                Building[] buildings = new Building[gameBoard.WorldMap[position].ParentChunk.Coords.Length];
                for (int i = 0; i < 3; i++)
                {
                    newCells[i] = gameBoard.WorldMap[positions[i]];
                    buildings[i] = gameBoard.WorldMap[positions[i]].ActualBuildings;
                }

                historic.Add(new(gameBoard.GetChunksCoords(position, rotation), rotation, ActualPlayerIndex, new(chunk, false),
                    newCells, buildings));
            }
            else
            {
                historic.Add(new(new[] { position }, rotation, ActualPlayerIndex, new(chunk, false)));
            }
        }

        /// <summary>
        /// Use this when you're in phase 2 during the placement of a building.
        /// It will store the different information on the cells.
        /// </summary>
        /// <param name="positions">Buildings positions</param>
        /// <param name="cells">Cells with the buildings</param>
        public void AddHistoric(Vector2Int[] positions, Cell[] cells, Building b)
        {
            Cell[] newCells = new Cell[cells.Length];
            Building[] buildings = new Building[cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                newCells[i] = new(cells[i]);
                buildings[i] = b;
            }

            historic.Add(new(positions, null, ActualPlayerIndex, newCells, buildings));
        }

        public void PrecedentPhase(bool AIMode = false)
        {
            if (actualPhase != TurnPhase.IAPlays)
            {
                int precedantPhaseValue =
                    Math.Abs(((int)actualPhase - 1) % (Enum.GetNames(typeof(TurnPhase)).Length - 1));
                actualPhase = (TurnPhase)precedantPhaseValue;
                if(!AIMode)
                    OnChangePhase(actualPhase);
            }
            else
            {
                actualPhase = TurnPhase.NextPlayer; //Change Player ?
                if(AIMode)
                    OnChangePhase(actualPhase);
            }
        }

        public void NextPhase(bool pioche = true,bool AIMode = false)
        {
            bool b = true;
            if (actualPhase != TurnPhase.IAPlays)
            {
                int nextPhaseValue = ((int)actualPhase + 1) % (Enum.GetNames(typeof(TurnPhase)).Length - 1);
                actualPhase = (TurnPhase)nextPhaseValue;

                if (actualPhase == TurnPhase.PlaceBuilding)
                {
                    b = pioche;
                    PlayerEliminated();

                    if (ActualPlayer.Eliminated)
                    {
                        actualPhase = TurnPhase.NextPlayer;
                        InitPlay(b);
                    }
                }
                if(!AIMode)
                    OnChangePhase(actualPhase);
                if (!b)
                {
                    actualChunk = pile.Draw();
                }
            }
            else
            {
                actualPhase = TurnPhase.NextPlayer;
                if(AIMode)
                    OnChangePhase(actualPhase);
            }
        }

        
        public Coup Undo(bool AIMode = false)
        {
            if (!historic.CanUndo)
                return null;

            Coup c = historic.Undo();
            if (c.chunk != null)
            {
                maxTurn++;
                gameBoard.RemoveChunk(gameBoard.GetChunksCoords(c.positions[0], (Rotation)c.rotation));
                if (c.cells[0] != null)
                {
                    for (int i = 0; i < c.cells.Length; i++)
                    {
                        gameBoard.WorldMap.Add(c.cells[i], c.positions[i]);
                        gameBoard.PlaceBuilding(c.cells[i], c.building[i], ActualPlayer, false);
                    }
                }

                if (players[c.playerIndex].Eliminated)
                {
                    pile.Stack(actualChunk);
                    players[c.playerIndex].Eliminated = false;
                    actualPhase = TurnPhase.PlaceBuilding;
                    ActualPlayerIndex = c.playerIndex;
                }

                Chunk chunk = new(c.chunk.Level, new(c.chunk.Coords[1].ActualBiome),
                    new(c.chunk.Coords[2].ActualBiome));
                pile.Stack(chunk);
                if (!players[c.playerIndex].Eliminated)
                    actualChunk = pile.Draw();
            }
            else
            {
                ActualPlayerIndex = c.playerIndex;
                for (int i = 0; i < c.cells.Length; i++)
                {
                    switch (c.building[i])
                    {
                        case Building.None:
                            break;
                        case Building.Temple:
                            ActualPlayer.NbTemple++;
                            break;
                        case Building.Tower:
                            ActualPlayer.NbTowers++;
                            break;
                        case Building.Barrack:
                            ActualPlayer.NbBarrack += gameBoard.WorldMap[c.positions[i]].ParentChunk.Level;
                            break;
                    }

                    c.cells[i].ActualBuildings = Building.None;
                    gameBoard.WorldMap.Add(c.cells[i], c.positions[i]);
                    pile.Stack(actualChunk);
                }
            }
            PrecedentPhase();
            return c;
        }
        
        
        public Coup Redo(bool AIMode = false)
        {
            if (!historic.CanRedo)
                return null;

            Coup c = historic.Redo();
            if (c.chunk == null)
            {
                for (int i = 0; i < c.cells.Length; i++)
                {
                    gameBoard.WorldMap.Add(c.cells[i], c.positions[i]);
                    gameBoard.PlaceBuilding(c.cells[i], c.building[i], ActualPlayer);
                }

                actualPhase = TurnPhase.NextPlayer;
                NextPlayer();
            }
            else
            {
                MaxTurn--;
                gameBoard.AddChunk(actualChunk, players[c.playerIndex], new(c.positions[0], (Rotation)c.rotation),
                    (Rotation)c.rotation);
                c.chunk = actualChunk;
                if (players[c.playerIndex].Eliminated)
                {
                    actualPhase = TurnPhase.NextPlayer;
                }
            }

            if (actualPlayer is AI ai && AIMode)
            {
                actualPhase = TurnPhase.IAPlays;
                AiChunk();
            }
            else
                NextPhase(false,AIMode);

            return c;
        }

        public Player CheckWinner()
        {
            Player p = null;
            if (maxTurn == 0)
            {
                p = NormalEnd;
                OnEndGame(p, GameEnd.NormalEnd);
            }

            Player tmp = EarlyEnd;
            if (tmp != null)
            {
                p = tmp;
                OnEndGame(p, GameEnd.EarlyEnd);
            }

            var tmp2 = Players.Where(p => !p.Eliminated);

            if (tmp2.Count() == 1)
            {
                p = tmp2.First();
                OnEndGame(tmp2.First(), GameEnd.LastPlayerStanding);
            }

            return p;
        }

        private Player EarlyEnd
        {
            get
            {
                foreach (Player p in Players)
                {
                    int completedBuildingTypes = 0;

                    if (p.NbTemple == 0)
                        completedBuildingTypes++;

                    if (p.NbBarrack == 0)
                        completedBuildingTypes++;

                    if (p.NbTowers == 0)
                        completedBuildingTypes++;

                    if (completedBuildingTypes >= 2)
                        return p;
                }

                return null;
            }
        }

        private Player NormalEnd
        {
            get
            {
                int maxTemple = 0;
                Player winner1 = null;
                foreach (Player p in Players)
                {
                    if (p.Eliminated)
                        continue;

                    if ((3 - p.NbTemple) > maxTemple)
                    {
                        maxTemple = (3 - p.NbTemple);
                        winner1 = p;
                    }
                }

                int egalityTemple = 0;
                foreach (Player p in Players)
                {
                    if (p.Eliminated)
                        continue;

                    if ((3 - p.NbTemple) == maxTemple)
                        egalityTemple++;
                }

                if (egalityTemple >= 2)
                {
                    int maxTower = 0;
                    Player winner2 = null;
                    foreach (Player p in Players)
                    {
                        if (p.Eliminated)
                            continue;

                        if ((2 - p.NbTowers) > maxTower)
                        {
                            maxTower = (2 - p.NbTowers);
                            winner2 = p;
                        }
                    }

                    int egalityTower = 0;
                    foreach (Player p in Players)
                    {
                        if (p.Eliminated)
                            continue;

                        if ((2 - p.NbTowers) == maxTower)
                            egalityTower++;
                    }

                    if (egalityTower >= 2)
                    {
                        int maxBarrack = 0;
                        Player winner3 = null;
                        foreach (Player p in Players)
                        {
                            if (p.Eliminated)
                                continue;

                            if ((20 - p.NbBarrack) > maxBarrack)
                            {
                                maxBarrack = (20 - p.NbBarrack);
                                winner3 = p;
                            }
                        }

                        return winner3;
                    }
                    else
                    {
                        return winner2;
                    }
                }
                else
                {
                    return winner1;
                }
            }
        }

        public void NextPlayer()
        {
            ActualPlayerIndex = (ActualPlayerIndex + 1) % NbPlayers;
        }

        private void PrecedentPlayer()
        {
            ActualPlayerIndex = Math.Abs((ActualPlayerIndex - 1) % NbPlayers);
        }

        public void InitPlay(bool pioche = true, bool nextPlayer = true)
        {
            //MeshRender();
            if (CheckWinner() != null)
            {
                return;
            }

            if (nextPlayer)
            {
                NextPlayer();

                while (ActualPlayer.Eliminated)
                {
                    NextPlayer();
                }
            }

            if (pioche && pile.NbKeeping > 0)
                actualChunk = pile.Draw();

            if (ActualPlayer is AI)
            {
                if (checkIa)
                {
                    actualPhase = TurnPhase.IAPlays;
                    OnChangePhase(actualPhase);
                    AiChunk();
                }
                else
                {
                    NextPhase();
                }
            }
            else
            {
                NextPhase();
            }

            checkIa = true;
        }
        
        public bool InitPlayIA()
        {
            if (CheckWinner() != null)
            {
                return false;
            }

            ActualPlayerIndex++;
            if (ActualPlayerIndex + 1 > NbPlayers)
            {
                ActualPlayerIndex = 0;
            }

            if (actualPlayer.Eliminated)
            {
                ActualPlayerIndex++;
                if (ActualPlayerIndex + 1 > NbPlayers)
                {
                    ActualPlayerIndex = 0;
                }
            }

            this.actualChunk = pile.Draw();

            NextPhaseIA();
            return true;
        }
        
        public void NextPhaseIA()
        {
            int nextPhaseValue = ((int)actualPhase + 1) % (Enum.GetNames(typeof(TurnPhase)).Length - 1);
                actualPhase = (TurnPhase)nextPhaseValue;

                if (actualPhase == TurnPhase.PlaceBuilding)
                {
                    PlayerEliminated();

                    if (actualPlayer.Eliminated)
                        InitPlayIA();
                }

                OnChangePhase(actualPhase);
        }
        public void Phase1IA(Chunk c,PointRotation pr, Rotation r)
        {
            if (gameBoard.AddChunk(c, ActualPlayer, pr, r))
            {
                AddHistoric(pr.point, r, c);
                NextPhaseIA();
                this.maxTurn--;
            }
        }
        public void Phase2IA(PointRotation pr, Building b)
        {
            Cell c = gameBoard.WorldMap[pr.point];
            if (ValidateBuilding(c, b))
            {
                NextPhaseIA();
                InitPlay();
            }
        }
        
        
        

        public void PlayerEliminated()
        {
            if (BarracksSlots().Length == 0 && TempleSlots(ActualPlayer).Length == 0 &&
                TowerSlots(ActualPlayer).Length == 0)
            {
                ActualPlayer.Eliminate();
                OnPlayerElimination(ActualPlayer);
            }
        }

        //Find where the cell is placed inside his parent chunk
        public int CellPositionInChunk(Vector2Int p)
        {
            if (gameBoard.WorldMap.IsVoid(p))
                return -1;

            Cell c = gameBoard.WorldMap[p];
            Chunk chunk = c.ParentChunk;

            return Array.IndexOf(chunk.Coords, c);
        }

        public void Phase1(PointRotation pr, Rotation r, bool ia = false)
        {
            if (ValidateTile(pr, r))
            {
                this.maxTurn--;
                if (!ia)
                    NextPhase();
            }
        }

        //Place building
        public void Phase2(Vector2Int pr, Building b, bool init = true)
        {
            Cell c = gameBoard.WorldMap[pr];
            if (ValidateBuilding(c, b))
            {
                NextPhase();
                InitPlay(init);
            }
        }


        private void AiChunk()
        {
            OnChangePhase(TurnPhase.IAPlays);
            actualPhase = TurnPhase.IAPlays;
            PointRotation pr = ((AI)ActualPlayer).PlayChunk();
            Rotation r = Rotation.N;
            for (int i = 0; i < 6; i++)
            {
                if (pr.rotations[i])
                    r = (Rotation)i;
            }

            OnAIChunkPlacement(pr);
            Phase1(pr, r, true);
        }

        public void ContinueAi()
        {
            NextPhase();
            InitPlay();
        }

        public void AiBuild()
        {
            (Building b, Vector2Int pos) = ((AI)actualPlayer).PlayBuild();
            PointRotation p = new(pos);
            Cell c = gameBoard.WorldMap[p.point];
            PlayerEliminated();
            if (!ActualPlayer.Eliminated)
            {
                List<Vector2Int> build = FindBiomesAroundVillage(p.Point);
                OnAIBuildingPlacement(b, build.ToArray());
                ValidateBuilding(c, b);
            }
        }

        public Texture2D ExportTexture(CustomRenderTexture crt)
        {
            var oldRT = RenderTexture.active;
            // Vector2Int center = CalculateCenter(gameBoard.GetChunksCoords(coord, r));

            Material mat = new Material(Shader.Find("MatTest"));
            var tex = new Texture2D(1024, 1024);
            tex.ReadPixels(new Rect(0, 0, 1024, 1024), 0, 0);
            RenderTexture.active = crt;
            mat.SetVector(Shader.PropertyToID("_Location"), new Vector2(-2, 0));
            mat.SetTexture(Shader.PropertyToID("_BeachMask"), tex);
            crt.Update();
            tex.Apply();
            RenderTexture.active = oldRT;
            return tex;
        }

        // public void MeshRender()
        // {
        //     CustomRenderTexture crt = new CustomRenderTexture(1024, 1024);
        //     Texture2D tex = ExportTexture(crt);
        //     GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //     plane.SetActive(true);
        //     Material mat = new Material(Shader.Find("MeshTest"));
        //     MeshRenderer mr = plane.transform.GetComponentInChildren<MeshRenderer>();
        //     mr.material = mat;
        //     mr.material.SetTexture(Shader.PropertyToID("_test"), tex);
        // }
        //
        // Vector2Int CalculateCenter(Vector2Int[] TriangleCoords)
        // {
        //     Vector2Int center = new Vector2Int();
        //     center.x = TriangleCoords[0].x + TriangleCoords[1].x + TriangleCoords[2].x;
        //     center.y = TriangleCoords[0].y + TriangleCoords[1].y + TriangleCoords[2].y;
        //     return center;
        // }

        public List<Vector2Int> FindBiomesAroundVillage(Vector2Int cell) =>
            gameBoard.FindBiomesAroundVillage(cell, ActualPlayer);


        public bool ValidateTile(PointRotation pr, Rotation r)
        {
            AddHistoric(pr.point, r, actualChunk);
            return gameBoard.AddChunk(actualChunk, ActualPlayer, pr, r);
        }

        public bool ValidateBuilding(Cell c, Building b)
        {
            bool building = false;

            List<Cell> cells = new();
            List<Vector2Int> sameBiomes = FindBiomesAroundVillage(gameBoard.GetCellCoord(c));
            if (sameBiomes.Count > 0)
            {
                foreach (Vector2Int cell in sameBiomes)
                {
                    cells.Add(new(gameBoard.WorldMap[cell]));
                }
            }

            building = gameBoard.PlaceBuilding(c, b, ActualPlayer);

            if (building)
                AddHistoric
                (
                    sameBiomes.Count > 0 ? sameBiomes.ToArray() : new[] { gameBoard.GetCellCoord(c) },
                    cells.Count > 0 ? cells.ToArray() : new[] { c },
                    b
                );

            return building;
        }

        public Vector2Int[] BarracksSlots()
        {
            return gameBoard.GetBarrackSlots(ActualPlayer);
        }

        public Vector2Int[] TowerSlots(Player actualPlayer)
        {
            return gameBoard.GetTowerSlots(ActualPlayer);
        }

        public Vector2Int[] TempleSlots(Player actualPlayer)
        {
            return gameBoard.GetTempleSlots(ActualPlayer);
        }

        public PointRotation[] ChunkSlots()
        {
            return gameBoard.GetChunkSlots();
        }

        public void SetChunkLevel(PointRotation pr)
        {
            gameBoard.SetChunkLevel(pr);
        }

        public bool IsVoid(Vector2Int p)
        {
            return gameBoard.WorldMap.IsVoid(p);
        }

        public int NumberOfAI
        {
            set { throw new NotImplementedException(); }
        }

        public Difficulty AIDifficulty
        {
            set { throw new NotImplementedException(); }
        }

        public int LevelAt(Vector2Int point) => gameBoard.WorldMap[point].ParentChunk.Level;
    }
}