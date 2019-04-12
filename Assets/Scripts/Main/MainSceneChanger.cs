using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneChanger : SceneChanger {
    public enum Level {
        Algdat = 0
    }

    private string[] levels = new string[] {
        "AlgDatHub"
    };

    public void FadeToLevel(Level level) {
        int i = (int)level;
        FadeToLevel(levels[i]);
    }

    public void GoToAlgDat() {
        FadeToLevel(Level.Algdat);
    }
}
