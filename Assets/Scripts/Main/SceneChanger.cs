using UnityEngine;
using Valve.VR;

public abstract class SceneChanger : MonoBehaviour {

    public static SceneChanger instance;

    private void Awake() {
        if (instance == null)
            instance = this;
        else Destroy(this.gameObject);
    }
	
    internal void FadeToLevel(string nextLevel) {
        SteamVR_LoadLevel.Begin(nextLevel, fadeOutTime: 3f);
    }
}
