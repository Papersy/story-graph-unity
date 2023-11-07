using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Player
{
    public class EquipmentManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] _weapons;
        [SerializeField] private GameObject _sword;
        
        [SerializeField] private GameObject _defaultWeapon;
        private GameObject _currentWeapon;
        private JToken _weapon;
        
        public static EquipmentManager Instance;
        
        private void Awake() => Instance = this;

        public void PickUpWeapon(JToken weapon)
        {
            _weapon = weapon;
            var weaponName = weapon["Name"].ToString().ToLower();
            
            foreach (var w in _weapons)
            {
                var wName = w.gameObject.name;
                if (wName == weaponName)
                {
                    w.SetActive(true);
                    _currentWeapon = w;
                    return;
                }
            }

            _defaultWeapon.SetActive(true);
            _currentWeapon = _defaultWeapon;
        }

        public void ClearEquipment(JToken equipment)
        {
            if(equipment == _weapon)
                _currentWeapon.SetActive(false);
        }
    }
}