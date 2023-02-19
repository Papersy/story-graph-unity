using UnityEngine;

namespace UI
{
    public class GameCanvas : MonoBehaviour
    {
        [SerializeField] private GameObject canvas;

        public void Show() =>
            canvas.SetActive(true);

        public void Hide() =>
            canvas.SetActive(false);
    }
}