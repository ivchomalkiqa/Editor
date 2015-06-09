using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class LoadMenu : MonoBehaviour {

	// This will be the object which will contain the buttons for the available segments.
	// In other words, all generated buttons will be children of this transform.
	public RectTransform buttonContainer;

	// A prefab for the segment selection button
	public GameObject buttonPrefab;

	public InputField selectedName;

	// Shows the load menu window with all of the available segments listed
	public void Show () {
		// Show the menu
		gameObject.SetActive (true);
		// Get all available segments
		string segmentDirectory = SegmentFactory.GetDirectory ();
		string[] segmentNames = Directory.GetFiles (segmentDirectory, 
		                                            "*" + SegmentFactory.GetExtension ());
		for (int i = 0; i < segmentNames.Length; i++) {
			// Remove the directory path and the extension of every string
			// so that the only thing left is the name of the segment itself
			segmentNames[i] = segmentNames[i].Remove (0, segmentDirectory.Length);
			// Remove the extension from the filename
			segmentNames[i] = segmentNames[i].Remove (segmentNames[i].Length - 4);
		}
		// Now make a button for every segment available and add it to the parent
		MakeButtons (segmentNames);
		// Stop the editor from working
		Editor.stopMouseEvents = true;
	}

	void MakeButtons (string[] names) {
		float buttonHeight = buttonPrefab.GetComponent<RectTransform> ().sizeDelta.y;
		// Set the height of the container to match the height of all buttons
		buttonContainer.sizeDelta =  new Vector2 (buttonContainer.sizeDelta.x,
		                                            names.Length * buttonHeight);
		for (int i = 0; i < names.Length; i++) {
			GameObject buttonObj = Instantiate <GameObject> (buttonPrefab);
			// Set the name of the game object
			buttonObj.name = names[i];
			// Set the button's name
			Text buttonText = buttonObj.GetComponentInChildren<Text> ();
			buttonText.text = names[i];
			// Get a reference to the RectTransform of the button to set its position
			RectTransform rt =  buttonObj.GetComponent<RectTransform> ();
			// Set the parent of this button to be the assets panel
			rt.SetParent (buttonContainer, false);
			// Now set the position
			rt.anchoredPosition = rt.anchoredPosition - new Vector2(0, i * buttonHeight);
			// Finally, add an onClick listener to the button
			buttonObj.GetComponent<Button> ().onClick.AddListener (() => OnSegmentButtonClick (buttonObj));
		}
	}

	public void OnSegmentButtonClick (GameObject button) {
		// Set the text of the input field to the name of the selected segment
		selectedName.text = button.GetComponentInChildren<Text> ().text;
	}

	public void Load () {
		// If no name was selected, return
		if (selectedName.text.Length == 0) {
			return;
		}
		// First clear the current level by destroying all children of the editor
		GameObject editor = GameObject.FindGameObjectWithTag ("Editor");
		foreach (Transform t in editor.transform) {
			Destroy (t.gameObject);
		}
		editor.transform.DetachChildren ();
		// Now load the new level, indicated by the selected name
		SegmentFactory.Instance.Load (selectedName.text);
		// Finally, retrieve the new game objects and set them as children of the editor
		List<GameObject> level = SegmentFactory.Instance.GetGameObjects ();
		foreach (GameObject asset in level) {
			asset.transform.SetParent (editor.transform);
		}

		Hide ();
	}

	public void Delete () {
		// If no segment name selected, return
		if (selectedName.text.Length == 0) {
			return;
		}

		// Delete file with the specified name
		File.Delete (SegmentFactory.GetPath (selectedName.text));
		// Find the button corresponding to the segment name and disable it
		foreach (Transform t in buttonContainer.transform) {
			if (t.gameObject.name == selectedName.text) {
				t.GetComponent<Button> ().interactable = false;
			}
		}
	}

	public void Hide () {
		// Destroy all the created buttons
		foreach (RectTransform t in buttonContainer) {
			DestroyImmediate (t.gameObject);
		}
		buttonContainer.DetachChildren ();
		// We are done, close the load window
		gameObject.SetActive (false);
		// Resume the editor
		Editor.stopMouseEvents = false;
	}
}
