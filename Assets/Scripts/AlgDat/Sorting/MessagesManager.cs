using UnityEngine;
using TMPro;
using System.Collections;

public class MessagesManager : MonoBehaviour
{
    public TextMeshPro message;
    private float duration = 4f;

    private IEnumerator current;

    public void SetMessage(string s) {
        if (current != null)
            StopCoroutine(current);
        current = ShowMessage(s);
        StartCoroutine(current);
    }

    public void Intro() {
        if( current != null) {
            StopCoroutine(current);
        }
        message.text =
            "Select an algorithm to get started!";
    }

    private IEnumerator ShowMessage(string s) {
        message.text = s;
        yield return new WaitForSeconds(duration);
        message.text = "";
        current = null;
    }
}
