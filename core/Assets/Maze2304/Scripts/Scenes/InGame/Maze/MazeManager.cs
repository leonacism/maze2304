using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.InGame.Maze
{
    public class MazeManager : MonoBehaviour
    {
        public bool specifySeed = false;
        public int seed = 0;
        public float removeWallProbability = 0.05f;
    
        public int width = 50;
        public int height = 50;

        public GameObject wallPrefab;
        public GameObject ceilPrefab;
        public GameObject pillarPrefab;
        public GameObject goalPrefab;


        private MazeGenerator mazeGenerator;

        private HashSet<(int, int)> pillarCoords;
        private HashSet<(int, int)> tangentWallCoords;
        private HashSet<(int, int)> normalWallCoords;

        private void Awake()
        {
            mazeGenerator = new PrimMazeGenerator(specifySeed? seed : (int) DateTime.Now.ToBinary(), removeWallProbability);
        }

        private void Start()
        {
            var maze = mazeGenerator.GenerateMaze(width, height);

            CreateCoords(maze);
            
            CreateMazeObjects();

            CreateGoalObject();
        }

        private void CreateCoords(MazeCell[,] maze)
        {
            pillarCoords = new HashSet<(int, int)>();
            tangentWallCoords = new HashSet<(int, int)>();
            normalWallCoords = new HashSet<(int, int)>();
            
            for (int x = 0; x < width; x++) for (int y = 0; y < height; y++)
            {
                var cell = maze[x, y];

                for (int i = 0; i < 4; i++) if (cell.walls[i])
                {
                    switch(i)
                    {
                        case 0: // 上
                            pillarCoords     .Add((x  ,y+1));
                            normalWallCoords .Add((x  ,y+1));
                            pillarCoords     .Add((x+1,y+1));
                            break;
                        case 1: // 右
                            pillarCoords     .Add((x+1,y+1));
                            tangentWallCoords.Add((x+1,y  ));
                            pillarCoords     .Add((x+1,y  ));
                            break;
                        case 2: // 下
                            pillarCoords     .Add((x+1,y  ));
                            normalWallCoords .Add((x  ,  y));
                            pillarCoords     .Add((x  ,y  ));
                            break;
                        case 3: // 左
                            pillarCoords     .Add((x  ,y  ));
                            tangentWallCoords.Add((x,  y  ));
                            pillarCoords     .Add((x  ,y+1));
                            break;
                    }
                }
            }
        }


        private void CreateMazeObjects()
        {
            float wallWidth = wallPrefab.transform.lossyScale.z;

            foreach (var (x, y) in pillarCoords)
            {
                var pillarPosition = new Vector3(wallWidth * (x - 0.5f), 0, wallWidth * (y - 0.5f));
                Instantiate(pillarPrefab, pillarPosition, Quaternion.Euler(0, 0, 0), transform);
            }

            foreach (var (x, y) in tangentWallCoords)
            {
                var wallPosition = new Vector3(wallWidth * (x - 0.5f), 0, wallWidth * y);
                Instantiate(wallPrefab, wallPosition, Quaternion.Euler(0, 0, 0), transform);
            }

            foreach (var (x, y) in normalWallCoords)
            {
                var wallPosition = new Vector3(wallWidth * x, 0, wallWidth * (y - 0.5f));
                Instantiate(wallPrefab, wallPosition, Quaternion.Euler(0, 90, 0), transform);
            }

            for (int x = 0; x < width; x++) for (int y = 0; y < height; y++)
            {
                var ceilPosition = new Vector3(wallWidth * x, 0, wallWidth * y);
                Instantiate(ceilPrefab, ceilPosition, Quaternion.Euler(0, 0, 0), transform);
            }
        }

        private void CreateGoalObject()
        {
            float wallWidth = wallPrefab.transform.lossyScale.z;

            var goalPosition = new Vector3(wallWidth * (width - 1), 1, wallWidth * (height - 1));
            GameObject obj = Instantiate(goalPrefab, goalPosition, Quaternion.Euler(45, 45, 45), transform);
        }
    }
}
