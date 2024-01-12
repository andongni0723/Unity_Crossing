using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FinalBossEvent : MonoBehaviour
{
    [Header("Component")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bossHealthText;
    public CanvasGroup fadeCanvasGroup;
    public SpriteRenderer backgroundSpriteRenderer;
    ChromaticAberration chromaticAberration =>
        GameManager.Instance.finalBossVolume.profile.components[0] as ChromaticAberration;
    
    [Header("Setting")]
    public List<string> BossEventStartTextList;
    public Color bossBackgroundColor;

    #region Event

    private void OnEnable()
    {
        EventHandler.BossEventPrepare += OnBossEventPrepare;
        EventHandler.FinalBossDead += OnFinalBossDead;
    }

    private void OnDisable()
    {
        EventHandler.BossEventPrepare -= OnBossEventPrepare;
        EventHandler.FinalBossDead -= OnFinalBossDead;
    }

    private void OnBossEventPrepare()
    {
        StartCoroutine(FinalBossPrepareAnimation());
    }

    private void OnFinalBossDead()
    {
        StartCoroutine(FinalBossDeadAnimation());
    }

    #endregion


    IEnumerator FinalBossPrepareAnimation()
    {
        scoreText.gameObject.SetActive(false);
        bossHealthText.gameObject.SetActive(true);
        bossHealthText.text = "";
        
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlayLaserSoundAudio(AudioManager.Instance.laserAccumulateSound);

        // text animation
        // bossHealthText.text = "100%";
        string text = BossEventStartTextList[Random.Range(0, BossEventStartTextList.Count)];
        for (int i = 0; i < text.Length; i++)
        {
            bossHealthText.text += text[i];
            yield return new WaitForSeconds(0.3f);
        }

        // Fade in
        Sequence sequence = DOTween.Sequence();
        sequence.Append(fadeCanvasGroup.DOFade(1, 1).OnComplete(() =>
        {
            // Black screen
            bossHealthText.text = "100%";
            backgroundSpriteRenderer.color = bossBackgroundColor;
            EnemySpawnManager.Instance.FinalBossGenerate();
        }));
        sequence.AppendInterval(1f);
        sequence.Append(fadeCanvasGroup.DOFade(0, 0).OnComplete(() =>
        {
            AudioManager.Instance.PlayBGM(AudioManager.Instance.finalBossBGM);
            chromaticAberration.intensity.value = 1;
            EventHandler.CallBossEventPrepareDone();
        }));

        yield return null;
    }

    IEnumerator FinalBossDeadAnimation()
    {
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlayBGM(AudioManager.Instance.gameBGM);
        chromaticAberration.intensity.value = 0;
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(backgroundSpriteRenderer.DOColor(Color.black, 1f));

        bossHealthText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);

        EventHandler.CallFinalBossDeadEventDone();
        yield return null;
    }
}
