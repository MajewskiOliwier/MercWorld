using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText;
    

    private List<ActionButtonUI> actionButtonUIList; 

    private void Awake() {
        actionButtonUIList = new List<ActionButtonUI>();    
    }

    private void Start() {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;

        UpdateActionPoints();
        CreateActionButtons();
        UpdateSelectedVisual();
    }


    private void CreateActionButtons(){
        foreach(Transform buttonTransform in actionButtonContainerTransform){
            Destroy(buttonTransform.gameObject);
        }

        actionButtonUIList.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        foreach(BaseAction baseAction in selectedUnit.GetBaseActionArray()){
            Transform actionButtonTransfrom = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransfrom.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);
            actionButtonUIList.Add(actionButtonUI);
        }
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e){
        CreateActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e){
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e){
        UpdateActionPoints();
    }

    private void UpdateSelectedVisual(){
        foreach(ActionButtonUI actionButtonUI in actionButtonUIList){
            //in case of border display error (or rahter lack there of) we should restructure the order of child objects
            actionButtonUI.UpdateSelectedVisual();
            // wszystkie dzieci w buttontransform wylaczyc
        }
        //wziasc aktualnie wybrany button i wlaczyc w jego dziecku enable na ,,selected"
    }

    private void UpdateActionPoints(){
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        actionPointsText.text = "AP: " + selectedUnit.GetActionPoints(); 
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e){
        UpdateActionPoints();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e){
        UpdateActionPoints();
    }
}
