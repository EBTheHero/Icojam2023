using UnityEngine;
using UnityEngine.UI;

public class DevToggleDefaultValue : MonoBehaviour
{
    void Start()
    {
        GetComponent<Toggle>().isOn = GameManager.Instance.DevMode;
    }

    public void SetDevModeOfInstance(bool value)
    {
        GameManager.Instance.DevMode = value;
    }
}
