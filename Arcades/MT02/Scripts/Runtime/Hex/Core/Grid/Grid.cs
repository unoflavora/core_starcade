using System;
using System.Collections.Generic;
using Agate.Modules.Hexa.Pathfinding;

namespace Agate.Starcade.Assets.Module.HexaCore.Grid
{
    public class Grid<T> : IGrid<T> where T : INode
    {
        public int Width {get; private set;}
        public int Height {get; private set;}
        public T[,] GridArray {get; private set;}
        public List<T> GridRows { get; private set; }

        // Source: https://www.redblobgames.com/grids/hexagons/
        private List<Coordinate> evenNeighbours = new List<Coordinate>()
        {
            // Upper Neighbours (Left - Right - Middle)
            new Coordinate(-1,  -1), new Coordinate(+1, -1),  new Coordinate(0, -1),
            // Lower Neighbours (Middle - Left - Right)
            new Coordinate(0,  +1), new Coordinate(-1, 0), new Coordinate(+1, 0),
        };

        private List<Coordinate> oddNeighbours = new List<Coordinate>()
        {
            // Upper Neighbours (Left - Right - Middle)
            new Coordinate(-1,  0), new Coordinate(1, 0),  new Coordinate(0, -1),
            // Lower Neighbours (Middle - Left - Right)
            new Coordinate(0, +1), new Coordinate(-1, +1), new Coordinate(+1, +1)
        };

        public Grid(int width, int height, Func<Grid<T>, int, int, T> createGridItem)
        {
            this.Width = width;
            this.Height = height;
            this.GridArray = new T[width, height];
            this.GridRows = new List<T>();

            for(int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridArray[x,y] = createGridItem(this, x, y);
                    if (GridArray[x,y] != null) 
                    {
                        GridArray[x,y].coordinate = new Coordinate(x,y);
                    }
                }
            }
            
            SetupNeighbours();
            SetupRows();
        }

        private void SetupRows()
        {
            for (int y = 0; y < Height; y++)
            {
                var oddRow = new List<T>();
                var evenRow = new List<T>();
                for (int x = 0; x < Width; x++)
                {
                    var item = GridArray[x, y];
                    if (item != null)
                    {
                        if(x % 2 == 0) evenRow.Add(item);
                        if(x % 2 == 1) oddRow.Add(item);
                    }
                }
                
                GridRows.AddRange(evenRow);
                GridRows.AddRange(oddRow);
            }
        }

        private void SetupNeighbours()
        {
            for(int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if(GridArray[x,y] != null)
                    {
                        GridArray[x,y].neighbours = new List<Coordinate>();
                        GridArray[x,y].lowerNeighbours = new List<Coordinate>();
                        GridArray[x,y].upperNeighbours = new List<Coordinate>();

                        var odd = x % 2 == 1;

                        var neighbours = odd ? oddNeighbours : evenNeighbours;

                        for (int i = 0; i < neighbours.Count; i++)
                        {
                            var neighbourCoordinate = neighbours[i];
                            
                            var _x = x + neighbourCoordinate.x;
                            var _y = y + neighbourCoordinate.y;
                            if (_x < Width && _y < Height && _x >= 0 && _y >= 0) 
                            {
                                if (GridArray[_x, _y] != null)
                                {
                                    var coordinate = new Coordinate(_x, _y);
                                    GridArray[x,y].neighbours.Add(coordinate);
                                    if(i < 3) GridArray[x,y].upperNeighbours.Add((coordinate));
                                    else GridArray[x,y].lowerNeighbours.Add((coordinate));
                                }
                            }
                            
                        }

                    }
                }
            }
        }

    }

    public interface IGrid<out T> where T : INode
    {
        public int Width {get;}
        public int Height {get;}
        public T[,] GridArray {get;}
    }
}
