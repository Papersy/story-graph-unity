using System;
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
                SetInfo();
            }
        }

        private void OnEnable()
        {
            NextFrame.onClick.AddListener(SkipFrame);
        }

        private void OnDisable()
        {
            NextFrame.onClick.RemoveListener(SkipFrame);
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
                return;
            }

            if (Dialog.dialog[DialogIndex].side == 1)
            {
                PlayerAvatar.gameObject.SetActive(true);
                NpcAvatar.gameObject.SetActive(false);
                
                NpcAvatar.sprite = Resources.Load<Sprite>("JsonFiles/AvatarIcon/default");
            }
            else if (Dialog.dialog[DialogIndex].side == 2)
            {
                PlayerAvatar.gameObject.SetActive(false);
                NpcAvatar.gameObject.SetActive(true);
            }
            
            Text.text = Dialog.dialog[DialogIndex].message;
        }
    }
}