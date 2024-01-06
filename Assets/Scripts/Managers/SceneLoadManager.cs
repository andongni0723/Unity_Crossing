using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : Singleton<SceneLoadManager>
{
    [Header("Components")]
    public CanvasGroup loadingCanvasGroup;
    [Header("Settings")] 
    [SceneName] public string startScene;
    
    [Header("Data")]
    [SceneName] public string basicSceneName;
    [SceneName] public string mainMenuSceneName;
    [SceneName] public string gameSceneName;
    
    private string currentSceneName;
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.name));
        };
    }

    public override void Awake()
    {
        base.Awake();
        currentSceneName = startScene;
        LoadStartScene();
    }

    public void ChangeScene(string sceneName)
    {
        // scene mode is additive
        Sequence sequence = DOTween.Sequence();
        sequence.Append(loadingCanvasGroup.DOFade(1, 0.5f).OnComplete(() =>
        {
            SceneManager.UnloadSceneAsync(currentSceneName);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            currentSceneName = sceneName;
        }));
        
        sequence.Append(loadingCanvasGroup.DOFade(0, 0.5f));
    }

    private void LoadStartScene()
    {
        SceneManager.LoadScene(startScene, LoadSceneMode.Additive);
    }
}
