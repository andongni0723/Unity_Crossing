using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Setting")] 
    public string highScoreLabelString = "Highest Score: ";
    public float scoreTextAnimationDuration = 0.01f;
    public float mainLightFadeInDuration = 1f;
    public float mainLightIntensity = 0.14f;

    [Header("Component")]
    public TextMeshProUGUI highScoreLabelText;
    public TextMeshProUGUI highScoreText;
    public CanvasGroup mainUI;
    public Light2D mainLight;
    public Light2D redLight;
    public Button startButton;
    public Button teachButton;

    private void Awake()
    {
        AudioManager.Instance.PlayBGM(AudioManager.Instance.openingBGM);
        StartCoroutine(StartSceneAnimation());
    }
    
    public void StartGame()
    {
        startButton.interactable = false;
        AudioManager.Instance.PlaySoundAudio(AudioManager.Instance.hitWallSound);
        AudioManager.Instance.BGMFade(0, 1);
        SceneLoadManager.Instance.ChangeScene(SceneLoadManager.Instance.gameSceneName);
    }

    public void Teach()
    {
        teachButton.interactable = false;
        AudioManager.Instance.PlaySoundAudio(AudioManager.Instance.hitWallSound);
        Debug.Log("Teach Scene");
    }

    IEnumerator StartSceneAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        mainLight.intensity = 0;
        redLight.intensity = 0;
        mainUI.alpha = 0;
        highScoreLabelText.text = "";
        highScoreText.text = "";

        // Do highScoreLabelText
        foreach (var t in highScoreLabelString)
        {
            highScoreLabelText.text += t;
            yield return new WaitForSeconds(scoreTextAnimationDuration);
        }

        yield return new WaitForSeconds(scoreTextAnimationDuration);

        // Do highScoreText
        string scoreString = MainGameManager.Instance.highScore.ToString();
        foreach(var t in scoreString)
        {
            highScoreText.text += t;
            yield return new WaitForSeconds(scoreTextAnimationDuration);
        }
        
        yield return new WaitForSeconds(1);
        // Background Fade In
        sequence.Append(DOTween.To(
            (x => mainLight.intensity = x), 
            0, mainLightIntensity, mainLightFadeInDuration));
        
        // Main UI Fade In
        sequence.Join(mainUI.DOFade(1, mainLightFadeInDuration).OnComplete(() =>
        {
            StartCoroutine(RedLightAnimation());
        }));

        yield return null;
    }
    
    IEnumerator RedLightAnimation()
    {
        while (true)
        {
            float random = Random.Range(0, 11) / 10.0f;
            Debug.Log(random);
            Sequence sequence = DOTween.Sequence();
            
            sequence.Append(DOTween.To(
                (x => redLight.intensity = x), 0, random, 0.5f));
            sequence.AppendInterval(2);
            sequence.Append(DOTween.To(
                (x => redLight.intensity = x), random, 0, 0.5f));
            sequence.AppendInterval(2);
            yield return sequence.WaitForCompletion();
        }
    }
}
