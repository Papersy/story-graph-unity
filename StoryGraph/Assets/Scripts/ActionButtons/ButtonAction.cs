using TMPro;
using UnityEngine;

namespace ActionButtons
{
    public class ButtonAction : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        
        public void SetText(string text) =>
            _text.text = text;
    }
}