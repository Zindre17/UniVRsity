using System.Collections;
using UnityEngine;

public abstract class Selectable : MonoBehaviour
{

    public Renderer rend;
    internal ColorManager cm;
    internal Coroutine hintRoutine;

    internal bool selected = false;
    public bool Selected {
        get { return selected; }
        set {
            if (selected != value) {
                selected = value;
                SetSelected(selected);
            }
        }
    }

    internal bool active = true;
    public bool Active {
        get { return active; }
        set {
            if(active != value) {
                active = value;
                SetActive(active);
            }
        }
    }

    public int Index { get; internal set; }
    public int Parent { get; internal set; }

    internal abstract void SetActive(bool a);
    internal abstract void SetSelected(bool s);
    internal abstract void UpdateColor();

    public void Hint() {
        if (hintRoutine != null) {
            StopCoroutine(hintRoutine);
        }
        hintRoutine = StartCoroutine(HintAnimation());
    }

    private IEnumerator HintAnimation() {
        UpdateColor();
        Color def = rend.material.color;
        Color hint = cm.hint;
        hint = new Color(hint.r, hint.g, hint.b, .5f);
        yield return new WaitForSeconds(.1f);
        rend.material.color = hint;
        yield return new WaitForSeconds(.3f);
        rend.material.color = def;
        yield return new WaitForSeconds(.3f);
        rend.material.color = hint;
        yield return new WaitForSeconds(.3f);
        rend.material.color = def;
        hintRoutine = null;
    }

}
