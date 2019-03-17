using UnityEngine;

public abstract class SceneChanger : MonoBehaviour {

    public static SceneChanger instance;

    private void Awake() {
        if (instance == null)
            instance = this;
        else Destroy(this.gameObject);
    }
	
    internal void FadeToLevel(string nextLevel) {
        Valve.VR.SteamVR_LoadLevel.Begin(nextLevel);
    }
}
