using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeAction : BaseAction
{
    [SerializeField] private Transform granadeProjectilePrefab;

    private int maxThrowDistance = 7;
    
    private void Update() {
        if(!isActive){
            return;
        }
    }

    public override string GetActionName(){
        return "Granade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition){
        return new EnemyAIAction{
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList(){
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition(); 

        for(int x = -maxThrowDistance; x <= maxThrowDistance; x++){
            for(int z = -maxThrowDistance; z <= maxThrowDistance ; z++){
                GridPosition offsetGridPostion = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPostion;

                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition)){
                    //Works when the grid Position is within range /not the same as it is standing on and within range/ isn't occupied
                    continue;
                }
                
                int testDistance = Math.Abs(x) + Math.Abs(z);
                if(testDistance > maxThrowDistance){
                    continue;
                }
                

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete){
        Transform granadeProjectileTransform = Instantiate(granadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        GranadeProjectile granadeProjectile = granadeProjectileTransform.GetComponent<GranadeProjectile>();
        granadeProjectile.Setup(gridPosition,OnGranadeBehaviourComplete);
        ActionStart(onActionComplete);
    }

    private void OnGranadeBehaviourComplete(){
        ActionComplete();
    }
}
