using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class TeachUnitData
{
    public string unitName;
    public bool isAutoSkip = false;

    [Space(10)] 
    public AudioClip textAudio;
    [TextArea]
    public string teachText;

    [Space(10)] 
    public GameObject teachUnitObject;

    public int thisUnitNeedFeedback = 0;

}

public class TeachManager : MonoBehaviour
{
    public List<TeachUnitData> TeachUnitDataList = new List<TeachUnitData>();
    private bool isPlayerJumpTeach = false;

    [Header("Component")] 
    public TextMeshProUGUI teachUIText;

    private WaitForSeconds autoSkipDuration = new WaitForSeconds(1);
    private int currentUnitGetFeedback = 0;
    
    private void Awake()
    {
        StartCoroutine(TeachAction());
    }

    #region Event

    private void OnEnable()
    {
        EventHandler.TeachUnitFeedback += OnTeachUnitFeedback;
    }

    private void OnDisable()
    {
        EventHandler.TeachUnitFeedback -= OnTeachUnitFeedback;
    }

    private void OnTeachUnitFeedback()
    {
        currentUnitGetFeedback++;
        
        
    }

    #endregion 

    private IEnumerator TeachAction()
    {
        teachUIText.text = "";
        yield return autoSkipDuration;
        
        foreach (var teachDetails in TeachUnitDataList)
        {
            isPlayerJumpTeach = false;
            currentUnitGetFeedback = 0;
            teachDetails.teachUnitObject.SetActive(true);

            // Teach Voice
            AudioManager.Instance.PlayVoiceAudio(teachDetails.textAudio);
            
            // Teach UI Text Animation
            teachUIText.text = "";
            string text = teachDetails.teachText;
            WaitForSeconds textAnimationDuration = new WaitForSeconds(3f / text.Length);
            
            foreach (char c in text)
            {
                teachUIText.text += c;
                Debug.Log(c);
                yield return textAnimationDuration;
            }

            // Check this Unit need player stand the square to skip
            if (teachDetails.isAutoSkip)
            {
                yield return new WaitUntil(() => AudioManager.Instance.CheckVoicePlayDone());
                isPlayerJumpTeach = true;
            }
            else
            {
                yield return new WaitUntil(() => currentUnitGetFeedback == teachDetails.thisUnitNeedFeedback);
                isPlayerJumpTeach = true;
            }

            // TODO: Check current get feedback is enough
            
            yield return new WaitUntil(() => isPlayerJumpTeach);
            teachDetails.teachUnitObject.SetActive(false);
        }
    }
}
