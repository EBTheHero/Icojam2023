using UnityEngine;

public class HexGrid : MonoBehaviour
{

	public int width = 6;
	public int height = 6;

	public HexCell cellPrefab;

	HexCell[,,] cells;

	public static HexGrid Instance;

	void Start()
	{
		if (Instance == null)
			Instance = this;

		cells = new HexCell[100, 100, 100];

		for (int z = 0, i = 0; z < height; z++)
		{
			for (int x = 0; x < width; x++)
			{
				CreateCell(x, z, i++);
			}
		}
	}

	void CreateCell(int x, int z, int i)
	{
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.z = 0f;
		position.y = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		StoreCell(cell);
		cell.UpdateText();
	}

	public void StoreCell(HexCell cell)
	{
		cells[cell.coordinates.X + 50, cell.coordinates.Y + 50, cell.coordinates.Z + 50] = cell;
	}

	public HexCell GetCell(int x, int y, int z)
	{
		return cells[x - 50, y - 50, z - 50];
	}

	public HexCell GetTopLeftCell(HexCell hexCell)
	{
		return GetCell(hexCell.coordinates.X - 1, hexCell.coordinates.Y - 1, hexCell.coordinates.Z + 1);
	}
	//public List<HexCell> GetAdjacentCells(HexCell cell)
	//{
	//	List<HexCell> cells = new List<HexCell>();

	//}


}