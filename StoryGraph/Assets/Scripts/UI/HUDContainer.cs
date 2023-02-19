using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HUDContainer : MonoBehaviour
    {
        public GameCanvas GameCanvas;
        
        private CanvasGroup canvasGroup;
        
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