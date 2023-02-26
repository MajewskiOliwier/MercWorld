using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControler : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private Vector3 targetFollowOffset;
    private CinemachineTransposer cinemachineTransposer;


    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 12f;

    private void Start() {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    private void Update(){
        HandleMovement();
        HandleRotation();
        HandleZoom();
        
        
    }

    private void HandleMovement(){
        Vector2 inputMoveDirection = InputManager.Instance.GetCameraMoveVector();
        float moveSpeed = 15f;

        
        Vector3 moveVector = transform.forward * inputMoveDirection.y + transform.right * inputMoveDirection.x; //Because GetCameraMoveVector returns vector2 inputMoveDirection.y changes move direction on z axis
        //Vector3 moveVector allows dynamically adjust camera movement direction so that eg. pressing ,,D" key will always move camera in right direction no matter the rotation
        transform.position += moveVector * moveSpeed * Time.deltaTime;

    }

    private void HandleRotation(){
        Vector3 rotationVector = new Vector3(0, 0, 0);

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();

        float rotationSpeed = 100f;
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    private void HandleZoom(){
        float zoomIncreaseAmount = 1f;  //Currently it does nothing but there might be a need to modify zoomAmount which could be done by modifying this variable
        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;
        
        Debug.Log(InputManager.Instance.GetCameraZoomAmount() );


        float zoomSpeed = 10f;
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y , MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
            
        cinemachineTransposer.m_FollowOffset = 
           Vector3.Lerp(cinemachineTransposer.m_FollowOffset,targetFollowOffset, Time.deltaTime*zoomSpeed);
    }
}
