using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RateButton : MonoBehaviour {

	private string ANDROID_RATE_URL = "market://details?id=com.RGN.GAMES.Super.Stacker";

	public void RateFunction() {
		Application.OpenURL(ANDROID_RATE_URL);
	}
}
