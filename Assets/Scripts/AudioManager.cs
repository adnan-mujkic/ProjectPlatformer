using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
   public AudioClip IntroMusic;
   public AudioClip MusicToPlay;
   public AudioClip BossMusic;
   public AudioClip SwooshSfx;
   public AudioClip HitSfx;
   public static AudioManager AM;
   public AudioSource SfxSource;
   public AudioSource MusicSource;
   public float TopMusicVolume;

   private void Awake() {
      if(AM == null) {
         AM = this;
      } else {
         Destroy(this.gameObject);
      }
      PlayMusic(IntroMusic, false, false, 1f);
   }

   public void StopMusic() {
      MusicSource.Stop();
   }
   public void PlayMusic(AudioClip clip, bool smooth, bool loop, float volume) {
      MusicSource.volume = smooth? 0 : volume;
      MusicSource.clip = clip;
      MusicSource.loop = loop;
      MusicSource.Play();
      StopAllCoroutines();
      if(smooth)
         StartCoroutine(SmoothOutMusic());
   }

   public void PlaySfx(AudioClip clip) {
      SfxSource.clip = clip;
      SfxSource.volume = 0.3f;
      SfxSource.loop = false;
      SfxSource.Play();
   }

   public void LowerVolume() {
      StartCoroutine(VolumeAnimation(0.1f, 0.02f));
   }

   public void HighVolume() {
      StartCoroutine(VolumeAnimation(0.05f, 0.1f));
   }

   private IEnumerator SmoothOutMusic() {
      float seconds = 0f;
      MusicSource.volume = 0f;
      while(seconds < 2f) {
         seconds += Time.deltaTime;
         MusicSource.volume = Mathf.Lerp(0f, TopMusicVolume, seconds / 2f);
         yield return new WaitForEndOfFrame();
      }
      MusicSource.volume = TopMusicVolume;
   }
   private IEnumerator VolumeAnimation(float from, float to) {
      float seconds = 0f;
      MusicSource.volume = 0f;
      MusicSource.Play();
      while(seconds < 2f) {
         seconds += Time.deltaTime;
         MusicSource.volume = Mathf.Lerp(from, to, seconds / 2f);
         yield return new WaitForEndOfFrame();
      }
      MusicSource.volume = to;
   }
}
