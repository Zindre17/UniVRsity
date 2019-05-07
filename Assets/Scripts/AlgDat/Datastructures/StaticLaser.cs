using System.Collections;
using UnityEngine;

public class StaticLaser : MonoBehaviour
{
    public GameObject laser;
    private readonly float thickness = 0.01f;

    public void Hide() {
        transform.localScale = Vector3.zero;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void Target(Vector3 target, bool animate) {
        transform.LookAt(target);
        float length = Vector3.Distance(transform.position, target);
        if (animate) {
            StartCoroutine(AnimateLaser(length));
        } else {
            transform.localScale = new Vector3(thickness, thickness, length);
        }
    }

    private IEnumerator AnimateLaser(float length) {
        float duration = 0.3f;
        float elapsed = 0;
        while(elapsed < duration) {
            float percent = elapsed / duration;
            Vector3 scale = new Vector3(thickness, thickness, length * percent);
            transform.localScale = scale;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(thickness, thickness, length);
    }
}
