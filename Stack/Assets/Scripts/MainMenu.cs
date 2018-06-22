using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public Text scoreText;

	private void Start() {
		scoreText.text = PlayerPrefs.GetInt ("score").ToString();			// getting the score
	}

	public void ToGame() {
		SceneManager.LoadScene ("Game");						// load the scene "GAME"
	}		
}
