using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonPlay : MonoBehaviour
{
    // Start is called before the first frame update
    public Button RulesButton;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayButton()
    {
        var i = PlayerPrefs.GetInt("ReadRules");

        if (i == 0)
            RulesButton.onClick.Invoke();
        else
            SceneManager.LoadScene(1);
    }
}
