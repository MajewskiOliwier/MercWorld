using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }        // SINGLETON PATTERN aka no need for multiple UnitActionSystem
    
    private const int MOVE_STRAIGHT_COST = 10;  //  h
    private const int MOVE_DIAGONAL_COST = 14;  //  g //normaly it would be 1.4 (1^2 + 1^2 = x^2 => sqrt(2)) by why bother with that when we can multiply by 10 and use int for calculations

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstacleLayerMask;
    
    private int width;
    private int height;
    private float cellSize;
    private GridSystem<PathNode> gridSystem;

    private void Awake() {
        if(Instance != null){
            Debug.LogError("There's more than one UnityActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //end of Singleton pattern

        
    }

    public void Setup(int width, int height, float cellSize){
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(width,height, cellSize, 
            (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);  //uncomment first part in need of debuging pathfind and occupied gridObjects (most likely doors haha)

        // GetNode(1,0).SetIsWalkable(false); // for testing , might be usefull when implementing cover/slowing obstacles
        for(int x = 0; x < width ; x++){
            for(int z = 0; z < height ; z++){
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float raycastOffsetDistance = 5f;    // TODO fix for a problem that raycast wouldn't work if it was fired from INSIDE obstacle | ALTERNATIVELY edit->projectSettings->Physics-> queries hit backfaces -> turn on
                if(Physics.Raycast(         //checks if raycast hit gameobject on layer obstacles if yes then it changes bool for isWalkable to false
                    worldPosition + Vector3.down * raycastOffsetDistance,
                    Vector3.up,
                    raycastOffsetDistance*2,
                    obstacleLayerMask))
                {
                        GetNode(x, z).SetIsWalkable(false);
                }
            }
        }

    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength){  //a* pathfinding algorithm implementation
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);

        openList.Add(startNode);

        for(int x = 0; x < gridSystem.GetWidth(); x++){
            for(int z = 0; z < gridSystem.GetHeight(); z++){
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while(openList.Count > 0){
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if(currentNode == endNode){
                //Reached final node
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode neighbourNode in GetNeighbourList(currentNode)){   //used for ignoring already searched neighbours
                if(closedList.Contains(neighbourNode)){
                    continue;
                }

                if(!neighbourNode.IsWalkable()){    //skips all nodes that are unwalkable
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());    //tentative == uncertain/anticipative
                if(tentativeGCost < neighbourNode.GetGCost()){
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if(!openList.Contains(neighbourNode)){
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        //No path found
        pathLength = 0;
        return null;
    }

    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB){
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining  = Mathf.Abs(xDistance - zDistance);
        return (MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance)) + (MOVE_STRAIGHT_COST * remaining); // for moving straight totalDistance abs(gpd.x)+abs(gpd.z) /* totalDistance * MOVE_STRAIGHT_COST */
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathnodeList){
        PathNode lowestFCostPathNode = pathnodeList[0];
        for(int i = 0; i < pathnodeList.Count; i++){
            if(pathnodeList[i].GetFCost() < lowestFCostPathNode.GetFCost()){
                lowestFCostPathNode = pathnodeList[i];
            }
        }

        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z){
        return gridSystem.GetGridObject(new GridPosition(x,z));
    }


    private List<PathNode> GetNeighbourList(PathNode currentNode){
        List<PathNode> neighbourList = new List<PathNode>();
        GridPosition gridPosition = currentNode.GetGridPosition();

        if((gridPosition.x - 1) >= 0){  //lots of ifs , refactor for more presentable code???
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0)); //LeftNode

            if((gridPosition.z - 1) >= 0){
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1)); //LeftDownNode
            }
            if((gridPosition.z + 1 ) < gridSystem.GetHeight()){  
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1)); //LeftUpNode
            }
        }

        if((gridPosition.x + 1) < gridSystem.GetWidth()){
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0)); //RightNode

            if((gridPosition.z - 1) >= 0){
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1)); //RightDownNode
            }
            if((gridPosition.z + 1) < gridSystem.GetHeight()){  
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1)); //RightUpNode
            }
        }

        if((gridPosition.z - 1) >= 0){
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1)); //DownNode
        }
        
        if((gridPosition.z + 1) < gridSystem.GetHeight()){ 
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1)); //UpNode
        }


        return neighbourList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode){
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);  //the element at the end of list is first node which requires for us to reverse this list
        PathNode currentNode = endNode;

        while(currentNode.GetCameFromPathNode() != null){   //if not null this has some node connected to it
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach(PathNode pathNode in pathNodeList){
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        return gridPositionList;
    }

    public bool ISWalkableGridPosition(GridPosition gridPosition){
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }

    public void SetISWalkableGridPosition(GridPosition gridPosition, bool isWalkable){
        gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition){
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;    //test this
    }

    public int GetPathLength(GridPosition startGridPosition,GridPosition endGridPosition){
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}
