using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class GameCanvas : MonoBehaviour
    {
        [SerializeField] private GameObject _canvas;
        [SerializeField] private InventoryUI _inventoryUI;
        [SerializeField] private EquipmentUI _equipmentUI;
        [SerializeField] private AllActions _allActions;

        public static GameCanvas Instance;
        
        public GameObject DiePanel;
        public LocationInfoUI LocationInfoUI;

        public static bool IsUiActive = false; 
        private IGameService _gameService;

        private void Awake()
        {
            Instance = this;
            _gameService = AllServices.Container.Single<IGameService>();
        }

        public void Update()
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                if(Draggable.IsDrag)
                    return;
                
                if (!_inventoryUI.isActiveAndEnabled)
                {
                    if(IsUiActive)
                        HideAllWindows();
                    
                    ShowMainInventory();
                    _equipmentUI.Show();
                    ShowCursor();

                    IsUiActive = true;
                }
                else
                {
                    HideAllWindows();
                    HideCursor();
                }

            }
            if (Keyboard.current.capsLockKey.wasPressedThisFrame)
            {
                if(Draggable.IsDrag)
                    return;
                
                if (!_allActions.isActiveAndEnabled)
                {
                    if(IsUiActive)
                        HideAllWindows();
                    
                    ShowCursor();
                    _allActions.Show();
                    _allActions.Init();

                    IsUiActive = true;
                }
                else
                {
                    HideAllWindows();
                    HideCursor();
                }
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if(Draggable.IsDrag)
                    return;
                
                HideAllWindows();
                HideCursor();
            }
        }

        private void OnEnable() =>
            _gameService.GetGameController().OnLocationChanged += StartNewLocationAnimation;

        private void OnDisable() =>
            _gameService.GetGameController().OnLocationChanged -= StartNewLocationAnimation;

        private void HideAllWindows()
        {
            _inventoryUI.HideInventory();
            _equipmentUI.Hide();
            _allActions.Hide();

            IsUiActive = false;
        }
        
        private void ShowMainInventory() => _inventoryUI.Show(_gameService.GetGameController().GetPlayerItems());

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