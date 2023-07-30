using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    private void OnMouseOver()
    {
        print("Hey!!!");
        SoundManager.Play("glitch_004");
    }

    private void OnMouseEnter()
    {
        print("Ho!!!");
    }

    private void OnMouseDown()
    {
        SoundManager.Play("confirmation_003");
    }
}
