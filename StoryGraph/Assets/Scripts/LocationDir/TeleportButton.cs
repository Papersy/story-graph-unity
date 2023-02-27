using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using UnityEngine;

namespace LocationDir
{
    public class TeleportButton : MonoBehaviour
    {
        private string _locationId;

        public void Init(string locationId) =>
            _locationId = locationId;
        
    }
}