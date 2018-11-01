using UnityEngine;
using Valve.VR;

public class SelectionHandler : MonoBehaviour {

    [SteamVR_DefaultActionSet("selection")]
    public SteamVR_ActionSet actionSet;

    [SteamVR_DefaultAction("select", "selection")]
    public SteamVR_Action_Boolean a_select;

    public Transform laserOrigin;
    public float maxDistance = 20f;

    private SortingElement prevHover;

    private void Start()
    {
        if(laserOrigin == null)
        {
            laserOrigin = transform;
        }
        actionSet.ActivateSecondary();
    }

    public delegate void SelectableClickedAction(SortingElement element);
    public static event SelectableClickedAction OnSelectableClicked;

    private bool CheckForSortingElement(RaycastHit hit)
    {
        SortingElement element = hit.collider.GetComponent<SortingElement>();
        if (element != null)
        {

            if (element != prevHover)
            {
                element.StartHover();
                if (prevHover != null)
                    prevHover.EndHover();
                prevHover = element;
            }
            if (a_select.GetStateDown(SteamVR_Input_Sources.Any))
            {
                //selectable.Toggle();
                if (OnSelectableClicked != null)
                    OnSelectableClicked(element);
            }
            return true;
        }
        return false;
    }
    
    private void EndHover()
    {
        if (prevHover != null)
        {
            prevHover.EndHover();
            prevHover = null;
        }
    }

    private void Update () {
        RaycastHit hit;
        Ray ray = new Ray(laserOrigin.position, laserOrigin.forward);
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (CheckForSortingElement(hit))
                return;
            else
                EndHover();
        }
        else
            EndHover();
	}
}
