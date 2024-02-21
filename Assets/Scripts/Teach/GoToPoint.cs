using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GoToPoint : MonoBehaviour
{
   [Header("Component")]
   public SpriteRenderer OutlintSpriteRenderer;
   public SpriteRenderer standHintsSpriteRenderer;

   [Header("Setting")] 
   public int teachUnitFeedbackCount = 1;
   public float standTime = 1f;

   private bool isCall = false;
   private bool isBackScene;
   private bool isPlayerStand = false;
   private Color initialOutLineColor;

   private void Awake()
   {
      standHintsSpriteRenderer.transform.localScale = Vector3.zero;
      initialOutLineColor = OutlintSpriteRenderer.color;
   }

   private void OnEnable()
   {
      EventHandler.TeachNextUnit += OnTeachNextUnit; // Initial
   }

   private void OnDisable()
   {
      EventHandler.TeachNextUnit -= OnTeachNextUnit;
      
   }

   private void OnTeachNextUnit()
   {
      isCall = false;
      standHintsSpriteRenderer.transform.localScale = Vector3.zero;
      OutlintSpriteRenderer.color = initialOutLineColor;
      standHintsSpriteRenderer.color = initialOutLineColor;
      
      if(TeachManager.Instance.currentTeachUnitName == "Unit10")
         isBackScene = true;
   }

   private void OnTriggerStay2D(Collider2D other)
   {
      if (!other.CompareTag("Player")) return;
      
      if (standHintsSpriteRenderer.transform.localScale.x >= 1.4f)
      {
         if (isCall) return; 
         
         StandDone();
         OutlintSpriteRenderer.DOColor(Color.green, 0.5f);
         standHintsSpriteRenderer.DOColor(Color.green, 0.5f);
         isCall = true;
      }
      else
      {
         OutlintSpriteRenderer.color = initialOutLineColor;
         standHintsSpriteRenderer.color = initialOutLineColor;
         standHintsSpriteRenderer.transform.localScale += 
            new Vector3(standTime * Time.deltaTime,
               standTime * Time.deltaTime, 
               standTime * Time.deltaTime);
      }
   }

   private void OnTriggerExit2D(Collider2D other)
   {
      if (!isCall)
      {
         standHintsSpriteRenderer.DOColor(Color.clear, 0.3f).OnComplete(() =>
         {
            standHintsSpriteRenderer.transform.localScale = Vector3.zero;
         });
      }
   }

   protected virtual void StandDone()
   {
      if (isBackScene)
      {
         SceneLoadManager.Instance.ChangeScene(SceneLoadManager.Instance.mainMenuSceneName);
         return;
      }
      
      EventHandler.CallTeachUnitFeedback(teachUnitFeedbackCount);
      EventHandler.CallSkipThisUnit();
   }
}
