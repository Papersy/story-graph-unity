using System;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DialogWindow : BaseWindow
    {
        public Image PlayerAvatar;
        public Image NpcAvatar;
        public TextMeshProUGUI Text;
        public Button NextFrame;

        public Button TakeItem;
        public Button GiveItem;
        public Button GroupCharacterBtn;
        public Button TradeBtn;
        
        public int DialogIndex = 0;

        private Dialog _dialog;
        public JToken NpcInfo;
        public GameObject Npc;
        public Dialog Dialog
        {
            get => _dialog;
            set
            {
                _dialog = value;
                DialogIndex = 0;
                
                CheckIfWeCanGiveItem();
                CheckIfWeCanGroupCharacter();
                CheckIfWeCanTakeItem();
                CheckIfWeCanTrade();
                
                SetInfo();
            }
        }

        private void OnEnable()
        {
            NextFrame.onClick.AddListener(SkipFrame);
            TakeItem.onClick.AddListener(TakeItemFunc);
            GiveItem.onClick.AddListener(GiveItemFunc);
            TradeBtn.onClick.AddListener(Trade);
            GroupCharacterBtn.onClick.AddListener(GroupCharacter);
        }

        private void OnDisable()
        {
            NextFrame.onClick.RemoveListener(SkipFrame);
            TakeItem.onClick.RemoveListener(TakeItemFunc);
            GiveItem.onClick.RemoveListener(GiveItemFunc);
            TradeBtn.onClick.RemoveListener(Trade);
            GroupCharacterBtn.onClick.RemoveListener(GroupCharacter);
        }


        private void SkipFrame()
        {
            DialogIndex++;
            
            SetInfo();
        }

        private void SetInfo()
        {
            if (DialogIndex >= Dialog.dialog.Count)
            {
                NextFrame.gameObject.SetActive(false);
                if(GameService.GetGameController().FindVariantOfGetItemFromNpc(Dialog.npc_name) != null)
                    TakeItem.gameObject.SetActive(true);
                if(GameService.GetGameController().FindVariantOfGiveItemToNpc(Dialog.npc_name) != null)
                    GiveItem.gameObject.SetActive(true);
                return;
            }

            if (Dialog.dialog[DialogIndex].side == 1)
            {
                PlayerAvatar.gameObject.SetActive(true);
                NpcAvatar.gameObject.SetActive(false);

                var icon = Resources.Load<Sprite>("JsonFiles/AvatarIcon/" + Dialog.dialog[DialogIndex].name);
                if(icon == null)
                    PlayerAvatar.sprite = Resources.Load<Sprite>("JsonFiles/AvatarIcon/default");
                else 
                    PlayerAvatar.sprite = icon;
            }
            else if (Dialog.dialog[DialogIndex].side == 2)
            {
                PlayerAvatar.gameObject.SetActive(false);
                NpcAvatar.gameObject.SetActive(true);
                
                var icon = Resources.Load<Sprite>("JsonFiles/AvatarIcon/" + Dialog.dialog[DialogIndex].name);
                if(icon == null)
                    NpcAvatar.sprite = Resources.Load<Sprite>("JsonFiles/AvatarIcon/default");
                else 
                    NpcAvatar.sprite = icon;
            }
            
            Text.text = Dialog.dialog[DialogIndex].message;
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

        private void TakeItemFunc()
        {
            AllServices.Container.Single<IGameService>().GetGameController().TakeItemFromNpc(NpcInfo["Name"].ToString());
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideDialog();
        }

        private void GiveItemFunc()
        {
            AllServices.Container.Single<IGameService>().GetGameController().GiveItemToNpc(NpcInfo["Name"].ToString());
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideDialog();
        }

        private void GroupCharacter()
        {
            AllServices.Container.Single<IGameService>().GetGameController().GroupCharacter(NpcInfo["Name"].ToString());
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideDialog();
        }

        private void Trade()
        {
            AllServices.Container.Single<IGameService>().GetGameController().TradeWithCharacter(NpcInfo["Name"].ToString());
            AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.HideDialog();
        }
    }
}