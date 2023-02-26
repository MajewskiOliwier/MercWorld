using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedGameObject;

    private BaseAction baseAction;

    public void SetBaseAction(BaseAction baseAction){
        this.baseAction = baseAction;
        textMeshPro.text = baseAction.GetActionName().ToUpper();

        //1 line below we use anonymus function/lambda expression of type void ,,Empty ()" instead of creating seprarate declaration
        button.onClick.AddListener(() => {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
            UpdateSelectedVisual();
        });
    }

    public void UpdateSelectedVisual(){
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedGameObject.SetActive(selectedBaseAction == baseAction);
    }
}
