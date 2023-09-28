using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ActionButtons
{
    public class ButtonAction : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        public Button Button;
        
        public void SetText(string text) =>
            _text.text = text;
    }
}