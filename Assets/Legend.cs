using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Legend : MonoBehaviour
{
    public Renderer cube1;
    public TextMeshProUGUI text1;

    public Renderer cube2;
    public TextMeshProUGUI text2;

    public Renderer cube3;
    public TextMeshProUGUI text3;

    public Renderer cube4;
    public TextMeshProUGUI text4;

    public Renderer cube5;
    public TextMeshProUGUI text5;

    // Start is called before the first frame update
    void Start()
    {
        ColorManager m = ColorManager.instance;
        cube1.material.color = m.unvisitedColor;
        text1.text = "unvisited";

        cube2.material.color = m.visitedColor;
        text2.text = "visited";

        cube3.material.color = m.nextColor;
        text3.text = "next";

        cube4.material.color = m.patternColor;
        text4.text = "part of pattern";

        cube5.material.color = m.seedColor;
        text5.text = "seed point";

    }

}
