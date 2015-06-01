using UnityEngine;
using System.Collections;

public class InstructionsRemover : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		// Remove the instructions when the mouse is pressed or scrolled
		if (Input.GetMouseButton (0) || Input.GetMouseButton (1) ||
			Input.GetAxis ("Mouse ScrollWheel") != 0) {
			gameObject.SetActive (false);
			// Also do the same for all children
			foreach (Transform child in transform) {
				child.gameObject.SetActive (false);
			}
		}
	}
}
