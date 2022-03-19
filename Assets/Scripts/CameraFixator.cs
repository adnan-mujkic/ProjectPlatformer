using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CameraFixator : MonoBehaviour
{
   public bool CanActivateOnlyOnce;
   public GameObject CameraPosition;
   public float FOV;
   public float AnimationTime;
   public GameObject Collisions;
   public GameObject CollisionsMesh;
   public bool animateCollision;
   public Vector3 collisionEndPos;
   [Header("Boss")]
   public bool TriggersBoss;
   public float BossTriggerOffset;
   CameraAi mainCamera;
   private int activationCount = 0;

   private void OnEnable() {
      Collisions.SetActive(false);
      mainCamera = Camera.main.GetComponent<CameraAi>();
   }

   private void OnTriggerEnter(Collider other) {
      if(activationCount > 0 && CanActivateOnlyOnce)
         return;

      if(other.gameObject.tag == "Player") {
         mainCamera.gameObject.GetComponent<CameraAi>().SwitchToCinematicCamera(FOV, AnimationTime, CameraPosition.transform);
         Collisions.SetActive(true);
         if(animateCollision) {
            CollisionsMesh.transform.DOLocalMove(collisionEndPos, 1f);
         }
         if(TriggersBoss) {
            StartCoroutine(StartBossTrigger());
         }
         activationCount++;
      }
   }

   private void OnTriggerExit(Collider other) {
      if(other.gameObject.tag == "Player") {
         Collisions.SetActive(false);
         mainCamera.gameObject.GetComponent<CameraAi>().ReleaseCinematicCamera();
      }
   }

   private IEnumerator StartBossTrigger() {
      yield return new WaitForSeconds(BossTriggerOffset);
      FindObjectOfType<FinalBossAi>().ActivateAi();
   }
}
