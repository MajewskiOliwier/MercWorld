using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleStatistics : Weapon
{
    private bool isPrimary = true;
    private int weaponDamageStatistic = 25;
    private int weaponRangeStatistic = 5;

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
