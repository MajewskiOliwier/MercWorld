using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSphere : MonoBehaviour, IInteractable
{
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    private bool isGreen;
    private GridPosition gridPosition;

    //variables for Interact method
    private Action onInteractComplete;
    private float timer;
    private bool isActive;

    private void Start(){
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        SetColorGreen();
    }

    private void Update() {
        if(!isActive){
            return;
        }

        timer -= Time.deltaTime;
        if(timer <= 0f){
            isActive = false;
            onInteractComplete();
        }
    }

    private void SetColorGreen(){
        isGreen = true;
        meshRenderer.material = greenMaterial;
    }

    private void SetColorRed(){
        isGreen = false;
        meshRenderer.material = redMaterial;
    }

    public void Interact(Action onInteractComplete){
        this.onInteractComplete = onInteractComplete;
        isActive = true;
        timer = 0.5f;
        if(isGreen){
            SetColorRed();
        }else{
            SetColorGreen();
        }
    }
}
