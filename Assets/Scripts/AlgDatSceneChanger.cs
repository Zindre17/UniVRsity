using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgDatSceneChanger : SceneChanger
{
    public enum Level {
        MainMenu = 0,
        Sorting = 1,
        Datastructures = 2
    }

    private readonly string[] levels = new string[] {
        "MainHub",
        "SortingAlgorithms",
        "Datastructures"
    };

    public void FadeToLevel(Level level) {
        int i = (int)level;
        FadeToLevel(levels[i]);
    }

    public void GoToMainMenu() {
        FadeToLevel(Level.MainMenu);
    }

    public void GoToSorting() {
        FadeToLevel(Level.Sorting);
    }

    public void GoToDataStructures() {
        FadeToLevel(Level.Datastructures);
    }

    public void GoToCourseMain() {
        FadeToLevel("AlgDatHub");
    }
}
