using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
   [Range(0f,10f)]
   public float SmoothSpeed;
   public Vector3 Offset;
   GameObject Player;

   // Start is called before the first frame update
   void Start()
   {
      Player = FindObjectOfType<Player>().gameObject;
   }

   // Update is called once per frame
   void LateUpdate()
   {
      Vector3 DesiredPos = new Vector3(Player.transform.position.x, Player.transform.position.y, -10f) + Offset;
      Vector3 SmoothPos = Vector3.Lerp(transform.position, DesiredPos, Time.deltaTime * SmoothSpeed);
      transform.position = SmoothPos;
   }
}
