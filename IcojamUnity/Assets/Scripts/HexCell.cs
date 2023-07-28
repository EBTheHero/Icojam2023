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

	public int TileToughtness = 2;
	public void UpdateVisuals()
	{
		textMeshPro.text = coordinates.ToString();
		//textMeshPro.text = TileToughtness.ToString();

		spriteRenderer.color = Owner == Force.Player ? PlayerColor : EnemyColor;
	}

	public void Highlight()
	{
		spriteRenderer.color = HighlightColor;
	}

	void OnMouseDown()
	{
		HexGrid.Instance.RefreshVisuals();

		var cells = HexGrid.Instance.GetAdjacentCells(this);

		foreach (var item in cells)
		{
			item.Highlight();
		}
	}

	public enum Force
	{
		Player,
		Enemy
	}
}