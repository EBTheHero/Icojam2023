using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main Instance;
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
