using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class AttackController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _attackObj;
        
        private InputAction leftMouseClick;
 
        private void Awake() {
            
            leftMouseClick = new InputAction(binding: "<Mouse>/leftButton");
            leftMouseClick.performed += ctx => LeftMouseClicked();
            leftMouseClick.Enable();
        }
 
        private void LeftMouseClicked() {
            // if(!GameCanvas.IsUiActive)
            // {
            //     StartCoroutine(Attack());
            // }
        }
        
        private IEnumerator Attack()
        {
            const string attack = "Attack";
            
            _animator.SetBool(attack, true);
            _attackObj.SetActive(true);

            yield return new WaitForSeconds(0.75f);
        
            _attackObj.SetActive(false);
            _animator.SetBool(attack, false);
            _attackObj.SetActive(false);
        }
    }
}