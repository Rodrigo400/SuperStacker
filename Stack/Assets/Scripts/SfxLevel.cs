using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SfxLevel : MonoBehaviour {

	public AudioMixer sfxMixer;

	public void MuteSfxLvl(float SFXVol) {
		sfxMixer.SetFloat ("SFXVol", -80);
	}

	public void AliveSfxLvl(float SFXVol) {
		sfxMixer.SetFloat ("SFXVol", 10);
	}
}
