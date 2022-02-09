using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarWrapper: MonoBehaviour
{
   public Color FullColor, EmptyColor;
   public Sprite HpBar;
   public Image[] HpBars;


   public void UpdateHp(int hp) {
      foreach(var item in HpBars) {
         item.sprite = HpBar;
         item.color = EmptyColor;
      }
      for(int i = 0; i < hp; i++) {
         HpBars[i].color = FullColor;
      }
   }
}
