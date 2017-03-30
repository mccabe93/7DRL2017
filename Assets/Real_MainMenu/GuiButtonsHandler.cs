using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GuiButtonsHandler : MonoBehaviour, IPointerClickHandler,
     IPointerDownHandler,
     IPointerUpHandler,
     IPointerEnterHandler,
     IPointerExitHandler,
     ISelectHandler
{ 

    private Color origColor;
    private Color origEffect;
    private Color purpley = new Color(139f/255f, 31f/255f, 194f/255f, 255f/255f);
    public Text myText;
    private Outline outline;

    void Start()
    {
        outline = myText.GetComponent<Outline>();
        origColor = myText.color;
        origEffect = outline.effectColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        myText.color = purpley;
        outline.effectColor = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        myText.color = origColor;
        outline.effectColor = origEffect;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

    public void OnSelect(BaseEventData eventData)
    {
        throw new NotImplementedException();
    }
}
