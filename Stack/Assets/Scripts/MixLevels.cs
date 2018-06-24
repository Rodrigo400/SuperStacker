using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixLevels : MonoBehaviour {

	public AudioMixer masterMixer;

	public void SetMasterLvl(float MasterVol) {
		masterMixer.SetFloat ("MasterVol", MasterVol);
	}
}
