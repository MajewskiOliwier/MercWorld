using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    [SerializeField] private int maxMoveDistance = 4;



    private List<Vector3> positionList;
    private int currentPositionIndex;

    float moveSpeed;
    float rotateSpeed;

    private void Update(){
        if(!isActive){
            return;
        }

        float stoppingDistance = 0.1f;
        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        
        rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward,moveDirection, Time.deltaTime*rotateSpeed);
        
        if(Vector3.Distance(transform.position, targetPosition) > stoppingDistance){
            moveSpeed = 4f;
            transform.position += moveDirection * Time.deltaTime * moveSpeed;
            
        }else{
            currentPositionIndex++;
            if(currentPositionIndex >= positionList.Count){
                OnStopMoving?.Invoke(this,EventArgs.Empty);

                ActionComplete();
            }
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete){    //gridPosition is target gridPosition, confirm and refactor later
            List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

            currentPositionIndex = 0;
            positionList = new List<Vector3>();
            foreach(GridPosition pathGridPosition in pathGridPositionList){
                positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
            }

            OnStartMoving?.Invoke(this, EventArgs.Empty);

            ActionStart(onActionComplete);
    }


    public override List<GridPosition> GetValidActionGridPositionList(){
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for(int x = -maxMoveDistance; x <= maxMoveDistance; x++){
            for(int z = -maxMoveDistance; z <= maxMoveDistance ; z++){
                GridPosition offsetGridPostion = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPostion;

                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition) || unitGridPosition == testGridPosition || LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)){
                    //Works when the grid Position is within range /not the same as it is standing on and within range/ isn't occupied
                    continue;
                }
                if(unitGridPosition == testGridPosition){
                    //Works when the grid Position is within range /not the same as it is standing on and within range/ isn't occupied
                    continue;
                }
                if(LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)){
                    //Works when the grid Position is within range /not the same as it is standing on and within range/ isn't occupied
                    continue;
                }

                if(!Pathfinding.Instance.ISWalkableGridPosition(testGridPosition)){
                    continue;
                }

                if(!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition)){
                    continue;
                }

                int pathFindingDistanceMultiplayer = 10;
                if(Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathFindingDistanceMultiplayer){
                    //Path length is too long
                    continue;
                }

            validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override string GetActionName(){
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition){

        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction{
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition*10,
        };
    }
}
