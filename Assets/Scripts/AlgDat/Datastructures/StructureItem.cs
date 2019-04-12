using UnityEngine;

public class StructureItem : MonoBehaviour {
    private int value = -1;
    public int Value {
        get { return this.value; }
        set { if (this.value == -1) this.value = value; }
    }

    public Transform disk;
    private float width;
    public float Width {
        get { return width; }
        set {
            if (value != width) {
                width = value;
                Vector3 old = disk.localScale;
                disk.localScale = new Vector3(old.x, width, old.z);
            }
        }
    }

    private void Awake() {
        width = disk.localScale.y;
    }
}
