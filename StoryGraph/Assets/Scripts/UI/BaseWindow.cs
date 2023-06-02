using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using UnityEngine;

namespace UI
{
    public class BaseWindow : MonoBehaviour
    {
        [SerializeField] private bool hideOnStart = true;

        public IGameService GameService;
        
        public void Awake()
        {
            GameService = AllServices.Container.Single<IGameService>();
            
            if (hideOnStart)
                Hide();
        }

        public void Show()
        {
            if (gameObject.activeSelf) return;
            gameObject.SetActive(true);

            OnShow();
        }

        public void Hide()
        {
            if (!gameObject.activeSelf) return;
            gameObject.SetActive(false);

            OnHide();
        }

        public virtual void OnShow()
        {
        }

        public virtual void OnHide()
        {
        }
    }
}