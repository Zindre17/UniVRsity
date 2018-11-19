using Valve.VR;
using UnityEngine;

public class EventManager : MonoBehaviour {

    
    [SteamVR_DefaultActionSet("selection")]
    public SteamVR_ActionSet actionSet;

    [SteamVR_DefaultAction("select", "selection")]
    public SteamVR_Action_Boolean a_select;

    [Range(10f, 100f)]
    public float maxDistance = 20f;

    public Transform laserOrigin;

    private Hoverable lastHover;
    private float lastButtonPress = 0;
    private float buttonPressBuffer = .1f;

    public delegate void StartHoverEvent(Hoverable h);
    public static event StartHoverEvent OnStartHover;

    public delegate void EndHoverEvent(Hoverable h);
    public static event EndHoverEvent OnEndHover;

    public delegate void SelectEvent(SortingElement s);
    public static event SelectEvent OnSelect;

    public delegate void UISelectEvent(UISelectable s);
    public static event UISelectEvent OnUISelect;

    public delegate void MenuSelectEvent(MenuSelectable s);
    public static event MenuSelectEvent OnMenuSelect;

    public delegate void MovementStartedEvent();
    public static event MovementStartedEvent OnMovementStarted;

    public delegate void MovementFinishedEvent();
    public static event MovementFinishedEvent OnMovementFinished;

    public delegate void ActionRejectedEvent();
    public static event ActionRejectedEvent OnActionRejected;

    public delegate void ActionAcceptedEvent();
    public static event ActionAcceptedEvent OnActionAccepted;

    public delegate void ActionCompletedEvent();
    public static event ActionCompletedEvent OnActionCompleted;

    public delegate void AlgorithmCompletedEvent();
    public static event AlgorithmCompletedEvent OnAlgorithmCompleted;

    private void Start() {
        if (laserOrigin == null)
            laserOrigin = transform;
        actionSet.ActivateSecondary();
    }

    private void Update() {
        RaycastHit hit;
        Ray ray = new Ray(laserOrigin.position, laserOrigin.forward);
        if (Physics.Raycast(ray, out hit, maxDistance)) {
            //check for hoverable component
            Hoverable h = hit.collider.GetComponentInParent<Hoverable>();
            if (h != null) {
                if (!h.Equals(lastHover)) {
                    EndHover();
                    OnStartHover(h);
                    lastHover = h;
                }
            } else 
                EndHover();

            if (IsSelecting() && Time.time - buttonPressBuffer > lastButtonPress) {
                lastButtonPress = Time.time;
                SortingElement s = hit.collider.GetComponentInParent<SortingElement>();
                if (s != null) {
                    OnSelect(s);
                }
                UISelectable uis = hit.collider.GetComponent<UISelectable>();
                if (uis != null)
                    OnUISelect(uis);
                MenuSelectable ms = hit.collider.GetComponent<MenuSelectable>();
                if (ms != null)
                {
                    OnMenuSelect(ms);
                }
            }

        } else
            EndHover();
    }

    private bool IsSelecting() {
        return a_select.GetStateDown(SteamVR_Input_Sources.Any);
    }

    private void EndHover() {
        if (lastHover != null) {
            OnEndHover(lastHover);
            lastHover = null;
        }
    }

    public static void StartedMovement() {
        if (OnMovementStarted != null)
            OnMovementStarted();
    }

    public static void FinishedMovement() {
        if (OnMovementFinished != null)
            OnMovementFinished();
    }

    public static void RejectAction() {
        if (OnActionRejected != null)
            OnActionRejected();
    }

    public static void AcceptAction() {
        if (OnActionAccepted != null)
            OnActionAccepted();
    }

    public static void ActionCompleted() {
        if (OnActionCompleted != null) OnActionCompleted();
    }

    public static void AlgorithmCompleted() {
        if (OnAlgorithmCompleted != null) OnAlgorithmCompleted();
    }
}
