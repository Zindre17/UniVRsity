using Valve.VR;
using UnityEngine;

public class EventManager : MonoBehaviour {

    
    //[SteamVR_DefaultActionSet("selection")]
    public SteamVR_ActionSet actionSet;

    //[SteamVR_DefaultAction("select", "selection")]
    public SteamVR_Action_Boolean a_select;

    [Range(10f, 50f)]
    public float maxDistance = 15f;

    public Transform laser;

    private Hoverable lastHover;
    private float lastButtonPress = 0;
    private readonly float buttonPressBuffer = .1f;

    public delegate void SelectEvent(SortingElement s);
    public static event SelectEvent OnSelect;

    public delegate void ActionCompletedEvent(bool reverse);
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

    public delegate void PixelSelectEvent(Pixel p);
    public static event PixelSelectEvent OnPixelSelected;

    public delegate void SeedChangedEvent(int index);
    public static event SeedChangedEvent OnSeedChanged;

    public delegate void ModeChangedEvent(Stage.Mode mode);
    public static event ModeChangedEvent OnModeChanged;

    public delegate void PartialActionEvent();
    public static event PartialActionEvent OnPartialActionComplete;

    private void Start() {
        if (laser == null)
            laser = transform;
        actionSet.Activate();
    }

    private void Update() {
        RaycastHit hit;
        Ray ray = new Ray(laser.position, laser.forward);
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
                Pixel p = hit.collider.GetComponentInParent<Pixel>();
                if (p != null) {
                    OnPixelSelected?.Invoke(p);
                    return;
                }
                SelectableReference slr = hit.collider.GetComponent<SelectableReference>();
                if(slr != null) {
                    Selectable sl = slr.s;
                    if (OnSelection != null && sl.Active) OnSelection(sl);
                    return;
                }
                UIButton b = hit.collider.GetComponentInParent<UIButton>();
                if (b != null) {
                    b.Press();
                    return;
                }
               
                
            }

        } 
    }

    public static void ChangeSeed(int index) {
        if (OnSeedChanged != null) OnSeedChanged(index);
    }

    public static void ChangeMode(Stage.Mode mode) {
        if (OnModeChanged != null) OnModeChanged(mode);
    }

    public static void MergeComplete() {
        if (OnMergeComplete != null) OnMergeComplete();
    }

    private bool IsSelecting() {
        return a_select.GetStateDown(SteamVR_Input_Sources.Any);
    }

    public static void ActionCompleted(bool reverse = false) {
        if (OnActionCompleted != null) OnActionCompleted(reverse);
    }

    public static void AlgorithmCompleted() {
        if (OnAlgorithmCompleted != null) OnAlgorithmCompleted();
    }

    public static void FocusChanged(int array, int start, int end) {
        if(start <= end && OnArrayInFocusChanged != null) {
            OnArrayInFocusChanged(array, start, end);
        }
    }

    public static void PartialActionComplete()
    {
        if (OnPartialActionComplete != null) OnPartialActionComplete();
    }
}
