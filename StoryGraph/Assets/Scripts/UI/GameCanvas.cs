using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class GameCanvas : MonoBehaviour
    {
        [SerializeField] private GameObject _canvas;
        [SerializeField] private InventoryUI _inventoryUI;

        public TeleportUI.TeleportUI TeleportUI;
        public LocationInfoUI LocationInfoUI;

        private IGameService _gameService;

        private void Awake()
        {
            _gameService = AllServices.Container.Single<IGameService>();
        }

        public void Update()
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                if (!_inventoryUI.isActiveAndEnabled)
                {
                    _inventoryUI.Show(_gameService.GetGameController().GetPlayerItems());
                    ShowCursor();
                }
                else
                {
                    _inventoryUI.HideInventory();
                    HideCursor();
                }
            }
        }

        private void OnEnable()
        {
            _gameService.GetGameController().OnLocationChanged += StartNewLocationAnimation;
        }
        
        private void OnDisable()
        {
            _gameService.GetGameController().OnLocationChanged -= StartNewLocationAnimation;
        }

        public void GenerateLocationButtons(JToken variants) => TeleportUI.GenerateLocationButtons(variants);

        public void ShowLocationContainer()
        {
            ShowCursor();
            TeleportUI.ShowLocationContainer();
        }

        public void HideLocationsContainer()
        {
            HideCursor();
            TeleportUI.HideLocationsContainer();
        }

        public void Show() =>
            _canvas.SetActive(true);

        public void Hide() =>
            _canvas.SetActive(false);

        private void StartNewLocationAnimation(string locationName) =>
            StartCoroutine(LocationInfoUI.NewLocationAnimation(locationName));
        
        

        private void ShowCursor()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        private void HideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}