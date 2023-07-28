using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CanvasArmee : MonoBehaviour
{
    [SerializeField] private GameObject dicePanelPrefab;
    [SerializeField] private GameObject plusPrefab;
    [SerializeField] private GameObject equalsPrefab;
    [SerializeField] private GameObject resultPanelPrefab;

    private Image dicePanelGroup;
    private Image[] dicePanels;
    private Image resultPanel;

    public void Init(byte nbDes)
    { 
        dicePanelGroup = GetComponentInChildren<Image>();
        dicePanels = new Image[nbDes];
        for (byte i = 0; i < nbDes; ++i)
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

    public void Animate(byte score1, byte score2, byte score3)
    {

    }
}
