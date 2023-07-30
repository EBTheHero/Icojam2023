using System.Collections.Generic;
using UnityEngine;

public class EnemyArmy : MonoBehaviour
{
	public bool EnDeplacement = false;
	public const float MOVE_SPEED = 1.7f;

	Vector2 destination;
	List<HexCell> path;

	private HexCell currentCell;
	public HexCell CurrentCell { get => currentCell; }
	[SerializeField] private Vector3Int startingCell;

	public static EnemyArmy Instance;

	SpriteRenderer spriteRenderer;
	private void Start()
	{
		Instance = this;
		spriteRenderer = GetComponent<SpriteRenderer>();
		currentCell = HexGrid.Instance.GetCell(startingCell.x, startingCell.y, startingCell.z);
	}


	private void Update()
	{
		spriteRenderer.enabled = currentCell != Main.Instance.EnemyHomeCell;

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



	public void InitierDeplacement(HexCell dest)
	{

		path = AStarPathfinding.FindPath(currentCell, dest, HexCell.Force.Enemy);

		if (path.Count == 0)
			path = AStarPathfinding.FindPath(currentCell, dest, HexCell.Force.OnlyRocks);

		transform.position = currentCell.transform.position;

		currentCell = dest;
		EnDeplacement = true;

		GoNextCell();
	}

	public void GoNextCell()
	{
		path.RemoveAt(0);
		if (path.Count > 0)
			destination = path[0].transform.position;

	}

	public bool IsMoving()
	{
		return EnDeplacement;
	}
}
