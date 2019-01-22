using UnityEngine;

[RequireComponent(typeof(Selectable))]
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
            }
        }
    }

    public float movementMagnitude = 0.4f;

    public Vector3 ArrayPos {
        get;
        set;
    }

    private Selectable selectable;
    private Movable movable;
    private Hoverable hoverable;

    private int count = 0;
    private SortingElement second = null;

    private void Awake() {
        if (selectable == null)
            selectable = GetComponent<Selectable>();
        if (movable == null)
            movable = GetComponent<Movable>();
        if (hoverable == null)
            hoverable = GetComponent<Hoverable>();
    }

    private void MoveToComparedPos() {
        movable.OnComplete += SingleComplete;
        movable.AddWayPoint(transform.position - Vector3.forward * movementMagnitude);
    }

    private void MoveFromComparedPos() {
        movable.AddWayPoint(transform.position + Vector3.forward * movementMagnitude);
    }

    public void Swap(SortingElement other) {

        second = other;
        second.movable.OnComplete += DoubleComplete;
        movable.OnComplete += DoubleComplete;

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
        movable.OnComplete += SingleComplete;
        Index = -1;
        ArrayPos = storepos;
        Correct = false;
        movable.speed = 2f;
        movable.AddWayPoint(new Vector3(transform.position.x, transform.position.y, storepos.z));
        movable.AddWayPoint(storepos);
    }

    public void CopyTo(SortingElement target) {
        movable.OnComplete += SingleComplete;
        Vector3 pos = transform.position;
        Vector3 tar = target.transform.position;
        if(Index == -1) {
            movable.AddWayPoint(new Vector3(tar.x, pos.y, pos.z));
            if (target.compared)
                movable.AddWayPoint(tar);
            else
                movable.AddWayPoint(tar - Vector3.forward * movementMagnitude);
        } else {
            if (compared) {
                if (target.compared)
                    movable.AddWayPoint(tar);
                else
                    movable.AddWayPoint(tar - Vector3.forward * movementMagnitude);
            } else {
                movable.AddWayPoint(transform.position - Vector3.forward * movementMagnitude);
                if (target.compared) {
                    movable.AddWayPoint(tar);
                    movable.AddWayPoint(tar + Vector3.forward * movementMagnitude);
                } else {
                    movable.AddWayPoint(tar - Vector3.forward * movementMagnitude);
                    movable.AddWayPoint(tar);
                }
            }
        }
        Index = target.Index;
        ArrayPos = target.ArrayPos;
        Destroy(target.gameObject);
    }

    private void SingleComplete() {
        movable.OnComplete -= SingleComplete;
        movable.speed = 1f;
        EventManager.ActionCompleted();
    }

    private void DoubleComplete() {
        count++;
        if(count == 2) {
            count = 0;
            movable.OnComplete -= DoubleComplete;
            second.movable.OnComplete -= DoubleComplete;
            EventManager.ActionCompleted();
        }
    }
}
