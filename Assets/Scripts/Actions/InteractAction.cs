using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction{
    private int maxInteractDistance = 1;

    private void Update() {
        if(!isActive){
            return;
        }
    }

    public override string GetActionName()
    {
        return "Interact";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction{
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition(); 

        for(int x = -maxInteractDistance; x <= maxInteractDistance; x++){   
            for(int z = -maxInteractDistance; z <= maxInteractDistance ; z++){
                GridPosition offsetGridPostion = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPostion;

                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition)){
                    //Works when the grid Position is within range /not the same as it is standing on and within range/ isn't occupied
                    continue;
                }
                
                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);
                if(interactable == null){
                    continue;   //no interactable on this gridposition
                }

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete){
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);

        interactable.Interact(onInteractComplete);

        ActionStart(onActionComplete);
    }
    
    private void onInteractComplete(){
        ActionComplete();
    }
}


