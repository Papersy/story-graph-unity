using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

namespace LocationDir
{
    public class Teleport : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI[] _teleportNames;

        private Transform _target;
        private JToken variant;
        public JToken Variant
        {
            get => variant;
            set
            {
                variant = value;
                foreach (var teleportName in _teleportNames)
                {
                    teleportName.text = variant[2]["WorldNodeName"].ToString();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                AllServices.Container.Single<IGameService>().GetGameController().ChangeLocation(Variant);
        }
    }
}