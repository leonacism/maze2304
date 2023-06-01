namespace Scenes.InGame.Maze
{
    public abstract class MazeGenerator
    {
        public MazeCell[,] GenerateMaze(int width, int height)
        {
            MazeCell[,] maze = new MazeCell[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    maze[x, y] = new MazeCell(x, y);
                }
            }

            Generate(maze);

            RemoveWalls(maze);

            return maze;
        }

        protected abstract void Generate(MazeCell[,] maze);

        protected abstract void RemoveWalls(MazeCell[,] maze);
    }
}
