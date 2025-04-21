using System.Collections.Generic;
using ColonyDefender.Core;
using UniRx;
using UnityEngine;

namespace ColonyDefender.Presentation.Views
{
    public class GridView : MonoBehaviour
    {
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private int gridWidth = 20;
        [SerializeField] private int gridHeight = 20;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material highlightMaterial;
        [SerializeField] private Material invalidPlacementMaterial;
        
        private readonly Dictionary<Vector2Int, GameObject> _cellObjects = new();
        private readonly CompositeDisposable _disposables = new();
        private IBuildingService _buildingService;
        private MapGrid _grid;
        private BuildingType? _selectedBuildingType;
        private Vector2Int? _hoverPosition;
        
        public void Initialize(MapGrid grid, IBuildingService buildingService, IReadOnlyReactiveProperty<BuildingType?> selectedBuildingType)
        {
            _grid = grid;
            _buildingService = buildingService;
            
            // subscribe to selected building type changes
            selectedBuildingType.Subscribe(type => 
            {
                _selectedBuildingType = type;
                UpdateCellHighlighting();
            }).AddTo(_disposables);
            
            CreateGrid();
        }
        
        private void CreateGrid()
        {
            // clear any existing cells
            foreach (var cell in _cellObjects.Values)
            {
                Destroy(cell);
            }
            _cellObjects.Clear();
            
            // create new grid
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    var position = new Vector2Int(x, y);
                    var worldPos = new Vector3(x * cellSize, 0, y * cellSize);
                    
                    var cellObj = Instantiate(cellPrefab, worldPos, Quaternion.Euler(90, 0, 0), transform);
                    cellObj.name = $"Cell_{x}_{y}";
                    cellObj.transform.localScale = new Vector3(cellSize * 0.9f, cellSize * 0.9f, 0.1f);
                    
                    var renderer = cellObj.GetComponent<Renderer>();
                    renderer.material = defaultMaterial;
                    
                    // add cell click handler
                    var cellClickHandler = cellObj.AddComponent<CellClickHandler>();
                    cellClickHandler.Initialize(position);
                    
                    _cellObjects[position] = cellObj;
                }
            }
        }
        
        private void Update()
        {
            // check for mouse hover over grid
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                var cellHandler = hit.collider.GetComponent<CellClickHandler>();
                if (cellHandler != null)
                {
                    // we're hovering over a cell
                    var newHoverPosition = cellHandler.GridPosition;
                    
                    if (_hoverPosition != newHoverPosition)
                    {
                        _hoverPosition = newHoverPosition;
                        UpdateCellHighlighting();
                    }
                }
                else
                {
                    if (_hoverPosition.HasValue)
                    {
                        _hoverPosition = null;
                        UpdateCellHighlighting();
                    }
                }
            }
            else
            {
                if (_hoverPosition.HasValue)
                {
                    _hoverPosition = null;
                    UpdateCellHighlighting();
                }
            }
        }
        
        private void UpdateCellHighlighting()
        {
            // reset all cells to default
            foreach (var cell in _cellObjects.Values)
            {
                cell.GetComponent<Renderer>().material = defaultMaterial;
            }
            
            // if we have a selected building type and are hovering over a cell
            if (_selectedBuildingType.HasValue && _hoverPosition.HasValue)
            {
                var buildingType = _selectedBuildingType.Value;
                var position = _hoverPosition.Value;
                
                // get size of the building (assuming we have this information)
                var config = GetBuildingConfig(buildingType);
                if (config == null) return;
                
                bool canPlace = _buildingService.CanPlaceBuilding(buildingType, position);
                var material = canPlace ? highlightMaterial : invalidPlacementMaterial;
                
                // highlight cells that would be occupied by the building
                for (int x = 0; x < config.Size.x; x++)
                {
                    for (int y = 0; y < config.Size.y; y++)
                    {
                        var cellPos = new Vector2Int(position.x + x, position.y + y);
                        if (_cellObjects.TryGetValue(cellPos, out var cellObj))
                        {
                            cellObj.GetComponent<Renderer>().material = material;
                        }
                    }
                }
            }
        }
        
        private BuildingConfig GetBuildingConfig(BuildingType type)
        {
            // this would ideally be injected or retrieved from a service
            // for simplicity, we'll assume 1x1 buildings for now
            return null;
        }
        
        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
    
    public class CellClickHandler : MonoBehaviour
    {
        public Vector2Int GridPosition { get; private set; }
        
        public void Initialize(Vector2Int position)
        {
            GridPosition = position;
        }
        
        private void OnMouseDown()
        {
            MessageBroker.Default.Publish(new CellClicked { GridPosition = GridPosition });
        }
    }
} 