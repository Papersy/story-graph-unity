using UnityEngine;
using UnityEngine.UI;

namespace ActionButtons
{
    public class ButtonTakeAndGive : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void SetImage(string itemName)
        {
            var sprite = Resources.Load<Sprite>($"JsonFiles/Items/{itemName}");
            
            if(sprite == null)
                return;

            _image.sprite = sprite;
        }
    }
}