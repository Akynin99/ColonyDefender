using System.Collections.Generic;
using UnityEngine;

namespace ColonyDefender.Core
{
    public class MapGrid
    {
        private readonly Dictionary<Vector2Int, Building> _buildings = new Dictionary<Vector2Int, Building>();
        private readonly HashSet<Vector2Int> _occupiedCells = new HashSet<Vector2Int>();
        
        public int Width { get; }
        public int Height { get; }
        
        public MapGrid(int width, int height)
        {
            Width = width;
            Height = height;
        }
        
        public bool CanPlaceBuilding(Vector2Int position, Vector2Int size)
        {
            // check if in bounds
            if (position.x < 0 || position.y < 0 || 
                position.x + size.x > Width || position.y + size.y > Height)
            {
                return false;
            }
            
            // check if cells are occupied
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var cell = new Vector2Int(position.x + x, position.y + y);
                    if (_occupiedCells.Contains(cell))
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }
        
        public bool PlaceBuilding(Building building)
        {
            if (!CanPlaceBuilding(building.Position, building.Size))
            {
                return false;
            }
            
            // mark cells as occupied
            for (int x = 0; x < building.Size.x; x++)
            {
                for (int y = 0; y < building.Size.y; y++)
                {
                    var cell = new Vector2Int(building.Position.x + x, building.Position.y + y);
                    _occupiedCells.Add(cell);
                }
            }
            
            // add building to main cell
            _buildings[building.Position] = building;
            
            return true;
        }
        
        public bool RemoveBuilding(Vector2Int position)
        {
            if (!_buildings.TryGetValue(position, out var building))
            {
                return false;
            }
            
            // free up occupied cells
            for (int x = 0; x < building.Size.x; x++)
            {
                for (int y = 0; y < building.Size.y; y++)
                {
                    var cell = new Vector2Int(building.Position.x + x, building.Position.y + y);
                    _occupiedCells.Remove(cell);
                }
            }
            
            _buildings.Remove(position);
            
            return true;
        }
        
        public Building GetBuildingAt(Vector2Int position)
        {
            return _buildings.TryGetValue(position, out var building) ? building : null;
        }
        
        public IEnumerable<Building> GetAllBuildings()
        {
            return _buildings.Values;
        }
        
        public bool IsCellOccupied(Vector2Int position)
        {
            return _occupiedCells.Contains(position);
        }
    }
} 