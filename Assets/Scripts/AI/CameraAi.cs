using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CameraAi: MonoBehaviour
{
   public ParticleSystem screenWaveEffect;
   Player player;
   CameraFollowPlayer cfp;
   Camera self;
   bool playingEffect = false;

   private void OnEnable() {
      player = FindObjectOfType<Player>();
      cfp = GetComponent<CameraFollowPlayer>();
      self = Camera.main;
      screenWaveEffect.gameObject.SetActive(false);
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

   public void PlayEffects() {
      if(playingEffect)
         return;
      StartCoroutine(DisplayEffectsAnimation());
   }
   public IEnumerator DisplayEffectsAnimation() {
      playingEffect = true;
      screenWaveEffect.gameObject.SetActive(true);
      screenWaveEffect.Play();
      yield return new WaitForSeconds(0.3f);
      screenWaveEffect.gameObject.SetActive(false);
      playingEffect = false;
   }

   public IEnumerator ShakeCam(float duration, float strength) {
      float elapsed = 0f;
      while(elapsed < duration) {
         yield return new WaitForEndOfFrame();
         float randX = UnityEngine.Random.Range(-1.0f, 1.0f) * strength;
         float randY = UnityEngine.Random.Range(-1.0f, 1.0f) * strength;

         transform.position = new Vector3(transform.position.x + randX, transform.position.y + randY, transform.position.z);
         elapsed += Time.deltaTime;
      }
   }
}
