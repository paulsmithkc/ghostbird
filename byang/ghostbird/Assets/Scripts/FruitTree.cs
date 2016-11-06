using UnityEngine;
using System.Collections;

public class FruitTree : TileObject
{
    public GameObject _eatIcon = null;
    public GameObject[] _fruits = new GameObject[0];
    private int _fruitsEaten = 0;

    void OnStart()
    {
        _eatIcon.SetActive(false);
        _fruitsEaten = 0;
    }

    public override void OnMouseEnter()
    {
        if (_fruitsEaten < _fruits.Length)
        {
            _eatIcon.SetActive(true);
        }
    }

    public override void OnMouseExit()
    {
        _eatIcon.SetActive(false);
    }

    public override void OnMouseDown()
    {
        var tile = this.GetComponentInParent<Tile>();
        if (tile != null)
        {
            tile.OnMouseDown();
        }
    }

    public bool EatFruit()
    {
        if (_fruitsEaten < _fruits.Length)
        {
            _fruits[_fruitsEaten].SetActive(false);
            ++_fruitsEaten;
            if (_fruitsEaten >= _fruits.Length)
            {
                _eatIcon.SetActive(false);
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
