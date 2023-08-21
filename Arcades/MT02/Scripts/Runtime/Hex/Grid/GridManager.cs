using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agate.Modules.Hexa.Pathfinding;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Backend.Data;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Grid;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Symbols;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Scriptable_Objects;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Utility;
using Starcade.Arcades.MT02.Scripts.Runtime.Hex.Calculator;
using UnityEngine;
using UnityEngine.Events;
using _grid = Agate.Starcade.Assets.Module.HexaCore.Grid;

namespace Starcade.Arcades.MT02.Scripts.Runtime.Hex.Grid
{
    public class GridManager : MonoBehaviour
    {
        [Header("Grid Config")]
        [SerializeField] private int width = 10;
        [SerializeField] public int height = 6;
        [SerializeField] private float cellSizeWidth = 10f;        
        [SerializeField] private float cellSizeHeight = 10f;
        [SerializeField] private int minMatch = 3;
        [SerializeField] private int idleTimeLimit = 10;
        
        [Header("Grid Object Needs")]
        [SerializeField] private MonstarmatchSymbols monstaMatchSymbols;
        [SerializeField] private Transform gridPrefab;
        [SerializeField] private GameObject shadowGrids;
        
        [Header("Grid State")]
        [SerializeField] private float _idleTime = 0;
        [SerializeField] private bool _isTimerPaused;
        [SerializeField] private bool _isClicked;
        [SerializeField] private bool _isRemovingPaths;
        private bool _isTutorial;
        
        private BreadthFirstSearch _pathfinder;

        #region Grid State
        public int RemainingSpinChance
        {
            get => _remainingSpinChance;
            set
            {
                _remainingSpinChance = value;
                if (_remainingSpinChance == 0)
                {
                    RunTimer(false);
                }
                else
                {
                    RunTimer(true);
                }
            }
        }

        public bool ProcessingSpin { get; set; }
        public bool ProcessingJackpot { get; set; }
        public bool IsRemovingPaths => _isRemovingPaths;

        private List<GridNode> _higlightedNode;
        private int _remainingSpinChance;
        

        #endregion
        
        #region Grid Data
        private int _matchCounter;
        public _grid.Grid<GridNode> Grid { get; set; }
        public _grid.Grid<GridNode> ShadowGrid;
        public List<MonstamatchSymbolData> SymbolsToGenerate { get; set; }
        public LinearCongruentialGenerator Rand { get; private set; }
        public List<GridNode> Paths;
        private System.Random _randomizer;
        private Filler _filler;
        private List<MonstaMatchSymbolSpecialConfig> _specialCoinConfig;
        private List<MonstamatchSymbolRewardConfig> _rewardConfig;
        public List<MonstamatchSymbolData> Symbols;
        [NonSerialized] public UnityEvent<CalculatedMatchResult, List<Coordinate>> OnMatchTiles;
        
        public bool IsClicked
        {
            get => _isClicked;
            set => _isClicked = value;
        }
        
        #endregion

        private void Awake()
        {
            OnMatchTiles = new UnityEvent<CalculatedMatchResult, List<Coordinate>>();
            _higlightedNode = new List<GridNode>();
            
            RunTimer(false);
        }

        private void FixedUpdate()
        {
            if (_isRemovingPaths || ProcessingSpin || ProcessingJackpot) return;
            
            CountTimer();

            if (Input.GetMouseButton(0) && _isClicked) GeneratePaths();

            else RemovePaths();

            if (_idleTime >= idleTimeLimit && !_isTutorial) ShowEligiblePath();
        }

        private void CountTimer()
        {
            if (_isTimerPaused) return;
            
            _idleTime += Time.fixedDeltaTime;
        }

        private void ShowEligiblePath()
        { 
            if (_higlightedNode.Count > 0) return;
            
           var list =  _pathfinder.FindMatchAll();
           _higlightedNode = list;
           foreach (var match in _higlightedNode)
           {
               match.EnableHighlight(true);
           }
        }
        
