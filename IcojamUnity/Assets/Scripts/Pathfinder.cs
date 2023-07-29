
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HexCell;
/// <summary>
/// Implementation of A* pathfinding algorithm.
/// </summary>
public static class AStarPathfinding
{
	/// <summary>
	/// Finds path from given start point to end point. Returns an empty list if the path couldn't be found.
	/// </summary>
	/// <param name="startPoint">Start tile.</param>
	/// <param name="endPoint">Destination tile.</param>
	public static List<HexCell> FindPath(HexCell startPoint, HexCell endPoint, Force asForce, bool worsePath = false)
	{
		List<HexCell> openPathTiles = new List<HexCell>();
		List<HexCell> closedPathTiles = new List<HexCell>();

		// Prepare the start tile.
		HexCell currentTile = startPoint;

		currentTile.g = 0;
		currentTile.h = GetEstimatedPathCost(startPoint.coordinates, endPoint.coordinates);

		// Add the start tile to the open list.
		openPathTiles.Add(currentTile);

		while (openPathTiles.Count != 0)
		{
			// Sorting the open list to get the tile with the lowest F.
			if (!worsePath)
				openPathTiles = openPathTiles.OrderBy(x => x.F).ThenByDescending(x => x.g).ToList();
			else
				openPathTiles = openPathTiles.OrderByDescending(x => x.F).ThenBy(x => x.g).ToList();
			currentTile = openPathTiles[0];

			// Removing the current tile from the open list and adding it to the closed list.
			openPathTiles.Remove(currentTile);
			closedPathTiles.Add(currentTile);

			int g = currentTile.g + 1;

			// If there is a target tile in the closed list, we have found a path.
			if (closedPathTiles.Contains(endPoint))
			{
				break;
			}

			// Investigating each adjacent tile of the current tile.
			foreach (HexCell adjacentTile in currentTile.AdjacentCells)
			{
				// Ignore not walkable adjacent tiles.
				if (asForce == Force.OnlyRocks)
				{
					// We're checking for paths regardless of owner. Only ignoring rocks
					if (adjacentTile.Owner == Force.NoOne)
						continue;
				}
				else if (!(asForce == Force.NoOne || adjacentTile.Owner == asForce))
				{
					continue;
				}

				// Ignore the tile if it's already in the closed list.
				if (closedPathTiles.Contains(adjacentTile))
				{
					continue;
				}

				// If it's not in the open list - add it and compute G and H.
				if (!(openPathTiles.Contains(adjacentTile)))
				{
					adjacentTile.g = g;
					adjacentTile.h = GetEstimatedPathCost(adjacentTile.coordinates, endPoint.coordinates);
					openPathTiles.Add(adjacentTile);
				}
				// Otherwise check if using current G we can get a lower value of F, if so update it's value.
				else if (adjacentTile.F > g + adjacentTile.h)
				{
					adjacentTile.g = g;
				}
			}
		}

		List<HexCell> finalPathTiles = new List<HexCell>();

		// Backtracking - setting the final path.
		if (closedPathTiles.Contains(endPoint))
		{
			currentTile = endPoint;
			finalPathTiles.Add(currentTile);

			for (int i = endPoint.g - 1; i >= 0; i--)
			{
				currentTile = closedPathTiles.Find(x => x.g == i && currentTile.AdjacentCells.Contains(x));
				finalPathTiles.Add(currentTile);
			}

			finalPathTiles.Reverse();
		}

		return finalPathTiles;
	}

	/// <summary>
	/// Returns estimated path cost from given start coordinates to target coordinates of hex tile using Manhattan distance.
	/// </summary>
	/// <param name="startPosition">Start coordinates.</param>
	/// <param name="targetPosition">Destination coordinates.</param>
	static int GetEstimatedPathCost(HexCoordinates startPosition, HexCoordinates targetPosition)
	{
		return Mathf.Max(Mathf.Abs(startPosition.Z - targetPosition.Z), Mathf.Max(Mathf.Abs(startPosition.X - targetPosition.X), Mathf.Abs(startPosition.Y - targetPosition.Y)));
	}
}