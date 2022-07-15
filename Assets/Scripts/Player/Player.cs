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


   public GameObject LoadScreen;
   public GameObject MainMenuUI;
   public GameObject DieScreen;
   public GameObject WinScreen;

   //Parry
   public int ParryCounter;
   public Text parryText;
   public Image parryImage;
   public bool parrying;
   public float freeParryStatus;
   public FinalBossAi FinalBoss;

   private Coroutine DamageTickCoroutine = null;
   private bool DamageShowCoroutine = false;
   private Dictionary<EDamageOverTimeType, DamageModifier> DamageModifierList;

   private bool startMenu = true;
   private bool Invincible;
   private bool Dead = false;

   private AudioManager am;

   private void Start() {
      ParryCounter = 3;
      parryText.text = ParryCounter.ToString();
      characterControl = GetComponent<CharacterControl>();
      if(HP == 0) {
         HP = 10;
         if(HpBar != null)
            HpBar.UpdateHp(HP);
      }
      FinalBoss = GameObject.FindObjectOfType<FinalBossAi>();
      DamageModifierList = new Dictionary<EDamageOverTimeType, DamageModifier>();
      cameraAi = Camera.main.GetComponent<CameraAi>();
      animator = GetComponent<Animator>();
      MainMenuUI.SetActive(true);
      DieScreen.SetActive(false);
      startMenu = true;
      GetComponent<CharacterControl>().enabled = false;
      am = cameraAi.GetComponent<AudioManager>();
   }

   // Update is called once per frame
   void Update() {
      if(Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)) {
         Parry();

      }
      if(Input.GetKeyDown(KeyCode.Space) && startMenu) {
         startMenu = false;
         MainMenuUI.SetActive(false);
         GetComponent<CharacterControl>().enabled = true;
         am.PlayMusic(am.MusicToPlay, true, true, am.TopMusicVolume);
         return;
      }
      if(Input.GetKeyDown(KeyCode.Space) && Dead) {
         SceneManager.LoadScene(0);
      }

      if(Input.GetKey(KeyCode.Escape)) {
         Application.Quit();
      }

      if(freeParryStatus <= 1) {
         freeParryStatus += Time.deltaTime / 10f;
         parryImage.fillAmount = freeParryStatus;
      }
   }

   private void OnTriggerEnter(Collider other) {
      if(other.tag == "DeathZone") {
         Die(true);
      }
   }

   public void Die(bool falling) {
      GetComponent<CharacterControl>().enabled = false;
      Camera.main.GetComponent<CameraFollowPlayer>().enabled = false;
      animator.SetTrigger("Die");
      if(falling) {
         StartCoroutine(WaitAndDisablePhysics());
      } else {
         if(!UpdateLives()) {
            DieScreen.SetActive(true);
            Dead = true;
            FindObjectOfType<FinalBossAi>().DeactivateAi();
         }
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
      StartCoroutine(cameraAi.ShakeCam(0.15f, 0.1f));
      animator.SetTrigger("Hit");
   }
   public void AddHp(int amount) {
      HP += amount;
      if(HP > 10)
         HP = 10;
      HpBar.UpdateHp(HP);
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
      GetComponent<Rigidbody>().velocity = Vector3.zero;
      transform.position = PlayerSpawn.position;
      yield return new WaitForSeconds(1);
      GetComponent<CharacterControl>().enabled = true;
      LoadScreen.SetActive(false);

      var finalBoss = GameObject.FindObjectOfType<FinalBossAi>();
      if(finalBoss == null) {
         SucessScreen.SetActive(true);
         GetComponent<CharacterControl>().enabled = false;
      }
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
      if(!parrying) {
         if(freeParryStatus >= 0.99f) {
            freeParryStatus = 0;
         } else if(ParryCounter > 0) {
            RemoveParry();
         } else {
            return;
         }
         parrying = true;
         animator.SetTrigger("Attacking");
         StartCoroutine(RestartParry());
         if(Vector3.Distance(FinalBoss.gameObject.transform.position, gameObject.transform.position) < 2f && FinalBoss.vunerable) {
            FinalBoss.TakeDamage(1);
            FinalBoss.playerParry = true;
            cameraAi.PlayEffects();
            am.PlaySfx(am.HitSfx);
         } else {
            am.PlaySfx(am.SwooshSfx);
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