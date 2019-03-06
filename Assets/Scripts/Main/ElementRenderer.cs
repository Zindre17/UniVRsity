using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementRenderer : MonoBehaviour {

    public Renderer rend;

    public Color defaultColor;
    public Color correctColor;
    public Color hoverInteractableColor;
    public Color hoverNotInteractableColor;
    [Range(0f, 10f)]
    public float hoverOutlineWidth = 0f;
    private Outline outline;

    private void Awake()
    {
        outline = gameObject.AddComponent<Outline>();
    }

    private void Start()
    {
        outline.OutlineWidth = 0f;
    }

    public void SetDefaultColor()
    {
        rend.material.color = defaultColor;
    }

    public void SetCorrectColor()
    {
        rend.material.color = correctColor;
    }

    public void StartHover(bool interactable)
    {
        outline.OutlineWidth = hoverOutlineWidth;
        if (interactable)
        {
            outline.OutlineColor = hoverInteractableColor;
        }
        else
        {
            outline.OutlineColor = hoverNotInteractableColor;
        }
    }
        
    public void EndHover()
    {
        outline.OutlineWidth = 0f;
    }

}
