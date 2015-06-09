﻿using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {

	Vector3 initialMousePosition;
	bool isPanning;

	public Editor edit;

	void Update () {
		// Do the panning of the camera first
		if (isPanning) {
			PanCamera ();
		}
		// Now determine wheter the RMB is up or down
		if (Input.GetMouseButtonDown (1)) {
			// The right mouse button is down, user might start dragging next frame, pan now
			initialMousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			isPanning = true;
		} else if (Input.GetMouseButtonUp (1)) {
			isPanning = false;
		}

		if (!edit.HasSelectedAsset () && Input.GetAxis ("Mouse ScrollWheel") < 0) {
			// zoom camera out if no asset is selected
			Camera.main.orthographicSize++;
		} else if (!edit.HasSelectedAsset () && Input.GetAxis ("Mouse ScrollWheel") > 0) {
			// zoom camera in if no asset is selected
			Camera.main.orthographicSize--;
		}
	}

	void PanCamera () {
		// Get the current position of the mouse
		Vector3 newMousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		// Get the difference between current and last position
		Vector3 pan = newMousePosition - initialMousePosition;
		pan.z = 0;	// Just make sure not to move in the z-axis
		// We found how much the mouse moved in space, move camera equally in 
		// the opposite direction, and reset the initialMousePosition vector with
		// the new position
		Vector3 tmp = transform.position;
		tmp -= pan;
		transform.position = tmp;
		initialMousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

	}
}
