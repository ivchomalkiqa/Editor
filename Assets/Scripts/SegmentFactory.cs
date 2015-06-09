using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SegmentFactory : MonoBehaviour {

	// Stuff used for singleton
	public static SegmentFactory Instance { get; protected set; }

	void Awake () {
		if (Instance == null) {
			Instance = this;
		}
	}

	// This is the current segment that is being manipulated and which will be 
	// saved to a file when the Save () method is called
	public LevelSegment CurrentSegment { get; protected set; }

	/**
	 * Saves the current segment to the disk.
	 */
	public void Save () {
		LevelSerializer.SaveLevel (CurrentSegment, GetPath (CurrentSegment.SegmentName));

		/*	UNCOMMENT THIS FOR A LOADING TEST
		LevelSegment test = LevelSerializer.LoadLevel (level.GetPath ());

		Debug.Log ("name: " + test.LevelName +
					"\ndifficulty: " + test.LevelDifficulty +
					"\nnumber of platforms: " + test.NumberOfPlatforms () +
					"\nhas spawner: " + test.HasSpawner ());
					*/
	}

	// Sets the current segment to a segment with the specified name,
	// difficulty, and containing all the assets which are children to
	// the 'Editor' tagged object in the scene.
	public void UpdateSegment(string name, int difficulty) {
		// Get all platforms that are part of the level
		// To do that, first get the editor object
		GameObject editor = GameObject.FindGameObjectWithTag ("Editor");
		if (!editor) {
			Debug.LogError ("Could not find an object with tag 'Editor'!");
			return;
		}

		if (name.Length == 0) {
			Debug.LogError ("Segment name is empty.");
			return;
		}
		if (difficulty < 0) {
			difficulty = 0;
		}
		
		// Make an array of all the game objects that are children 
		// of the editor game object. Those are platforms part of the segment.
		List<GameObject> levelPlatforms = new List<GameObject> ();
		foreach (Transform t in editor.transform) {
			levelPlatforms.Add (t.gameObject);
		}
		// Create a LevelSegment with the gathered data
		CurrentSegment = new LevelSegment (name, difficulty, levelPlatforms);
	}


	/*
	 * Converts the current level segment into a list of GameObjects
	 * and returns that list.
	 */
	public List<GameObject> GetGameObjects () {
		if (CurrentSegment == null) {
			Debug.LogError ("Current segment is empty.");
			return null;
		}
		// Create a list which will contain all assets
		List<GameObject> assetList = new List<GameObject> ();
		// Load all prefabs of assets
		GameObject[] assetPrefabs = Resources.LoadAll<GameObject> ("AssetPrefabs");
		// First process the spawner, if such exists in the segment
		if (CurrentSegment.HasSpawner ()) {
			assetList.Add (MakeObject (CurrentSegment.spawner, assetPrefabs));
		}
		// Now create the remaining objects out of the platforms contained in the level 
		foreach (Platform p in CurrentSegment.platforms) {
			assetList.Add (MakeObject (p, assetPrefabs));
		}
		// DONE
		return assetList;
	}

	/*
	 * Creates a GameObject from a given platform and list of assets, if the asset
	 * required for the specific type of platform is found. In other words, it converts
	 * a Platform object into a GameObject.
	 */
	public static GameObject MakeObject (Platform platform, GameObject[] assets) {
		// Find the prefab for the type of platform
		GameObject prefab = null;
		foreach (GameObject asset in assets) {
			if (platform.type.Contains (asset.name)) {
				// We found the prefab
				prefab = asset;
				break;
			}
		}
		if (prefab) {
			GameObject o = Instantiate (prefab);
			Vector3 position = new Vector3 (platform.positionX, platform.positionY, o.transform.position.z);
			Vector3 scale = new Vector3 (platform.scaleX, platform.scaleY, o.transform.localScale.z);
			// Set the position and scale of the object and return it
			o.transform.position = position;
			o.transform.localScale = scale;
			return o;
		} else {
			// We could not find the prefab for the type of platform passed
			return null;
		}
	}

	/*
	 * Loads the file matching the level segment with the specified name and
	 * sets it as the current segment overwriting any previous current segment.
	 */ 
	public void Load (string name) {
		CurrentSegment = LoadSegment (name);
	}

	/*
	 * Loads the level saved into the file corresponding to the specified
	 * name and returns the LevelSegment object
	 * that was retrieved from the file.
	 */
	private static LevelSegment LoadSegment(string name) {
		string path = GetPath (name);
		return LevelSerializer.LoadLevel (path);
	}

	// TODO move to SaveMenu class
	public void SetDisplayed (bool display) {
		if (!display) {
			// Clear all input fields before hiding the menu
			InputField[] fields = GetComponentsInChildren<InputField> ();
			foreach (InputField field in fields) {
				field.text = "";
			}
		}
		gameObject.SetActive (display);
	}

	// Returns the path where a level with the specified name would be saved
	public static string GetPath (string name) {
		string fileName = name + ".seg";
		string path = Application.dataPath + "/Segments/";
		return path + fileName;
	}

	// Returns the directory where a levels are saved
	public static string GetDirectory () {
		return Application.dataPath + "/Segments/";
	}

	public static string GetExtension () {
		return ".seg";
	}
}