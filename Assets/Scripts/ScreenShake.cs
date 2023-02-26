using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }        //no need for multiple ScreenShakes

    private CinemachineImpulseSource cinemachineImpulseSource;

    private void Awake() {
        if(Instance != null){
            Debug.LogError("There's more than one UnityActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    } 

    private void Update(){
        //if(Input.GetKeyDown(KeyCode.T)){
        //    cinemachineImpulseSource.GenerateImpulse();
        //}
    }

    public void Shake(float intensity = 1f){
        cinemachineImpulseSource.GenerateImpulse(intensity);
    }
}
