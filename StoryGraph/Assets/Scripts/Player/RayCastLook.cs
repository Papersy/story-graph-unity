using System.Collections;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using InteractableItems;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    class RayCastLook : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Camera _mainCamera;

        private const float Distance = 2.5f;
        private RaycastHit _hit;
    
        private float _ePressedTime = 0f;
        private GameController _gameController;
        private GameCanvas _gameCanvas;
        
        private void Awake()
        {
            _gameController = AllServices.Container.Single<IGameService>().GetGameController();
            _gameCanvas = AllServices.Container.Single<IUIService>().HudContainer.GameCanvas;
        }
    }
}