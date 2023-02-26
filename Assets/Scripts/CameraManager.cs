using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject actionCameraGameObject;

    //variables needed for placing camera behind shooters back
    Vector3 cameraCharacterHeight;
    Vector3 shootDir;
    Vector3 shoulderOffset;
    Vector3 actionCameraPosition;
    Unit shooterUnit;
    Unit targetUnit;
    float shoulderOffsetAmount = 0.5f;

    private void Awake() {    
        HideActionCamera();
    }

    private void Start(){
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
    }

    private void ShowActionCamera(){
        actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera(){
        actionCameraGameObject.SetActive(false);
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e){
        switch(sender){  //in case of action requiring different camera the switch may be expanded upon (eg. following granade like in demonhunters)
            case ShootAction shootAction:{
                shooterUnit = shootAction.GetUnit();
                targetUnit = shootAction.GetTargetUnit();

                cameraCharacterHeight = Vector3.up * 1.7f;
                shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

                shoulderOffset = Quaternion.Euler(0,90,0) * shootDir * shoulderOffsetAmount;

                actionCameraPosition = shooterUnit.GetWorldPosition() + cameraCharacterHeight+ shoulderOffset + (shootDir * -1);
                actionCameraGameObject.transform.position = actionCameraPosition; // places camera on x and z axis (?)
                actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight); // places camera at specific height

                ShowActionCamera();
                break;
            }
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e){
        switch(sender){
            case ShootAction shootAction:{
                HideActionCamera();
                break;
            }
        }
    }


}
