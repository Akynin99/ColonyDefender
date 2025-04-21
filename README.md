# Colony Defender  
*A Tower-Defense Strategy Game with UniRx & UniTask*

---

## **Architecture Overview**  

### **Layered Structure**  
#### 1. **Game Domain** (Pure Business Logic)  
- **Models**:  
  - `ColonyData`: Reactive properties (`ReactiveProperty<int>`) for resources (energy, minerals, population).  
  - `WaveConfig`: ScriptableObject with enemy prefabs, spawn intervals, and boss triggers.  
- **Interfaces**:  
  - `IResourceService`, `IWaveManager`, `ISaveService` for decoupled dependencies.  

#### 2. **Infrastructure** (Service Implementations)  
- **Services**:  
  - `ResourceService`: Reactive resource management with `CompositeDisposable` for cleanup.  
  - `WaveSpawner`: Async enemy spawning via `UniTask.Delay` and cancellation tokens.  
- **Adapters**: Bridges between pure logic and Unity-specific code.  

#### 3. **Presentation** (Unity Components)  
- **Views**:  
  - `ResourcePanel`: Binds reactive properties to UI using optimized `SubscribeWithState`.  
  - `BuildingView`: UniRx-based click detection with `Observable.EveryUpdate`.  
- **ViewModel**:  
  - `ColonyViewModel`: Mediates Model-View interaction via `MessageBroker` events.  
