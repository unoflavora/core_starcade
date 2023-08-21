using System.Collections.Generic;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Grid;
using Starcade.Arcades.MT02.Scripts.Runtime.Hex.Grid;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Utility
{
    public class HeightComparer : IComparer<GridNode>
    {
        public int Compare(GridNode g1, GridNode g2)
        {
            var c1 = g1.coordinate;
            var c2 = g2.coordinate;
            // Compare the y-coordinates of the tiles
            // So that the paths is traced from the smallest y
            // If the y-coordinate of g1 is less than the y-coordinate of g2, return -1
            if(c1.y < c2.y) return -1;
            // If the y-coordinate of g1 is greater than the y-coordinate of g2, return 1
            if(c1.y > c2.y) return 1;
            // If the y-coordinates are equal, return 0
            return 0;
        }
    }
}