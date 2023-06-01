using Common.Random;
using System.Collections.Generic;

namespace Scenes.InGame.Maze
{
    public class PrimMazeGenerator : MazeGenerator
    {
        static private int[] dx = {0, 1, 0, -1};
        static private int[] dy = {1, 0, -1, 0};

        private RandomGenerator rndGen;
        private float removeWallProbability;

        public PrimMazeGenerator(int seed, float removeWallProbability = 0.1f)
        {
            rndGen = new RandomGenerator(seed);
            this.removeWallProbability = removeWallProbability;
        }

        protected override void Generate(MazeCell[,] maze)
        {
            var frontier = new List<MazeCell>();

            // 始点を決める
            var start = maze[0, 0];
            start.visited = true;

            AddNeighborsToFrontier(maze, start, frontier);

            while (frontier.Count > 0)
            {
                var current = frontier[rndGen.Range(0, frontier.Count)];

                var visitedNeighbor = GetRandomVisitedNeighbor(maze, current);
                if (visitedNeighbor != null)
                {
                    RemoveWallBetween(current, visitedNeighbor);
                }

                current.visited = true;
                frontier.Remove(current);
                AddNeighborsToFrontier(maze, current, frontier);
            }
        }

        private void AddNeighborsToFrontier(MazeCell[,] maze, MazeCell cell, List<MazeCell> frontier)
        {
            var width = maze.GetLength(0);
            var height = maze.GetLength(1);

            for (int i = 0; i < 4; i++)
            {
                var x = cell.x + dx[i];
                var y = cell.y + dy[i];

                if (0 <= x && x < width && 0 <= y && y < height)
                {
                    var neighborCell = maze[x, y];

                    if (!neighborCell.visited && !frontier.Contains(neighborCell))
                    {
                        frontier.Add(neighborCell);
                    }
                }
            }
        }

        private MazeCell GetRandomVisitedNeighbor(MazeCell[,] maze, MazeCell cell)
        {
            var width = maze.GetLength(0);
            var height = maze.GetLength(1);
            var visitedNeighbors = new List<MazeCell>();

            for (int i = 0; i < 4; i++)
            {
                var x = cell.x + dx[i];
                var y = cell.y + dy[i];

                if (0 <= x && x < width && 0 <= y && y < height)
                {
                    var neighborCell = maze[x, y];

                    if (neighborCell.visited)
                    {
                        visitedNeighbors.Add(neighborCell);
                    }
                }
            }

            return visitedNeighbors.Count > 0? visitedNeighbors[rndGen.Range(0, visitedNeighbors.Count)] : null;
        }

        private void RemoveWallBetween(MazeCell a, MazeCell b)
        {
            if (a.x == b.x)
            {
                if (a.y > b.y) // aがbの上に隣接している場合
                {
                    a.walls[2] = false; // 下
                    b.walls[0] = false; // 上
                }
                else // aがbの下に隣接している場合
                {
                    b.walls[2] = false; // 下
                    a.walls[0] = false; // 上
                }
            }
            else if (a.y == b.y)
            {
                if (a.x > b.x) // aがbの右に隣接している場合
                {
                    b.walls[1] = false; // 右
                    a.walls[3] = false; // 左
                }
                else // aがbの左に隣接している場合
                {
                    a.walls[1] = false; // 右
                    b.walls[3] = false; // 左
                }
            }
        }

        protected override void RemoveWalls(MazeCell[,] maze)
        {
            var width = maze.GetLength(0);
            var height = maze.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var x_ = x + dx[i];
                        var y_ = y + dy[i];

                        if (0 <= x_ && x_ < width && 0 <= y_ && y_ < height)
                        {
                            if (rndGen.Range(0f, 1f) < removeWallProbability)
                            {
                                maze[x , y ].walls[i]           = false;
                                maze[x_, y_].walls[(i + 2) % 4] = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
