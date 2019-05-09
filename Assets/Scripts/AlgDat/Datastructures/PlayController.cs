using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayController : MonoBehaviour
{

    public UIButton add, remove, usecase;

    public void UpdateStatus(bool animating)
    {
        add.Active = !animating;
        remove.Active = !animating;
        usecase.Active = !animating;
    }
}
