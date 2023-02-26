using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchAction : BaseAction
{
    private bool usesPrimary = true;

    private void Start(){   
        Setup();    // turns off all rhand gameObjects and the turn back primaryWeapon 
        WeaponSwitched();   //assigns weapon stats
    }

    private void Update() {
        if(!isActive){
            return;
        }
        Switch();
        ActionComplete();
    }

    public override string GetActionName(){
        return "Switch";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition){
        return new EnemyAIAction{
            gridPosition = gridPosition,
            actionValue = 10,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList(){
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition>{
            unitGridPosition
        };
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete){

        ActionStart(onActionComplete);
    }

    public override int GetActionPointsCost(){
        return 0;
    }

    private void WeaponSwitched(){
        UnitActionSystem.Instance.GetWeaponStats();
    }

    private void Switch(){  //it works but it breaks GridSystemVisual
        foreach(Transform transform in unit.GetComponent<WeaponSelector>().GetWeaponHoldingTransform()){
            if(transform.TryGetComponent<Weapon>(out Weapon weapon)){
                
                 if(!transform.gameObject.activeInHierarchy){   //pistol starts hidden in 
                    transform.gameObject.SetActive(true);
                    usesPrimary = false;
                    UnitActionSystem.Instance.GetSelectedUnit().ChangeWeapon(true);
                }else{
                    transform.gameObject.SetActive(false);
                    usesPrimary = true;
                    UnitActionSystem.Instance.GetSelectedUnit().ChangeWeapon(false);
                }

            }
        }
        WeaponSwitched();
        Debug.Log("Using primary ? "+usesPrimary);
    }

    private void Setup(){
        foreach(Transform transform in unit.GetComponent<WeaponSelector>().GetWeaponHoldingTransform()){
            transform.gameObject.SetActive(false);
            if((transform.TryGetComponent<Weapon>(out Weapon weapon)) && weapon.GetIsPrimary()){
                transform.gameObject.SetActive(true);
                usesPrimary = true;
            }
        }
    }
}
