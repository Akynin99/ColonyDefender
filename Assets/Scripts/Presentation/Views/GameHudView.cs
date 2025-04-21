using System;
using System.Collections.Generic;
using ColonyDefender;
using ColonyDefender.Core;
using ColonyDefender.Presentation;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ColonyDefender.Presentation.Views
{
    public class GameHudView : MonoBehaviour
    {
        [Header("Resources Panel")]
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private TextMeshProUGUI mineralsText;
        [SerializeField] private TextMeshProUGUI populationText;
        [SerializeField] private TextMeshProUGUI energyProductionText;
        [SerializeField] private TextMeshProUGUI mineralsProductionText;
        
        [Header("Wave Info Panel")]
        [SerializeField] private TextMeshProUGUI waveNameText;
        [SerializeField] private TextMeshProUGUI enemiesRemainingText;
        [SerializeField] private GameObject bossIndicator;
        
        [Header("Building Panel")]
        [SerializeField] private GameObject buildingButtonPrefab;
        [SerializeField] private Transform buildingButtonsContainer;
        [SerializeField] private Color selectedBuildingColor;
        [SerializeField] private Color normalBuildingColor;
        
        [Header("Notification")]
        [SerializeField] private GameObject notificationPanel;
        [SerializeField] private TextMeshProUGUI notificationText;
        [SerializeField] private float notificationDuration = 3f;
        
        private ColonyViewModel _viewModel;
        private readonly CompositeDisposable _disposables = new();
        private readonly Dictionary<BuildingType, Button> _buildingButtons = new();
        
        private void Awake()
        {
            notificationPanel.SetActive(false);
            
            MessageBroker.Default.Receive<ShowNotification>()
                .Subscribe(OnShowNotification)
                .AddTo(_disposables);
        }
        
        public void Initialize(ColonyViewModel viewModel, BuildingConfig[] availableBuildings)
        {
            _viewModel = viewModel;
            
            // bind resource texts
            _viewModel.Energy.Subscribe(v => energyText.text = $"Energy: {v}")
                .AddTo(_disposables);
                
            _viewModel.Minerals.Subscribe(v => mineralsText.text = $"Minerals: {v}")
                .AddTo(_disposables);
                
            _viewModel.Population.Subscribe(v => 
                populationText.text = $"Population: {v}/{_viewModel.MaxPopulation.Value}")
                .AddTo(_disposables);
                
            _viewModel.MaxPopulation.Subscribe(v => 
                populationText.text = $"Population: {_viewModel.Population.Value}/{v}")
                .AddTo(_disposables);
                
            _viewModel.EnergyProduction.Subscribe(v => 
                energyProductionText.text = $"Energy: +{v}/cycle")
                .AddTo(_disposables);
                
            _viewModel.MineralProduction.Subscribe(v => 
                mineralsProductionText.text = $"Minerals: +{v}/cycle")
                .AddTo(_disposables);
                
            // bind wave info
            _viewModel.CurrentWaveName.Subscribe(v => waveNameText.text = v)
                .AddTo(_disposables);
                
            _viewModel.EnemiesRemaining.Subscribe(v => 
                enemiesRemainingText.text = $"Enemies: {v}")
                .AddTo(_disposables);
                
            _viewModel.IsBossActive.Subscribe(v => bossIndicator.SetActive(v))
                .AddTo(_disposables);
                
            // create building buttons
            CreateBuildingButtons(availableBuildings);
            
            // update selected building button
            _viewModel.SelectedBuildingType.Subscribe(OnSelectedBuildingChanged)
                .AddTo(_disposables);
        }
        
        private void CreateBuildingButtons(BuildingConfig[] buildingConfigs)
        {
            // clear existing buttons
            foreach (Transform child in buildingButtonsContainer)
            {
                Destroy(child.gameObject);
            }
            _buildingButtons.Clear();
            
            // create new buttons
            foreach (var config in buildingConfigs)
            {
                var buttonObj = Instantiate(buildingButtonPrefab, buildingButtonsContainer);
                var button = buttonObj.GetComponent<Button>();
                var image = buttonObj.transform.Find("Icon").GetComponent<Image>();
                var nameText = buttonObj.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                var costText = buttonObj.transform.Find("Cost").GetComponent<TextMeshProUGUI>();
                
                image.sprite = config.Icon;
                nameText.text = config.Type.ToString();
                costText.text = $"E: {config.EnergyCost} M: {config.MineralCost}";
                
                button.onClick.AddListener(() => OnBuildingButtonClicked(config.Type));
                
                _buildingButtons[config.Type] = button;
            }
        }
        
        private void OnBuildingButtonClicked(BuildingType buildingType)
        {
            MessageBroker.Default.Publish(new BuildingClicked 
            { 
                BuildingType = buildingType 
            });
        }
        
        private void OnSelectedBuildingChanged(BuildingType? buildingType)
        {
            // reset all button colors
            foreach (var button in _buildingButtons.Values)
            {
                button.GetComponent<Image>().color = normalBuildingColor;
            }
            
            // highlight selected button
            if (buildingType.HasValue && _buildingButtons.TryGetValue(buildingType.Value, out var selectedButton))
            {
                selectedButton.GetComponent<Image>().color = selectedBuildingColor;
            }
        }
        
        private void OnShowNotification(ShowNotification notification)
        {
            notificationText.text = notification.Message;
            notificationPanel.SetActive(true);
            
            Observable.Timer(TimeSpan.FromSeconds(notification.Duration))
                .Subscribe(_ => notificationPanel.SetActive(false))
                .AddTo(_disposables);
        }
        
        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
} 