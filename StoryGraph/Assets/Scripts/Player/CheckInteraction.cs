using System.Collections.Generic;
using InteractableItems;
using Npc;
using UnityEngine;

namespace Player
{
    public class CheckInteraction : MonoBehaviour
    {
        public static Dictionary<string, string> CollisionIds = new Dictionary<string, string>();
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ItemWrapper item))
            {
                var itemId = item.Item.ItemInfo["Id"].ToString();
                var itemName = item.Item.ItemInfo["Name"].ToString();
                CollisionIds.Add(itemId, itemName);
                
                Debug.Log("Add item");
                Debug.Log(CollisionIds.Count);
            }

            if (other.TryGetComponent(out NpcWrapper npc))
            {
                var npcId = npc.Npc.NpcInfo["Id"].ToString();
                var npcName = npc.Npc.NpcInfo["Name"].ToString();
                CollisionIds.Add(npcId, npcName);  
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out ItemWrapper item))
            {
                var itemId = item.Item.ItemInfo["Id"].ToString();
                CollisionIds.Remove(itemId);
                
                Debug.Log("Remove Item");
                Debug.Log(CollisionIds.Count);
            }
            if (other.TryGetComponent(out NpcWrapper npc))
            {
                var npcId = npc.Npc.NpcInfo["Id"].ToString();
                CollisionIds.Remove(npcId);  
            }
        }
    }
}