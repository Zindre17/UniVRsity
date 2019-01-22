using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {

    public static SceneChanger instance;

    private int levelToLoad;

    private readonly string[] levels = new string[1]{"MainHub"};

    public enum Level:int{
        MainHub = 0,
        AlgDat = 1
    }

    public void Awake() {
        if (instance == null)
            instance = this;
        else Destroy(this.gameObject);
    }
	
    public void FadeToLevel(int nextLevel) {
        levelToLoad = nextLevel;
        Valve.VR.SteamVR_LoadLevel.Begin(levels[levelToLoad]);
    }

    public void Update() {
        if (Input.GetMouseButtonDown(0))
            FadeToLevel(0);
    }
}
