using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CameraFixator : MonoBehaviour
{
   public bool CanActivateOnlyOnce;
   public GameObject Collisions;
   public GameObject CollisionsMesh;
   public bool animateCollision;
   public Vector3 collisionEndPos;
   public Vector3 offset;
   public float CameraFollowSpeedOffset;
   [Header("Boss")]
   public bool TriggersBoss;
   public float BossTriggerOffset;
   public int BossArenaIndex;

   CameraAi mainCamera;
   int activationCount = 0;
   Vector3 defaultOffset;
   float defaultCameraSpeedOffset;
   Vector3 collisionStartPos;

   private void OnEnable() {
      if(Collisions != null)
         Collisions.SetActive(false);
      mainCamera = Camera.main.GetComponent<CameraAi>();
      defaultOffset = mainCamera.gameObject.GetComponent<CameraFollowPlayer>().Offset;
      defaultCameraSpeedOffset = mainCamera.gameObject.GetComponent<CameraFollowPlayer>().SmoothSpeed;
      if(CollisionsMesh != null)
         collisionStartPos = CollisionsMesh.transform.localPosition;
   }

   private void OnTriggerEnter(Collider other) {
      if(activationCount > 0 && CanActivateOnlyOnce)
         return;

      if(other.gameObject.tag == "Player") {
         var defaultOffset = mainCamera.gameObject.GetComponent<CameraFollowPlayer>().Offset;
         mainCamera.gameObject.GetComponent<CameraFollowPlayer>().Offset = defaultOffset + offset;
         mainCamera.gameObject.GetComponent<CameraFollowPlayer>().SmoothSpeed = defaultCameraSpeedOffset + CameraFollowSpeedOffset;
         ToggleCollision(false);
         if(TriggersBoss) {
            var am = mainCamera.GetComponent<AudioManager>();
            am.PlayMusic(am.BossMusic, true, true, am.TopMusicVolume);
            StartCoroutine(StartBossTrigger());
         }
         activationCount++;
      }
   }

   private void OnTriggerExit(Collider other) {
      if(other.gameObject.tag == "Player") {
         ToggleCollision(false);
         mainCamera.gameObject.GetComponent<CameraFollowPlayer>().Offset = defaultOffset;
         mainCamera.gameObject.GetComponent<CameraFollowPlayer>().SmoothSpeed = defaultCameraSpeedOffset;
      }
   }

   private IEnumerator StartBossTrigger() {
      yield return new WaitForSeconds(BossTriggerOffset);
      FindObjectOfType<FinalBossAi>().ActivateAi();
   }

   public void ToggleCollision(bool active) {
      if(Collisions == null)
         return;
      Collisions.SetActive(!active);
      if(animateCollision) {
         CollisionsMesh.transform.DOLocalMove(active? collisionStartPos : collisionEndPos, 1f);
      }
   }
}
