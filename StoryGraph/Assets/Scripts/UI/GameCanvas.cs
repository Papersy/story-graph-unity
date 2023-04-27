using System;
using System.Collections;
using System.Collections.Generic;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameCanvas : MonoBehaviour
    {
        [SerializeField] private GameObject canvas;
        [SerializeField] private InventoryUI inventoryUI;

        #region Switch Location
        [SerializeField] private Button btnPrefab;
        [SerializeField] private GameObject container;
        [SerializeField] private GameObject buttonsContainer;
        #endregion

        #region New location
        [SerializeField] private GameObject newLocationPanel;
        [SerializeField] private TextMeshProUGUI newLocationText;
        #endregion

        private IGameService _gameService;

        private void Awake()
        {
            _gameService = AllServices.Container.Single<IGameService>();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if(!inventoryUI.isActiveAndEnabled)
                    inventoryUI.Show(_gameService.GetGameController().GetPlayerItems());
                else 
                    inventoryUI.Hide();
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

        public void GenerateLocationButtons(JToken variants)
        {
            foreach (var variant in variants)
            {
                var btn = Instantiate(btnPrefab, Vector3.one, Quaternion.identity);
                btn.transform.SetParent(buttonsContainer.transform);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = _gameService.GetGameController().GetLocationNameById(variant[2]["WorldNodeId"].ToString());
                btn.GetComponent<Button>().onClick.AddListener(() => _gameService.GetGameController().ChangeLocation(variant[2]["WorldNodeId"].ToString()));
            }
        }

        public void ShowLocationContainer()
        {
            ShowCursor();
            container.SetActive(true);
        }

        public void HideLocationsContainer()
        {
            HideCursor();
            container.SetActive(false);
        }

        public void Show() =>
            canvas.SetActive(true);

        public void Hide() =>
            canvas.SetActive(false);

        private void StartNewLocationAnimation(string locationName) =>
            StartCoroutine(NewLocationAnimation(locationName));
        
        private IEnumerator NewLocationAnimation(string locationName)
        {
            newLocationPanel.SetActive(true);
            newLocationText.text = locationName;

            yield return new WaitForSeconds(2f);
            
            newLocationPanel.SetActive(false);
        }

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