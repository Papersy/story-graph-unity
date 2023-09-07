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
        [SerializeField] private ItemUI _itemUI;
        [SerializeField] private DialogWindow _dialogWindow;
        [SerializeField] private EquipmentUI _equipmentUI;
        [SerializeField] private GameObject _actionWindow;
        public GameObject DiePanel;
        
        public LocationInfoUI LocationInfoUI;

        public DialogWindow DialogWindow => _dialogWindow;
        public InventoryUI InventoryUI => _inventoryUI;
        public ItemUI ItemUI => _itemUI;
        public EquipmentUI EquipmentUI => _equipmentUI;

        public static bool IsUiActive = false; 
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
                    ShowMainInventory();
                    ShowEquipment();
                }
                else
                {
                    HideMainInventory();
                    HideEquipment();
                    _itemUI.Hide();
                    _actionWindow.SetActive(false);
                }
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                _inventoryUI.HideInventory();
                _dialogWindow.Hide();
                _itemUI.Hide();
                _equipmentUI.Hide();
                _actionWindow.SetActive(false);
                
                HideCursor();
            }
        }

        private void OnEnable() =>
            _gameService.GetGameController().OnLocationChanged += StartNewLocationAnimation;

        private void OnDisable() =>
            _gameService.GetGameController().OnLocationChanged -= StartNewLocationAnimation;


        public void ShowMainInventory()
        {
            IsUiActive = true;
            _inventoryUI.Show(_gameService.GetGameController().GetPlayerItems());
            ShowCursor();
        }
        
        public void HideMainInventory()
        {
            IsUiActive = false;
            _inventoryUI.HideInventory();
            HideCursor();
        }

        private void ShowEquipment()
        {
            _equipmentUI.Show();
        }

        private void HideEquipment()
        {
            _equipmentUI.Hide();
        }

        public void ShowDialog()
        {
            IsUiActive = true;
            DialogWindow.Show();
            ShowCursor();
        }

        public void HideDialog()
        {
            IsUiActive = false;
            DialogWindow.Hide();
            HideCursor();
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