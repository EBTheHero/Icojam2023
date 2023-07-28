using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main Instance;

    public Armee[] Armies;

    private Armee armee;

    public Armee SelectedArmee
    {
        get { return armee; }
        set
        {
            if (armee != null)
                armee.SetSelected(false);

            armee = value;
            if (armee != null)
                armee.SetSelected(true);
        }
    }

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
