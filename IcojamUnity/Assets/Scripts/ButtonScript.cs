using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Play("glitch_004");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Play("confirmation_003");
    }
}
