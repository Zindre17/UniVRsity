﻿using UnityEngine;
using Valve.VR;

public class SelectionHandler : MonoBehaviour {

    [SteamVR_DefaultActionSet("selection")]
    public SteamVR_ActionSet actionSet;

    [SteamVR_DefaultAction("select", "selection")]
    public SteamVR_Action_Boolean a_select;

    public Transform laserOrigin;
    public float maxDistance = 20f;

    private Selectable prevHover;

    private void Start()
    {
        if(laserOrigin == null)
        {
            laserOrigin = transform;
        }
        actionSet.ActivateSecondary();
    }

    private void Update () {
        RaycastHit hit;
        Ray ray = new Ray(laserOrigin.position, laserOrigin.forward);
        if(Physics.Raycast(ray, out hit, maxDistance))
        {
            //Debug.Log("Ray hit!");
            Selectable selectable = hit.collider.GetComponent<Selectable>();
            if(selectable != null)
            {
                //Debug.Log("Target has focusable component!");
                if(selectable != prevHover)
                {
                    selectable.StartHover();
                    if(prevHover!= null)
                        prevHover.EndHover();
                    prevHover = selectable;
                }
                if (a_select.GetStateDown(SteamVR_Input_Sources.Any))
                {
                    //Debug.Log("selected");
                    selectable.Toggle();
                }
            }
            else
            {
                if (prevHover != null)
                {
                    prevHover.EndHover();
                    prevHover = null;
                }
            }

        }
        else
        {
            if(prevHover!= null)
            {
                prevHover.EndHover();
                prevHover = null;
            }
        }
	}
}
