using System;
using System.Collections.Generic;

namespace Agate.Modules.Hexa.Pathfinding 
{
    // A coordinate in a hexagonal grid
    [Serializable] public struct Coordinate 
    {
        public int x;
        public int y;

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public static Coordinate Up => new Coordinate(0, -1);
        public static Coordinate Down => new Coordinate(0, 1);
        public Coordinate NeighbourDownRight => new Coordinate(x, y) + (x % 2 == 0 
            ? new Coordinate(1, 0) 
            : new Coordinate(1, 1));
        public Coordinate NeighbourDownLeft  => new Coordinate(x, y) + (x % 2 == 0
            ? new Coordinate(-1, 0)
            : new Coordinate(-1, 1));
        
        public Coordinate NeighbourDownRightEvenQ => new Coordinate(x, y) + (x % 2 != 0 
            ? new Coordinate(-1, 0) 
            : new Coordinate(-1, 1));
        public Coordinate NeighbourDownLeftEvenQ  => new Coordinate(x, y) + (x % 2 != 0
            ? new Coordinate(1, 0)
            : new Coordinate(1, 1));
        
        
        
        public Coordinate NeighbourDownCenter => new Coordinate(x, y) + Down;
        public static int Distance(Coordinate a, Coordinate b)
        {
            return (int) Math.Sqrt(Math.Pow((b.x - a.x), 2) + Math.Pow((b.y - a.y), 2));
        }
        public override string ToString()
        {
            return $"{this.x}, {this.y}";
        }

        public static bool operator == (Coordinate c1, Coordinate c2) 
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool operator != (Coordinate c1, Coordinate c2) 
        {
            return c1.x != c2.x && c1.y != c2.y;
        }
        public static Coordinate operator + (Coordinate c1, Coordinate c2)
        {
            return new Coordinate(c1.x + c2.x, c1.y + c2.y);
        }
       
    }

    // Data structure within a node in a hexagonal grid
    public interface ISymbol 
    {
        public string Id {get; set;}
        public int Index {get; set;}
        public double Percentage {get; set;}
        public MonstaMatchSymbolTypesEnum Type { get; set; }
        public bool IsSpecial { get; set; }
    }
    
    public enum MonstaMatchSymbolTypesEnum
    {
        Rare = 0,
        General = 1
    }

    // A node in a hexagonal grid
    public interface INode
    {
        public Coordinate coordinate {get; set;}
        public List<Coordinate> neighbours {get; set;}
        public List<Coordinate> upperNeighbours {get; set;}
        public List<Coordinate> lowerNeighbours {get; set;}

    }
}

