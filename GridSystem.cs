using System;
using Grid;
using TMPro;
using UnityEngine;



namespace Grid 
{
    public class GridSystem<TGridObject> 
    {
        private readonly float _cellSize;
        private readonly TGridObject[,] _gridObjects;
        private readonly int _lenght;
        private readonly int _width;
        private bool _shouldDebug;

        public GridSystem(int width, int lenght, float cellSize,Func<GridSystem<TGridObject>,GridPosition, TGridObject> createGridObject) {
            _width = width;
            _lenght = lenght;
            _cellSize = cellSize;

            _gridObjects = new TGridObject[width, lenght];

            for (var x = 0; x < _width; x++)
            for (var z = 0; z < _lenght; z++) {
                var gridPosition = new GridPosition(x, z);
                _gridObjects[x, z] = createGridObject(this, gridPosition);
            }
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition) {
            return new Vector3(gridPosition.x, 0, gridPosition.z) * _cellSize;
        }
        public Vector3 GetWorldPosition(GridPosition gridPosition,float a)
        {
            return new Vector3(gridPosition.x, 0, gridPosition.z) * _cellSize;
        }

        public GridPosition GetGridPosition(Vector3 worldPosition) {
            return new GridPosition(Mathf.RoundToInt(worldPosition.x / _cellSize),
                Mathf.RoundToInt(worldPosition.z / _cellSize));
        }

        public TGridObject GetGridObject(GridPosition pos) {
            return _gridObjects[pos.x , pos.z];
        }

       public void CreateDebugObjekts(Transform debugPrefab, bool shouldDebugUpdate) {
           for (var x = 0; x < _width; x++)
           for (var z = 0; z < _lenght; z++) {
               GridPosition gridPosition = new GridPosition(x, z);
               Transform obj = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition,0), Quaternion.identity);
               GridDebugObj debugObj = obj.GetComponent<GridDebugObj>();
               debugObj.SetGridObj(GetGridObject(gridPosition), shouldDebugUpdate);
           }
       }

        public bool IsValidGridPosition(GridPosition gridPosition) {
            return gridPosition.x >= 0
                   && gridPosition.z >= 0
                   && gridPosition.x < _width
                   && gridPosition.z < _lenght;
        }

        public int GetWidth()
        {
            return _width;
        }

        public int GetLength()
        {
            return _lenght;
        }
    }
}

namespace Grid
{
    [Serializable]
    public struct GridPosition : IEquatable<GridPosition>
    {
        public int x;
        public int z;

        public bool Equals(GridPosition other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is GridPosition position && x == position.x && z == position.z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, z);
        }

        public GridPosition(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public override string ToString()
        {
            return "x: " + x + " z: " + z;
        }

        public static bool operator ==(GridPosition a, GridPosition b)
        {
            return a.x == b.x && a.z == b.z;
        }

        public static bool operator !=(GridPosition a, GridPosition b)
        {
            return !(a == b);
        }

        public static GridPosition operator +(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.x + b.x, a.z + b.z);
        }

        public static GridPosition operator -(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.x - b.x, a.z - b.z);
        }

        public static GridPosition operator *(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.x * b.x, a.z * b.z);
        }

        public static GridPosition operator /(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.x / b.x, a.z / b.z);
        }

        public static GridPosition operator *(GridPosition a, int b)
        {
            return new GridPosition(a.x * b, a.z * b);
        }

        public static GridPosition operator /(GridPosition a, int b)
        {
            return new GridPosition(a.x / b, a.z / b);
        }

        public static int GetDistance(GridPosition a, GridPosition b)
        {
            int r = 0;
            r = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.z - b.z, 2)));
            return r;
        }
    }
}
namespace Grid
{
    public class GridDebugObj : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _tmPro;
        private object _gridObject;
        private bool _shouldDebugUpdate;
        private bool DebugMode;

        private void Awake()
        {

        }

        private void Update()
        {
            if (!(_shouldDebugUpdate && DebugMode)) return;
            UpdateVisual();
        }

        public virtual void SetGridObj(object gridObject, bool shouldDebug)
        {
            _gridObject = gridObject;
            _shouldDebugUpdate = shouldDebug;
        }

        protected virtual void UpdateVisual()
        {
            _tmPro.SetText(_gridObject.ToString());
        }

    }
}