using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LocationInfoUI : BaseWindow
    {
        [SerializeField] private GameObject _newLocationPanel;
        [SerializeField] private TextMeshProUGUI _newLocationText;

        public IEnumerator NewLocationAnimation(string locationName)
        {
            _newLocationPanel.SetActive(true);
            _newLocationText.text = locationName;

            yield return new WaitForSeconds(2f);
            
            _newLocationPanel.SetActive(false);
        }
    }
}