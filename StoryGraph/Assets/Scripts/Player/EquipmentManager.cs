using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Player
{
    public class EquipmentManager : MonoBehaviour
    {
        [SerializeField] private GameObject _sword;
        
        public static EquipmentManager Instance;

        private JToken _weapon;
        
        private void Awake()
        {
            Instance = this;
        }

        public void PickUpWeapon(JToken weapon)
        {
            _weapon = weapon;

            switch (weapon["Name"].ToString())
            {
                case "Sword":
                    _sword.SetActive(true);
                    break;
            }
        }

        public void ClearEquipment(JToken equipment)
        {
            if(equipment == _weapon)
                ClearWeapon();
        }

        private void ClearWeapon()
        {
            _sword.SetActive(false);
        }
        
    }
}