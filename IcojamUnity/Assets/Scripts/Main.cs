using UnityEngine;
using static HexCell;

public class Main : MonoBehaviour
{
    public static Main Instance;

    public Armee[] Armies;

    private Armee selectedArmee;

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
    public bool PlayerTurn { get; private set; } = true;

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

    // Update is called once per frame
    void Update()
    {

    }

    public void EndTurn()
    {
        PlayerTurn = false;
        EnemyAI.Instance.AttemptAttack();

        foreach (var arme in Armies)
        {
            arme.Used = false;
        }

        EnemyAI.Instance.PickCell();

        PlayerTurn = true;
    }
}
