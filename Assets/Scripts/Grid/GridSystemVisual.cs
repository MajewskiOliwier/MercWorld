using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }


    //variables and struct needed for custom color for actions
    [Serializable] public struct GridVisualTypeMaterial{
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType{
        White,
        Blue,
        Red,
        RedSoft,
        Yellow
    }
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
    //end

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    
    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private void Awake(){
        if(Instance != null){
            Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    private void Start(){
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight()
        ];
        for(int x = 0; x < LevelGrid.Instance.GetWidth(); x++){
            for(int z = 0; z < LevelGrid.Instance.GetHeight(); z++){
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity); 
                //it is grouping the Single instances of grid system under one parent in order to clear the hierarchy
                gridSystemVisualSingleTransform.transform.parent = GameObject.Find("GroupedObjects").transform;

                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
                
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;

        UpdateGridVisual();
    }

    public void HideAllGridPosition(){
        for(int x = 0; x < LevelGrid.Instance.GetWidth(); x++){
            for(int z = 0; z < LevelGrid.Instance.GetHeight(); z++){
                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType){
        List<GridPosition> gridPositionList = new List<GridPosition>();
        Debug.Log("ShowGridPositionRange");

        for(int x = -range; x <= range; x++){
            for(int z = -range; z <= range; z++){
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);
                
                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition)){
                    continue;
                }

                int testDistance = Math.Abs(x) + Math.Abs(z); // for more triangular range check ShootAction for more information
                if(testDistance > range){
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType){
        List<GridPosition> gridPositionList = new List<GridPosition>();
        
        for(int x = -range; x <= range; x++){
            for(int z = -range; z <= range; z++){
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition)){
                    continue;
                }
                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType){
        foreach(GridPosition gridPosition in gridPositionList){
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void UpdateGridVisual(){
        HideAllGridPosition();
        
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction  = UnitActionSystem.Instance.GetSelectedAction();
        GridVisualType gridVisualType;

        
        int weaponRange = UnitActionSystem.Instance.getUnitRange();
        
        switch(selectedAction){ //switch test type of action and assign the specific color to it
            default://gets assigned color form the top case
            case MoveAction moveAction:{
                gridVisualType = GridVisualType.White;
                break;
            }
            case SpinAction spinAction:{
                gridVisualType = GridVisualType.Blue;
                break;
            }
            case ShootAction shootAction:{
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(),weaponRange, GridVisualType.RedSoft);
                break;
            }
            case GranadeAction granadeAction:{
                gridVisualType = GridVisualType.Yellow;
                break;
            }
            case SwordAction swordAction:{
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(),swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft);    
                break;
            }
            case InteractAction interactAction:{
                gridVisualType = GridVisualType.Blue;   
                break;
            }
            case SwitchAction switchAction:{
                gridVisualType = GridVisualType.Yellow;   
                break;
            }
        }
        ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e){
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e){
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType){
        foreach(GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList){
            if(gridVisualTypeMaterial.gridVisualType == gridVisualType){
                return gridVisualTypeMaterial.material;
            }
        }
        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType "+gridVisualType);
        return null; // failsafe
    }
}
