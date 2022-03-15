using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Assets.Scripts.Enums;

public class Player: MonoBehaviour
{
   //HP
   public static int Lives;
   public static int HP;
   public HpBarWrapper HpBar;
   public GameObject SucessScreen;
   public bool Shielding;
   CharacterControl characterControl;
   public GameObject Laptop;
   public GameObject PlayerModel;

   public static int Score;
   public static int Keys;
   public Text ScoreText;
   public GameObject GameOverScreen;
   public Button ReplayButton;
   public static DialogueMap[] DialoguesEnabled;

   public float Volume;
   private AudioSource source;
   public bool HasShield;

   [SerializeField]
   private Canvas ShieldChargeCanvas;
   [SerializeField]
   private Image ShieldChargePercentage;
   public float ChargePercentage;
   public float HoldingShieldFor;

   //Parry
   public int ParryCounter;
   public Text parryText;
   public GameObject ParryModel;
   public bool parrying;

   private Coroutine DamageTickCoroutine = null;
   private Dictionary<EDamageOverTimeType, DamageModifier> DamageModifierList;

   private void Start() {
      ParryModel.SetActive(false);
      ParryCounter = 0;
      parryText.text = ParryCounter.ToString();
      HasShield = true;
      source = FindObjectOfType<AudioSource>();
      characterControl = GetComponent<CharacterControl>();
      if(HP == 0) {
         HP = 10;
         if(HpBar != null)
            HpBar.UpdateHp(HP);
      }
      if(DialoguesEnabled == null) {
         var allDialogues = FindObjectsOfType<DialogueTrigger>();
         DialoguesEnabled = new DialogueMap[allDialogues.Length];
         for(int i = 0; i < allDialogues.Length; i++) {
            DialoguesEnabled[i].index = allDialogues[i].index;
            DialoguesEnabled[i].enabled = true;
         }
      }

      DamageModifierList = new Dictionary<EDamageOverTimeType, DamageModifier>();
   }

   // Update is called once per frame
   void Update() {
      if(Input.GetMouseButtonDown(0)) {
         Parry();
      }
      if(Input.GetKey(KeyCode.Escape))
         Application.Quit();
   }

   private void OnTriggerEnter(Collider other) {
      if(other.tag == "DeathZone") {
         Die(true);
      }
   }

   public void Die(bool falling) {
      GetComponent<CharacterControl>().enabled = false;
      Camera.main.GetComponent<CameraFollowPlayer>().enabled = false;
      if(falling) {
         StartCoroutine(WaitAndDisablePhysics());
      } else {
         if(!UpdateLives())
            SceneManager.LoadScene(0);
      }

   }
   IEnumerator WaitAndDisablePhysics() {
      yield return new WaitForSeconds(0.5f);
      GetComponent<Rigidbody2D>().simulated = false;
      if(!UpdateLives())
         SceneManager.LoadScene(2);
   }
   public void AddScore(int score) {
      Score += score;
      ScoreText.text = "Score: " + Score.ToString();
   }
   public void ReloadGame() {
      SceneManager.LoadScene(0);
   }
   public bool UpdateLives() {
      Lives--;
      if(Lives == 0) {
         GameOverScreen.SetActive(true);
         ReplayButton.Select();
         EventSystem.current.SetSelectedGameObject(null);
         EventSystem.current.SetSelectedGameObject(ReplayButton.gameObject);
         return true;
      }
      return false;
   }
   public void AddKey() {
      Keys++;
      if(Keys == 2) {
         SucessScreen.SetActive(true);
         GetComponent<BoxCollider2D>().enabled = false;
         GetComponent<Rigidbody2D>().simulated = false;
      }
   }
   public void SwitchVolume(float vol) {
      Volume = vol;
      source.volume = vol;
   }
   public void TakeDamage(int amount) {
      HP-=amount;
      if(HP <= 0) {
         HP = 0;
         Die(false);
      }
      HpBar.UpdateHp(HP);
   }
   public void AddParry() {
      ParryCounter++;
      parryText.text = ParryCounter.ToString();
   }
   public void RemoveParry() {
      ParryCounter--;
      parryText.text = ParryCounter.ToString();
   }
   public void Parry() {
      if(ParryCounter > 0 && !parrying) {
         parrying = true;
         ParryModel.SetActive(true);
         ParryModel.GetComponent<Animator>().Play("SwordSlashAnimation");
         StartCoroutine(RestartParry());
         RemoveParry();
      }
   }
   private IEnumerator RestartParry() {
      yield return new WaitForSeconds(0.5f);
      parrying = false;
      ParryModel.SetActive(false);
   }
   private IEnumerator TickDamageModifier() {
      while(DamageModifierList.Count > 0) {
         var keys = DamageModifierList.Keys.ToList();
         var keysToRemove = new List<EDamageOverTimeType>();
         for(int i = 0; i < keys.Count; i++) {
            var key = keys[i];
               if(DamageModifierList[key].TicksLeft > 0) {
                  DamageModifierList[key].TicksLeft--;
                  //TakeDamage(DamageModifierList[key].DamagePerSecond);
               } else {
                  keysToRemove.Add(key);
               }
         }
         foreach(var key in keysToRemove) {
            DamageModifierList.Remove(key);
         }
         yield return new WaitForSeconds(1f);
      }
   }
   public void StartDamageTick(DamageModifier modifier, EDamageOverTimeType damageType) {
      if(DamageModifierList.ContainsKey(damageType))
         DamageModifierList[damageType] = modifier;
      else
         DamageModifierList.Add(damageType, modifier);

      if(DamageTickCoroutine == null)
         DamageTickCoroutine = StartCoroutine(TickDamageModifier());
   }
}

   public struct DialogueMap
   {
      public int index;
      public bool enabled;
   }