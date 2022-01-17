using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaptopController: MonoBehaviour
{
   [SerializeField]
   [Range(0.1f,1f)]
   float LaptopFlySpeed;
   Vector3 StartTransform;
   Player player;
   public int HP;


   [SerializeField]
   GameObject LaptopBar;
   [SerializeField]
   Image[] BarSlots;

   public int currentChargingIndex;
   public float curretnChargingAmount;

   Coroutine shieldCharge;
   Coroutine laptopFlyingCoroutine;

   private void Start() {
      player = FindObjectOfType<Player>();
      BarSlots = LaptopBar.GetComponentsInChildren<Image>();
      HP = currentChargingIndex = 5;
   }

   public void LaunchSelf(float charge, bool right) {
      if(laptopFlyingCoroutine != null)
         StopCoroutine(laptopFlyingCoroutine);
      player.HasShield = false;
      laptopFlyingCoroutine = StartCoroutine(LaunchAndReturn(charge / 2f, right));
   }
   public void ReturnEarly() {
      if(laptopFlyingCoroutine != null)
         StopCoroutine(laptopFlyingCoroutine);
      StartCoroutine(Return());
   }
   public IEnumerator LaunchAndReturn(float charge, bool right) {
      transform.SetParent(null);
      float seconds = 0f;
      float direction = right ? LaptopFlySpeed : -LaptopFlySpeed;
      while(seconds < charge) {
         gameObject.transform.position = new Vector3(transform.position.x + direction, transform.position.y, transform.position.z);
         seconds += Time.deltaTime;
         yield return new WaitForEndOfFrame();
      }
      StartCoroutine(Return());
   }
   private IEnumerator Return() {
      float seconds = 0;
      Vector3 currentPos = transform.position;
      float timeToReturn = 0.5f;
      while(seconds < timeToReturn) {
         gameObject.transform.position = Vector3.Lerp(currentPos, player.transform.position, seconds / timeToReturn);
         seconds += Time.deltaTime;
         yield return new WaitForEndOfFrame();
      }
      transform.SetParent(player.transform);
      gameObject.transform.position = player.transform.position;
      player.HasShield = true;
   }
   
   public bool DamageShield() {
      if(HP <= 0)
         return false;
      HP--;
      if(shieldCharge != null)
         StopCoroutine(shieldCharge);
      shieldCharge = StartCoroutine(ChargeShield());
      return true;
   }

   private void UpdateShieldImages() {
      for(int i = 1; i < 6; i++) {
         if(i <= HP)
            BarSlots[i].fillAmount = 1f;
         else
            BarSlots[i].fillAmount = 0f;
      }
      if(HP < 5)
         BarSlots[currentChargingIndex].fillAmount = curretnChargingAmount;
   }

   private void RequeueCharge() {
      if(HP < 5) {
         if(shieldCharge != null)
            StopCoroutine(shieldCharge);
         shieldCharge = StartCoroutine(ChargeShield());
      }
   }

   private IEnumerator ChargeShield() {
      currentChargingIndex = HP + 1;
      while(curretnChargingAmount < 1) {
         curretnChargingAmount += Time.deltaTime / 5f;
         UpdateShieldImages();
         yield return new WaitForEndOfFrame();
      }
      curretnChargingAmount = 0f;
      HP++;
      UpdateShieldImages();
      RequeueCharge();
   }
}
