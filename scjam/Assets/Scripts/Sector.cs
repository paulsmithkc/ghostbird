using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Sector : MonoBehaviour {

    // Designer
    private Room[] _rooms = new Room[0];
    
    void Start () {
        _rooms = GetComponentsInChildren<Room>();
    }
	
	void Update () {
	}
}
