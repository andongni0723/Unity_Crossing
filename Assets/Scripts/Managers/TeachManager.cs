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

public class TeachManager : Singleton<TeachManager>
{
    public List<TeachUnitData> TeachUnitDataList = new List<TeachUnitData>();
    private bool isPlayerJumpTeach = false;

    [Header("Component")] 
    public GameObject player;
    public TextMeshProUGUI teachUIText;
    public TextMeshProUGUI hintUIText;

    private WaitForSeconds autoSkipDuration = new WaitForSeconds(1);
    private string currentTeachUnitName = "";
    private int currentUnitGetFeedback = 0;
    // private bool isCallSkip = false;

    public override void Awake()
    {
        base.Awake();  
        StartCoroutine(TeachAction());
        player = GameObject.FindWithTag("Player");
    }

    #region Event

    private void OnEnable()
    {
        EventHandler.SkipThisUnit += OnSkipThisUnit; // Skip this unit
        EventHandler.TeachUnitFeedback += OnTeachUnitFeedback; // Add feedback
        EventHandler.PlayerCrossing += OnPlayerCrossing; // Check and add feedback
        EventHandler.TeachNextUnit += OnTeachNextUnit; // Close hint text
    }

    private void OnDisable()
    {
        EventHandler.SkipThisUnit -= OnSkipThisUnit; // Skip this unit
        EventHandler.TeachUnitFeedback -= OnTeachUnitFeedback;
        EventHandler.PlayerCrossing -= OnPlayerCrossing;
        EventHandler.TeachNextUnit -= OnTeachNextUnit; 
    }

    private void OnSkipThisUnit()
    {
        isPlayerJumpTeach = true;
    }

    private void OnTeachNextUnit()
    {
        if(currentTeachUnitName != "Unit0")
            hintUIText.gameObject.SetActive(false);
    }

    private void OnPlayerCrossing(Vector3 obj)
    {
        if(currentTeachUnitName == "Unit6")
            EventHandler.CallTeachUnitFeedback(1);
    }

    private void OnTeachUnitFeedback(int amount)
    {
        currentUnitGetFeedback += amount;
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
            currentTeachUnitName = teachDetails.unitName;
            teachDetails.teachUnitObject.SetActive(true);
            EventHandler.CallTeachNextUnit();

            // Teach Voice
            AudioManager.Instance.PlayVoiceAudio(teachDetails.textAudio);
            
            // Teach UI Text Animation
            teachUIText.text = "";
            string text = teachDetails.teachText;
            WaitForSeconds textAnimationDuration = new WaitForSeconds(3f / text.Length);
            
            foreach (char c in text)
            {
                teachUIText.text += c;
                yield return textAnimationDuration;
            }
            
            // Check this Unit need player stand the square to skip
            if (teachDetails.isAutoSkip)
            {
                yield return new WaitUntil(() => isPlayerJumpTeach);
            }
            else
            {
                yield return new WaitUntil(() => currentUnitGetFeedback >= teachDetails.thisUnitNeedFeedback);
            }
            // isPlayerJumpTeach = true;

            // yield return new WaitUntil(() => isPlayerJumpTeach);
            teachDetails.teachUnitObject.SetActive(false);
        }
    }
}
