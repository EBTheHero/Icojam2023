using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{
	private Force owner = Force.Player;
	public HexCoordinates coordinates;
	public SpriteRenderer spriteRenderer;

	public Color PlayerColor;
	public Color EnemyColor;
	public Color RockColor;
	public Color HighlightColor;

	public TMPro.TextMeshProUGUI textMeshPro;

	public ParticleSystem SurroundParticle;
	public int AmountParticle = 15;

	public byte TileToughness = 5;
	private bool showCoords;

	public bool IsHome => this == Main.Instance.EnemyHomeCell || this == Main.Instance.HomeCell;

	public bool ShowCoords
	{
		get { return showCoords; }
		set
		{
			showCoords = value;
			UpdateVisuals();
		}
	}


	#region
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

	#endregion

	public List<HexCell> AdjacentCells
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

	public Force Owner
	{
		get => owner; set
		{

			owner = value;

			if (value == Force.Player && Main.Instance.EnemyHomeCell == this)
			{
				// WIN
				Main.Instance.Win();
			}
			else if (value == Force.Enemy && Main.Instance.HomeCell == this)
			{
				// LOSE
				Main.Instance.Lose();
			}
			else if (value == Force.Enemy || value == Force.Player)
				HexGrid.Instance.CheckForCircledTiles();

		}
	}

	/// <summary>
	/// When you want to set the owner without checking for win condition or circling
	/// </summary>
	public Force OwnerNoCall
	{
		get => owner; set
		{

			owner = value;
		}
	}


	public void UpdateVisuals()
	{
		if (showCoords)
			textMeshPro.text = coordinates.ToString();
		else if (Main.Instance.EnemyHomeCell != this)
			textMeshPro.text = Owner == Force.Enemy ? TileToughness.ToString() : "";

		switch (Owner)
		{
			case Force.Player:
				spriteRenderer.color = PlayerColor;
				break;
			case Force.Enemy:
				spriteRenderer.color = EnemyColor;
				break;
			case Force.NoOne:
				spriteRenderer.color = RockColor;
				break;
			default:
				break;
		}
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
		if (GameObject.Find("DevToggle").GetComponent<Toggle>().isOn)
		{
			switch (this.Owner)
			{
				case HexCell.Force.Player:
					this.Owner = HexCell.Force.Enemy;
					break;
				case HexCell.Force.Enemy:
					this.Owner = HexCell.Force.Player;
					break;
				case HexCell.Force.NoOne:
					break;
				default:
					break;
			}
			UpdateVisuals();
		}
		else if (GameObject.Find("DevRock").GetComponent<Toggle>().isOn)
		{
			owner = Force.NoOne;
			UpdateVisuals();
		}
		else
		{
			if (Main.Instance.SelectedArmee != null)
			{

				if (Owner == Force.Enemy && HexGrid.Instance.GetAlliedAdjacentCell(this).Count > 0)
					Main.Instance.SelectedArmee.PrepareToAttackCell(this);
				else if (Owner == Force.Player && !IsOccupied())
				{
					Main.Instance.SelectedArmee.InitierDeplacement(this);
					Main.Instance.SelectedArmee.TargetCell = null;
				}
			}
		}

		//path testing

		//var path = AStarPathfinding.FindPath(Main.Instance.HomeCell, this, Force.OnlyRocks);
		//HexGrid.Instance.RefreshVisuals();
		//foreach (var item in path)
		//{
		//	item.spriteRenderer.color = Color.cyan;
		//}

		//path = AStarPathfinding.FindPath(Main.Instance.HomeCell, this, Force.Player, true);

		//foreach (var item in path)
		//{
		//	item.spriteRenderer.color = new Color(item.spriteRenderer.color.r, 1, item.spriteRenderer.color.b);
		//}
	}

	public Armee IsOccupied()
	{
		for (byte i = 0; i < Main.Instance.Armies.Count; ++i)
		{
			if (Main.Instance.Armies[i].CurrentCell == this)
				return Main.Instance.Armies[i];
		}
		return null;
	}

	public bool IsTarget()
	{
		for (byte i = 0; i < Main.Instance.Armies.Count; ++i)
		{
			if (Main.Instance.Armies[i].TargetCell == this)
				return true;
		}
		return false;
	}

	public void PlaySurrounded()
	{
		SurroundParticle.Emit(AmountParticle);
	}

	public enum Force
	{
		Player,
		Enemy,
		NoOne,
		OnlyRocks
	}
}