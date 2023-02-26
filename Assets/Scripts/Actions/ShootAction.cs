using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction{     

    public static event EventHandler<OnShootEventArgs> OnAnyShoot; //created for screenShakeAction in order to avoid the need to keep track of every GO with ShootAction

    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs{
        public Unit targetedUnit;
        public Unit shootingUnit;
    }

    private enum State{
        Aiming,
        Shooting,
        Cooloff,
    }
    private float shootingStateTime = 0.1f;
    private float coolOffStateTime = 0.5f;
    private float aimingStateTime;


    [SerializeField] private LayerMask obstaclesLayerMask;
    private State state;
    [SerializeField] private int maxShootDistance = 7;  //TODO add method that allows for modifying maximum shooting range, assign wp stat dmg right before shooting
    float rotateSpeed = 3f;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;


    private void Update(){
        if(!isActive){
            return;
        }

        stateTimer -= Time.deltaTime;
        switch(state){
            case State.Aiming:{
                
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime*rotateSpeed);
                break;
            }
            case State.Shooting:{
                if(canShootBullet){
                    Shoot();
                    canShootBullet = false;
                }
                break;
            }
            case State.Cooloff:{
                break;
            }
        }

        if(stateTimer <= 0f){
            NextState();
        }
    }

    private void NextState(){
        switch(state){
            case State.Aiming:{
                if(stateTimer <= 0f){
                    state = State.Shooting;
                    stateTimer = shootingStateTime;
                }
                break;
            }
            case State.Shooting:{
                if(stateTimer <= 0f){
                    state = State.Cooloff;
                    stateTimer = coolOffStateTime;
                }
                break;
            }
            case State.Cooloff:{
                ActionComplete();
                break;
            }
        }
        

    }

    private void Shoot(){
        OnAnyShoot?.Invoke(this,new OnShootEventArgs{
            targetedUnit = targetUnit,
            shootingUnit = unit
        });

        OnShoot?.Invoke(this,new OnShootEventArgs{
            targetedUnit = targetUnit,
            shootingUnit = unit
        });

        targetUnit.Damage(UnitActionSystem.Instance.getUnitDamage()); // TODO take different weapon types and pass their damage value statistic
    }

    public override string GetActionName(){
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositionList(){    //overloaded function but only ,,first" one requires override keyword
        GridPosition unitGridPosition = unit.GetGridPosition();
        return  GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition){
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        
        maxShootDistance = UnitActionSystem.Instance.getUnitRange();
        for(int x = -maxShootDistance; x <= maxShootDistance; x++){
            for(int z = -maxShootDistance; z <= maxShootDistance ; z++){
                GridPosition offsetGridPostion = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPostion;

                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition)){
                    //Works when the grid Position is within range /not the same as it is standing on and within range/ isn't occupied
                    continue;
                }
                
                //for more triangular LOS might be useful with cone of vision
                //without it the shooting range is a square around the shooter
                int testDistance = Math.Abs(x) + Math.Abs(z);
                if(testDistance > maxShootDistance){
                    continue;
                }
                

                
                /*
                //for range testing purposes
                validGridPositionList.Add(testGridPosition);
                continue;
                */
                
                if(!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)){
                    //Gridposition ISN'T occupied with something to shoot at
                    continue;
                }
                //By the time we run below line we already checked with the function above if there are targets
                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if(targetUnit.IsEnemy() == unit.IsEnemy()){
                    //Both units are on the same 'Team'
                    continue;
                }

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition);
                float unitShoulderHeight = 1.7f;
                
                if(Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight,
                                  shootDir,
                                  Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                                  obstaclesLayerMask))
                {
                    //blocked by an obstacle
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete){
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.Aiming;
        aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        canShootBullet = true;
    
        ActionStart(onActionComplete);
    }

    public Unit GetTargetUnit(){
        return targetUnit;
    }

    public int GetMaxShootDistance(){
        maxShootDistance = UnitActionSystem.Instance.getUnitRange();
        return maxShootDistance;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition){
        
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition); 


        return new EnemyAIAction{
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition){     //to be used in loop checking all positions valid for moving
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}
