using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Player
{
    public class AttackController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private InputAction leftMouseClick;
 
        private void Awake() {
            leftMouseClick = new InputAction(binding: "<Mouse>/leftButton");
            leftMouseClick.performed += ctx => LeftMouseClicked();
            leftMouseClick.Enable();
        }
 
        private void LeftMouseClicked() {
            if(!GameCanvas.IsUiActive)
            {
                StartCoroutine(Attack());
                print("LeftMouseClicked");
            }
        }
        
        private IEnumerator Attack()
        {
            const string attack = "Attack";
            
            _animator.SetBool(attack, true);

            yield return new WaitForSeconds(0.75f);
        
            _animator.SetBool(attack, false);
        }
    }
}