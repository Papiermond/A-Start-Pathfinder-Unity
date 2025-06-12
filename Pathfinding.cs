using System.Collections.Generic;
using UnityEngine;
using Grid;


namespace PathFinder
{
    public class Pathfinding : MonoBehaviour
    {
        public static Pathfinding instance { get; private set; }

        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        private int _width;
        private int _length;


        [SerializeField] private Transform gridDebugPrefab;
        [SerializeField] static private bool update = false;
        [SerializeField] static private LayerMask layerMask;
        private GridSystem<PathNode> _gridSys;

        private void Awake()
        {
            //Singleton init
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);
        }

        // Init New GridSystem
        public void Init(int width, int length, float cellSize)
        {
            _width = width;
            _length = length;
            _gridSys = new GridSystem<PathNode>(width, length, cellSize,
                (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));

            //Create Debug Objects for the new GridSystem
            if (update)
                _gridSys.CreateDebugObjekts(gridDebugPrefab, update);

            ClacIsWalkable();
        }

        public void ClacIsWalkable()
        {
            //Walk through the whole grid to determiner if the node is walkable or not
            for (int x = 0; x < _width; x++)
            for (int z = 0; z < _length; z++)
            {
                float offset = 5f;
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = _gridSys.GetWorldPosition(gridPosition);

                if (Physics.Raycast(worldPosition + Vector3.down * offset, Vector3.up, offset * 2, layerMask))
                {
                    GetNode(x, z).SetIsWalkable(false);
                }
            }
        }

