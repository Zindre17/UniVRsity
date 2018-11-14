using UnityEngine;

[RequireComponent(typeof(NewSelectable))]
[RequireComponent(typeof(Movable))]
[RequireComponent(typeof(Hoverable))]
public class SortingElement : MonoBehaviour {

    public int Index { get; set; }

    private int size;
    public int Size {
        get { return size; }
        set {
            size = value;
            transform.localScale = new Vector3(1, size, 1);
        }
    }

    private bool correct = false;
    public bool Correct {
        get { return correct; }
        set {
            if(correct != value) {
                correct = value;
                selectable.Correct = value;
            }
        }
    }

    private bool compared = false;
    public bool Compared {
        get { return compared; }
        set {
            if(compared != value) {
                compared = value;
                if (compared)
                    MoveToComparedPos();
                else
                    MoveFromComparedPos();
            }
        }
    }

    private bool selected = false;
    public bool Selected {
        get { return selected; }
        set {
            if (selected != value) {
                selected = value;
                selectable.Selected = value;
                if (Compared) {
                    Compared = false;
                }
            }
        }
    }

    public float movementMagnitude = 0.4f;

    public Vector3 ArrayPos {
        get;
        set;
    }

    private NewSelectable selectable;
    private Movable movable;
    private Hoverable hoverable;

    private void Awake() {
        if (selectable == null)
            selectable = GetComponent<NewSelectable>();
        if (movable == null)
            movable = GetComponent<Movable>();
        if (hoverable == null)
            hoverable = GetComponent<Hoverable>();
    }

    private void MoveToComparedPos() {
        movable.AddWayPoint(transform.position - Vector3.forward * movementMagnitude);
    }

    private void MoveFromComparedPos() {
        movable.AddWayPoint(transform.position + Vector3.forward * movementMagnitude);
    }

    public void Swap(SortingElement other) {
        // swap default position
        Vector3 temp = ArrayPos;
        ArrayPos = other.ArrayPos;
        other.ArrayPos = temp;

        // swap index
        int _temp = Index;
        Index = other.Index;
        other.Index = _temp;

        // move to eachohters position
        Vector3 s1, s2;
        s1 = transform.position;
        s2 = other.transform.position;
        if (compared && other.Compared) {
            movable.AddWayPoint(s2);
            other.movable.AddWayPoint(s2 - Vector3.forward * movementMagnitude);
            other.movable.AddWayPoint(s1 - Vector3.forward * movementMagnitude);
            other.movable.AddWayPoint(s1);
           
        } else if (compared || other.compared) {
            movable.AddWayPoint(s1 - Vector3.forward * movementMagnitude);
            movable.AddWayPoint(new Vector3(s2.x, s2.y, s1.z - movementMagnitude));
            movable.AddWayPoint(new Vector3(s2.x, s2.y, s1.z));

            other.movable.AddWayPoint(s2 - Vector3.forward * movementMagnitude);
            other.movable.AddWayPoint(new Vector3(s1.x, s1.y, s2.z - movementMagnitude));
            other.movable.AddWayPoint(new Vector3(s1.x, s1.y, s2.z));
           
        } else {
            movable.AddWayPoint(s1 - Vector3.forward * movementMagnitude * 2);
            movable.AddWayPoint(s2 - Vector3.forward * movementMagnitude * 2);
            movable.AddWayPoint(s2);

            other.movable.AddWayPoint(s2 - Vector3.forward * movementMagnitude);
            other.movable.AddWayPoint(s1 - Vector3.forward * movementMagnitude);
            other.movable.AddWayPoint(s1);
           
        }
    }

    public void Store(Vector3 storepos) {
        Index = -1;
        movable.AddWayPoint(new Vector3(transform.position.x, transform.position.y, storepos.z));
        movable.AddWayPoint(storepos);
    }
}
