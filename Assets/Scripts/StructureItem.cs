using UnityEngine;

public class StructureItem : MonoBehaviour {
    private int value = -1;
    public int Value {
        get { return this.value; }
        set { if (this.value == -1) this.value = value; }
    }
}
