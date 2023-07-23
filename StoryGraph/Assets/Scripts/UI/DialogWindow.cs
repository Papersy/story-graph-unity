using System;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class DialogWindow : BaseWindow
    {
        public Image PlayerAvatar;
        public Image NpcAvatar;
        public TextMeshProUGUI Text;
        public Button NextFrame;

        public Button GetItem;
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
            if(DialogIndex >= Dialog.dialog.Count)
                return;

            if (Dialog.dialog[DialogIndex].side == 1)
            {
                PlayerAvatar.gameObject.SetActive(true);
                NpcAvatar.gameObject.SetActive(false);
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