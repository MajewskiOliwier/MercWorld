using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private bool invert;
    private Transform cameraTransform;

    Vector3 dirToCamera;

    private void Awake() {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate(){
        if(invert){
            dirToCamera = (cameraTransform.position - transform.position).normalized;
            transform.LookAt(transform.position + dirToCamera * -1);
        }else{
            transform.LookAt(cameraTransform);
        }
    }
}
