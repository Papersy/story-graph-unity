using UnityEngine;

namespace LocationDir
{
    public class CharacterInfo : MonoBehaviour
    {
        [SerializeField] private string name;

        public string GetName() => name;
    }
}