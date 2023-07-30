using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	[System.NonSerialized] public HexCell AttackedCell;

	private HexCell attackingCell;

	public HexCell AttackingCell
	{
		get { return attackingCell; }
		set
		{
			if (attackingCell != null)
				attackingCell.SetFadeAnim(false);
			attackingCell = value;
			if (attackingCell != null)
				attackingCell.SetFadeAnim(true);
		}
	}

	public Arrow attackArrow;
	public Animator enemyArmy;
	HexCell lastPickedAttackedCell;

	public static EnemyAI Instance;
	// Start is called before the first frame update
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	public void AttemptAttack()
	{
		if (AttackingCell != null)
		{
			AttackedCell.Owner = HexCell.Force.Enemy;
			AttackedCell.UpdateVisuals();
			EnemyArmy.Instance.InitierDeplacement(AttackedCell);
			SoundManager.Play("back_004");

			AttackingCell = null;
			AttackedCell = null;
		}
	}

	public void CounterAttackSuccess()
	{
		AttackedCell = null;
		AttackingCell = null;

		attackArrow.Pop();
		attackArrow.HideArrow();
		EnemyArmy.Instance.InitierDeplacement(Main.Instance.EnemyHomeCell);
		enemyArmy.SetBool("death", false);
	}

	public void PickCell()
	{
		bool foundGoodStrat = false;
		// Can it attack the player home?
		if (HexGrid.Instance.GetEnemyAdjacentCell(Main.Instance.HomeCell).Count > 0)
		{
			// HOME VULNERABLE
			if (lastPickedAttackedCell != Main.Instance.HomeCell)
			{
				AttackedCell = Main.Instance.HomeCell;
				AttackingCell = FindBestAttackingCell(AttackedCell);
				Debug.Log("AI strat: Attack home");
				foundGoodStrat = true;
			}
			else
			{
				Debug.Log("AI: Attack player home strat skipped");
			}
		}

		if (!foundGoodStrat)
		{
			// Find vulnerable cells for surrounding

			// find candidates
			var potentialCandidates = new List<HexCell>();
			List<HexCell> playerCells = HexGrid.Instance.GetOwnerCells(HexCell.Force.Player);

			foreach (var item in playerCells)
			{
				var vulnerableCells = GetSingleCuttable(item, lastPickedAttackedCell);

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
			foreach (var potentialAttackingCell in HexGrid.Instance.GetOwnerCells(HexCell.Force.Enemy))
			{
				var path = AStarPathfinding.FindPath(potentialAttackingCell, Main.Instance.HomeCell, HexCell.Force.OnlyRocks);
				if (path.Count > 0)
				{
					var cellToAttack = path[1];
					if (cellToAttack != lastPickedAttackedCell && cellToAttack.Owner == HexCell.Force.Player)
						results.Add((path.Count, potentialAttackingCell, cellToAttack));
					else
					{
						// attempt a different cell
						var surrounding = HexGrid.Instance.GetAlliedAdjacentCell(potentialAttackingCell);
						surrounding.Remove(lastPickedAttackedCell);
						if (surrounding.Count > 0)
							results.Add((path.Count, potentialAttackingCell, surrounding.First()));
					}
				}
			}

			if (results.Count > 0)
			{
				AttackingCell = results.OrderBy(x => x.Item1).First().Item2;
				AttackedCell = results.OrderBy(x => x.Item1).First().Item3;


				foundGoodStrat = true;
				Debug.Log("AI strat: Attack closest cell to player home");
			}
			else
			{
				Debug.Log("AI: no valid cell to attack (only possible one has been picked last turn");

			}

		}

		if (!foundGoodStrat)
		{
			lastPickedAttackedCell = null;
			attackArrow.HideArrow();
			EnemyArmy.Instance.InitierDeplacement(Main.Instance.EnemyHomeCell);
		}
		else
		{
			lastPickedAttackedCell = AttackedCell;
			attackArrow.UpdateArrow(AttackingCell.transform, AttackedCell.transform);
			EnemyArmy.Instance.InitierDeplacement(AttackingCell);
		}

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
	HexCell GetSingleCuttable(HexCell hexCell, HexCell ignoreCell = null)
	{
		var alliedCells = HexGrid.Instance.GetAlliedAdjacentCell(hexCell);
		if (ignoreCell != null)
			alliedCells.Remove(ignoreCell);

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

	public void CheckIfArmyDead(HexCell cell)
	{
		if (cell == AttackingCell)
			enemyArmy.SetBool("death", true);
	}
}
