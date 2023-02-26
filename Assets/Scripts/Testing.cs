using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Unit unit;     

    private void Start(){
    }

    private void Update(){

        //screenShake testing
        /*
        if(Input.GetKeyDown(KeyCode.T)){
            ScreenShake.Instance.Shake(5f);
        }
        */



        /*
        if(Input.GetKeyDown(KeyCode.T)){
            //GridSystemVisual.Instance.HideAllGridPosition();
            //GridSystemVisual.Instance.ShowGridPositionList(unit.GetMoveAction().GetValidActionGridPositionList());

            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());   //add if checking if Gridposition is walkable
            GridPosition startGridPosition = new GridPosition(0, 0);

            List<GridPosition> gridPositionList = Pathfinding.Instance.FindPath(startGridPosition, mouseGridPosition);

            
            //if(!Pathfinding.Instance.ISWalkableGridPosition(testGridPosition)){   // modify this to fix testing unwalkable on unwalkable terrain error 
            //        continue;
            //}

            for(int i = 0; i < (gridPositionList.Count - 1); i++){ 
                Debug.DrawLine(LevelGrid.Instance.GetWorldPosition(gridPositionList[i]),
                               LevelGrid.Instance.GetWorldPosition(gridPositionList[i+1]),
                               Color.red,
                               10f
                );
            }
        }
        */
    }
}