        private void Start()
        {
            ProcessingSpin = false;
            
            Paths = new List<GridNode>();
            ShadowGrid = new _grid.Grid<GridNode>(width, height, (grid, x, y) => {
                var odd = x % 2 == 1;

                if (odd && y > height - 2) return null;
                
                // initiating objects
                var prefabTransform = Instantiate(gridPrefab, GetWorldPosition(x, -y), Quaternion.identity, shadowGrids.transform);
                var gridObject = prefabTransform.GetComponent<GridNode>();
                
                gridObject.transform.Find("Hex").gameObject.SetActive(false);
                gridObject.text.gameObject.SetActive(false); 
                gridObject.symbol.SetupData(null);
                gridObject.HideSelectedVfx();
                gridObject.RemovePointer();
                gridObject.EnableHighlight(false);
       
                return gridObject;
            });
            SpawnEmptyBoard();
            // var lcgKey = long.Parse(DateTime.Now.ToString("MMddHHmmssfff"));
        }
        public void Init(MonstamatchInitData data)
        {
            _specialCoinConfig = data.Config.SpecialCoinConfig;
            Symbols = data.Config.Symbols;
            _rewardConfig = data.Config.RewardSymbols;
            _isTutorial = data.IsTutorial;
            
            foreach (var symbol in Symbols)
            {
                symbol.sprite = GetSpriteForSymbol(symbol);
            }
            
            SymbolsToGenerate = Symbols.Where(t => t.Type != MonstaMatchSymbolTypesEnum.Rare).ToList();
            _filler = new Filler(this);
        }

        private Sprite GetSpriteForSymbol(MonstamatchSymbolData symbol)
        {
            var symbolData = monstaMatchSymbols.symbols.Find(s => s.Id == symbol.Id);
            if (symbolData != null) return symbolData.sprite;
            return null;
        }

        private void ResetTimer()
        {
            _idleTime = 0;
        }

        public void RunTimer(bool run)
        {
            _isTimerPaused = !run;
            
            if(_isTimerPaused) ResetTimer();
            Debug.Log("Timer is Set to " + run);
        }

        public void RandomizeBoardSymbols()
        {
            //spawn board (empty symbol)
            SpawnEmptyBoard();

            //spawn rare symbol
            var isSpawnRare = Rand.Next(100) < Symbols[0].Percentage;
            List<Coordinate> rareSymbolCoordinates = new();
            if (isSpawnRare) rareSymbolCoordinates = SpawnRareSymbols();
            
            //spawn general symbol, skips rare symbol
            var symbolPercentages = SymbolsToGenerate.Select(t => t.Percentage).ToList();
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    if (rareSymbolCoordinates.Any(t => t.x == x && t.y == y)) continue;
                    if (Grid.GridArray[x, y] != null)
                    {
                        var symbolIndexPicked = MonstaMatchSymbolPicker.PickNextSymbol(SymbolsToGenerate, Rand);
                        MonstamatchSymbolData newSymbol = symbolIndexPicked;
                        newSymbol.sprite = GetSpriteForSymbol(newSymbol);
                        Grid.GridArray[x, y].symbol.SetupData(newSymbol, null);
                        Grid.GridArray[x, y].symbol.gameObject.SetActive(false);
                    }
                }
            }

