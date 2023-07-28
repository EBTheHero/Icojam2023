using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Armee : MonoBehaviour
{
    public const float DUREE_DEPLACEMENT = 1f;
    public static readonly byte[] DE = new byte[6] { 0, 2, 3, 4, 5, 10 };

    [SerializeField] private byte nbDes = 0;
    [SerializeField] private GameObject dicePanelPrefab;
    [SerializeField] private GameObject plusPrefab;
    [SerializeField] private GameObject equalsPrefab;
    [SerializeField] private GameObject resultPanelPrefab;

    public bool EnDeplacement { get; private set; } = false;
    private float t = 0f;
    private Vector2 posDepart = new Vector2();
    private Vector2 destination = new Vector2();
    private Canvas canvas;
    private Image dicePanelGroup;
    private Image[] dicePanels;
    private Image resultPanel;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        dicePanelGroup = canvas.GetComponentInChildren<Image>();
        canvas.worldCamera = Camera.main;
        dicePanels = new Image[nbDes];
        for(byte i = 0; i < nbDes; ++i)
        {
            if (i > 0)
                Instantiate(plusPrefab, dicePanelGroup.transform);
            dicePanels[i] = Instantiate(dicePanelPrefab, dicePanelGroup.transform).GetComponent<Image>();
        }
        if (nbDes > 1)
        {
            Instantiate(equalsPrefab, dicePanelGroup.transform);
            resultPanel = Instantiate(resultPanelPrefab, dicePanelGroup.transform).GetComponent<Image>();
        }
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
