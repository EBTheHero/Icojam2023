using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public HexCell AttackingCell;
    public HexCell AttackedCell;

    public GameObject Arrow;

    public static EnemyAI Instance;
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
                var vulnerableCells = GetVulnerableCuttableCells(item);

                if (vulnerableCells.Count > 0)
                {
                    potentialCandidates.AddRange(vulnerableCells);
                }
            }

            if (potentialCandidates.Count > 0)
            {
                var potentialAttacks = new List<(HexCell, HexCell)>();

                // find the strongest attack amongst candidates
                foreach (var item in potentialCandidates)
                {
                    potentialAttacks.Add((item, FindBestAttackingCell(item)));
                }

                var attack = potentialAttacks.OrderByDescending(item => item.Item2.TileToughness).First();
                AttackingCell = attack.Item2;
                AttackedCell = attack.Item1;

                Debug.Log("AI strat: Surround strongest cut");
                foundGoodStrat = true;
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

        UpdateArrow();
    }

    public HexCell FindBestAttackingCell(HexCell attackedCell)
    {
        return HexGrid.Instance.GetEnemyAdjacentCell(attackedCell).OrderByDescending(item => item.TileToughness).First();
    }

    void UpdateArrow()
    {
        Arrow.transform.position = Vector2.MoveTowards(AttackingCell.transform.position, AttackedCell.transform.position, HexMetrics.outerRadius);
        Arrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, AttackedCell.transform.position - AttackingCell.transform.position);
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
}
