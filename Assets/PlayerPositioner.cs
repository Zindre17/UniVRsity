using UnityEngine;
using Valve.VR.InteractionSystem;

public class PlayerPositioner : MonoBehaviour
{
    private bool done = false;
    private void Update()
    {
        if (!done) {
            Player p = FindObjectOfType<Player>();
            if (p != null) {
                p.transform.position = transform.position;
                done = true;
                enabled = false;
            }
        }
    }
}
