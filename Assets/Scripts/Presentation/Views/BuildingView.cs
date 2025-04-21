using ColonyDefender.Core;
using UniRx;
using UnityEngine;

namespace ColonyDefender.Presentation
{
    public class BuildingView : MonoBehaviour
    {
        [SerializeField] private BuildingType buildingType;
    
        private void Start()
        {
            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Select(_ => GetClickWorldPosition())
                .Where(IsClickOnBuilding)
                .Subscribe(_ => OnClick())
                .AddTo(this);
        }

        private void OnClick()
        {
            MessageBroker.Default.Publish(new BuildingClicked { BuildingType = buildingType });
        }

        private Vector2 GetClickWorldPosition()
        {
            // TODO: add logic
            return new Vector2();
        }

        private bool IsClickOnBuilding(Vector2 clickPos)
        {
            // TODO: add logic
            return false;
        }
    }
}