using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDead;
    public event EventHandler OnDamaged;

    [SerializeField] private int health = 100;
    private int healthMax = 100;

    private void Awake() {
        healthMax = health;
    }

    public void TakeDamage(int damageAmount){
        health -= damageAmount;

        if(health < 0){
            health = 0;
        }
        OnDamaged?.Invoke(this,EventArgs.Empty);
        if(health == 0){
            Die();
        }
        Debug.Log(health);
    }

    private void Die(){
        OnDead?.Invoke(this, EventArgs.Empty);    
    }

    public float GetHealthNormalized(){
        return ((float)health/(float)healthMax);
    }
}