            //spawn special coins
            var specialPickIndex = PickIndex(_specialCoinConfig.Select(t => t.Percentage).ToList(), Rand);
            var specialPicked = _specialCoinConfig[specialPickIndex];
            if (specialPicked.Amount > 0) SpawnSpecialCoins(specialPicked.Amount);
        }
     
        private void GeneratePaths()
        {
            ResetTimer();
            if (_remainingSpinChance < 1) return;
            var pos = Input.mousePosition;
            pos.z = Camera.main.transform.position.z;
            var mousePos = Camera.main.ScreenToWorldPoint(pos);

            GetCoordinate(mousePos, out int x, out int y);

            if (x >= 0 && y >= 0)
            {

                var item = Grid.GridArray[x, y];
                if (item == null) return;
                if (item.symbol.Data.sprite == null) return;

                if (Paths.Count > 0 && !Paths[Paths.Count - 1].neighbours.Contains(item.coordinate)) return;
                if (Paths.Count > 0 && item.symbol.Id != Paths[0].symbol.Id) return;

                // Adding Item
                if (!Paths.Contains(item))
                {
                    if (Paths.Count > 0)
                    {
                        Paths[Paths.Count - 1].EnablePointerTo(item.transform.position);
                        Paths[Paths.Count - 1].ScaleDownSymbol();
                    }
                    
                    item.ShowSelectedVfx();
                    item.ScaleUpSymbol();
                    Paths.Add(item);
                    ArcadeAudioHelper.PlayMatchNotes(Paths.Count <= 8 ? Paths.Count : 8);
                }

                // Removing Item
                if (Paths.Count > 1 && Paths.Contains(item) && item == Paths[Paths.Count -2])
                {
                    Paths[Paths.Count -1].HideSelectedVfx();

                    Paths[Paths.Count -2].RemovePointer();
                    Paths[Paths.Count -2].ScaleUpSymbol();

                    Paths.RemoveAt(Paths.Count - 1);
                    ArcadeAudioHelper.PlayMatchNotes(Paths.Count <= 8 ? Paths.Count : 8);
                }
            }
        }

        private GridNode ObjectCreator(_grid.Grid<GridNode> grid, int x, int y)
        {
            var odd = x % 2 == 1;

            if (odd && y > height - 2) return null;
            
            // initiating objects
            var prefabTransform = Instantiate(gridPrefab, GetWorldPosition(x, y), Quaternion.identity, transform);
            var gridObject = prefabTransform.GetComponent<GridNode>();
            
            // // debugging tile coordinate            
            gridObject.text.text = x + "," + y;
            gridObject.transform.name = x + "," + y;
            
            // unselect object
            gridObject.HideSelectedVfx();
            gridObject.EnableHighlight(false);
            
            return gridObject;
        }
        private async void RemovePaths()
        {
            if (Paths.Count <= 0) return;
            
            ResetTimer();

            foreach(var path in Paths)
            {
                path.HideSelectedVfx();
                path.RemovePointer();
            }

            if (Paths.Count < minMatch)
            {
                Paths.Clear();
                return;
            }
            
            _isRemovingPaths = true;

            foreach (var node in _higlightedNode)
            {
                node.EnableHighlight(false);
            }
            
            _higlightedNode.Clear();
            
            RemainingSpinChance--;
            
            var result = MatchCalculator.CalculateMatchResult(Paths, _rewardConfig);
            await _filler.RemovePaths(Paths.Count);
            OnMatchTiles.Invoke(result, Paths.Select(path => path.coordinate).ToList());
        }

        public async Task FillEmptyPaths()
        {
            await _filler.Fill();
            
            _isRemovingPaths = false;
        }


        public void ClearUpperQueue()
        {
            for (int x = 0; x < width; x++)
            for (int _y = 1; _y < height; _y++)
                if (ShadowGrid.GridArray[x, _y] != null)
                {
                    ShadowGrid.GridArray[x, _y].symbol.SetupData(null);
                }
        }
        
        public void SetItemMovable(bool enabled)
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++) 
                if (Grid.GridArray[x, y] != null) Grid.GridArray[x, y].symbol.IsMovable = enabled;
        }

        public async Task Spin(long lcgKey, List<MonstamatchSymbolData> boardState = null)
        {
            ProcessingSpin = true;
            
            InitiateRandom(lcgKey);
            
            foreach (var node in _higlightedNode)
            {
                node.EnableHighlight(false);
            }
            _higlightedNode.Clear();

            for (var x = 0; x < Grid.Width; x++)
            {
                for (var y = 0; y < Grid.Height; y++)
                {
                    var item = Grid.GridArray[x, y];
                    if (item != null)
                    {
                        item.symbol.IsMovable = true;
                    
                        var to = new Vector3(item.transform.position.x, item.transform.position.y - (cellSizeHeight * height) - cellSizeHeight, item.transform.position.z);
                    
                        item.symbol.AddPath(item.transform.position, to);
                    }
                }

                await Task.Delay(100);
            }

            while (Grid.GridArray[width - 1, height - 1].symbol.isMoving) await Task.Yield();
            
            await ShowSymbols(boardState);
            
            ProcessingSpin = false;
        }

        public void InitiateRandom(long lcgKey)
        {
            Rand = new LinearCongruentialGenerator(lcgKey);
            // string lcgs = "";
            // Debug.Log(lcgKey);
            // for (int i = 0; i < 100; i++)
            // {
            //     lcgs += Rand.Next(100);
            //     lcgs += " ";
            // }
            // Debug.Log(lcgs);
        }
        
        public async Task ShowSymbols(List<MonstamatchSymbolData> boardState)
        {
            if (boardState == null) RandomizeBoardSymbols();
            else
            {
                var i = 0;
                
                GridIterator(item =>
                {
                    var data = boardState[i];
                    data.sprite = GetSpriteForSymbol(data);
                    item.symbol.SetupData(data);
                    item.symbol.SetPosition(new Vector3(100,100, 100));
                    i++;
                });
            }

            for (var x = 0; x < Grid.Width; x++)
            {
                for (var y = 0; y < Grid.Height; y++)
                {
                    var item = Grid.GridArray[x, y];
                    if (item != null)
                    {
                        item.symbol.IsMovable = true;
                        var from = new Vector3(item.transform.position.x, item.transform.position.y + (cellSizeHeight * height) + cellSizeHeight, item.transform.position.z);
                        item.symbol.AddPath(from, item.transform.position, true);
                        Grid.GridArray[x, y].symbol.gameObject.SetActive(true);
                    }
                }

                await Task.Delay(80);
            }
            
            while (Grid.GridArray[width - 1, height - 1].symbol.isMoving) await Task.Yield();
        }

        private void GridIterator(UnityAction<GridNode> callback)
        {
            for (var x = 0; x < Grid.Width; x++)
            {
                for (var y = 0; y < Grid.Height; y++)
                {
                    var item = Grid.GridArray[x, y];
                    if (item != null)
                    {
                        callback(item);
                    }
                }
            }
        }


        private void SpawnEmptyBoard()
        {
            if (Grid == null)
            {
                Grid = new _grid.Grid<GridNode>(width, height, ObjectCreator);
                _pathfinder = new BreadthFirstSearch(Grid);
            }
            else
            {
                for (int x = 0; x < Grid.Width; x++)
                {
                    for (int y = 0; y < Grid.Height; y++)
                    {
                        if (Grid.GridArray[x, y] != null)
                        {
                            Grid.GridArray[x,y].symbol.SetupData(null);
                        }
                    }
                }
            }
        }
        private void SpawnSpecialCoins(int amount)
        {
            int spawned = 0;
            while (spawned < amount)
            {
                int x = (int)Rand.Next(Grid.Width - 1);
                int y = (int)Rand.Next(Grid.Height - 1);
                if (Grid.GridArray[x, y] != null)
                {
                    if (Grid.GridArray[x, y].symbol.Data.Type != MonstaMatchSymbolTypesEnum.Rare &&
                        !Grid.GridArray[x, y].symbol.Data.IsSpecial)
                    {
                        Grid.GridArray[x, y].symbol.SetSymbolSpecial(true);
                        spawned++;
                    }
                }
            }
        }
        private List<Coordinate> SpawnRareSymbols()
        {
            var selectedX = (int)Rand.Next(Grid.Width - 1);
            var selectedY = (int)Rand.Next(Grid.Height - 1);
            List<Coordinate> rareSymbolNeighbour = Grid.GridArray[selectedX, selectedY].neighbours;
            int pos1 = (int)Rand.Next(rareSymbolNeighbour.Count);
            int pos2 = (int)Rand.Next(rareSymbolNeighbour.Count);
            while (pos2 == pos1) pos2 = (int)Rand.Next(rareSymbolNeighbour.Count);

            List<Coordinate> coordinateToSpawnRareSymbol = new() {
                new Coordinate(selectedX, selectedY),
                rareSymbolNeighbour[pos1],
                rareSymbolNeighbour[pos2]
            };

            foreach (var item in coordinateToSpawnRareSymbol)
            {
                Grid.GridArray[item.x, item.y].symbol.SetupData(Symbols[0]);
                Grid.GridArray[item.x, item.y].symbol.SetSymbolSpecial(false);
            }

            return coordinateToSpawnRareSymbol;
        }
        private int PickIndex(List<double> itemPercentages, LinearCongruentialGenerator rnd)
        {
            var totalPercentage = itemPercentages.Sum();
            var targetPercentage = rnd.Next(Convert.ToInt64(totalPercentage));
            var targetIndex = -1;
            double accumulatedPercentage = 0;
            for (int i = 0; i < itemPercentages.Count; i++)
            {
                var p = itemPercentages[i];
                accumulatedPercentage += p;
                if (targetPercentage <= accumulatedPercentage)
                {
                    targetIndex = i;
                    break;
                }
            }
            return targetIndex;
        }
        public Vector3 GetWorldPosition(int x, int y)
        {
            var yPos = y * -cellSizeHeight;
            var xPos = x * cellSizeWidth * .75f;
            if (x % 2 == 1) yPos += -cellSizeHeight * .5f;

            var position = new Vector3(xPos, yPos - transform.position.y  , 0);
            return position;
        }
        
        private void GetCoordinate(Vector3 worldPosition, out int x, out int y)
        {
            var roughX = Mathf.RoundToInt((worldPosition.x) / cellSizeWidth / .75f);
            var offset = (roughX % 2 == 1 ? -cellSizeHeight * .5f : 0);
            var roughY = Mathf.RoundToInt((worldPosition.y + transform.position.y - offset) / -cellSizeHeight);

            if (roughX > width - 1 || roughY > height - 1 || roughX < 0 || roughY < 0)
            {
                x = -1;
                y = -1;
                return;
            };

            GridNode gridNode = Grid.GridArray[roughX, roughY];
            Coordinate closestXY = new Coordinate(roughX, roughY);

            if(gridNode != null && gridNode.neighbours != null && gridNode.neighbours.Count > 1)
            {
                foreach (var neighbor in gridNode.neighbours)
                {
                    var _x = neighbor.x;
                    var _y = neighbor.y;

                    if (Vector3.Distance(worldPosition, GetWorldPosition(_x, _y)) < Vector3.Distance(worldPosition, GetWorldPosition(closestXY.x, closestXY.y)))
                    {
                        closestXY = neighbor;
                    }
                }
            }
            

            x = closestXY.x;
            y = closestXY.y;
        }
    }
}