using System.Collections.Generic;
using ActionButtons;
using CodeBase.Infrastructure.Services;
using Infrastructure;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace UI
{
    public class DialogWindow : BaseWindow
    {
        [SerializeField] private ButtonTakeAndGive _takeItemPrefab;
        [SerializeField] private ButtonTakeAndGive _giveItemPrefab;
        [SerializeField] private ButtonTrade _tradePrefab;
        [SerializeField] private Button _npcActionPrefab;
        
        [SerializeField] private GameObject _dialogBtnPrefab;
        [SerializeField] private Transform _scrollViewContent;
        [SerializeField] private GameObject _actionWindow;
        [SerializeField] private GameObject _actionContent;
        
        public Image PlayerAvatar;
        public Image NpcAvatar;
        public TextMeshProUGUI Text;
        public Button NextFrame;

        public Button TakeItem;
        public Button GiveItem;
        public Button GroupCharacterBtn;
        public Button TradeBtn;
        public Button AskNpc;
        public Button Action;
        
        public JToken NpcInfo;
        public GameObject Npc;

        private int _index;
        private List<JToken> _knowledgeFromNpcList;

        private void OnEnable()
        {
            TakeItem.onClick.AddListener(TakeItemFunc);
            GiveItem.onClick.AddListener(GiveItemFunc);
            TradeBtn.onClick.AddListener(Trade);
            AskNpc.onClick.AddListener(Ask);
            Action.onClick.AddListener(Actions);
            GroupCharacterBtn.onClick.AddListener(GroupCharacter);
        }

        private void OnDisable()
        {
            TakeItem.onClick.RemoveListener(TakeItemFunc);
            GiveItem.onClick.RemoveListener(GiveItemFunc);
            TradeBtn.onClick.RemoveListener(Trade);
            AskNpc.onClick.RemoveListener(Ask);
            Action.onClick.RemoveListener(Actions);
            GroupCharacterBtn.onClick.RemoveListener(GroupCharacter);
        }

        public void InitDialog()
        {
            _index = 0;
            Text.gameObject.SetActive(false);
            _scrollViewContent.gameObject.SetActive(true);
            _knowledgeFromNpcList = new List<JToken>();
            
            CheckIfWeCanGiveItem();
            CheckIfWeCanGroupCharacter();
            CheckIfWeCanTakeItem();
            CheckIfWeCanTrade();
            
            CheckIfNpcCanGetKnowledgeFromConversation();
            CheckIfNpcCanGiveKnowledgeFromConversation();
        }

        private void CheckIfWeCanTakeItem()
        {
            var result = AllServices.Container.Single<IGameService>().GetGameController().CanWeTakeFromNpc(NpcInfo["Name"].ToString());
            
            if(result)
                TakeItem.gameObject.SetActive(true);
            else
                TakeItem.gameObject.SetActive(false);
        }

        private void CheckIfWeCanGiveItem()
        {
            var result = AllServices.Container.Single<IGameService>().GetGameController().CanWeGiveToNpc(NpcInfo["Name"].ToString());
            
            if(result)
                GiveItem.gameObject.SetActive(true);
            else
                GiveItem.gameObject.SetActive(false);
        }

        private void CheckIfWeCanGroupCharacter()
        {
            var result = AllServices.Container.Single<IGameService>().GetGameController().CanBeGrouped(NpcInfo["Name"].ToString());
            
            if(result)
                GroupCharacterBtn.gameObject.SetActive(true);
            else
                GroupCharacterBtn.gameObject.SetActive(false);
        }

        private void CheckIfWeCanTrade()
        {
            var result = AllServices.Container.Single<IGameService>().GetGameController().CanTradeItem(NpcInfo["Name"].ToString());
            
            if(result)
                TradeBtn.gameObject.SetActive(true);
            else
                TradeBtn.gameObject.SetActive(false);
        }

        private void CheckIfNpcCanGetKnowledgeFromConversation()
        {
            var variant = AllServices.Container.Single<IGameService>().GetGameController()
                .FindVariantsOfGetKnowledgeInConversation(NpcInfo["Id"].ToString());

            for (var i = _scrollViewContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(_scrollViewContent.transform.GetChild(i).gameObject);
            }
            
            if (variant != null)
            {
                for (int i = 0; i < variant.Count; i++)
                {
                    var container = variant[i];
                    var variant2 = container[2]["WorldNodeAttr"];
                    if (variant2 != null)
                    {
                        if (variant2["Knowledge"] != null)
                        {
                            var knowledge = variant2["Knowledge"].ToString();
                            var btn = Instantiate(_dialogBtnPrefab, _scrollViewContent);
                            btn.GetComponentInChildren<TextMeshProUGUI>().text = GetPolishPart(knowledge);
                            btn.GetComponent<Button>().onClick.AddListener(() => PickButton(container));
                        }
                    }
                }
            }
        }

        private void CheckIfNpcCanGiveKnowledgeFromConversation()
        {
            _knowledgeFromNpcList = AllServices.Container.Single<IGameService>().GetGameController()
                .FindVariantsOfGetKnowledgeInConversationFromNpc(NpcInfo["Id"].ToString());
        }

        private void PickButton(JToken variant)
        {
            AllServices.Container.Single<IGameService>().GetGameController().GetKnowledgeFromConversation(variant);
            
            _scrollViewContent.gameObject.SetActive(false);
            Text.gameObject.SetActive(true);
            Text.text = "Ow, dzieki za informacje!";
        }

        private void TakeItemFunc()
        {
            _actionWindow.gameObject.SetActive(true);
            
            for (var i = _actionContent.transform.childCount - 1; i >= 0; i--)
                Destroy(_actionContent.transform.GetChild(i).gameObject);
            
            
            var list = AllServices.Container.Single<IGameService>().GetGameController().FindVariantsOfTakeItemFunc(NpcInfo["Name"].ToString());

            for (int i = 0; i < list.Count; i++)
            {
                var btn = Instantiate(_takeItemPrefab, _actionContent.transform);
                var itemName = list[i][3]["WorldNodeName"].ToString();
                
                btn.SetImage(itemName);
            }
        }

        private void GiveItemFunc()
        {
            _actionWindow.gameObject.SetActive(true);
            
            for (var i = _actionContent.transform.childCount - 1; i >= 0; i--)
                Destroy(_actionContent.transform.GetChild(i).gameObject);
            
            
            var list = AllServices.Container.Single<IGameService>().GetGameController().FindVariantsOfGiveItemFunc(NpcInfo["Name"].ToString());

            for (int i = 0; i < list.Count; i++)
            {
                var btn = Instantiate(_giveItemPrefab, _actionContent.transform);
                var itemName = list[i][3]["WorldNodeName"].ToString();
                
                btn.SetImage(itemName);
            }
        }

        private void GroupCharacter()
        {
            AllServices.Container.Single<IGameService>().GetGameController().GroupCharacter(NpcInfo["Name"].ToString());
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideDialog();
        }

        private void Trade()
        {
            _actionWindow.gameObject.SetActive(true);
            
            for (var i = _actionContent.transform.childCount - 1; i >= 0; i--)
                Destroy(_actionContent.transform.GetChild(i).gameObject);
            
            
            var list = AllServices.Container.Single<IGameService>().GetGameController().FindVariantsOfTradeItem(NpcInfo["Name"].ToString());

            for (int i = 0; i < list.Count; i++)
            {
                var btn = Instantiate(_tradePrefab, _actionContent.transform);
                var itemName1 = list[i][2]["WorldNodeName"].ToString();
                var itemName2 = list[i][4]["WorldNodeName"].ToString();
                
                btn.SetImage(itemName1, itemName2);
            }
        }

        private void Actions()
        {
            _actionWindow.gameObject.SetActive(true);
            
            for (var i = _actionContent.transform.childCount - 1; i >= 0; i--)
                Destroy(_actionContent.transform.GetChild(i).gameObject);
            
            
            var list = AllServices.Container.Single<IGameService>().GetGameController().FindVariantsOfActionWithNpc(NpcInfo["Id"].ToString());

            for (int i = 0; i < list.Count; i++)
            {
                var btn = Instantiate(_npcActionPrefab, _actionContent.transform);
                // var itemName = list[i][3]["WorldNodeName"].ToString();
            }
        }

        private void Ask()
        {
            Text.gameObject.SetActive(true);
            _scrollViewContent.gameObject.SetActive(false);

            if (_knowledgeFromNpcList == null)
            {
                Text.text = "Nie mam wiecej informacji dla ciebie";
                return;
            }
            
            if (_index < _knowledgeFromNpcList.Count)
            {
                var container = _knowledgeFromNpcList[_index];
                Debug.Log(container);
                if (container != null)
                {
                    var variant = container[2];
                    if (variant != null)
                    {
                        var worldNode = variant["WorldNodeAttr"];
                        if (worldNode != null)
                        {
                            var knowledge = worldNode["Knowledge"];
                            if (knowledge != null)
                            {
                                Text.text = knowledge.ToString();
                                AllServices.Container.Single<IGameService>().GetGameController().GetKnowledgeFromPerson(_knowledgeFromNpcList[_index]);
                            }
                        }
                    }
                }

                _index++;
            }
            else
                Text.text = "Nie mam wiecej informacji dla ciebie";
        }
        
        private string GetPolishPart(string title)
        {
            char[] delimiter = {'/'};
        
            string[] words = title.Split(delimiter);
            string firstWord = words[1].Trim();

            return firstWord;
        }
    }
}