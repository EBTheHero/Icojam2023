using System.Linq;
using UnityEngine;

public class Armee : MonoBehaviour
{
    public const float DUREE_DEPLACEMENT = 1f;
    public static readonly byte[] DE = new byte[6] { 0, 2, 3, 4, 5, 10 };

    [SerializeField] private byte nbDes = 0;
    [SerializeField] private Vector3Int startingCell;

    public bool EnDeplacement { get; private set; } = false;
    public bool Used { get; set; }

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

    public bool Combattre(byte scoreABattre)
    {
        byte score1 = DE[Random.Range(0, 5)];
        byte score2 = 0;
        byte score3 = 0;
        if (nbDes > 1)
            score2 = DE[Random.Range(0, 5)];
        if (nbDes == 3)
            score3 = DE[Random.Range(0, 5)];
        bool victory = (score1 + score2 + score3) >= scoreABattre;
        canvas.enabled = true;
        canvasArmee.Animate(score1, score2, score3, victory);
        return victory;
    }

    public void InitierDeplacement(HexCell dest)
    {
        currentCell = dest;
        EnDeplacement = true;
        t = 0;
        posDepart = transform.position;
        destination = dest.transform.position;
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
            }
        }
    }

    public void ReadyToAttackCell(HexCell cell)
    {
        if (Used || !Main.Instance.PlayerTurn)
            return;
        TargetCell = cell;
        if (!HexGrid.Instance.AreAdjacent(currentCell, cell))
        {
            var nearbyCell = HexGrid.Instance.GetAlliedAdjacentCell(cell).First();
            InitierDeplacement(nearbyCell);
        }
        else
            Combattre(TargetCell.TileToughness);
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
            spriteRenderer.color = Color.white;
        }
    }

    public void ResolveCombat(bool victory)
    {
        canvas.enabled = false;
        if(victory)
        {
            TargetCell.Owner = HexCell.Force.Player;
            TargetCell.UpdateVisuals();
            InitierDeplacement(TargetCell);
        }
        TargetCell = null;
        Used = true;
    }
}
