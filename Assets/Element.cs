using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour {

    //private int index = 0;
    public int Index {
        get;set;
    }
    //private int size = 0;
    public int Size {
        get;set;
    }

    public new bool Equals(object other) {
        if (other.GetType() == GetType()) {
            return ((Element)other).Index == Index;
        } else return false;
    }
}
