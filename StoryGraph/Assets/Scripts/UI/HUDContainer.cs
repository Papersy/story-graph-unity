using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HUDContainer : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            HideUI();
            DontDestroyOnLoad(this);
        }
        
        public void ShowUI()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void HideUI()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}