using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AssetButtonsManager : MonoBehaviour {

	// This array contains all prefabs for the assets that are used to build a level
	// e.g. different types of platforms, player spawner, etc.
	GameObject[] assetPrefabs;

	// A prefab for the button that will serve as a template for all buttons
	public GameObject buttonPrefab;
	// A reference to the assets panel which will contain all the buttons
	public Transform assetsPanel;
	// This is how much space will be allocated for every button
	public float buttonHeight;

	// This static field can be accessed from other scripts in order for them to find out
	// which button was clicked by the user, i.e. which asset was se
	public static GameObject selectedPrefab { get; protected set;}
	// Keep a reference to the last selected button to simulate toggle behaviour
	private GameObject selectedButton;

	// Use this for initialization
	void Start () {
		selectedPrefab = null;
		// Load all prefabs from the Assets/Resources/AssetPrefabs folder
		assetPrefabs = Resources.LoadAll<GameObject> ("AssetPrefabs");

		MakeButtons (assetPrefabs);
	}

	// Creates buttons inside the assets portion of the ui for every
	// gameobject contained in the array parameter. The buttons are
	// arranged vertically, starting from the top.
	void MakeButtons (GameObject[] objects) {
		for (int i = 0; i < objects.Length; i++) {
			GameObject buttonObj = Instantiate <GameObject> (buttonPrefab);
			// Set the button's name
			Text buttonText = buttonObj.GetComponentInChildren<Text> ();
			buttonText.text = objects[i].name;
			// Get a reference to the RectTransform of the button to set its position
			RectTransform rt =  buttonObj.GetComponent<RectTransform> ();
			// Set the parent of this button to be the assets panel
			rt.SetParent (assetsPanel, false);
			// Now set the position
			rt.anchoredPosition = rt.anchoredPosition - new Vector2(0, i * buttonHeight);
			// Finally, add an onClick listener to the button
			buttonObj.GetComponent<Button> ().onClick.AddListener (() => OnAssetButtonClick (buttonObj));
		}
	}

	public void OnAssetButtonClick (GameObject button) {
		// Find the asset with the matching name
		string buttonName = button.GetComponentInChildren<Text> ().text;
		// Check if this button is being clicked for the second time
		if (selectedPrefab && buttonName == selectedPrefab.name) {
			Debug.Log ("Button being deselected: " + buttonName);
			selectedPrefab = null;
			// Unhighlight selected button
			SetButtonNormal (selectedButton);
			selectedButton = null;
		} else {
			// This is a new button being pressed, look for the prefab that matches
			// the button
			bool found = false;
			foreach (GameObject asset in assetPrefabs) {
				if (asset.name == buttonName) {
					// Found the matching asset 
					selectedPrefab = asset;
					found = true;
					// Unhighlight the previous button if there was such
					// and then highlight the new selected button
					if (selectedButton) {
						// Unhighlight selected button
						SetButtonNormal (selectedButton);
					}
					selectedButton = button;
					// Highlight new selected button
					SetButtonPressed (selectedButton);
					Debug.Log ("Button being selected: " + buttonName);
					break;
				}
			}
			if (!found) {
				Debug.LogError ("No matching asset was found for the button pressed. This should not happen!");
			}
		}
	}

	public static void SetButtonPressed (GameObject button) {
		// Get the animator of this button and set its pressed condition to true
		Animator anim = button.GetComponent<Animator> ();
		if (!anim) {
			Debug.LogError ("A button without an animator was pressed!");
			return;
		}
		anim.SetBool ("Normal", false);
		anim.SetBool ("Pressed", true);
	}

	public static void SetButtonNormal (GameObject button) {
		// Get the animator of this button and set its normal condition to true
		Animator anim = button.GetComponent<Animator> ();
		if (!anim) {
			Debug.LogError ("A button without an animator was pressed!");
			return;
		}
		anim.SetBool ("Normal", true);
		anim.SetBool ("Pressed", false);
	}
}
