using System.Collections.Generic;
using ActionButtons;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

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
        
        public TextMeshProUGUI Text;
        
        public Button GroupCharacterBtn;
        public Button AskNpc;
        public Button Action;
        
        public JToken NpcInfo;
        public GameObject Npc;

        private int _index;
        private List<JToken> _knowledgeFromNpcList;

        private void OnEnable()
        {
            AskNpc.onClick.AddListener(Ask);
            Action.onClick.AddListener(InitActions);
            GroupCharacterBtn.onClick.AddListener(GroupCharacter);
        }

        private void OnDisable()
        {
            AskNpc.onClick.RemoveListener(Ask);
            Action.onClick.RemoveListener(InitActions);
            GroupCharacterBtn.onClick.RemoveListener(GroupCharacter);
        }

        public void InitDialog()
        {
            _index = 0;
            Text.gameObject.SetActive(false);
            _scrollViewContent.gameObject.SetActive(true);
            _knowledgeFromNpcList = new List<JToken>();
            
            CheckIfWeCanGroupCharacter();

            CheckIfNpcCanGetKnowledgeFromConversation();
            CheckIfNpcCanGiveKnowledgeFromConversation();
        }

        private void CheckIfWeCanGroupCharacter()
        {
            var result = AllServices.Container.Single<IGameService>().GetGameController().CanBeGrouped(NpcInfo["Name"].ToString());

            GroupCharacterBtn.gameObject.SetActive(result);
        }

        private void InitActions()
        {
            _actionWindow.gameObject.SetActive(true);
            ClearContentView(_actionContent.transform);
            
            Trade();
            Actions();
            TakeItemFunc();
            GiveItemFunc();
            MakeDeal();
        }
        
        private void CheckIfNpcCanGetKnowledgeFromConversation()
        {
            var variant = AllServices.Container.Single<IGameService>().GetGameController()
                .FindVariantsOfGetKnowledgeInConversation(NpcInfo["Id"].ToString());

            ClearContentView(_scrollViewContent.transform);

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
                            btn.GetComponent<Button>().onClick.AddListener(() => GetKnowledge(container));
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

        private void GetKnowledge(JToken variant)
        {
            AllServices.Container.Single<IGameService>().GetGameController().GetKnowledgeFromConversation(variant);
            
            _scrollViewContent.gameObject.SetActive(false);
            Text.gameObject.SetActive(true);
            Text.text = "Ow, dzieki za informacje!";
        }

        private void TakeItemFunc()
        {
            var list = AllServices.Container.Single<IGameService>().GetGameController().FindVariantsOfTakeItemFunc(NpcInfo["Name"].ToString());

            if(list == null)
                return;
            
            foreach (var variant in list)
            {
                var btn = Instantiate(_takeItemPrefab, _actionContent.transform);
                var itemName = variant[3]["WorldNodeName"].ToString();
                
                btn.SetImage(itemName);
                btn.SetText("Take");
                btn.GetComponent<Button>().onClick.AddListener(() => TakeItemHelper(variant));
            }
        }

        private void TakeItemHelper(JToken variant)
        {
            AllServices.Container.Single<IGameService>().GetGameController().TakeItemFromNpc(NpcInfo["Name"].ToString(), variant);
            _actionWindow.gameObject.SetActive(false);
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideDialog();
        }

        private void GiveItemFunc()
        {
            var list = AllServices.Container.Single<IGameService>().GetGameController().FindVariantsOfGiveItemFunc(NpcInfo["Name"].ToString());

            if(list == null)
                return;
            
            foreach (var variant in list)
            {
                var btn = Instantiate(_giveItemPrefab, _actionContent.transform);
                var itemName = variant[3]["WorldNodeName"].ToString();
                
                btn.SetImage(itemName);
                btn.SetText("Give");
                btn.GetComponent<Button>().onClick.AddListener(() => GiveItemHelper(variant));
            }
        }

        private void GiveItemHelper(JToken variant)
        {
            AllServices.Container.Single<IGameService>().GetGameController().GiveItemToNpc(NpcInfo["Name"].ToString(), variant);
            _actionWindow.gameObject.SetActive(false);
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideDialog();
        }

        private void GroupCharacter()
        {
            AllServices.Container.Single<IGameService>().GetGameController().GroupCharacter(NpcInfo["Name"].ToString());
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideDialog();
            
            Destroy(Npc);
        }

        private void Trade()
        {
            var list = AllServices.Container.Single<IGameService>().GetGameController().FindVariantsOfTradeItem(NpcInfo["Name"].ToString());

            if(list == null)
                return;
            
            foreach (var variant in list)
            {
                var btn = Instantiate(_tradePrefab, _actionContent.transform);
                var itemName1 = variant[2]["WorldNodeName"].ToString();
                var itemName2 = variant[4]["WorldNodeName"].ToString();
                
                btn.SetImage(itemName1, itemName2);
                btn.GetComponent<Button>().onClick.AddListener(() => TradeHelper(variant));
            }
        }

        private void TradeHelper(JToken variant)
        {
            AllServices.Container.Single<IGameService>().GetGameController().TradeWithCharacter(NpcInfo["Name"].ToString(), variant);
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideDialog();
            _actionWindow.gameObject.SetActive(false);
        }

        private void Actions()
        {
            var list = AllServices.Container.Single<IGameService>().GetGameController().FindProductionsOfActionWithNpc(NpcInfo["Id"].ToString());

            if(list == null)
                return;
            
            foreach (var variable in list)
            {
                var btn = Instantiate(_npcActionPrefab, _actionContent.transform);
                btn.GetComponent<ButtonAction>().SetText(variable.Key["prod"]["Title"].ToString());
                btn.GetComponent<Button>().onClick.AddListener(() => ActionHelper(variable));
            }
        }

        private void ActionHelper(KeyValuePair<JToken, List<JToken>> variable)
        {
            AllServices.Container.Single<IGameService>().GetGameController().SendAction(variable.Key["prod"], variable.Value[0]);
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideDialog();
            _actionWindow.gameObject.SetActive(false);
        }

        private void Ask()
        {
            Text.gameObject.SetActive(true);
            _scrollViewContent.gameObject.SetActive(false);

            if (_knowledgeFromNpcList == null)
            {
                Text.text = "Nie mam informacji dla ciebie";
                return;
            }
            
            if (_index < _knowledgeFromNpcList.Count)
            {
                var container = _knowledgeFromNpcList[_index];

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
                Text.text = "Nie mam informacji dla ciebie";
        }

        private void MakeDeal()
        {
            var list = AllServices.Container.Single<IGameService>().GetGameController().FindVariantsOfMakeADeal(NpcInfo["Id"].ToString());

            if(list == null)
                return;
            
            foreach (var variable in list)
            {
                var btn = Instantiate(_takeItemPrefab, _actionContent.transform);
                var text = "";
                var cost = "";
                var name = "";
                
                foreach (var element in variable)
                {
                    var playerId = AllServices.Container.Single<IGameService>().GetGameController().GetMainPlayerId();
                    var playerStatus = element["LSNodeRef"].ToString();
                    
                    if (playerStatus == "Buyer" && element["WorldNodeId"].ToString() == playerId)
                        text = "Buy";
                    
                    else if (playerStatus == "Seller" && element["WorldNodeId"].ToString() == playerId)
                        text = "Sell";

                    if (element["LSNodeRef"].ToString() == "Something")
                    {
                        var nodeAttr = element["WorldNodeAttr"];
                        name = element["WorldNodeName"].ToString();
                        cost = nodeAttr["Value"].ToString();
                    }
                }

                text += " " + cost;
                btn.GetComponent<ButtonTakeAndGive>().SetImage(name);
                btn.GetComponent<ButtonTakeAndGive>().SetText(text);
            }
        }
        
        private string GetPolishPart(string title)
        {
            char[] delimiter = {'/'};
        
            string[] words = title.Split(delimiter);

            if (words.Length > 1)
                return words[1].Trim();
            
            return words[0].Trim();
        }

        private void ClearContentView(Transform content)
        {
            if(content.childCount == 0)
                return;
            
            for (var i = content.childCount - 1; i >= 0; i--)
                Destroy(content.GetChild(i).gameObject);
        }
    }
}