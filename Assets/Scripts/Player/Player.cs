using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Assets.Scripts.Enums;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class Player: MonoBehaviour
{
   //HP
   public static int Lives;
   public static int HP;
   public HpBarWrapper HpBar;
   public GameObject SucessScreen;
   CharacterControl characterControl;
   CameraAi cameraAi;
   Animator animator;
   public GameObject PlayerModel;
   public Transform PlayerSpawn;

   public static int Score;
   public static int Keys;
   public Text ScoreText;
   public GameObject GameOverScreen;
   public Button ReplayButton;
   public static DialogueMap[] DialoguesEnabled;

   public float Volume;
   private AudioSource source;

   public GameObject LoadScreen;
   [SerializeField]
   private Canvas ShieldChargeCanvas;
   [SerializeField]
   private Image ShieldChargePercentage;
   public float ChargePercentage;
   public float HoldingShieldFor;

   //Parry
   public int ParryCounter;
   public Text parryText;
   public bool parrying;
   public FinalBossAi FinalBoss;

   private Coroutine DamageTickCoroutine = null;
   private bool DamageShowCoroutine = false;
   private Dictionary<EDamageOverTimeType, DamageModifier> DamageModifierList;


   private bool Invincible;

   private void Start() {
      ParryCounter = 3;
      parryText.text = ParryCounter.ToString();
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
      FinalBoss = GameObject.FindObjectOfType<FinalBossAi>();
      DamageModifierList = new Dictionary<EDamageOverTimeType, DamageModifier>();
      cameraAi = Camera.main.GetComponent<CameraAi>();
      animator = GetComponent<Animator>();
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
      if(Invincible)
         return;

      HP -= amount;
      if(HP <= 0) {
         HP = 0;
         Die(false);
      }
      HpBar.UpdateHp(HP);
      if(!DamageShowCoroutine)
         StartCoroutine(ShowDamageIndication());
      cameraAi.transform.DOShakePosition(0.2f, 0.2f, 100);
      animator.SetTrigger("Hit");
   }
   public void MakeInvincible(int seconds) {
      if(!Invincible)
         StartCoroutine(CountDownInvincible(seconds));
   }
   private IEnumerator CountDownInvincible(int seconds) {
      Invincible = true;
      yield return new WaitForSeconds(seconds);
      Invincible = false;
   }
   private IEnumerator LoadReturn(int offset) {
      yield return new WaitForSeconds(offset);
      LoadScreen.SetActive(true);
      MakeInvincible(1);
      GetComponent<CharacterControl>().enabled = false;
      transform.position = PlayerSpawn.position;
      yield return new WaitForSeconds(1);
      GetComponent<CharacterControl>().enabled = true;
      LoadScreen.SetActive(false);
   }
   public void ReturnToSpawn(int offset = 0) {
      StartCoroutine(LoadReturn(offset));
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
         animator.SetTrigger("Attacking");
         StartCoroutine(RestartParry());
         RemoveParry();
         if(Vector3.Distance(FinalBoss.gameObject.transform.position, gameObject.transform.position) < 2f && FinalBoss.vunerable) {
            FinalBoss.TakeDamage(1);
            FinalBoss.playerParry = true;
            cameraAi.PlayEffects();
         }
      }
   }
   private IEnumerator RestartParry() {
      yield return new WaitForSeconds(1f);
      parrying = false;
      animator.ResetTrigger("Attacking");
   }

   private IEnumerator ShowDamageIndication() {
      DamageShowCoroutine = true;
      PlayerModel.GetComponent<MeshRenderer>().material.color = new Color(255f / 255f, 100f / 255f, 100f / 255f);
      yield return new WaitForSeconds(0.2f);
      PlayerModel.GetComponent<MeshRenderer>().material.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
      DamageShowCoroutine = false;
      yield return new WaitForSeconds(0.2f);
      animator.ResetTrigger("Hit");
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