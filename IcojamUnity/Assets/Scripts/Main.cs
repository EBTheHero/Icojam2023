using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HexCell;

public class Main : MonoBehaviour
{
    public static Main Instance;

    public List<Armee> Armies = new List<Armee>();

    private Armee selectedArmee;
    [SerializeField] private Button endTurnButton;

    public HexCell HomeCell;
    public HexCell EnemyHomeCell;

    public HexCell GetHome(Force force)
    {
        if (force == Force.Enemy)
            return EnemyHomeCell;
        else
            return HomeCell;
    }

    public int Difficulty = 11;
    public float TileDistanceMultiplicatior = 0.5f;
    public int NumberOfRocks = 1;
    public int RockSize = 1;
    public bool PlayerTurn { get; private set; } = true;

    public GameObject WinUI;
    public GameObject LoseUI;

    public Armee SelectedArmee
    {
        get { return selectedArmee; }
        set
        {
            if (selectedArmee != null)
                selectedArmee.SetSelected(false);

            selectedArmee = value;
            if (selectedArmee != null)
                selectedArmee.SetSelected(true);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void EndTurn()
    {
        PlayerTurn = false;
        SelectedArmee = null;

        StartCoroutine(PlayerAttack());
        PlayerTurn = true;
    }

    public void ToggleCoords()
    {
        foreach (var cell in HexGrid.Instance.cellsList)
        {
            cell.ShowCoords = !cell.ShowCoords;
        }
    }

    public void DisableEndTurn()
    {
        endTurnButton.interactable = false;
    }

    public void VerifyEndTurnEnabled()
    {
        foreach (Armee a in Armies)
        {
            if (a.EnDeplacement)
                return;
        }
        endTurnButton.interactable = true;
    }

    private IEnumerator PlayerAttack()
    {
        foreach (var arme in Armies)
        {
            if (arme.TargetCell != null)
            {
                arme.AttackCell();
                yield return new WaitWhile(arme.IsFighting);
            }
        }
        EnemyAI.Instance.AttemptAttack();
        EnemyAI.Instance.PickCell();
        foreach (var arme in Armies)
        {
            if (arme.CurrentCell.Owner == Force.Enemy)
                arme.InitierDeplacement(HomeCell);
        }
    }

    public void Win()
    {
        WinUI.SetActive(true);
    }

    public void Lose()
    {
        LoseUI.SetActive(true);
    }
}
