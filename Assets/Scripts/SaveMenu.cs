using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SaveMenu : MonoBehaviour {

	// This is the name field where the user enters the name of what he wants to save
	public InputField nameField;

	// This is the difficulty field for the segment's difficulty rating
	public InputField difficultyField;

	public void Show () {
		// Show the menu
		gameObject.SetActive (true);
		// Set the current name and difficulty to the current segment's
		LevelSegment current = SegmentFactory.Instance.CurrentSegment;
		if (current != null) {
			nameField.text = current.SegmentName;
			difficultyField.text = current.SegmentDifficulty + "";
		}
		// Disable the editor
		Editor.stopMouseEvents = true;
	}

	public void Save() {
		if (nameField.text.Length == 0) {
			return;
		}

		int rating = 0;
		int.TryParse (difficultyField.text, out rating);
		// Update the current segment to match the editor's contents
		SegmentFactory.Instance.UpdateSegment (nameField.text, rating);
		// Save to file
		SegmentFactory.Instance.Save ();
		// Close the menu
		gameObject.SetActive (false);
	}

	public void Hide() {
		// Clear any input
		nameField.text = "";
		difficultyField.text = "";
		// Hide the window
		gameObject.SetActive (false);
		// Resume the editor
		Editor.stopMouseEvents = false;
	}
}
