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
   public float standTime = 1f;

   private bool isCall = false;
   private bool isPlayerStand = false;

   private void Awake()
   {
      standHintsSpriteRenderer.transform.localScale = Vector3.zero;
   }

   private void Update()
   {
      ScaleAnimation();
   }

   private void OnTriggerStay2D(Collider2D other)
   {
      if (!other.CompareTag("Player")) return;
      isPlayerStand = true;

   }

   private void OnTriggerExit2D(Collider2D other)
   {
      if (!isCall)
      {
         standHintsSpriteRenderer.transform.localScale = Vector3.zero;
         isPlayerStand = false;
      }
   }

   private void ScaleAnimation()
   {
      if(!isPlayerStand) return;
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
         standHintsSpriteRenderer.transform.localScale += 
            new Vector3(standTime * Time.deltaTime,
               standTime * Time.deltaTime, 
               standTime * Time.deltaTime);
      }
   }

   protected virtual void StandDone()
   {
      EventHandler.CallTeachUnitFeedback();
   }
}
