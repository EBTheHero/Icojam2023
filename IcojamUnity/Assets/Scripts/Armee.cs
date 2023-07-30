using System.Collections.Generic;
using UnityEngine;

public class Armee : MonoBehaviour
{
    public const float DUREE_DEPLACEMENT = 1f;
    public const float MOVE_SPEED = 4f;
    public static readonly byte[] DE = new byte[6] { 0, 2, 3, 4, 5, 10 };

    [SerializeField] private byte nbDes = 0;
    [SerializeField] private Vector3Int startingCell;

    public Animator animator;
    public Arrow AttackArrow;
    public bool EnDeplacement { get; private set; } = false;
    public bool Fighting { get; private set; } = false;


    private float t = 0f;
    private Vector2 posDepart = new Vector2();
    private Vector2 destination = new Vector2();
    private Canvas canvas;
    private CanvasArmee canvasArmee;
    public Animation OccupiedAnimation;
    private HexCell currentCell;
    private HexCell targetCell;

    List<HexCell> path;

    public HexCell TargetCell
    {
        get => targetCell; set
        {
            targetCell = value;
            if (targetCell != null)
            {
                AttackArrow.UpdateArrow(transform, targetCell.transform);
                animator.SetBool("ifAttack", true);
            }
            else
            {
                AttackArrow.HideArrow();
                animator.SetBool("ifAttack", false);
            }
            SetSelected(Main.Instance.SelectedArmee == this);
        }
    }

    public HexCell CurrentCell { get => currentCell; }

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
        canvasArmee.Animate(score1, score2, score3, victory, TargetCell);
    }

    public void InitierDeplacement(HexCell dest)
    {
        path = AStarPathfinding.FindPath(currentCell, dest, HexCell.Force.Player);

        if (path.Count == 0)
            path = AStarPathfinding.FindPath(currentCell, dest, HexCell.Force.OnlyRocks);

        currentCell = dest;
        EnDeplacement = true;

        GoNextCell();

        foreach (var item in Main.Instance.Armies)
        {
            if (item != this)
                item.MoveOutOfTheWayOfCell(dest);
        }
    }

    public void GoNextCell()
    {
        path.RemoveAt(0);
        if (path.Count > 0)
            destination = path[0].transform.position;

    }

    private void Update()
    {
        if (EnDeplacement)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, MOVE_SPEED * Time.deltaTime);
            if (transform.position == (Vector3)destination)
            {
                if (path.Count > 0)
                {
                    GoNextCell();
                }
                else
                {
                    transform.position = destination;
                    EnDeplacement = false;
                }
            }
        }
    }

    public void PrepareToAttackCell(HexCell cell)
    {
        if (!HexGrid.Instance.AreAdjacent(currentCell, cell))
        {
            var cells = HexGrid.Instance.GetAlliedAdjacentCell(cell);
            var occupyingArmies = new List<Armee>();
            for (byte i = 0; i < cells.Count; ++i)
            {
                Armee occupying = cells[i].IsOccupied();
                if (occupying == null)
                {
                    TargetCell = cell;
                    InitierDeplacement(cells[i]);
                    return;
                }
                else
                {
                    occupyingArmies.Add(occupying);
                }
            }
            // All tiles occupied
            foreach (var army in occupyingArmies)
                army.PlayOccupied();
            // TODO: G�rer l'impossibilit� de se positionner sur la cellule.
        }
        else
            TargetCell = cell;
    }

    public void AttackCell()
    {
        if (targetCell.Owner != HexCell.Force.Enemy)
        {
            TargetCell = null;
            return;
        }
        Fighting = true;
        animator.SetBool("ifAttack", true);
        Combattre(TargetCell.TileToughness);
    }

    void OnMouseDown()
    {
        if (Main.Instance.PlayerTurn)
            Main.Instance.SelectedArmee = this;
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            spriteRenderer.color = Color.yellow;
        }
        else
            spriteRenderer.color = TargetCell == null ? Color.white : Color.green;
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
                SoundManager.Play("475246__aurea__military-snaredrum");
                InitierDeplacement(TargetCell);
            }
        }

        TargetCell = null;
        Fighting = false;

    }

    public bool IsFighting()
    {
        return Fighting;
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

    public void PlayOccupied()
    {
        OccupiedAnimation.Play();
    }
}
