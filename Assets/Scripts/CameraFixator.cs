using UnityEngine;
using DG.Tweening;

public class CameraFixator : MonoBehaviour
{
   public GameObject CameraPosition;
   public float FOV;
   public float AnimationTime;
   public GameObject Collisions;
   public GameObject CollisionsMesh;
   public bool animateCollision;
   public Vector3 collisionEndPos;
   CameraAi mainCamera;

   private void OnEnable() {
      Collisions.SetActive(false);
      mainCamera = Camera.main.GetComponent<CameraAi>();
   }

   private void OnTriggerEnter(Collider other) {
      if(other.gameObject.tag == "Player") {
         mainCamera.gameObject.GetComponent<CameraAi>().SwitchToCinematicCamera(FOV, AnimationTime, CameraPosition.transform);
         Collisions.SetActive(true);
         if(animateCollision) {
            CollisionsMesh.transform.DOLocalMove(collisionEndPos, 1f);
         }
      }
   }
   private void OnTriggerExit(Collider other) {
      if(other.gameObject.tag == "Player") {
         Collisions.SetActive(false);
         mainCamera.gameObject.GetComponent<CameraAi>().ReleaseCinematicCamera();
      }
   }
}
