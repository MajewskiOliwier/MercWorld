using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }

    public event EventHandler OnTurnChanged;

    private int turnNumber;
    private bool isPlayerTurn;

    private void Awake() {
        if(Instance != null){
            Debug.LogError("There's more than one TurnSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        turnNumber = 1;
        isPlayerTurn = true;
    }

    public void NextTurn(){
       isPlayerTurn = !isPlayerTurn;
       if(isPlayerTurn){
            turnNumber++; 
       }
       OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnNumber(){
        return turnNumber;
    }
    
    public bool IsPlayerTurn(){
        return isPlayerTurn;
    }
}
