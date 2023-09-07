using UnityEngine;
using UnityEngine.UI;

namespace ActionButtons
{
    public class ButtonTrade : MonoBehaviour
    {
        [SerializeField] private Image _image1;
        [SerializeField] private Image _image2;
        
        public void SetImage(string itemName1, string itemName2)
        {
            var sprite1 = Resources.Load<Sprite>($"JsonFiles/Items/{itemName1}");
            
            if(sprite1 == null)
                return;

            _image2.sprite = sprite1;
            
            var sprite2 = Resources.Load<Sprite>($"JsonFiles/Items/{itemName2}");
            
            if(sprite2 == null)
                return;

            _image2.sprite = sprite2;
        }
    }
}