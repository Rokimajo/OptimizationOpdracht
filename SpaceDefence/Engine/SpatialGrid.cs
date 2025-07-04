using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SpaceDefence;

public class SpatialGrid
    {
        private readonly int _cellSize;
        private readonly int _gridWidth;
        private readonly int _gridHeight;
        private readonly List<GameObject>[,] _grid;

        public SpatialGrid(Rectangle worldBounds, int cellSize = 64)
        {
            _cellSize = cellSize;
            _gridWidth = (worldBounds.Width / cellSize) + 1;
            _gridHeight = (worldBounds.Height / cellSize) + 1;
            _grid = new List<GameObject>[_gridWidth, _gridHeight];
            
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    _grid[x, y] = new List<GameObject>();
                }
            }
        }

        public void Clear()
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    _grid[x, y].Clear();
                }
            }
        }

        public void Insert(GameObject obj)
        {
            
            var bounds = obj.GetPosition();
            
            int minX = Math.Max(0, bounds.Left / _cellSize);
            int maxX = Math.Min(_gridWidth - 1, bounds.Right / _cellSize);
            int minY = Math.Max(0, bounds.Top / _cellSize);
            int maxY = Math.Min(_gridHeight - 1, bounds.Bottom / _cellSize);
            
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    _grid[x, y].Add(obj);
                }
            }
        }

        public List<GameObject> GetNearbyObjects(GameObject obj)
        {
            var nearbyObjects = new List<GameObject>();
            var bounds = obj.GetPosition();
            
            int minX = Math.Max(0, bounds.Left / _cellSize);
            int maxX = Math.Min(_gridWidth - 1, bounds.Right / _cellSize);
            int minY = Math.Max(0, bounds.Top / _cellSize);
            int maxY = Math.Min(_gridHeight - 1, bounds.Bottom / _cellSize);
            
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    foreach (var other in _grid[x, y])
                    {
                        if (other != obj && !nearbyObjects.Contains(other))
                        {
                            nearbyObjects.Add(other);
                        }
                    }
                }
            }

            return nearbyObjects;
        }
    }