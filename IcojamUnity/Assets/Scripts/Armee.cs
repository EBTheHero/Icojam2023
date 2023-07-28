using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armee : MonoBehaviour
{
    public const float DUREE_DEPLACEMENT = 1f;
    public static readonly byte[] DE = new byte[6] { 0, 2, 3, 4, 5, 10 };

    [SerializeField] private byte nbDes = 0;
    [SerializeField] private GameObject dicePanel;

    public bool EnDeplacement { get; private set; } = false;
    private float t = 0f;
    private Vector2 posDepart = new Vector2();
    private Vector2 destination = new Vector2();
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
        
    }

    public bool Combattre(byte scoreABattre)
    {
        byte score = 0;
        for (byte i = 0; i < nbDes; ++nbDes)
            score += DE[Random.Range(0, 5)];
        return score >= scoreABattre;
    }

    public void InitierDeplacement(Vector2 dest)
    {
        EnDeplacement = true;
        t = 0;
        posDepart = transform.position;
        destination = dest;
    }

    private void Update()
    {
        if(EnDeplacement)
        {
            t += Time.deltaTime;
            transform.position = Vector2.Lerp(posDepart, destination, t / DUREE_DEPLACEMENT);
            if (t >= DUREE_DEPLACEMENT)
            {
                transform.position = destination;
                EnDeplacement = false;
            }
        }
    }
}