using TMPro;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public TextMeshPro state;
    public TextMeshPro pseudo1;
    public TextMeshPro pseudo2;

    public GameObject extension;

    public void SetState(string s) {
        state.text = s;
    }

    public void SetCode1(string c) {
        pseudo1.text = c;
    }

    public void SetCode2(string c) {
        if (c == "")
            extension.SetActive(false);
        else
            extension.SetActive(true);
        pseudo2.text = c;
    }

}
