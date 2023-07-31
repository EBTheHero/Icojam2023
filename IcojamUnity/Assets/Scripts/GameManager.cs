using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[NonSerialized] public bool DevMode = false;


	public static GameManager Instance = null;

	private void Awake()
	{
#if UNITY_EDITOR
		DevMode = true;

#endif

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
