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
    
    [Space(15)]
    public Color mainLightRedColor;
    public Color mainLightGreenColor;
    public Color logoTextRedColor;
    public Color logoTextGreenColor;
    public Color textRedColor;
    public Color textGreenColor;
    public Color iconRedColor;
    public Color iconGreenColor;
    public float changeThemeDuration = 1f;
    public float checkBoxOpenDuration = 0.5f;

    [Header("Component")]
    public TextMeshProUGUI highScoreLabelText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI logoText;
    public TextMeshProUGUI versionText;
    public CanvasGroup mainUI;
    public Light2D mainLight;
    public Light2D redLight;
    public Button startButton;
    public Button teachButton;
    
    [Space(15)]
    public GameObject checkBox;
    public Button checkBoxYesButton;
    public Button checkBoxNoButton;

    private void Awake()
    {
        AudioManager.Instance.PlayBGM(AudioManager.Instance.openingBGM);
        StartCoroutine(ChangeTheme(mainLightGreenColor, logoTextGreenColor, textGreenColor, iconGreenColor)); 
        
        StartCoroutine(StartSceneAnimation());
        //StartCoroutine(ChengeThemeTest()); // TODO: Test
    }

    #region Button Method

    public void StartGame()
    {
        startButton.interactable = false;
        AudioManager.Instance.PlaySoundAudio(AudioManager.Instance.hitWallSound);
        AudioManager.Instance.BGMFade(0, 1.5f);
        SceneLoadManager.Instance.ChangeScene(SceneLoadManager.Instance.gameSceneName);
    }

    public void Teach()
    {
        teachButton.interactable = false;
        AudioManager.Instance.PlaySoundAudio(AudioManager.Instance.hitWallSound);
        AudioManager.Instance.BGMFade(0, 1.5f);
        SceneLoadManager.Instance.ChangeScene(SceneLoadManager.Instance.teachSceneName);
    }

    public void Hard()
    {
        CheckBoxOpen();
    }

    public void CheckBoxYes()
    {
        startButton.interactable = false;
        MainGameManager.Instance.isHardMode = true;
        AudioManager.Instance.PlaySoundAudio(AudioManager.Instance.hitWallSound);
        AudioManager.Instance.BGMFade(0, 1.5f);
        SceneLoadManager.Instance.ChangeScene(SceneLoadManager.Instance.gameSceneName);
    }
    
    public void CheckBoxNo()
    {
        StartCoroutine(ChangeTheme(mainLightGreenColor, logoTextGreenColor, textGreenColor, iconGreenColor)); 
        CheckBoxClose();
    }

    #endregion
    
    private void CheckBoxOpen()
    {
        StartCoroutine(ChangeTheme(mainLightRedColor, logoTextRedColor, textRedColor, iconRedColor));
        checkBox.transform.localScale = Vector3.zero;
        checkBox.SetActive(true);
        checkBox.transform.DOScale(1, checkBoxOpenDuration); 
    }

    private void CheckBoxClose()
    {
        StartCoroutine(ChangeTheme(mainLightGreenColor, logoTextGreenColor, textGreenColor, iconGreenColor));
        checkBox.transform.localScale = Vector3.one;
        checkBox.transform.DOScale(0, checkBoxOpenDuration).OnComplete(() =>
        {
            checkBox.SetActive(false); 
        });  
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

    IEnumerator ChangeTheme(Color lightTargetColor, Color logoTargetColor, Color textTargetColor, Color iconTargetColor)
    {
        Sequence sequence = DOTween.Sequence();
        
        // Text and Button Color Change
        sequence.Append(logoText.DOColor(logoTargetColor, changeThemeDuration));
        sequence.Join(versionText.DOColor(logoTargetColor, changeThemeDuration));
        sequence.Join(highScoreText.DOColor(textTargetColor, changeThemeDuration));
        sequence.Join(teachButton.GetComponent<Image>().DOColor(iconTargetColor, changeThemeDuration));
        sequence.Join(startButton.GetComponent<Image>().DOColor(iconTargetColor, changeThemeDuration));
        
        // Light Color Change
        Sequence backgroundSequence = DOTween.Sequence();
        backgroundSequence.Append(DOTween.To(
            (x => mainLight.color = new Color(x, mainLight.color.g, mainLight.color.b)),
            mainLight.color.r, lightTargetColor.r, changeThemeDuration / 2.0f));
        backgroundSequence.Append(DOTween.To(
            (x => mainLight.color = new Color(mainLight.color.r, x, mainLight.color.b)),
            mainLight.color.g, lightTargetColor.g, changeThemeDuration / 2.0f)); 
        
        yield return sequence.WaitForCompletion();   
    }
}
