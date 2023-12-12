using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FinalBossEvent : MonoBehaviour
{
    [Header("Component")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bossHealthText;
    public CanvasGroup fadeCanvasGroup;
    public SpriteRenderer backgroundSpriteRenderer;
    
    [Header("Setting")]
    public Color bossBackgroundColor;

    #region Event

    private void OnEnable()
    {
        EventHandler.BossEventPrepare += OnBossEventPrepare;
    }

    private void OnDisable()
    {
        EventHandler.BossEventPrepare -= OnBossEventPrepare;
    }

    private void OnBossEventPrepare()
    {
        StartCoroutine(FinalBossPrepareAnimation());
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
        string text = "Boss!!!";
        for (int i = 0; i < text.Length; i++)
        {
            bossHealthText.text += text[i];
            yield return new WaitForSeconds(0.5f);
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
            EventHandler.CallBossEventPrepareDone();
        }));

        yield return null;
    }
    
    
}
