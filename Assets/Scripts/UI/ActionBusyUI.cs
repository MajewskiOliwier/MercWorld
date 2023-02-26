using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{

    private void Start(){
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;

        hideBusyUI();
    }

    private void showBusyUI(){
        gameObject.SetActive(true);
    }

    private void hideBusyUI(){
        gameObject.SetActive(false);
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy){
        if(isBusy){
            showBusyUI();
        }else{
            hideBusyUI();
        }
    }

}
