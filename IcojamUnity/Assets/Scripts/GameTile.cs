using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTile : MonoBehaviour
{
    public Force Owner;
    public TileBase TileReference;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public enum Force
    {
        Player,
        Enemy
    }
}
