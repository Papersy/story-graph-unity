using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using InteractableItems;
using UnityEngine;

namespace Player
{
    public class CheckInteraction : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ItemWrapper item))
            {
                var gmController = AllServices.Container.Single<IGameService>().GetGameController();
                if (gmController.CanGetKnowledgeFromItem(item.Item.ItemInfo["Id"]?.ToString()))
                    gmController.GetKnowledgeFromItem(item.Item.ItemInfo["Id"]?.ToString());
            }
        }
    }
}