using Valve.VR;
using UnityEngine;

public class EventHandler : MonoBehaviour
{

    [SteamVR_DefaultActionSet("selection")]
    public SteamVR_ActionSet actionSet;

    [SteamVR_DefaultAction("select", "selection")]
    public SteamVR_Action_Boolean a_select;

    public Transform pointer;
    [Range(10,50)]
    public float maxDistance = 15f;

    private Hoverable lastHover;

    private float lastButtonPress = 0;
    [Range(0,1)]
    public readonly float buttonPressBuffer = .1f;

    public delegate void PixelSelectEvent(Pixel p);
    public static event PixelSelectEvent OnPixelSelected;

    public delegate void SeedChangedEvent(int index);
    public static event SeedChangedEvent OnSeedChanged;

    public delegate void ModeChangedEvent(Stage.Mode mode);
    public static event ModeChangedEvent OnModeChanged;

    private void Start() {
        actionSet.ActivateSecondary();
    }

    void Update()
    {
        Ray ray = new Ray(pointer.position, pointer.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
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

            if(IsSelecting() && Time.time - buttonPressBuffer > lastButtonPress) {
                lastButtonPress = Time.time;
                Pixel p = hit.collider.GetComponentInParent<Pixel>();
                if (p != null) {
                    if(OnPixelSelected!=null) OnPixelSelected(p);
                }
                UIButton b = hit.collider.GetComponentInParent<UIButton>();
                if (b != null) {
                    b.Press();
                }
            }

        }
    }

    private bool IsSelecting() {
        return a_select.GetStateDown(SteamVR_Input_Sources.Any);
    }

    public static void ChangeSeed(int index) {
        if (OnSeedChanged != null) OnSeedChanged(index);
    }

    public static void ChangeMode(Stage.Mode mode) {
        if (OnModeChanged != null) OnModeChanged(mode);
    }

}
