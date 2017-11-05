using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map
{
    public const int GRID_SIZE = 64;
    public const int GAP = 1;

    public static float DROP_SPEED = 800;
    public int row = 12;
    public int col = 8;
    public int gridType = 5;

    public Color[] gridColors;

    public List<Node> selfBlankNodeList = new List<Node>();
    public List<Node> enermyBlankNodeList = new List<Node>();


    public List<Grid> boxList = null;
    public Dictionary<int, List<Grid>> selfColGridMap = new Dictionary<int, List<Grid>>();
    public Dictionary<int, List<Grid>> enermyColGridMap = new Dictionary<int, List<Grid>>();

    public Transform gridPrefab { get; private set; }
    private Transform gridParent;

    public Grid SpawnGrid(bool self, Node node = null)
    {
        List<Node> nodeList = selfBlankNodeList;
        if (!self)
        {
            nodeList = enermyBlankNodeList;
        }
        Transform grid = GameObject.Instantiate<Transform>(gridPrefab);
        grid.SetParent(gridParent);
        grid.localScale = Vector3.one;
        grid.localPosition = new Vector3(node.x * (GAP + GRID_SIZE), node.y * (GAP + GRID_SIZE), 0);
        Grid gridCom = grid.GetComponent<Grid>();
        gridCom.selfSide = self;
        gridCom.Color = gridColors[Random.Range(0, gridColors.Length - 1)];
       // gridCom.Color = gridColors[3];
        return gridCom;
    }

    public void AddColGridsMap(Grid grid)
    {
        if (grid.Y < boxList[grid.X].Y)
        {
            selfColGridMap[grid.X].Add(grid);
        }
        else
        {
            enermyColGridMap[grid.X].Add(grid);
        }
    }

    public void Remove(Grid grid)
    {
        List<Grid> toRemoveList = new List<Grid>();
        bool removedSelf = false;
        int x = grid.X;

        int colCount = selfColGridMap[x].Count;
        if (colCount >= 3 &&
            selfColGridMap[x][colCount - 3].Color == grid.Color &&
            selfColGridMap[x][colCount - 2].Color == grid.Color)
        {
            toRemoveList.Add(selfColGridMap[x][colCount - 3]);
            toRemoveList.Add(selfColGridMap[x][colCount - 2]);
            selfColGridMap[x].RemoveAt(colCount - 3);
            selfColGridMap[x].RemoveAt(colCount - 2);
            if(!removedSelf)
            {
                toRemoveList.Add(grid);
                selfColGridMap[x].RemoveAt(selfColGridMap[x].Count - 1);
                removedSelf = true;
            }
        }

        int[] flags = new int[5] { -1, -1, grid.Y, -1, -1 };

        for (int i = x - 1; i >= x - 2 && i >= 0; i--)
        {
            bool finded = false;
            List<Grid> gridList = selfColGridMap[i];
            for(int j = 0; j < gridList.Count; j++)
            {
                if( gridList[j] != null && gridList[j].Y == grid.Y && gridList[j].Color == grid.Color)
                {
                    flags[i - x + 2] = j;
                    finded = true;
                    break;
                }
            }
            if (!finded) break;
        }

        for (int i = x + 1; i <= x + 2 && i < col; i++)
        {
            bool finded = false;
            List<Grid> gridList = selfColGridMap[i];
            for (int j = 0; j < gridList.Count; j++)
            {
                if (gridList[j] != null && gridList[j].Y == grid.Y && gridList[j].Color == grid.Color)
                {
                    flags[i - x + 2] = j;
                    finded = true;
                    break;
                }
            }
            if (!finded) break;
        }


        int min = 2;
        int max = 2;
        for(int i = 1; i >= 0 ; i--)
        {
            if(flags[i] != -1)
            {
                min--;
                continue;
            }
            break;
        }
        for (int i = 3; i < 5; i++)
        {
            if (flags[i] != -1)
            {
                max++;
                continue;
            }
            break;
        }

        if(max - min + 1 >= 3 )
        {
            for(int i = min; i <= max; i++)
            {
                if (i == 2) continue;
                toRemoveList.Add(selfColGridMap[x + i - 2][flags[i]]);
                selfColGridMap[x + i - 2].RemoveAt(flags[i]);
            }
            if (!removedSelf)
            {
                toRemoveList.Add(grid);
                selfColGridMap[x].RemoveAt(selfColGridMap[x].Count - 1);
                removedSelf = true;
            }
        }

        for (int i = 0; i < toRemoveList.Count; i++)
        {
            GameObject.Destroy(toRemoveList[i].gameObject);
           // StartDroping();
        }
    }

    public void StartDroping()
    {
        for (int i = 0; i < col; i++)
        {
            List<Grid> colGridList = selfColGridMap[i];
            for (int j = 0; j < colGridList.Count; j++)
            {
                Grid grid = colGridList[j];
                if (j == 0)
                {
                    if (boxList[i].Y - grid.Y > 1)
                    {
                        grid.StartDrop(boxList[i]);
                    }
                    continue;
                }
                grid.StartDrop(colGridList[i - 1]);
            }
        }
    }

    public void RemoveBlankNode(bool self, Node node)
    {
        List<Node> nodeList = selfBlankNodeList;
        if (!self)
        {
            nodeList = enermyBlankNodeList;
        }
        for (int i = 0; i < nodeList.Count; i++)
        {
            if (nodeList[i].Equals(node))
            {
                nodeList.RemoveAt(i);
                return;
            }
        }
    }

    public void Init(Transform gridParent)
    {
        this.gridParent = gridParent;
        gridPrefab = Resources.Load<Transform>("Grid/Grid");
        boxList = new List<Grid>(col);
        for (int i = 0; i < col; i++)
        {
            boxList.Add(null);
        }
        gridColors = new Color[5] { Color.blue, Color.cyan, Color.green, Color.red, Color.yellow };//改
        for (int i = 0; i < col; i++)
        {
            selfColGridMap[i] = new List<Grid>();
            enermyColGridMap[i] = new List<Grid>();
        }
        Draw();
    }

    void Draw()
    {
        int rowCount = (row >> 1) - 1;
        for (int i = rowCount; i <= row >> 1; i++)
        {
            for (int j = 0; j < col; j++)
            {
                Transform grid = GameObject.Instantiate<Transform>(gridPrefab);
                grid.SetParent(gridParent);
                grid.localScale = Vector3.one;
                grid.localPosition = new Vector3(j * (GAP + GRID_SIZE), i * (GAP + GRID_SIZE), 0);
                Grid gridCom = grid.GetComponent<Grid>();
                if (i == row >> 1)
                {
                    if (j % 2 == 0)
                    {
                        boxList[j] = gridCom;
                    }
                    else
                    {
                        enermyColGridMap[j].Add(gridCom);
                    }
                    gridCom.Color = j % 2 == 0 ? Color.black : gridColors[UnityEngine.Random.Range(0, gridType)];
                    gridCom.selfSide = false;
                }
                else
                {
                    if (j % 2 != 0)
                    {
                        boxList[j] = gridCom;
                    }
                    else
                    {
                        selfColGridMap[j].Add(gridCom);
                    }
                    gridCom.Color = j % 2 == 0 ? gridColors[UnityEngine.Random.Range(0, gridType)] : Color.black;
                    gridCom.selfSide = true;
                }
            }
        }

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < col; j++)
            {
                selfBlankNodeList.Add(new Node(j, i));
                enermyBlankNodeList.Add(new Node(j, i + (row >> 1) + 1));
            }
        }
    }

}

public class Node
{
    public int x;
    public int y;
    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return string.Format("({0},{1})", x, y);
    }

    public override bool Equals(object obj)
    {
        Node node = obj as Node;
        return node.x == x && node.y == y;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
