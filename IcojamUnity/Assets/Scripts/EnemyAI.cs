using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	public HexCell AttackingCell;
	public HexCell AttackedCell;

	public Arrow attackArrow;

	public static EnemyAI Instance;
	// Start is called before the first frame update
	void Awake()
	{
		if (Instance == null)
			Instance = this;
	}

	public void AttemptAttack()
	{
		if (AttackingCell != null)
		{
			AttackedCell.Owner = HexCell.Force.Enemy;
			AttackedCell.UpdateVisuals();
		}
	}

	public void CounterAttackSuccess()
	{
		AttackedCell = null;
		AttackingCell = null;

		attackArrow.HideArrow();
	}

	public void PickCell()
	{
		bool foundGoodStrat = false;
		// Can it attack the player home?
		if (HexGrid.Instance.GetEnemyAdjacentCell(Main.Instance.HomeCell).Count > 0)
		{
			// HOME VULNERABLE
			AttackedCell = Main.Instance.HomeCell;
			AttackingCell = FindBestAttackingCell(AttackedCell);
			Debug.Log("AI strat: Attack home");
			foundGoodStrat = true;
		}

		if (!foundGoodStrat)
		{
			// Find vulnerable cells for surrounding

			// find candidates
			var potentialCandidates = new List<HexCell>();
			foreach (var item in HexGrid.Instance.GetOwnerCells(HexCell.Force.Player))
			{
				var vulnerableCells = GetSingleCuttable(item);

				if (vulnerableCells != null)
				{
					potentialCandidates.Add(vulnerableCells);
				}
			}

			if (potentialCandidates.Count > 0)
			{
				var potentialAttacks = new List<(HexCell, HexCell)>();

				// find the strongest attack amongst candidates
				foreach (var item in potentialCandidates)
				{
					HexCell hexCell = FindBestAttackingCell(item);
					if (hexCell != null)
						potentialAttacks.Add((item, hexCell));
				}

				if (potentialAttacks.Count > 0)
				{
					var attack = potentialAttacks.OrderByDescending(item => item.Item2.TileToughness).First();
					AttackingCell = attack.Item2;
					AttackedCell = attack.Item1;

					Debug.Log("AI strat: Surround strongest cut");
					foundGoodStrat = true;
				}
			}
		}


		if (!foundGoodStrat)
		{
			// Find the closest cell to the player home
			var results = new List<(int, HexCell, HexCell)>();
			foreach (var item in HexGrid.Instance.GetOwnerCells(HexCell.Force.Enemy))
			{
				var path = AStarPathfinding.FindPath(item, Main.Instance.HomeCell, HexCell.Force.NoOne);
				if (path.Count > 0)
					results.Add((path.Count, item, path[1]));
			}

			AttackingCell = results.OrderBy(x => x.Item1).First().Item2;
			AttackedCell = results.OrderBy(x => x.Item1).First().Item3;

			Debug.Log("AI strat: Attack closest cell to player home");
		}

		attackArrow.UpdateArrow(AttackingCell.transform, AttackedCell.transform);

	}

	public HexCell FindBestAttackingCell(HexCell attackedCell)
	{
		List<HexCell> attackingCells = HexGrid.Instance.GetEnemyAdjacentCell(attackedCell);
		if (attackingCells.Count > 0)
			return attackingCells.OrderByDescending(item => item.TileToughness).First();
		else
			return null;
	}

	List<HexCell> GetVulnerableCuttableCells(HexCell hexCell)
	{
		var bestPath = AStarPathfinding.FindPath(hexCell, Main.Instance.HomeCell, HexCell.Force.Player);
		var worsePath = AStarPathfinding.FindPath(hexCell, Main.Instance.HomeCell, HexCell.Force.Player, true);

		if (bestPath.Count > 2 && worsePath.Count > 2)
		{
			// remove first and last cells, as one is the current cell and the other is home
			bestPath.RemoveAt(0);
			bestPath.RemoveAt(bestPath.Count - 1);

			worsePath.RemoveAt(0);
			worsePath.RemoveAt(worsePath.Count - 1);

			List<HexCell> hexCells = bestPath.Intersect<HexCell>(worsePath).ToList();
			foreach (var cell in hexCells)
				cell.spriteRenderer.color = Color.cyan;
			return hexCells;
		}
		else
		{
			return new List<HexCell>();
		}


	}

	// return a list of player cells that can be isolated
	HexCell GetSingleCuttable(HexCell hexCell)
	{
		var alliedCells = HexGrid.Instance.GetAlliedAdjacentCell(hexCell);
		if (alliedCells.Count == 1)
		{
			// this cell can be isolated
			return alliedCells[0];
		}
		else
		{
			return null;
		}
	}
}
