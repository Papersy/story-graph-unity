using System;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
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
        public Button StartBattle;

        public int DialogIndex = 0;

        private Dialog _dialog;
        public Dialog Dialog
        {
            get => _dialog;
            set
            {
                _dialog = value;
                DialogIndex = 0;
                
                TakeItem.gameObject.SetActive(false);
                GiveItem.gameObject.SetActive(false);
                
                SetInfo();
            }
        }

        private void OnEnable()
        {
            NextFrame.onClick.AddListener(SkipFrame);
            TakeItem.onClick.AddListener(TakeItemFunc);
            GiveItem.onClick.AddListener(GiveItemFunc);
        }

        private void OnDisable()
        {
            NextFrame.onClick.RemoveListener(SkipFrame);
            TakeItem.onClick.RemoveListener(TakeItemFunc);
            GiveItem.onClick.RemoveListener(GiveItemFunc);
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

        private void TakeItemFunc() =>
            AllServices.Container.Single<IGameService>().GetGameController().TakeItemFromNpc(Dialog.npc_name);
        
        private void GiveItemFunc() =>
            AllServices.Container.Single<IGameService>().GetGameController().GiveItemToNpc(Dialog.npc_name);
    }
}