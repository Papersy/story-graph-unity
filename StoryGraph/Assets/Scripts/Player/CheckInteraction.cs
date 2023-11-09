using System.Collections.Generic;
using InteractableItems;
using Npc;
using UnityEngine;

namespace Player
{
    public class CheckInteraction : MonoBehaviour
    {
        public static Dictionary<string, Transform> CollisionIds = new Dictionary<string, Transform>();
        private static Collider[] hitColliders;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ItemWrapper item))
            {
                var itemId = item.Item.ItemInfo["Id"].ToString();
                var itemName = item.Item.ItemInfo["Name"].ToString();
                CollisionIds.Add(itemId, item.transform);
                // CollisionIds.Add(itemId, itemName);
            }

            if (other.TryGetComponent(out NpcWrapper npc))
            {
                var npcId = npc.Npc.NpcInfo["Id"].ToString();
                var npcName = npc.Npc.NpcInfo["Name"].ToString();
                CollisionIds.Add(npcId, npc.transform);  
                // CollisionIds.Add(npcId, npcName);  
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out ItemWrapper item))
            {
                var itemId = item.Item.ItemInfo["Id"].ToString();
                CollisionIds.Remove(itemId);
            }
            if (other.TryGetComponent(out NpcWrapper npc))
            {
                var npcId = npc.Npc.NpcInfo["Id"].ToString();
                CollisionIds.Remove(npcId);  
            }
        }
    }
}