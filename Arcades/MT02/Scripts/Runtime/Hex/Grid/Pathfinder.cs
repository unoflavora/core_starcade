using System;
using System.Collections.Generic;
using System.Linq;
using Agate.Starcade.Assets.Module.HexaCore.Grid;
using Starcade.Arcades.MT02.Scripts.Runtime.Hex.Grid;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Grid
{
    public class BreadthFirstSearch
    {
        private readonly Dictionary<string, GridNode> reached = new();
        private readonly Queue<GridNode> frontier = new();
        private readonly Grid<GridNode> Grid;
        private GridNode currentNode;

        public BreadthFirstSearch(Grid<GridNode> grid)
        {
            Grid = grid;
        }

        public List<GridNode> FindMatchAll()
        {
            reached.Clear();
            return BFSCheck();
        }

        private List<GridNode> BFSCheck()
        {
            List<List<GridNode>> lines = new();

            GridIterator((row, column) =>
            {
                List<GridNode> matches = StartSearch(Grid.GridArray[row, column]);
                ClearNeighborFlags();
                frontier.Clear();
                reached.Clear();
                if (matches != null && matches.Count > 2)
                {
                    lines.Add(matches);
                }
            });

            return lines[new Random().Next(0, lines.Count)];
        }

        private List<GridNode> StartSearch(GridNode startNode)
        {
            var coordinate = $"{startNode.coordinate.x}{startNode.coordinate.y}";
            if (reached.ContainsKey(coordinate)) return null;

            List<GridNode> nodes = new();

            frontier.Enqueue(startNode);
            reached.Add(coordinate, startNode);

            while (frontier.Count > 0)
            {
                currentNode = frontier.Dequeue();
                nodes.Add(currentNode);
                ExploreNeighbors();
            }

            frontier.Clear();
            return nodes;
        }

        private void ExploreNeighbors()
        {
            foreach (var neighbor in currentNode.neighbours)
            {
                string neighborName = Grid.GridArray[neighbor.x, neighbor.y].symbol.Id;
                string nodeName = currentNode.symbol.Id;
                bool isNodeMatch = neighborName == nodeName;

                if (isNodeMatch && !reached.ContainsKey($"{neighbor.x}{neighbor.y}"))
                {
                    var node = Grid.GridArray[neighbor.x, neighbor.y];
                    reached.Add($"{neighbor.x}{neighbor.y}", node);
                    node.IsExplored = true;
                    frontier.Enqueue(node);
                    break;
                }
            }
        }

        private void ClearNeighborFlags()
        {
            GridIterator((row, column) =>
            {
                Grid.GridArray[row, column].IsExplored = false;
            });
        }

        private void GridIterator(Action<int, int> callback)
        {
            for (int row = 0; row < Grid.Width; row++)
            {
                for (int col = 0; col < Grid.Height; col++)
                {
                    if (Grid.GridArray[row, col] != null)
                        callback(row, col);
                }
            }
        }

    }
}