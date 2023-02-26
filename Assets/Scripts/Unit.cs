using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 9; //Will require refactoring after addition of traits

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy;
    private bool usesPrimary;

    //grid variables
    private GridPosition gridPosition;
    private GridPosition oldGridPosition;

    //statistics variables
    private int actionPoints = ACTION_POINTS_MAX;
    private HealthSystem healthSystem;
    
    //ActionTypes: UPDATE: no longer required now we utilize c# generics which cycles through the list in search for passed ActionType
    private BaseAction[] baseActionArray;


    private void Awake() {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start() {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this,EventArgs.Empty);
    }
    private void Update() {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(newGridPosition != gridPosition){
            oldGridPosition = gridPosition;
            gridPosition = newGridPosition;

            LevelGrid.Instance.UnitMovedGridPosition(this.GetComponent<Unit>(), oldGridPosition, newGridPosition);
        }
    }

    public T GetAction<T>() where T: BaseAction{    //c# generics replaces for example public MoveAction GetMoveAction(){return moveAction;}
        foreach(BaseAction baseAction in baseActionArray){
            if(baseAction is T){
                return (T)baseAction;
            }
        }
        return null;
    }

    public GridPosition GetGridPosition(){
        return gridPosition;
    }

    public Vector3 GetWorldPosition(){
        return transform.position;
    }

    public BaseAction[] GetBaseActionArray(){
        return baseActionArray;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction){
        if(CanSpendActionPointsToTakeAction(baseAction)){
            SpedActionPoints(baseAction.GetActionPointsCost());
            return true;
        }else{
            //TODO vfx that shows lack of AP to spend
            return false;
        }
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction){
        // alternative form : ,,return actionpoints >= baseAction.GetActionPointsCost();"

        if(actionPoints >= baseAction.GetActionPointsCost()){
            return true;
        }else{
            return false;
        }
    }

    private void SpedActionPoints(int amountToSpend){
        //So long as this function is used after checking if we are able to spend AP it doesn't need more validating
        actionPoints -= amountToSpend;

        OnAnyActionPointsChanged?.Invoke(this,EventArgs.Empty);
    }

    public int GetActionPoints(){
        return actionPoints;
    }

    private void TurnSystem_OnTurnChanged(object Sender, EventArgs e){
        if((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) || (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn())){
            actionPoints = ACTION_POINTS_MAX;
            
            OnAnyActionPointsChanged?.Invoke(this,EventArgs.Empty);
        }
    }

    public bool IsEnemy(){
        return isEnemy;
    }

    public void Damage(int damageAmount){
        healthSystem.TakeDamage(damageAmount);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e){
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);// TODO add animation instead of ragdoll?

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }
    
    public float GetHealthNormalized(){
        return healthSystem.GetHealthNormalized();  //how the fuck does this work
    }

    public bool IsUsingPrimary(){
        return usesPrimary;
    }

    public void ChangeWeapon(bool isPrimary){
        usesPrimary = isPrimary;
    }
}
