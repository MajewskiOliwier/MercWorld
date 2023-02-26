using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }        //no need for multiple UnitActionSystem

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    [SerializeField] private Unit selectedUnit; //
    [SerializeField] private Transform selectedUnitTransform; //
    [SerializeField] private LayerMask selectedUnitMask;

    private int selectedUnitDamage;
    private int selectedUnitRange;

    private bool isBusy;
    private BaseAction selectedAction;

    private void Awake(){
        if(Instance != null){
            Debug.LogError("There's more than one UnityActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        SetSelectedUnit(selectedUnit);
        SetSelectedUnitTransform(selectedUnitTransform);
        GetWeaponStats();
    }

    private void Update(){
        if(isBusy){
            return;
        }

        if(!TurnSystem.Instance.IsPlayerTurn()){
            return;
        }

        if(EventSystem.current.IsPointerOverGameObject()){
            return;
        }

        if(TryHandleUnitSelection()){
            return;
        }

        HandleSelectedAction();
    }

    private void HandleSelectedAction(){
        if(InputManager.Instance.IsMouseButtonDownThisFrame()){
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if(!selectedAction.IsValidActionGridPosition(mouseGridPosition)){
                return ;
            }
            
            if(!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction)){
                return ;
            }
            
            SetBusy();
            
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);

            OnActionStarted?.Invoke(this,EventArgs.Empty);
        }
    }

    private void SetBusy(){
        isBusy = true;

        OnBusyChanged?.Invoke(this,isBusy);
    }

    private void ClearBusy(){
        isBusy = false;

        OnBusyChanged?.Invoke(this,isBusy);
    }

    private bool TryHandleUnitSelection(){
        if(InputManager.Instance.IsMouseButtonDownThisFrame()){
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, selectedUnitMask)){
                if((raycastHit.transform.TryGetComponent<Unit>(out Unit unit)) && 
                   (raycastHit.transform.TryGetComponent<Transform>(out Transform unitTransform))){
                    if(unit == selectedUnit){
                        //unit is already selected
                        return false;
                    }
                    if(unit.IsEnemy()){
                        //Clicked on an enemy
                        return false;
                    }
                    
                    SetSelectedUnitTransform(unitTransform);
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        return false;
    }   

    private void SetSelectedUnit(Unit unit){
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>());
        OnSelectedUnitChanged?.Invoke(this,EventArgs.Empty);
        /* wykonuje to samo co ponizszy kod
        if(OnSelectedUnitChanged != null){
            OnSelectedUnitChanged(this, EventArgs.Empty);
        }
        */
    }

    private void SetSelectedUnitTransform(Transform unitTransform){
        selectedUnitTransform = unitTransform;
    }

    public void SetSelectedAction(BaseAction baseAction){
        selectedAction = baseAction;
        OnSelectedActionChanged?.Invoke(this,EventArgs.Empty);
    }

    public Unit GetSelectedUnit(){
        return selectedUnit;
    } 

    public Transform GetSelectedUnitTransform(){
        return selectedUnitTransform;
    }

    public BaseAction GetSelectedAction(){
        return selectedAction;
    }

    
    public void GetWeaponStats(){
        Transform unitTransform = GetSelectedUnitTransform();
        foreach(Transform transform in unitTransform.GetComponent<WeaponSelector>().GetWeaponHoldingTransform()){

            if((transform.TryGetComponent<Weapon>(out Weapon weapon)) && (transform.gameObject.activeInHierarchy)){
                selectedUnitDamage = weapon.GetWeaponDamageStatistic();
                selectedUnitRange = weapon.GetWeaponRangeStatistic();
            }
        }
        Debug.Log(selectedUnitRange);
    }

    public int getUnitDamage(){
        return selectedUnitDamage;
    }

    public int getUnitRange(){
        return selectedUnitRange;
    }
    
}

