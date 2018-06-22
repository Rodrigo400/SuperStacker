using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveRubble : MonoBehaviour {

	// delete rubble when hitting plane
	private void OnCollisionEnter(Collision col) {
		Destroy (col.gameObject);
	}
}
