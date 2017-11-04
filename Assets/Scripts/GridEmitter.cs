using System;
using System.Collections.Generic;

public class GridEmitter
{
    public bool isSelf { get; private set; }
    public int gridCount { get; private set; }

    private Map _curMap;
    private List<Grid> _curGrids = new List<Grid>();
    public void Init(bool self,int gridCount = 3)
    {
        this.isSelf = self;
        this.gridCount = gridCount;
        GameController gmCtrl = Singleton.GetInstance("GameController") as GameController;
        _curMap = gmCtrl.map;
    }

    public List<Grid> SpawnGrids()
    {
        Clear(true);
        int indexCol = UnityEngine.Random.Range(1, _curMap.col - 1);
        for(int i = 0; i < gridCount; i++)
        {
            _curGrids.Add(_curMap.SpawnGrid(isSelf, new Node(indexCol - 1 + i, isSelf ? -1 : _curMap.row)));
        }
        return _curGrids;
    }

    public void Step(int stepDir)
    {
        if (_curGrids.Count == 0) return;
        if (stepDir == -1 && _curGrids[0].X == 0) return;
        if (stepDir == 1 && _curGrids[2].X == _curMap.col - 1) return;
        for (int i = 0; i < _curGrids.Count; i++)
        {
            Grid grid = _curGrids[i];
            grid.transform.localPosition = new UnityEngine.Vector3((grid.X + stepDir) * (Map.GRID_SIZE + Map.GAP), grid.Y * (Map.GRID_SIZE + Map.GAP), 0);
        }
    }

    public void Shot()
    {
        if (_curGrids.Count == 0) return;
        for(int i = 0; i < _curGrids.Count; i++)
        {
            Grid grid = _curGrids[i];
            List<Grid> colGrids = _curMap.selfColGridMap[grid.X];
            if(colGrids.Count == 0)
            {
                grid.StartDrop(_curMap.boxList[grid.X]);
            }
            else
            {
                grid.StartDrop(colGrids[colGrids.Count - 1]);
            }
        }
        Clear(false);
    }

    private void Clear(bool destroy)
    {
        if (_curGrids.Count != 0)
        {
            if(destroy)
            {
                for (int i = 0; i < _curGrids.Count; i++)
                {
                    UnityEngine.GameObject.Destroy(_curGrids[i].gameObject);
                }
            }
            _curGrids.Clear();
        }
    }
}
