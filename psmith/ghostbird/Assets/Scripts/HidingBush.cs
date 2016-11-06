using UnityEngine;
using System.Collections;

public class HidingBush : TileObject
{
    public GameObject _hideIcon = null;
    
    void OnStart()
    {
        _hideIcon.SetActive(false);
    }

    public override void OnMouseEnter()
    {
        _hideIcon.SetActive(true);
    }

    public override void OnMouseExit()
    {
        _hideIcon.SetActive(false);
    }

    public override void OnMouseDown()
    {
        var tile = this.GetComponentInParent<Tile>();
        if (tile != null)
        {
            tile.OnMouseDown();
        }
    }
}
