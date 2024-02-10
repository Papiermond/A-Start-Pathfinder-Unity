using Grid;
using TMPro;
using UnityEngine;


namespace PathFinder
{


    public class PathfindingGridDebugObject : GridDebugObj
    {
        [SerializeField] private TextMeshPro hCostText;
        [SerializeField] private TextMeshPro gCostText;
        [SerializeField] private TextMeshPro fCostText;
        [SerializeField] private bool isWalkable;

        private PathNode pathNode;

        public override void SetGridObj(object gridObject, bool shouldDebug)
        {
            base.SetGridObj(gridObject, shouldDebug);
            pathNode = (PathNode)gridObject;


            
        }

        protected override void UpdateVisual()
        {
            base.UpdateVisual();
            UpdatePathNodeText();
        }

        private void UpdatePathNodeText()
        {
            hCostText.text = pathNode.GetHCost().ToString();
            gCostText.text = pathNode.GetGCost().ToString();
            fCostText.text = pathNode.GetFCost().ToString();
            isWalkable = pathNode.GetIsWalkable();

        }
    }
}
