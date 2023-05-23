using System;
using System.Collections;
using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using LocationDir;
using UnityEngine;
using UnityEngine.InputSystem;

class RayCastLook : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float distance;

    private RaycastHit _hit;
    
    private void FixedUpdate()
    {
        var rayDirection = mainCamera.transform.forward;
        var startPosDirection = startPoint.forward;
        Debug.DrawRay(startPoint.position, rayDirection * distance, Color.green);

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (Mathf.Abs(startPosDirection.x - rayDirection.x) > 0.4f)
            {
                Debug.Log("Too far of my head!!!");
                return;
            }
            
            if (Physics.Raycast(startPoint.position, rayDirection, out _hit, distance))
            {
                StartCoroutine(PickUp());
            }
        }
    }

    private IEnumerator PickUp()
    {
        animator.SetBool("PickUp", true);

        yield return new WaitForSeconds(0.75f);
        
        animator.SetBool("PickUp", false);
        
        var itemName = _hit.transform.GetComponent<Item>().ItemInfo["Name"].ToString();
        AllServices.Container.Single<IGameService>().GetGameController().PickItem(itemName);
                
        Destroy(_hit.transform.gameObject);
    }
}