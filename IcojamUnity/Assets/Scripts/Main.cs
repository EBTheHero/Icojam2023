using UnityEngine;
using UnityEngine.Tilemaps;

public class Main : MonoBehaviour
{
    public static Main Instance;
    public Tilemap MainTilemap;
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
}
