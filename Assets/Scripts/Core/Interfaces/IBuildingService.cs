using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace ColonyDefender.Core
{
    public interface IBuildingService
    {
        /// <summary>
        /// Checks if a building can be placed at the given position
        /// </summary>
        bool CanPlaceBuilding(BuildingType type, Vector2Int position);
        
        /// <summary>
        /// Places a building at the given position
        /// </summary>
        UniTask<bool> PlaceBuildingAsync(BuildingType type, Vector2Int position, CancellationToken token);
        
        /// <summary>
        /// Removes the building at the given position
        /// </summary>
        bool RemoveBuilding(Vector2Int position);
        
        /// <summary>
        /// Observable for building placement events
        /// </summary>
        IObservable<BuildingPlaced> OnBuildingPlaced { get; }
        
        /// <summary>
        /// Observable for building removal events
        /// </summary>
        IObservable<BuildingRemoved> OnBuildingRemoved { get; }
    }
} 