using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main Instance;

    public Armee[] Armies;
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

    public void EndTurn()
    {
        foreach (var armee in Armies)
        {
            // Armee.attack
        }
    }
}
