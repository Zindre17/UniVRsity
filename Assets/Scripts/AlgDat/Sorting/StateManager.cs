using TMPro;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public TextMeshPro state;
    public TextMeshPro pseudo1;
    public TextMeshPro pseudo2;
    public TextMeshPro pseudo3;

    public GameObject extension;
    public GameObject extensionTop;



    public void SetState(string s) {
        state.text = s;
    }

    public void SetCode1(string c) {
        pseudo1.text = c;
    }

    public void SetCode2(string c, bool a) {
        if(c == "")
        {
            extension.SetActive(false);
            extensionTop.SetActive(false);
        }
        else
        {
            extension.SetActive(true);
            extensionTop.SetActive(a);
        }
        pseudo3.text = a ? c : "";
        pseudo2.text = a ? "" : c;
    }
}
