using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tools;
using UnityEngine;

public class Legend : MonoBehaviour
{
    public Renderer UnvisitedCube;

    public Renderer VisitedCube;

    public Renderer AddedCube;

    public Renderer NextCube;

    public Renderer PatternCube;

    public Renderer SeedCube;

    // Start is called before the first frame update
    void Start()
    {
        ColorManager colorManager = ColorManager.instance;
        UnvisitedCube.material.color = colorManager.unvisited;

        VisitedCube.material.color = colorManager.visited;

        AddedCube.material.color = colorManager.added;

        NextCube.material.color = colorManager.next;

        PatternCube.material.color = colorManager.pattern;

        SeedCube.material.color = colorManager.seed;
    }

}
