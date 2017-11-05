using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    [HideInInspector]
    public bool selfSide;

    private bool _dropping;
    private Color _color;
    private Grid _followGrid;

    private void TimeEventUpdate(float deltaTime)
    {
        if (_dropping)
        {
            if (selfSide)
            {
                if (transform.localPosition.y < _followGrid.transform.localPosition.y - Map.GRID_SIZE - Map.GAP)
                {
                    transform.localPosition += Vector3.up * Map.DROP_SPEED * deltaTime;
                }
                else
                {
                    if (_followGrid.Dropping)
                    {
                        transform.localPosition += Vector3.up * Map.DROP_SPEED * deltaTime;
                    }
                    else
                    {
                        transform.localPosition = new Vector3(_followGrid.X * (Map.GAP + Map.GRID_SIZE), (_followGrid.Y - 1) * (Map.GAP + Map.GRID_SIZE), 0);
                        StopDrop();
                    }
                }
            }
            else
            {
                if (transform.localPosition.y > _followGrid.transform.localPosition.y + Map.GRID_SIZE + Map.GAP)
                {
                    transform.localPosition -= Vector3.up * Map.DROP_SPEED * deltaTime;
                }
                else
                {
                    if (_followGrid.Dropping)
                    {
                        transform.localPosition -= Vector3.up * Map.DROP_SPEED * deltaTime;
                    }
                    else
                    {
                        transform.localPosition = _followGrid.transform.localPosition + new Vector3(_followGrid.X * (Map.GAP + Map.GRID_SIZE), (_followGrid.Y + 1) * (Map.GAP + Map.GRID_SIZE), 0);
                        StopDrop();
                    }
                }
            }
        }
    }

    public int Y
    {
        get { return (int)transform.localPosition.y / Map.GRID_SIZE; }
    }

    public int X
    {
        get { return (int)transform.localPosition.x / Map.GRID_SIZE; }
    }

    public bool Dropping
    {
        get { return _dropping; }
    }

    public Color Color
    {
        get { return _color; }
        set
        {
            _color = value;
            GetComponent<Image>().color = value;
        }
    }

    public void StartDrop(Grid grid)
    {
        _followGrid = grid;
        _dropping = true;
        TimeMgr.Instance.AddFrameTimeEvent(GetInstanceID().ToString(), TimeEventUpdate);
    }

    public void StopDrop()
    {
       // Debug.Log(string.Format("StopDrop: X={0},Y={1}", X, Y));
        _dropping = false;
        TimeMgr.Instance.RemoveTimeEvent(GetInstanceID().ToString());
        GameController gmCtrl = Singleton.GetInstance("GameController") as GameController;
        gmCtrl.TryRemove(this);
    }

    void OnDestroy()
    {
        TimeMgr.Instance.RemoveTimeEvent(GetInstanceID().ToString());
    }
}
