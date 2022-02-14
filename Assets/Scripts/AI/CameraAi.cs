using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CameraAi: MonoBehaviour
{
   Player player;
   CameraFollowPlayer cfp;
   Camera self;

   private void OnEnable() {
      player = FindObjectOfType<Player>();
      cfp = GetComponent<CameraFollowPlayer>();
      self = Camera.main;
   }

   public void SwitchToCinematicCamera(float fov, float time, Transform transform) {
      cfp.enabled = false;
      StartCoroutine(AnimateFov(time, fov));
      self.transform.DOMove(transform.position, time);
      self.transform.DORotate(transform.rotation.eulerAngles, time);
   }

   IEnumerator AnimateFov(float time, float fov) {
      float seconds = 0f;
      float startFov = self.fieldOfView;
      while(seconds <= time) {
         self.fieldOfView = Mathf.Lerp(startFov, fov, seconds / time);
         seconds += Time.deltaTime;
         yield return new WaitForEndOfFrame();
      }
   }
   public void ReleaseCinematicCamera() {
      cfp.enabled = true;
      StartCoroutine(AnimateFov(0.5f, 60));
   }
}
