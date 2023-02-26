using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    [SerializeField] Transform weaponHoldingTransform;  //no need to change that
    bool isUsingPrimary = true;

    public bool GetIsUsingPrimary(){
        return isUsingPrimary;
    }

    public void InvertIsUsingPrimary(){
        isUsingPrimary = !isUsingPrimary;
    }

    public Transform GetWeaponHoldingTransform(){
        return weaponHoldingTransform;
    }

}
