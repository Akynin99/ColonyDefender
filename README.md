# Colony Defender  
*A Tower-Defense Strategy Game with UniRx & UniTask*

---


## **Project Structure**

The project follows a clean architecture approach with scripts organized by their responsibilities:

### Core Layer
**Main Installer:**
- `GameInstaller.cs`: Main dependency injection and game initialization

**Models:**
- `ColonyData.cs`: Colony resources and production data
- `Building.cs`: Building entity model
- `BuildingConfig.cs`: ScriptableObject for building configurations
- `Enemy.cs`: Enemy entity model
- `EnemyConfig.cs`: ScriptableObject for enemy configurations
- `MapGrid.cs`: Grid system for building placement
- `WaveConfig.cs`: ScriptableObject for wave configurations

**Interfaces:**
- `IBuildingService.cs`: Building management interface
- `IResourceService.cs`: Resource management interface
- `ISaveService.cs`: Save/load system interface
- `IWaveManager.cs`: Wave management interface

**Events:**
- `BuildingEvents.cs`: Building-related events (placement, removal, damage)
- `EnemyEvents.cs`: Enemy-related events (spawn, defeat, damage)
- `WaveEvents.cs`: Wave-related events (start, complete)
- `UIEvents.cs`: UI-related events (clicks, notifications)

### Infrastructure Layer
**Services:**
- `BuildingService.cs`: Implementation of building placement and management
- `ResourceService.cs`: Resource production and management implementation
- `SaveService.cs`: Save and load game data implementation
- `WaveSpawner.cs`: Enemy wave spawning and management implementation

### Presentation Layer
**ViewModels:**
- `ColonyViewModel.cs`: Main view model for connecting game state to UI

**Views:**
- `BuildingView.cs`: Building selection view
- `GameHudView.cs`: Main game HUD with resource display and controls
- `GridView.cs`: Grid visualization and interaction
- `ResourcePanel.cs`: Resource display panel

### **Key Features**

- **Reactive Programming**: Using UniRx for automatic UI updates when the model changes.
- **Asynchronous Operations**: Applying UniTask for efficient asynchronous work without blocking the game thread.
- To be Continued...