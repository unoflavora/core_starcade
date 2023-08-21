using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agate.Modules.Hexa.Pathfinding;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Grid;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Symbols;
using Agate.Starcade.Scripts.Runtime.Utilities;

namespace Starcade.Arcades.MT02.Scripts.Runtime.Hex.Grid
{
    public class Filler
    {
        private GridManager _gridManager;

        public Filler(GridManager gridManager)
        {
            _gridManager = gridManager;
        }

        public async Task Fill()
        {
            _gridManager.Paths = new List<GridNode>();
            var nullExist = true;
            var movingItems = new List<MonstamatchSymbol>();
            while(nullExist)
            {
                nullExist = false;
                for (int y = _gridManager.Grid.Height - 1; y > -1; y--)
                {
                    for (int x = 0; x < _gridManager.Grid.Width; x++)
                    {
                        var currentItem = _gridManager.Grid.GridArray[x, y];
                        if (currentItem == null) continue;
                        var isFilled = Fill(currentItem);
                        if (isFilled)
                        {
                            movingItems.Add((currentItem.symbol));
                            nullExist = true;
                        }
                    }
                }
             
                
            }
            
            _gridManager.ClearUpperQueue();
            _gridManager.SetItemMovable(true);
            
            await AsyncUtility.WaitUntil(() => movingItems.TrueForAll(item => item.Paths.Count == 0));

            _gridManager.SetItemMovable(false);
        }

        public async Task RemovePaths(int objectCount)
        {
            await Task.WhenAll(_gridManager.Paths.Select(path => { return path.DestroySymbol(objectCount); }).ToArray());
            ArcadeAudioHelper.PlayMatchPop();
        }


        private bool Fill(GridNode item)
        {
            if (item.symbol.Data == null)
            {
                if (item.coordinate.y == 0)
                {
                    FillFirstRow(item);
                }
                return true;
            }
            
            if (CoordinateExist(item.coordinate.NeighbourDownCenter))
            {
                var coordinate = item.coordinate.NeighbourDownCenter;
                var itemToCheck = _gridManager.Grid.GridArray[coordinate.x, coordinate.y];
                if (itemToCheck != null && itemToCheck.symbol.Data == null)
                {
                    Swap(item, itemToCheck);
                    return true;
                }
            }

            if (CoordinateExist(item.coordinate.NeighbourDownLeft))
            {
                var neighbourDownLeft = _gridManager.Grid.GridArray[item.coordinate.NeighbourDownLeft.x, item.coordinate.NeighbourDownLeft.y];
                if (neighbourDownLeft != null && neighbourDownLeft.symbol.Data == null)
                {
                    var isFill = FillBelow(item, neighbourDownLeft);
                    if (isFill) return true;
                }
            }
            
            // cek kanan bawahnya
            if (CoordinateExist(item.coordinate.NeighbourDownRight))
            {
                var neighbourDownRight = _gridManager.Grid.GridArray[item.coordinate.NeighbourDownRight.x, item.coordinate.NeighbourDownRight.y];
                if (neighbourDownRight != null && neighbourDownRight.symbol.Data == null)
                {
                    var isFill = FillBelow(item, neighbourDownRight);
                    if (isFill) return true;
                }
            }
            
            return false;
        }

        private bool CoordinateExist(Coordinate coordinateRightBelow)
        {
            return coordinateRightBelow.x < _gridManager.Grid.Width && coordinateRightBelow.y < _gridManager.Grid.Height && coordinateRightBelow.x >= 0 && coordinateRightBelow.y >= 0;
        }

        private bool FillBelow(GridNode item, GridNode itemToCheck)
        {
            var coordinateAbove = itemToCheck.coordinate + Coordinate.Up;
            if (CoordinateExist(coordinateAbove))
            {
                var aboveItem = _gridManager.Grid.GridArray[coordinateAbove.x, coordinateAbove.y];
                if (aboveItem != null && aboveItem.symbol.Data != null)
                {
                    return false;
                }

                Swap(item, itemToCheck);
                return true;
            }

            return false;
        }

        private static void Swap(GridNode source, GridNode itemToFill)
        {
            itemToFill.symbol.SetupData(source.symbol.Data, source.symbol.Paths);
            itemToFill.symbol.isMoving = source.symbol.isMoving;

            source.symbol.SetupData(null);
            source.symbol.isMoving = false;

            if (itemToFill.symbol.Paths.Count > 0)
            {
                var startPos = source.symbol.startingPos;
                itemToFill.symbol.startingPos = startPos;
                itemToFill.symbol.SetPosition(startPos);
            }

            itemToFill.symbol.AddPath(source.transform.position, itemToFill.transform.position);
        }

        private void FillFirstRow(GridNode item)
        {
            var symbolData = MonstaMatchSymbolPicker.PickNextSymbol(_gridManager.SymbolsToGenerate, _gridManager.Rand);
            item.symbol.SetupData(symbolData);
            var indexY = 0;

            for (int _y = 1; _y < _gridManager.height; _y++)
            {
                var nullableItem = _gridManager.ShadowGrid.GridArray[item.coordinate.x, _y];
                if (nullableItem == null) continue;

                if (nullableItem.symbol.Data == null)
                {
                    indexY = _y;
                    nullableItem.symbol.SetupData(item.symbol.Data);
                    break;
                }
            }

            if (indexY == 0) indexY = _gridManager.height - 1;
            
            var from = _gridManager.GetWorldPosition(item.coordinate.x, -indexY);
            
            item.symbol.AddPath(from, item.transform.position);

            // for (int i = indexY; i > 0; i--)
            // {
            //     var fromCoord = new Coordinate(item.coordinate.x, -i);
            //     var toCoord = new Coordinate(item.coordinate.x, -(i - 1));
            //     var from = _gridManager.GetWorldPosition(fromCoord.x, fromCoord.y);
            //     var to = _gridManager.GetWorldPosition(toCoord.x, toCoord.y);
            // }
        }
    }
}