using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
#if UNITY_EDITOR
    public bool DevMode = true;
#else
    public bool DevMode = false;
#endif
    
    public static GameManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }

    public static void BackToMainMenu()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene("Menu");
    }

    public void SetDevMode(bool activated)
    {
        DevMode = activated;
    }
}
