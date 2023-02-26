using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectionVisual : MonoBehaviour
{
    //variables and struct needed for custom color for actions
    [Serializable] public struct WeaponTypeEquipped{
        public WeaponType gridVisualType;
        public Transform weaponPrefab;
    }
    public enum WeaponType{
        Pistol,
        Rifle 
    }
    [SerializeField] private List<WeaponTypeEquipped> weaponTypeEquippedList;
    
}
