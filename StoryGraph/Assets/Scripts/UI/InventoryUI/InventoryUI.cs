using System.Collections.Generic;
using System.Linq;
using ActionButtons;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InventoryUI : BaseWindow
    {
        private const int InventorySize = 15;
        
        [SerializeField] private InventoryTile _inventoryTilePrefab;
        [SerializeField] private Draggable _itemPrefab;
        [SerializeField] private ButtonAction _narrationTextPrefab;
        
        [SerializeField] private GameObject _itemsInventoryContainer;
        [SerializeField] private GameObject _tilesContainer;
        [SerializeField] private Transform _inventoryRoot;
        [SerializeField] private Transform _narrationRoot;
        [SerializeField] private Transform _narrationContent;
        [SerializeField] private Transform _groupedParent;
        [SerializeField] private GroupView _groupView;
        [SerializeField] private Button _narrationBtn;
        [SerializeField] private TextMeshProUGUI _moneyTxt;
        [SerializeField] private TextMeshProUGUI _hpTxt;
        [SerializeField] private List<InventoryTile> _tiles = new List<InventoryTile>();
        
        public static List<string> EquipmentId = new List<string>();

        private void OnEnable() => _narrationBtn.onClick.AddListener(ShowNarrations);
        private void OnDisable() => _narrationBtn.onClick.RemoveListener(ShowNarrations);

        public override void Awake()
        {
            base.Awake();
            
            GenerateTiles();
        }

        public void Show(JToken items)
        {
            _itemsInventoryContainer.SetActive(true);
            _narrationRoot.gameObject.SetActive(false);
            _groupedParent.gameObject.SetActive(false);
            
            UpdatePlayerCharacteristics();
            ShowInventoryItems(items);
            UpdateGrouped(AllServices.Container.Single<IGameService>().GetGameController().GetPlayerCharacters());
        }

        public void HideInventory()
        {
            Hide();
            _itemsInventoryContainer.SetActive(false);
        }

        private void UpdatePlayerCharacteristics()
        {
            var playerInfo = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerInfo();
            var attributes = playerInfo["Attributes"];
            if (attributes != null)
            {
                var hp = attributes["HP"];
                var money = attributes["Money"];
                if (hp != null)
                    _hpTxt.text = "Health: " + hp;
                if (money != null)
                    _moneyTxt.text = "Money: " + money;
            }
        }

        private void UpdateGrouped(JToken characters)
        {
            if (characters == null)
                return;

            if (_groupedParent.childCount > 0)
            {
                for (var i = _groupedParent.childCount - 1; i >= 0; i--)
                    Destroy(_groupedParent.GetChild(i).gameObject);
            }
            
            foreach (var character in characters)
            {
                var item = Instantiate(_groupView, _groupedParent);
                item.SetText(character["Name"].ToString());
            }
        }
        
        private void ShowInventoryItems(JToken items)
        {
            ClearInventory();

            if(items == null)
                return;
            
            var index = 0;
            foreach (var item in items)
            {
                if(IsEquipped(item))
                    continue;
                
                var itemObj = Instantiate(_itemPrefab, _tiles[index].transform);
                itemObj.Init(_inventoryRoot, item);
                itemObj.Type = InventoryType.Main;
                _tiles[index].PutItem(itemObj);

                index++;
            }
        }
        
        private void ClearInventory()
        {
            foreach (var tile in _tiles)
            {
                if (tile.transform.childCount > 0)
                {
                    for (int i = 0; i < tile.transform.childCount; i++)
                        Destroy(tile.transform.GetChild(i).gameObject);
                }
            }
        }
        
        private void GenerateTiles()
        {
            for (var i = 0; i < InventorySize; i++)
            {
                var tile = Instantiate(_inventoryTilePrefab, _tilesContainer.transform);
                tile.CurrentType = InventoryType.Main;
                _tiles.Add(tile);
            }
        }

        private bool IsEquipped(JToken item) => 
            EquipmentId.Any(equip => equip.Equals(item["Id"].ToString()));

        public static void RemoveFromEquipment(string id)
        {
            foreach (var equip in EquipmentId.Where(equip => equip.Equals(id)))
            {
                EquipmentId.Remove(equip);
                break;
            }
        }

        private void ShowNarrations()
        {
            _narrationRoot.gameObject.SetActive(true);
            
            var playerInfo = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerInfo();
            var narration = playerInfo["Narration"];

            ClearContentView(_narrationContent);

            if(narration == null)
                return;
            
            foreach (var narr in narration)
            {
                var att = narr["Attributes"];
                var know = att["Knowledge"];
                
                var obj = Instantiate(_narrationTextPrefab, _narrationContent);
                obj.SetText(GetPolishPart(know.ToString()));
            }
        }
        
        private void ClearContentView(Transform content)
        {
            if(content.childCount == 0)
                return;
            
            for (var i = content.childCount - 1; i >= 0; i--)
                Destroy(content.GetChild(i).gameObject);
        }
        
        private string GetPolishPart(string title)
        {
            char[] delimiter = {'/'};
        
            string[] words = title.Split(delimiter);

            if (words.Length > 1)
                return words[1].Trim();
            
            return words[0].Trim();
        }
    }
}