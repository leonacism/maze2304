using Common.Random;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scenes.InGame.Maze
{
    public class MapDebug : MonoBehaviour
    {
        static private int[] dx = {0, 1, 0, -1};
        static private int[] dy = {1, 0, -1, 0};

        public bool specifySeed = false;
        public int seed = 0;
        public GameObject tilePrefab;

        private RandomGenerator rndGen;
        private HashSet<(int, int)> visited;

        private void Start()
        {
            rndGen = new RandomGenerator(specifySeed? seed : (int) DateTime.Now.ToBinary());
            visited = new HashSet<(int, int)>();

            List<MazeCell> maze = new List<MazeCell>();
            
            int x = 0, y = 0;
            for (int i = 0; i < 50; i++)
            {
                maze.Add(new MazeCell(x, y));
                visited.Add((x, y));

                var dir = rndGen.Range(0, 4);

                while (visited.Contains((x + dx[dir], y + dy[dir])))
                {
                    (x, y) = visited.ElementAt(rndGen.Range(0, visited.Count));
                    dir = rndGen.Range(0, 4);
                }

                //Debug.Log($"{(x, y)} -> {(x + dx[dir], y + dy[dir])}");

                x += dx[dir];
                y += dy[dir];
            }
            
            for (int i = 0; i < maze.Count; i++)
            {
                var cell = maze[i];
                var value = (i + 1f) / maze.Count;
                fill(cell.x, cell.y, new Color(value, value, value));
            }
        }

        private void fill(int x, int y, Color color)
        {
            GameObject obj = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.Euler(-90, 0, 0));
            Material material = obj.GetComponent<Renderer>()?.material;
            if (material != null)
            {
                material.color = color;
            }
        }
    }
}
