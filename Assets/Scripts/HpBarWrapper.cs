using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarWrapper : MonoBehaviour
{
   public Sprite HpBar;
   public Sprite HpBarEmpty;
   public Image[] HpBars;


    public void UpdateHp(int hp){
       foreach (var item in HpBars)
       {
           item.sprite = HpBarEmpty;
       }
       for (int i = 0; i < hp; i++)
       {
           HpBars[i].sprite = HpBar;
       }
    }
}
