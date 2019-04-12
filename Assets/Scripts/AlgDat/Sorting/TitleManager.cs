using UnityEngine;
using TMPro;

public class TitleManager : MonoBehaviour
{
    public TextMeshPro title;

    public void SetTitle(string t) {
        title.text = t;
    }
}
