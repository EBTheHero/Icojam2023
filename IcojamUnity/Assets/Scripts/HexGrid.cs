using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexGrid : MonoBehaviour
{

	public int width = 7;
	public int height = 7;

	public HexCell cellPrefab;

	HexCell[,,] cells;
	public List<HexCell> cellsList = new List<HexCell>();


	public static HexGrid Instance;

	void Awake()
	{
		if (Instance == null)
			Instance = this;

		GeneratePlayField();

	}

	public void GeneratePlayField()
	{
		if (cellsList != null && cellsList.Count > 0)
			foreach (var cell in cellsList)
				Destroy(cell.gameObject);

		cellsList.Clear();

		cells = new HexCell[20, 20, 20];

		for (int z = 0, i = 0; z < height; z++)
		{
			for (int x = 0; x < width; x++)
			{
				CreateCell(x, z, i++);
			}
		}

		foreach (var item in cells)
			if (item != null)
				cellsList.Add(item);

		// Rock Generation
		for (int i = 0; i < Main.Instance.NumberOfRocks; i++)
		{
			HexCell sourceRock = null;
			while (sourceRock == null)
			{
				var randomPick = cellsList[Random.Range(0, cellsList.Count)];
				if (CellValidForRockification(randomPick))
					sourceRock = randomPick;
			}

			sourceRock.Owner = HexCell.Force.NoOne;

			var currentRockSnake = sourceRock;

			for (int x = 0; x < Main.Instance.RockSize - 1; x++)
			{
				var pickableRocks = GetEnemyAdjacentCell(currentRockSnake);
				pickableRocks = pickableRocks.Where(c => CellValidForRockification(c)).ToList();
				if (pickableRocks.Count == 0)
					break;
				var newRock = pickableRocks[Random.Range(0, pickableRocks.Count)];
				newRock.Owner = HexCell.Force.NoOne;
				currentRockSnake = newRock;
			}
		}


		foreach (var item in cellsList)
		{
			item.InitStats();
			item.UpdateVisuals();
		}
	}

	public List<HexCell> GetOwnerCells(HexCell.Force force)
	{
		return cellsList.Where(x => x.Owner == force).ToList();
	}

	void CreateCell(int x, int z, int i)
	{
		// Triming
		var hexCoords = HexCoordinates.FromOffsetCoordinates(x, z);
		if (hexCoords.Y >= -1 || hexCoords.Y <= -10
			|| hexCoords.X >= 7 || hexCoords.X <= -2)
			return;

		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.z = 0f;
		position.y = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = hexCoords;

		if (cell.coordinates.X <= 5 && cell.coordinates.X >= 3
			&& cell.coordinates.Y <= -3 && cell.coordinates.Y >= -5
			&& cell.coordinates.Z == 0)
			cell.Owner = HexCell.Force.Player;
		else
			cell.Owner = HexCell.Force.Enemy;

		if (cell.coordinates == new HexCoordinates(3, 1))
			cell.Owner = HexCell.Force.Player;

		if (cell.coordinates == new HexCoordinates(4, 1))
			cell.Owner = HexCell.Force.Player;

		if (cell.coordinates == new HexCoordinates(4, 0))
			Main.Instance.HomeCell = cell;

		if (cell.coordinates == new HexCoordinates(1, 6))
			Main.Instance.EnemyHomeCell = cell;

		StoreCell(cell);
	}

	public void CheckForCircledTiles()
	{
		foreach (var item in cellsList)
		{

			var path = AStarPathfinding.FindPath(item, Main.Instance.GetHome(item.Owner), item.Owner);
			if (path.Count <= 0)
			{
				// Tile is circled!
				switch (item.Owner)
				{
					case HexCell.Force.Player:
						item.Owner = HexCell.Force.Enemy;
						break;
					case HexCell.Force.Enemy:


						item.Owner = HexCell.Force.Player;
						var secondTest = AStarPathfinding.FindPath(item, Main.Instance.GetHome(item.Owner), item.Owner);
						if (secondTest.Count <= 0)
						{
							// This tile is secluded from both forces. Become rock
							item.Owner = HexCell.Force.NoOne;
						}
						else if (EnemyAI.Instance.AttackingCell != null && EnemyAI.Instance.AttackingCell == item) // Circled the attacking tile
							EnemyAI.Instance.CounterAttackSuccess();
						break;
					case HexCell.Force.NoOne:
						break;
					default:
						break;
				}
				item.UpdateVisuals();
			}
		}
	}

	public void StoreCell(HexCell cell)
	{
		cells[cell.coordinates.X + 10, cell.coordinates.Y + 10, cell.coordinates.Z + 10] = cell;
	}

	public HexCell GetCell(int x, int y, int z)
	{
		return cells[x + 10, y + 10, z + 10];
	}

	public HexCell GetTopLeftCell(HexCell hexCell)
	{
		return GetCell(hexCell.coordinates.X, hexCell.coordinates.Y + 1, hexCell.coordinates.Z - 1);
	}

	public HexCell GetTopRightCell(HexCell hexCell)
	{
		return GetCell(hexCell.coordinates.X + 1, hexCell.coordinates.Y, hexCell.coordinates.Z - 1);
	}

	public HexCell GetLeftCell(HexCell hexCell)
	{
		return GetCell(hexCell.coordinates.X - 1, hexCell.coordinates.Y + 1, hexCell.coordinates.Z);
	}

	public HexCell GetRightCell(HexCell hexCell)
	{
		return GetCell(hexCell.coordinates.X + 1, hexCell.coordinates.Y - 1, hexCell.coordinates.Z);
	}

	public HexCell GetBottomLeftCell(HexCell hexCell)
	{
		return GetCell(hexCell.coordinates.X - 1, hexCell.coordinates.Y, hexCell.coordinates.Z + 1);
	}

	public HexCell GetBottomRightCell(HexCell hexCell)
	{
		return GetCell(hexCell.coordinates.X, hexCell.coordinates.Y - 1, hexCell.coordinates.Z + 1);
	}

	public List<HexCell> GetAdjacentCells(HexCell cell)
	{
		List<HexCell> cellList = new List<HexCell>();

		HexCell adjacent = GetTopLeftCell(cell);
		if (adjacent != null)
			cellList.Add(adjacent);

		adjacent = GetTopRightCell(cell);
		if (adjacent != null)
			cellList.Add(adjacent);

		adjacent = GetLeftCell(cell);
		if (adjacent != null)
			cellList.Add(adjacent);

		adjacent = GetRightCell(cell);
		if (adjacent != null)
			cellList.Add(adjacent);

		adjacent = GetBottomLeftCell(cell);
		if (adjacent != null)
			cellList.Add(adjacent);

		adjacent = GetBottomRightCell(cell);
		if (adjacent != null)
			cellList.Add(adjacent);

		return cellList;
	}

	public List<HexCell> GetAlliedAdjacentCell(HexCell cell)
	{
		var list = GetAdjacentCells(cell);

		return list.Where(x => x.Owner == HexCell.Force.Player).ToList();
	}

	public List<HexCell> GetEnemyAdjacentCell(HexCell cell)
	{
		var list = GetAdjacentCells(cell);

		return list.Where(x => x.Owner == HexCell.Force.Enemy).ToList();
	}

	public bool AreAdjacent(HexCell cell1, HexCell cell2)
	{
		return GetAdjacentCells(cell1).Contains(cell2);
	}

	public void RefreshVisuals()
	{
		foreach (var item in cellsList)
		{
			if (item != null)
				item.UpdateVisuals();
		}
	}

	public bool CellValidForRockification(HexCell cell)
	{
		if (cell.Owner == HexCell.Force.Enemy && !cell.IsHome)
		{
			// Check if making this cell a rock would prevent a playable game
			var force = cell.Owner;
			cell.OwnerNoCall = HexCell.Force.NoOne;

			var path = AStarPathfinding.FindPath(Main.Instance.EnemyHomeCell, Main.Instance.HomeCell, HexCell.Force.OnlyRocks);

			cell.OwnerNoCall = force;

			// return true if there's a path.
			if (path.Count == 0)
				Debug.Log("A rock failed rockification due to pathfinding");
			return path.Count >= 1;
		}
		else
			return false;
	}
}