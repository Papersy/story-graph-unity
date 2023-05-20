using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private GameObject _playerMesh;

        public Transform Transform => _playerMesh.transform;
        private Vector3 velocity;
        
    
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
        

        public void EnableCharacterController(bool status)
        {
            _characterController.enabled = status;
            _playerMesh.SetActive(status);
        }
    }
}