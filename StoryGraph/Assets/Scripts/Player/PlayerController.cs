using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float speed = 12f;
        [SerializeField] private float gravity = 9.81f;

        private Vector3 velocity;
    
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        private void Update()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            _characterController.Move(move * speed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;

            _characterController.Move(velocity * Time.deltaTime);
        }

        public void EnableCharacterController(bool status) =>
            _characterController.enabled = status;
    }
}