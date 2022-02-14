using UnityEngine;

public class CameraFixator : MonoBehaviour
{
   public GameObject CameraPosition;
   public float FOV;
   public float AnimationTime;
   public GameObject Collisions;
   CameraAi mainCamera;

   private void OnEnable() {
      Collisions.SetActive(false);
      mainCamera = Camera.main.GetComponent<CameraAi>();
   }

   private void OnTriggerEnter(Collider other) {
      if(other.gameObject.tag == "Player") {
         Collisions.SetActive(true);
         mainCamera.gameObject.GetComponent<CameraAi>().SwitchToCinematicCamera(FOV, AnimationTime, CameraPosition.transform);
      }
   }
   private void OnTriggerExit(Collider other) {
      if(other.gameObject.tag == "Player") {
         Collisions.SetActive(false);
         mainCamera.gameObject.GetComponent<CameraAi>().ReleaseCinematicCamera();
      }
   }
}
