using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
   public static DontDestroy self;
   // Start is called before the first frame update
   void Awake()
   {
      if (self == null)
      {
         self = this;
         DontDestroyOnLoad(gameObject);
      }
      else
         Destroy(gameObject);
   }
}
