using System;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

namespace LocationDir
{
    public class Teleport : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _teleportName;

        private Transform _target;
        private JToken variant;
        public JToken Variant
        {
            get => variant;
            set
            {
                variant = value;
                _teleportName.text = variant[2]["WorldNodeName"].ToString();
            }
        }

        // private void Start()
        // {
        //     _target = AllServices.Container.Single<IGameService>().GetGameController().GetPlayerTransform();
        // }
        //
        // public void Update()
        // {
        //     if(_target != null)
        //         _teleportName.transform.LookAt(_target);
        // }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                AllServices.Container.Single<IGameService>().GetGameController().ChangeLocation(Variant);
        }
    }
}