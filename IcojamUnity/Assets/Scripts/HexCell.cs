using UnityEngine;

public class HexCell : MonoBehaviour
{
	public Force Owner;
	public HexCoordinates coordinates;

	public TMPro.TextMeshProUGUI textMeshPro;

	public void UpdateText()
	{
		textMeshPro.text = coordinates.ToString();
	}

	public enum Force
	{
		Player,
		Enemy
	}
}