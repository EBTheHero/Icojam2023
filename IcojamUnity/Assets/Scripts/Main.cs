using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main Instance;

    public Armee[] Armies;

    private Armee selectedArmee;

    public HexCell HomeCell;
    public HexCell EnemyHomeCell;

    public int Difficulty = 11;
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
    void Start()
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
    }
}
