using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    //bool isPrimary;
    //int weaponDamage;
    
    public abstract int GetWeaponDamageStatistic();

    public abstract int GetWeaponRangeStatistic();

    public abstract bool GetIsPrimary();
    
}
