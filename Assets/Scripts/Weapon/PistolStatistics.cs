using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolStatistics : Weapon
{
    private bool isPrimary = false;
    private int weaponDamageStatistic = 10;
    private int weaponRangeStatistic = 3;

    public override int GetWeaponDamageStatistic(){
        return weaponDamageStatistic;
    }

    public override int GetWeaponRangeStatistic(){
        return weaponRangeStatistic;
    }

    public override bool GetIsPrimary(){
        return isPrimary;
    }
}
