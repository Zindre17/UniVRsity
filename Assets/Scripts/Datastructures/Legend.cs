using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        ColorManager m = ColorManager.instance;
        UnvisitedCube.material.color = m.unvisitedColor;

        VisitedCube.material.color = m.visitedColor;

        AddedCube.material.color = m.addedColor;

        NextCube.material.color = m.nextColor;

        PatternCube.material.color = m.patternColor;

        SeedCube.material.color = m.seedColor;
    }

}