        public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition,
            out int pathlenght)
        {
            // Create Lists to Calculate and return later
            List<PathNode> openNodes = new List<PathNode>();
            List<PathNode> closedNodes = new List<PathNode>();

            //Create PathNodes for each of the two points
            PathNode startNode = _gridSys.GetGridObject(startGridPosition);
            PathNode endNode = _gridSys.GetGridObject(endGridPosition);

            openNodes.Add(startNode);

            //loop through all nodes to Clear them
            for (int x = 0; x < _gridSys.GetWidth(); x++)
            {
                for (int z = 0; z < _gridSys.GetLength(); z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);
                    PathNode pathNode = _gridSys.GetGridObject(gridPosition);

                    // Reset all nodes
                    pathNode.SetGCost(int.MaxValue);
                    pathNode.SetHCost(0);
                    pathNode.CalculateFCost();
                    pathNode.ResetCameFromPathNode();


                }
            }

            // Calc start node Values
            startNode.SetGCost(0);
            startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
            startNode.CalculateFCost();


            //Cicel through the Nodes 
            while (openNodes.Count > 0)
            {
                PathNode currentPathNode = GetLowestFCostPathNode(openNodes);

                if (currentPathNode == endNode)
                {
                    //Reached Final Node
                    pathlenght = endNode.GetFCost();
                    return CalculatePath(endNode);
                }

                openNodes.Remove(currentPathNode);
                closedNodes.Add(currentPathNode);

                foreach (PathNode neighbourNode in GetNeighboringNodes(currentPathNode))
                {
                    //Check if node is not already closed-off
                    if (closedNodes.Contains(neighbourNode)) continue;
                    if (!neighbourNode.GetIsWalkable())
                    {
                        closedNodes.Add(neighbourNode);
                        continue;
                    }

                    int tentativeGCost =
                        currentPathNode.GetGCost() + CalculateDistance(currentPathNode.GetGridPosition(),
                            neighbourNode.GetGridPosition());

                    if (tentativeGCost < neighbourNode.GetGCost())
                    {
                        neighbourNode.SetCameFromPathNode(currentPathNode);
                        neighbourNode.SetGCost(tentativeGCost);
                        neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                        neighbourNode.CalculateFCost();

                        if (!openNodes.Contains(neighbourNode))
                        {
                            openNodes.Add(neighbourNode);
                        }
                    }
                }

            }

            // No Path found
            pathlenght = 0;
            return null;
        }

        private List<GridPosition> CalculatePath(PathNode endNode)
        {
            List<PathNode> pathNodes = new List<PathNode>();
            pathNodes.Add(endNode);

            PathNode currentNode = endNode;

            while (currentNode.GetCamFromPathNode() != null)
            {
                pathNodes.Add(currentNode.GetCamFromPathNode());
                currentNode = currentNode.GetCamFromPathNode();
            }

            pathNodes.Reverse();


            List<GridPosition> pathPositions = new List<GridPosition>();
            foreach (PathNode pathNode in pathNodes)
            {
                pathPositions.Add(pathNode.GetGridPosition());
            }

            return pathPositions;
        }


        //Calc a rough Distance between the start node and the end node
        public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
        {
            GridPosition gridPositionDistance = gridPositionA - gridPositionB;

            int xDistance = Mathf.Abs(gridPositionDistance.x);
            int zDistance = Mathf.Abs(gridPositionDistance.z);
            int remaining = Mathf.Abs(xDistance - zDistance);

            return /*MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) +*/ MOVE_STRAIGHT_COST * remaining;
        }

        //Get the PathNode with the Lowest FCost
        private PathNode GetLowestFCostPathNode(List<PathNode> pathNodes)
        {
            PathNode LowestFCostNode = pathNodes[0];

            for (int i = 0; i < pathNodes.Count; i++)
            {
                if (pathNodes[i].GetFCost() < LowestFCostNode.GetFCost())
                {
                    LowestFCostNode = pathNodes[i];
                }
            }

            return LowestFCostNode;
        }

        //Get all Neighboring Nodes of the current node to circle through
        private List<PathNode> GetNeighboringNodes(PathNode currentNode)
        {
            List<PathNode> neighbors = new List<PathNode>();

            GridPosition gridPosition = currentNode.GetGridPosition();

            //Left
            if (gridPosition.x - 1 >= 0)
            {
                neighbors.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));

                // //LeftUp
                // if(gridPosition.z + 1 < _gridSys.GetLength())
                //     neighbors.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
                //
                // //LeftDown
                // if(gridPosition.z -1 >= 0 )
                //     neighbors.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
            }

            //Right
            if (gridPosition.x + 1 < _gridSys.GetWidth())
            {
                neighbors.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));

                //RightUp
                // if(gridPosition.z + 1 < _gridSys.GetLength())
                //     neighbors.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
                //
                // //RightDown
                // if(gridPosition.z -1 >= 0 )
                //     neighbors.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));

            }

            //Down
            if (gridPosition.z - 1 >= 0)
            {
                neighbors.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));
            }

            //Up
            if (gridPosition.z + 1 < _gridSys.GetLength())
            {
                neighbors.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));
            }



            return neighbors;
        }

        // Get The Node with the given GridPositions
        private PathNode GetNode(int gridPositionX, int gridPositionZ)
        {
            return _gridSys.GetGridObject(new GridPosition(gridPositionX, gridPositionZ));
        }

        public bool isWalkableGridPosition(GridPosition gridPosition)
        {
            return GetNode(gridPosition.x, gridPosition.z).GetIsWalkable();
        }

        public bool HasPath(GridPosition s_gridPosition, GridPosition e_gridPosition)
        {
            return FindPath(s_gridPosition, e_gridPosition, out int pathlenght) != null;
        }

        public int GetPathLenght(GridPosition startGridPosition, GridPosition endGridPosition)
        {
            FindPath(startGridPosition, endGridPosition, out int pathlenght);
            return pathlenght;
        }

        public PathNode GetPathNode(GridPosition gridPosition)
        {
            return _gridSys.GetGridObject(gridPosition);
        }
        
        public GridPosition GetGridPosition(Vector3 position)
        {
            return _gridSys.GetGridPosition(position);
        }

    }
}