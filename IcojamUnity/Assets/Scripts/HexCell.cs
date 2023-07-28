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
	public void UpdateVisuals()
	{
		//textMeshPro.text = coordinates.ToString();
		textMeshPro.text = Owner == Force.Enemy ? TileToughness.ToString() : "";

		spriteRenderer.color = Owner == Force.Player ? PlayerColor : EnemyColor;
	}

	public void InitStats()
	{
		TileToughness = coordinates.Z + 3;
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
		Enemy
	}
}