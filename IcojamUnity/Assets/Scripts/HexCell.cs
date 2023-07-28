using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
	public Force Owner = Force.Player;
	public HexCoordinates coordinates;
	public SpriteRenderer spriteRenderer;

	public Color PlayerColor;
	public Color EnemyColor;
	public Color HighlightColor;

	public TMPro.TextMeshProUGUI textMeshPro;

	public byte TileToughness = 5;

	/// <summary>
	/// Sum of G and H.
	/// </summary>
	public int F => g + h;

	/// <summary>
	/// Cost from start tile to this tile.
	/// </summary>
	public int g;

	/// <summary>
	/// Estimated cost from this tile to destination tile.
	/// </summary>
	public int h;

	private List<HexCell> adjacentTiles;

	public List<HexCell> AdjacentTiles
	{
		get
		{
			if (adjacentTiles == null)
			{
				adjacentTiles = HexGrid.Instance.GetAdjacentCells(this);
			}

			return adjacentTiles;
		}

	}


	public void UpdateVisuals()
	{
		//textMeshPro.text = coordinates.ToString();
		textMeshPro.text = Owner == Force.Enemy ? TileToughness.ToString() : "";

		spriteRenderer.color = Owner == Force.Player ? PlayerColor : EnemyColor;
	}

	public void InitStats()
	{
		TileToughness = (byte)(Main.Instance.Difficulty - Main.Instance.TileDistanceMultiplicatior * (AStarPathfinding.FindPath(Main.Instance.EnemyHomeCell, this, Force.NoOne).Count - 1));
	}

	public void Highlight()
	{
		spriteRenderer.color = HighlightColor;
	}

	void OnMouseDown()
	{
		if (Main.Instance.SelectedArmee != null && Owner == Force.Enemy)
		{
			if (HexGrid.Instance.GetAlliedAdjacentCell(this).Count > 0)
				Main.Instance.SelectedArmee.ReadyToAttackCell(this);
		}
	}

	public enum Force
	{
		Player,
		Enemy,
		NoOne
	}
}