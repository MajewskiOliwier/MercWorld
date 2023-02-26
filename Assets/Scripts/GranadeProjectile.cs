using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeProjectile : MonoBehaviour
{
    public static event EventHandler onAnyGranadeExploded;

    [SerializeField] private Transform granadeExplodeVfxPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;

    private Action OnGranadeBehaviourComplete;
    private Vector3 targetPosition;
    private float totalDistance;
    private Vector3 positionXZ;


    [SerializeField] private float moveSpeed = 15f;
    private float reachedTargetDistance= 0.2f; //how close granade needs to be near the target for it to explode
    [SerializeField] private float damageRadius= 4f;

    private void Update(){
        Vector3 moveDir = (targetPosition - positionXZ).normalized;
        positionXZ += moveDir * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;   //Distance is normal decreasing but for this calculation it has to INCREASE

        float maxHeight = totalDistance / 4f;    //maxHeight is calculated that way so the closer the throw is being made the smaller arc 
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x , positionY, positionXZ.z); 
       
        if(Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance){
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);

            foreach(Collider collider in colliderArray){    //UHHH what about walls explosion go through walls
                if(collider.TryGetComponent<Unit>(out Unit targetUnit)){
                    targetUnit.Damage(30);      //Consider making damage dropoff based on the offset from centre of explosion (remember that diagonal are different size)
                }

                if(collider.TryGetComponent<DestructableCrate>(out DestructableCrate destructableCrate)){
                    destructableCrate.Damage();      //Destructible create ??? maybe not with granade but with some axe or sth
                }
            }
            onAnyGranadeExploded?.Invoke(this, EventArgs.Empty);

            trailRenderer.transform.parent = null;
            Instantiate(granadeExplodeVfxPrefab, targetPosition+ Vector3.up * 1f, Quaternion.identity);

            Destroy(gameObject);

            OnGranadeBehaviourComplete();
        }
    }
    public void Setup(GridPosition targetGridPosition, Action OnGranadeBehaviourComplete){
        this.OnGranadeBehaviourComplete = OnGranadeBehaviourComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
