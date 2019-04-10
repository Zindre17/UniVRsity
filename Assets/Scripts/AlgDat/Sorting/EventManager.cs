using Valve.VR;
using UnityEngine;

public class EventManager : MonoBehaviour {

    
    //[SteamVR_DefaultActionSet("selection")]
    public SteamVR_ActionSet actionSet;

    //[SteamVR_DefaultAction("select", "selection")]
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

    public delegate void ArraySelectEvent(ArrayManager a);
    public static event ArraySelectEvent OnArraySelect;

    public delegate void PArraySelectEvent(PartialArray p);
    public static event PArraySelectEvent OnPArraySelect;

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

    public delegate void ArrayInFocusChanged(int array,int start, int end);
    public static event ArrayInFocusChanged OnArrayInFocusChanged;

    public delegate void EmptySelectEvent(EmptyElement e);
    public static event EmptySelectEvent OnEmptySelect;

    public delegate void MergeCompleteEvent();
    public static event MergeCompleteEvent OnMergeComplete;

    public delegate void SelectionEvent(Selectable s);
    public static event SelectionEvent OnSelection;

    private void Start() {
        if (laserOrigin == null)
            laserOrigin = transform;
        actionSet.Activate();
    }

    private void Update() {
        RaycastHit hit;
        Ray ray = new Ray(laserOrigin.position, laserOrigin.forward);
        if (Physics.Raycast(ray, out hit, maxDistance)) {
            //check for hoverable component
            Hoverable h = hit.collider.GetComponentInParent<Hoverable>();
            if (h != null) {
                if (!h.Equals(lastHover)) {
                    if (lastHover != null) lastHover.EndHover();
                    h.StartHover();
                    lastHover = h;
                }
            } else {
                if (lastHover != null) lastHover.EndHover();
                lastHover = null;
            }

            if (IsSelecting() && Time.time - buttonPressBuffer > lastButtonPress) {
                lastButtonPress = Time.time;
                Selectable sl = hit.collider.GetComponentInParent<Selectable>();
                if(sl != null) {
                    if (OnSelection != null) OnSelection(sl);
                    return;
                }
                UIButton b = hit.collider.GetComponentInParent<UIButton>();
                if (b != null) {
                    b.Press();
                    return;
                }
                SortingElement s = hit.collider.GetComponentInParent<SortingElement>();
                if (s != null && s.InFocus && s.Active) {
                    if(OnSelect != null) OnSelect(s);
                    return;
                }
                EmptyElement e = hit.collider.GetComponentInParent<EmptyElement>();
                if (e != null) {
                    if (OnEmptySelect != null) OnEmptySelect(e);
                    return;
                }
                ArrayManager a = hit.collider.GetComponentInParent<ArrayManager>();
                if (a != null) {
                    if (OnArraySelect != null && a.Active) OnArraySelect(a);
                    return;
                }
                PartialArray p = hit.collider.GetComponentInParent<PartialArray>();
                if (p != null) {
                    if (OnPArraySelect != null && p.Active) OnPArraySelect(p);
                    return;
                }
                /*
                UISelectable uis = hit.collider.GetComponent<UISelectable>();
                if (uis != null)
                    OnUISelect(uis);
                MenuSelectable ms = hit.collider.GetComponent<MenuSelectable>();
                if (ms != null)
                {
                    OnMenuSelect(ms);
                }
                AlgorithmSelectable als = hit.collider.GetComponent<AlgorithmSelectable>();
                if (als != null) {
                    als.Press();
                }
                */
            }

        } 
    }

    public void MergeComplete() {
        if (OnMergeComplete != null) OnMergeComplete();
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

    public static void FocusChanged(int array, int start, int end) {
        if(start < end && OnArrayInFocusChanged != null) {
            OnArrayInFocusChanged(array, start, end);
        }
    }
}
