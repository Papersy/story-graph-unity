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

        private void FixedUpdate()
        {
            var rayDirection = _mainCamera.transform.forward;
            var startPosDirection = _startPoint.forward;
            Debug.DrawRay(_startPoint.position, rayDirection * Distance, Color.green);
            
            //Check if player want to get item from flour
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Debug.Log("E pressed");
                
                if (Mathf.Abs(startPosDirection.x - rayDirection.x) > 0.4f)
                {
                    Debug.Log("Too far of my head!!!");
                    return;
                }
            
                if (Physics.Raycast(_startPoint.position, rayDirection * Distance, out _hit, Distance))
                {
                    if (_hit.transform.gameObject.CompareTag("Pickable"))
                    {
                        var itemName = _hit.transform.GetComponent<Item>().ItemInfo["Name"]?.ToString();
                       
                        if (AllServices.Container.Single<IGameService>().GetGameController().CanPickUpItem(itemName))
                        {
                            Debug.Log("CanPickUp");
                            StartCoroutine(PickUp());
                        }
                        else 
                            Debug.Log("Cant pickup");
                    }
                    else if (_hit.transform.gameObject.CompareTag("Npc"))
                    {
                        Debug.Log("All good, you can talk!");
                        _hit.transform.gameObject.GetComponent<Npc.Npc>().StartDialog();
                    }
                }
            }
            //Check if player want to open item
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                if (Mathf.Abs(startPosDirection.x - rayDirection.x) > 0.4f)
                {
                    Debug.Log("Too far of my head!!!");
                    return;
                }
                if (Physics.Raycast(_startPoint.position, rayDirection, out _hit, Distance))
                {
                    if (_hit.transform.gameObject.CompareTag("Pickable"))
                    {
                        if(_gameController.IsItStore(_hit.transform.GetComponent<Item>().ItemInfo["Name"]?.ToString(), "Putting item in"))
                            StartCoroutine(Open());
                    }
                }
            }
        }

        private IEnumerator PickUp()
        {
            const string pickup = "PickUp";
            
            _animator.SetBool(pickup, true);

            yield return new WaitForSeconds(0.75f);
        
            _animator.SetBool(pickup, false);
        
            var itemName = _hit.transform.GetComponent<Item>().ItemInfo["Name"]?.ToString();
            AllServices.Container.Single<IGameService>().GetGameController().PickItem(itemName);
                
            Destroy(_hit.transform.gameObject);
        }

        private IEnumerator Open()
        {
            const string open = "Open";
            
            yield return new WaitForSeconds(0.75f);

            var item = _hit.transform.GetComponent<Item>();
            _gameCanvas.ShowMainInventory();
            _gameCanvas.ItemUI.Show();
            _gameCanvas.ItemUI.ShowOpakowanieItems(item);
        }
    }
}