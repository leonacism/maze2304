namespace Scenes.InGame.Maze
{
    public class MazeCell
    {
        public int x, y;
        public bool visited = false;
        public bool[] walls = {true, true, true, true}; // 上, 右, 下, 左

        public MazeCell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
