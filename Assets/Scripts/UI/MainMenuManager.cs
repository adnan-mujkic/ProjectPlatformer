using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager: MonoBehaviour
{
   public Button PlayButton;


   // Start is called before the first frame update
   void Start() {
      PlayButton.Select();
      EventSystem.current.SetSelectedGameObject(null);
      EventSystem.current.SetSelectedGameObject(PlayButton.gameObject);
   }

   public void ExitGame() {
      Application.Quit();
   }
   public void LoadLevel() {
      Player.HP = 10;
      Player.Lives = 3;
      Player.Score = 0;
      Player.Keys = 0;
      Player.DialoguesEnabled = null;
      CharacterControl.SprintEnabled = false;
      SceneManager.LoadScene(2);
   }
}
