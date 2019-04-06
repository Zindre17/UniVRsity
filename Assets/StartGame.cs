using System.Collections;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartG());    
    }
    
    private IEnumerator StartG() {
        yield return new WaitForEndOfFrame();
        Valve.VR.SteamVR_LoadLevel.Begin("Start");
    }
}
