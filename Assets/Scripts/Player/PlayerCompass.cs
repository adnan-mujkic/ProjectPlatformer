using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCompass: MonoBehaviour
{
   Player player;
   FinalBossAi finalBoss;
   bool opened;
   public Image compassPointer;
   public GameObject compasCanvas;

   // Start is called before the first frame update
   void Start() {
      player = GetComponent<Player>();
      finalBoss = FindObjectOfType<FinalBossAi>();
   }

   // Update is called once per frame
   void Update() {
      if(Input.GetKeyDown(KeyCode.C)) {
         opened = !opened;
         compasCanvas.SetActive(opened);
      }
      if(player != null && finalBoss != null && opened) {
         float angle = CalculateAngle(player.transform.position, finalBoss.transform.position);
         compassPointer.transform.rotation = Quaternion.Euler(0, 0, angle);
      }
   }
   public static float CalculateAngle(Vector3 from, Vector3 to) {
      return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
   }
}
