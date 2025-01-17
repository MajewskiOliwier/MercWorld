using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    public static event EventHandler OnAnySwordHit; 

    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;
    //variables use with States
    private enum State{
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit,
    }
    private State state;
    private float stateTimer;

    private Unit targetUnit;
    private int maxSwordDistance = 1;


    private void Update(){
        if(!isActive){
            return;
        }

        stateTimer -= Time.deltaTime;
        switch(state){
            case State.SwingingSwordBeforeHit:{
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime*rotateSpeed);
                break;
            }
            case State.SwingingSwordAfterHit:{
                break;
            }
        }

        if(stateTimer <= 0f){
            NextState();
        }
    }

    private void NextState(){
        switch(state){
            case State.SwingingSwordBeforeHit:{
                state = State.SwingingSwordAfterHit;
                float afterHitStateTime = 0.5f;
                stateTimer = afterHitStateTime;
                targetUnit.Damage(100);
                OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                break;
            }
            case State.SwingingSwordAfterHit:{
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
            }
        }
        
        Debug.Log(state);
    }

    public override string GetActionName()
    {
        return "Sword";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction{
            gridPosition = gridPosition,
            actionValue = 200,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList(){
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition(); 

        for(int x = -maxSwordDistance; x <= maxSwordDistance; x++){   
            for(int z = -maxSwordDistance; z <= maxSwordDistance ; z++){
                GridPosition offsetGridPostion = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPostion;

                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition)){
                    //Works when the grid Position is within range /not the same as it is standing on and within range/ isn't occupied
                    continue;
                }
                
                if(!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)){
                    //Gridposition ISN'T occupied with something to shoot at
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if(targetUnit.IsEnemy() == unit.IsEnemy()){
                    //Both units are on the same 'Team'
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete){
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        state = State.SwingingSwordBeforeHit;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;
        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }
    
    public int GetMaxSwordDistance(){       //Later refactor for different weapon (mostly knives and new stuff)
        return maxSwordDistance;
    }
}
