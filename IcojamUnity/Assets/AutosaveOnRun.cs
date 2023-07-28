#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;



[InitializeOnLoad]
public class AutosaveOnRun: ScriptableObject
{
	static AutosaveOnRun()
	{
        EditorApplication.playModeStateChanged += TimeToSave;
	}

    private static void TimeToSave(PlayModeStateChange state)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
        {
            Debug.Log("Auto-Saving scene before entering Play mode: " + EditorSceneManager.GetActiveScene().name);

            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
        }
    }
}
#endif