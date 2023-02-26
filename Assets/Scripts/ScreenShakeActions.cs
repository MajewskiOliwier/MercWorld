using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private void Start(){
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        GranadeProjectile.onAnyGranadeExploded += GranadeProjectile_OnAnyGranadeExploded;
        SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit;
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e){
        ScreenShake.Instance.Shake();
    }

    private void GranadeProjectile_OnAnyGranadeExploded(object sender, EventArgs e){
        ScreenShake.Instance.Shake(5f);
    }

    private void SwordAction_OnAnySwordHit(object sender, EventArgs e){
        ScreenShake.Instance.Shake(2f);
    }

}
