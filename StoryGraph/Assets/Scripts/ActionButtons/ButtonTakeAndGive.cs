using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ActionButtons
{
    public class ButtonTakeAndGive : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        public void SetText(string text) =>
            _text.text = text;
        
        public void SetImage(string itemName)
        {
            var sprite = Resources.Load<Sprite>($"JsonFiles/Items/{itemName}");
            
            if(sprite == null)
                return;

            _image.sprite = sprite;
        }
    }
}