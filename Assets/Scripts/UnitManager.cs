using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{

    public static UnitManager Instance { get; private set; }    //no need for multiple UnitManager

    private List<Unit> unitList;
    private List<Unit> friendlyUnitList;
    private List<Unit> enemyUnitList;


    private void Awake() {
        if(Instance != null){
            Debug.LogError("There's more than one UnityActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        unitList = new List<Unit>();
        friendlyUnitList = new List<Unit>();
        enemyUnitList = new List<Unit>();
    }

    private void Start(){               //Make sure that this UnitManager,Start runs after Unit.Start otherwise it won't add units to the list properly 
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e){
        Unit unit = sender as Unit;

        // Debug.Log(unit + " spawned"); // for debuging spawned units, will be useful later when enemies may spawn mid mission

        unitList.Add(unit);
        if(unit.IsEnemy()){
            enemyUnitList.Add(unit);
        }else{
            friendlyUnitList.Add(unit);
        }

    }
    
    private void Unit_OnAnyUnitDead(object sender, EventArgs e){
        Unit unit = sender as Unit;

        //Debug.Log(unit + " died"); // for debugging in case there is discrepancy in the lists

        unitList.Remove(unit);
        if(unit.IsEnemy()){
            enemyUnitList.Remove(unit);
        }else{
            friendlyUnitList.Remove(unit);
        }
    }

    public List<Unit>GetUnitList(){
        return unitList;
    }
    

    public List<Unit>GetFriendlyUnitList(){ // might be usefull when adding more check when a friendly soldier dies
        return friendlyUnitList;
    }
    

    public List<Unit>GetEnemyUnitList(){
        return enemyUnitList;
    }
}
