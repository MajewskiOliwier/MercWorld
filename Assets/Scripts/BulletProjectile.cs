using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRender;
    [SerializeField] private Transform bulletHitVfxPrefab;

    private Vector3 targetPosition;
    private float moveSpeed = 200f;
    private float distanceBeforeMoving;
    private float distanceAfterMoving;

    public void Setup(Vector3 targetPosition){
        this.targetPosition = targetPosition;
    }

    private void Update() {
        Vector3 moveDir = (targetPosition - transform.position).normalized;

        distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);
        
        if(distanceBeforeMoving < distanceAfterMoving){
            transform.position = targetPosition;
            
            trailRender.transform.parent = null;

            Destroy(gameObject);

            Instantiate(bulletHitVfxPrefab, targetPosition, Quaternion.identity);
        }
    }
}
