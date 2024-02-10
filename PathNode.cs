using Grid;

namespace PathFinder
{
    public class PathNode
    {
        private GridPosition gridPosition;

        private PathNode cameFromPathNode;

        private int gCost;
        private int hCost;
        private int fCost;
        private bool isWalkable = true;
        

        public PathNode(GridPosition gridPosition)
        {
            this.gridPosition = gridPosition;
        }



        public override string ToString() => gridPosition.ToString();

        public int GetHCost() => hCost;

        public int GetGCost() => gCost;

        public int GetFCost() => fCost;

        public void SetGCost(int i) => gCost = i;

        public void SetHCost(int i) => hCost = i;

        public void CalculateFCost() => fCost = gCost + hCost;

        public void ResetCameFromPathNode() => cameFromPathNode = null;

        public GridPosition GetGridPosition() => gridPosition;

        public void SetCameFromPathNode(PathNode neighbourNode) => cameFromPathNode = neighbourNode;

        public PathNode GetCamFromPathNode() => cameFromPathNode;

        public bool GetIsWalkable() => isWalkable;
        
        public void SetIsWalkable(bool b) => isWalkable = b;
    }
}