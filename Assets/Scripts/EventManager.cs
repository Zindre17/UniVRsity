using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    //public delegate void ClickAction();
    //public static event ClickAction OnClicked;


    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(Screen.width / 2 - 50, 5, 100, 30), "Click"))
    //    {
    //        if (OnClicked != null)
    //            OnClicked();
    //    }
    //}

    public delegate void SelectionAction();
    public static event SelectionAction OnSelection;

    public void TrySelecting()
    {
        if(OnSelection != null)
        {
            OnSelection();
        }
    }
}
