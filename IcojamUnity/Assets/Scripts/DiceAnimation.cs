using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] diceSprites;
    private Image image;


    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void StartAnimation()
    {
        InvokeRepeating("Animate", 0.1f, 0.1f);
    }

    public void StopAnimation(byte score)
    {
        CancelInvoke();
        image.sprite = diceSprites[System.Array.IndexOf(Armee.DE, score)];
    }

    private void Animate()
    {
        Sprite s;
        do
        {
            s = diceSprites[Random.Range(0, 6)];
        } while (s == image.sprite);
        image.sprite = s;
    }

}
