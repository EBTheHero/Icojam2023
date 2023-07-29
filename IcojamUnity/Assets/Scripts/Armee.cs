using System.Linq;
using UnityEngine;

public class Armee : MonoBehaviour
{
    public const float DUREE_DEPLACEMENT = 1f;
    public static readonly byte[] DE = new byte[6] { 0, 2, 3, 4, 5, 10 };

    [SerializeField] private byte nbDes = 0;
    [SerializeField] private Vector3Int startingCell;

    public Animator animator;

    public bool EnDeplacement { get; private set; } = false;
    public bool Fighting { get; private set; } = false;
    private bool used;

    public bool Used
    {
        get { return used; }
        set
        {
            used = value;
            spriteRenderer.color = value ? Color.gray : Color.white;
        }
    }


    private float t = 0f;
    private Vector2 posDepart = new Vector2();
    private Vector2 destination = new Vector2();
    private Canvas canvas;
    private CanvasArmee canvasArmee;
    private HexCell currentCell;

    [System.NonSerialized] public HexCell TargetCell;

    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
        canvasArmee = canvas.GetComponent<CanvasArmee>();
        canvasArmee.Init(nbDes);
    }

    private void Start()
    {
        currentCell = HexGrid.Instance.GetCell(startingCell.x, startingCell.y, startingCell.z);
    }

    public void Combattre(byte scoreABattre)
    {
        if (EnDeplacement || Fighting)
            return;
        Fighting = true;
        Main.Instance.DisableEndTurn();
        byte score1 = DE[Random.Range(0, 6)];
        byte score2 = 0;
        byte score3 = 0;
        if (nbDes > 1)
            score2 = DE[Random.Range(0, 6)];
        if (nbDes == 3)
            score3 = DE[Random.Range(0, 6)];
        bool victory = (score1 + score2 + score3) >= scoreABattre;
        canvas.enabled = true;
        canvasArmee.Animate(score1, score2, score3, victory);
    }

    public void InitierDeplacement(HexCell dest)
    {
        Main.Instance.DisableEndTurn();
        currentCell = dest;
        EnDeplacement = true;
        t = 0;
        posDepart = transform.position;
        destination = dest.transform.position;

        foreach (var item in Main.Instance.Armies)
        {
            if (item != this)
                item.MoveOutOfTheWayOfCell(dest);
        }
    }

    private void Update()
    {
        if (EnDeplacement)
        {
            t += Time.deltaTime;
            transform.position = Vector2.Lerp(posDepart, destination, t / DUREE_DEPLACEMENT);
            if (t >= DUREE_DEPLACEMENT)
            {
                transform.position = destination;
                EnDeplacement = false;
                if (TargetCell != null)
                    Combattre(TargetCell.TileToughness);
                else
                    Main.Instance.EnableEndTurn();
            }
        }
    }

    public void ReadyToAttackCell(HexCell cell)
    {
        if (Used || !Main.Instance.PlayerTurn || EnDeplacement || Fighting)
            return;
        TargetCell = cell;
        animator.SetBool("ifAttack", true);
        if (cell == EnemyAI.Instance.AttackingCell)
        {
            // Counter attack!
            if (currentCell == EnemyAI.Instance.AttackedCell)
                Combattre(cell.TileToughness);
            else
                InitierDeplacement(EnemyAI.Instance.AttackedCell);
        }
        else if (!HexGrid.Instance.AreAdjacent(currentCell, cell))
            InitierDeplacement(HexGrid.Instance.GetAlliedAdjacentCell(cell).First());
        else
            Combattre(cell.TileToughness);
    }

    void OnMouseDown()
    {
        if (Used || !Main.Instance.PlayerTurn)
            return;
        Main.Instance.SelectedArmee = this;
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            spriteRenderer.color = Color.yellow;
        }
        else
        {
            spriteRenderer.color = used ? Color.gray : Color.white;
        }
    }

    public void ResolveCombat(bool victory)
    {
        canvas.enabled = false;
        animator.SetBool("ifAttack", false);
        if (victory)
        {
            if (TargetCell == EnemyAI.Instance.AttackingCell)
            {
                // counter attack success
                EnemyAI.Instance.CounterAttackSuccess();
            }
            else
            {
                TargetCell.Owner = HexCell.Force.Player;
                TargetCell.UpdateVisuals();
                InitierDeplacement(TargetCell);
            }
        }
        TargetCell = null;
        Used = true;
        Fighting = false;
        Main.Instance.EnableEndTurn();
        Main.Instance.SelectedArmee = null;
    }

    public void MoveOutOfTheWayOfCell(HexCell cell)
    {
        if (currentCell == cell)
        {
            // Army is in the way. Moving to another tile.
            var surroundingTiles = HexGrid.Instance.GetAlliedAdjacentCell(currentCell);
            if (surroundingTiles != null && surroundingTiles.Count > 0)
                InitierDeplacement(surroundingTiles[0]);
        }
    }
}
