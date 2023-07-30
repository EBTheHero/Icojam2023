using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevElements : MonoBehaviour
{
    private void Start()
    {
        if(GameManager.DevMode)
        {
            foreach (Transform t in transform)
                t.gameObject.SetActive(true);
        }
        else
            Destroy(gameObject);
    }
}
