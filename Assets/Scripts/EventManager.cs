using Valve.VR;
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

    public delegate void StartHoverAction(Hoverable h);
    public static event StartHoverAction OnStartHover;

    public delegate void EndHoverAction(Hoverable h);
    public static event EndHoverAction OnEndHover;

    public delegate void SelectAction(NewSelectable s);
    public static event SelectAction OnSelect;

    public delegate void UISelectAction(UISelectable s);
    public static event UISelectAction OnUISelect;

    public delegate void MovementStartedAction();
    public static event MovementStartedAction OnMovementStarted;

    public delegate void MovementFinishedAction();
    public static event MovementFinishedAction OnMovementFinished;

    public delegate void ActionRejectedAction();
    public static event ActionRejectedAction OnActionRejected;

    public delegate void ActionAcceptedAction();
    public static event ActionAcceptedAction OnActionAccepted;

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
                NewSelectable s = hit.collider.GetComponentInParent<NewSelectable>();
                if (s != null) {
                    OnSelect(s);
                }
                UISelectable uis = hit.collider.GetComponent<UISelectable>();
                if (uis != null)
                    OnUISelect(uis);
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
}
