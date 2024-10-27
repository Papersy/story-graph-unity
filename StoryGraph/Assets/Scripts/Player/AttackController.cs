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
 
        // private void Awake() {
        //     
        //     leftMouseClick = new InputAction(binding: "<Mouse>/leftButton");
        //     leftMouseClick.performed += ctx => LeftMouseClicked();
        //     leftMouseClick.Enable();
        // }
        //
        // private void LeftMouseClicked() {
        //     // if(!GameCanvas.IsUiActive)
        //     // {
        //     //     StartCoroutine(Attack());
        //     // }
        // }
    }
}