#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class CustomKeys : Editor
{
    [MenuItem("Custom Keys/Unload Scene %#U")]
    static void EditorCustomKeysUnloadScene()
    {
        // Get All selected object id
        foreach (var id in Selection.instanceIDs)
        {
            // Try to get scene to save and close it
            Scene tryGetScene = InstanceIDToScene(id);

            if (tryGetScene != default && tryGetScene.isLoaded)
            {
                EditorSceneManager.SaveScene(tryGetScene);
                EditorSceneManager.CloseScene(tryGetScene, false);
            }
            
            Debug.Log($"Save and Unload Scene \"{tryGetScene.name}\"");
        }
    }
    
    [MenuItem("Custom Keys/Load Scene %#L")]
    static void EditorCustomKeysLoadScene()
    { 
        // Get All selected object id
        foreach (var id in Selection.instanceIDs)
        {
            // Try to get scene and open it
            Scene tryGetScene = InstanceIDToScene(id);
            
            if(tryGetScene != default)
                EditorSceneManager.OpenScene(tryGetScene.path, OpenSceneMode.Additive);
            
            Debug.Log($"Open Scene \"{tryGetScene.name}\"");

        }
    }

    
    /// <summary>
    /// Get All Scene and compare each scene instanceID, return equal id scene 
    /// </summary>
    /// <param name="instanceID"></param>
    /// <returns></returns>
    public static Scene InstanceIDToScene(int instanceID)
    {
        // Get All Scene
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene currentScene = SceneManager.GetSceneAt(i);

            // Compare
            if (currentScene.handle == instanceID)
            {
                return currentScene;
            }
        }
        
        return default;
    }
    
    [MenuItem("Custom Keys/Create C# Scripts %#C")]
    public static void CreateNewCSharpScript()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
            "/Applications/Unity/Hub/Editor/2022.3.10f1/Unity.app/Contents/Resources/ScriptTemplates/81-C# Script-NewBehaviourScript.cs.txt",
            "NewBehaviourScript.cs");
    }
}

#endif
