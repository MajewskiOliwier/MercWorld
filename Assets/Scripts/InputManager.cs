#define USE_NEW_INPUT_SYSTEM        // if we comment this line from the begining #else will take the effect 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour   // !!! for this to work in make sure that: Edit->Project Settings->Player->Configuration-> Active Input Handling->Both
{
    public static InputManager Instance { get; private set; }        //no need for multiple InputManagers

    private PlayerInputActions playerInputActions; 

    private void Awake(){
        if(Instance != null){
            Debug.LogError("There's more than one UnityActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }


    public Vector2 GetMouseScreenPosition(){
        #if USE_NEW_INPUT_SYSTEM //it works in compiletime and not in runtime
            return Mouse.current.position.ReadValue();  //new input system (requires ,,using UnityEngine.InputSystem")
        #else
            return Input.mousePosition; //old input system
        #endif   
    }
    
    public bool IsMouseButtonDownThisFrame(){
        #if USE_NEW_INPUT_SYSTEM
            return playerInputActions.Player.Click.WasPressedThisFrame();
        #else
            return Input.GetMouseButtonDown(0);
        #endif
        
    }

    public Vector2 GetCameraMoveVector(){
        #if USE_NEW_INPUT_SYSTEM
            return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
        #else
            Vector2 inputMoveDirection = new Vector2(0, 0);
            if(Input.GetKey(KeyCode.W)){
                inputMoveDirection.y = +1f;
            }
            if(Input.GetKey(KeyCode.S)){
                inputMoveDirection.y = -1f;
            }
            if(Input.GetKey(KeyCode.A)){
                inputMoveDirection.x = -1f;
            }
            if(Input.GetKey(KeyCode.D)){
                inputMoveDirection.x = +1f;
            }
            return inputMoveDirection;
        #endif
    }

    public float GetCameraRotateAmount(){
        #if USE_NEW_INPUT_SYSTEM
            return playerInputActions.Player.CameraRotate.ReadValue<float>();
        #else
            float rotateAmount = 0f;
            if(Input.GetKey(KeyCode.Q)){
                rotateAmount = -1f;
            }
            if(Input.GetKey(KeyCode.E)){
                rotateAmount = +1f;
            }
            return rotateAmount;
        #endif
    }

    public float GetCameraZoomAmount(){
        #if USE_NEW_INPUT_SYSTEM
            return playerInputActions.Player.CameraZoom.ReadValue<float>();
        #else
            float zoomAmount = 0f;
            //Input.mouseScrollDelta.y works like Input.GetKey it work once it is activated as opposed to Input.GetKeyDown which is working for as long as the button is pressed;
            // for inverted zooming it is just a matter of chaning higher than  to less than
            if(Input.mouseScrollDelta.y > 0){
                zoomAmount = -1f;
            }
            if(Input.mouseScrollDelta.y < 0){
                zoomAmount = +1f;
            }

            return zoomAmount;
        #endif
    }

}
