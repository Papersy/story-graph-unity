using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TeleportUI
{
    public class TeleportUI : BaseWindow
    {
        public Button BtnPrefab;
        public GameObject Container;
        public GameObject ButtonsContainer;
        public List<Button> Buttons;

        public void GenerateLocationButtons(JToken variants)
        {
            foreach (var button in Buttons)
                button.gameObject.SetActive(false);

            foreach (var variant in variants)
            {
                var btn = GetFreeButton();
                btn.gameObject.SetActive(true);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = variant[2]["WorldNodeName"].ToString();
                btn.GetComponent<Button>().onClick.RemoveAllListeners();
                btn.GetComponent<Button>().onClick.AddListener(() => GameService.GetGameController().ChangeLocation(variant[2]["WorldNodeId"].ToString(), variant));
            }
        }
        
        public void ShowLocationContainer() => Container.SetActive(true);

        public void HideLocationsContainer() => Container.SetActive(false);

        private Button GetFreeButton()
        {
            foreach (var button in Buttons)
            {
                if (!button.IsActive())
                    return button;
            }
            
            var btn = Instantiate(BtnPrefab, Vector3.one, Quaternion.identity);
            btn.transform.SetParent(ButtonsContainer.transform);
            Buttons.Add(btn);

            return btn;
        }
    }
    
}