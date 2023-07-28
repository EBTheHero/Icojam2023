using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CanvasArmee : MonoBehaviour
{
    [SerializeField] private GameObject dicePanelPrefab;
    [SerializeField] private GameObject plusPrefab;
    [SerializeField] private GameObject equalsPrefab;
    [SerializeField] private GameObject resultPanelPrefab;

    private Animation anim;
    private Image dicePanelGroup;
    private Image[] dicePanels;
    private TextMeshProUGUI resultPanel;
    private Armee army;

    public const float ANIMATION_LENGTH = 2.0f;
    public const float RESULT_DISPLAY_LENGTH = 2.0f;

    private void Awake()
    {
        army = GetComponentInParent<Armee>();
        anim = GetComponent<Animation>();
    }

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
            resultPanel = Instantiate(resultPanelPrefab, dicePanelGroup.transform).GetComponent<TextMeshProUGUI>();
        }
    }

    public void Animate(byte score1, byte score2, byte score3, bool victory)
    {
        foreach (Image i in dicePanels)
            i.GetComponent<DiceAnimation>().StartAnimation();
        StartCoroutine(Resolve(score1, score2, score3, victory));
    }

    private IEnumerator Resolve(byte score1, byte score2, byte score3, bool victory)
    {
        yield return new WaitForSeconds(ANIMATION_LENGTH);

        for (byte i = 0; i < dicePanels.Length; ++i)
        {
            dicePanels[i].GetComponent<DiceAnimation>().StopAnimation(i == 0 ? score1 : i == 1 ? score2 : score3);
        }

        if (resultPanel != null)
        {
            resultPanel.color = victory ? Color.green : Color.red;
            resultPanel.text = (score1 + score2 + score3).ToString();
        }

        anim.Play();
        yield return new WaitForSeconds(RESULT_DISPLAY_LENGTH);

        army.ResolveCombat(victory);
        if (resultPanel != null)
        {
            resultPanel.text = "?";
            resultPanel.color = Color.white;
        }
    }
}
