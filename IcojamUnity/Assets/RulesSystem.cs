using UnityEngine;
using UnityEngine.SceneManagement;

public class RulesSystem : MonoBehaviour
{
    int index = 0;

    public GameObject[] Pages;
    // Start is called before the first frame update
    private void Awake()
    {
        index = 0;
        UpdatePageVisual();
    }

    private void OnEnable()
    {
        index = 0;
        UpdatePageVisual();
        PlayerPrefs.SetInt("ReadRules", 1);
        PlayerPrefs.Save();
    }

    public void NextPage()
    {
        index++;
        if (Pages.Length == index)
            index = 0;
        UpdatePageVisual();
    }

    void UpdatePageVisual()
    {
        foreach (var page in Pages)
        {
            page.SetActive(false);
        }

        Pages[index].SetActive(true);
    }

    public void LaunchGame()
    {
        SceneManager.LoadScene(1);
    }
}
