using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingElement : MonoBehaviour {

    public int Index { get; set; }
    public int Size { get; set; }

    private bool correct = false;
    public ElementRenderer rend;

    public Selectable selectable;
    public Movable movable;

    public bool Correct
    {
        get { return correct; }
        set
        {
            correct = value;
            if (correct)
            {
                rend.SetCorrectColor();
            }
            else
            {
                rend.SetDefaultColor();
            }
        }
    }

    public void Move(Vector3[] waypoints)
    {
        movable.SetPath(waypoints);
    }

    public void Move(Vector3 wayPoint)
    {
        movable.SetPath(wayPoint);
    }

    public void StartHover()
    {

    }

    public void EndHover()
    {

    }
}
