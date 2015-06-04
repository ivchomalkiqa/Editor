using UnityEngine;
using System.Collections;

/* This script is responsible for the majority of
 * the editor's most important functions, such as 
 * placing, moving and resizing assets on the editor
 * board. 
 */
public class Editor : MonoBehaviour {

	// The width of the assets panel in screen pixels, this is necessary
	// for ignoring all mouse clicks that occur in the assets pane in this script
	public static float panelWidth = 150f;
	// This will be the shader used for highlighted (or selected) assets
	public Shader highlightedShader;

	// If the user clicks on an existing asset, it becomes selected
	GameObject selectedAsset;
	// A backup of the original shader used by the selected asset
	Shader selectedAssetShader;
	// The position where the mouse was when the LMB (left mouse button) was pressed down
	Vector3 mouseDownPosition;
	// Since there should be only one spawner per level, I will keep a reference to the instance
	// of the spawner to make things easier
	GameObject spawner;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			// Ignore clicks inside the assets panel
			if (Input.mousePosition.x < 150) {
				DeselectAsset ();
				return;
			}
			// Save the mouse down position
			mouseDownPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			// Now check whether we clicked on the board or on an existing asset
			// To do that, create a ray from the mouse position pointing forward
			// and see if that ray hits an asset.
			RaycastHit hit;
			if (RaycastFromMouse (out hit)) {
				// Clear any previous selection
				DeselectAsset ();
				// Select the asset under the cursor
				selectedAsset = hit.collider.gameObject;
				HighlightSelectedAsset ();
			} else {
				if (!DeselectAsset ()) {
					// Create a new asset at the provided position in case
					// an asset was not deselected, i.e. no asset was selected in the first place
					CreateAsset(mouseDownPosition);
				}
			}
		}

		// If the mouse is being held down while an asset is selected, then move the asset as
		// much as the mouse is moved
		if (Input.GetMouseButton (0) && selectedAsset) {
			MoveAsset (selectedAsset);
		}
	}

	bool DeselectAsset () {
		if (selectedAsset) {
			UnhighlightSelectedAsset ();
			selectedAsset = null;
			return true;
		} else {
			return false;
		}
	}

	// Creates an asset at the specified position, but ignoring
	// the z-coordinate. The asset created corresponds to the selectedPrefab
	// from the AssetButtonManager script. If no prefab is selected, then 
	// this method does nothing.
	protected void CreateAsset(Vector3 position) {
		// First get the type of asset that has been selected
		GameObject assetPrefab = AssetButtonsManager.selectedPrefab;

		if (assetPrefab && assetPrefab.name != "Spawner") {
			// We have an asset which is selected; instantiate it
			GameObject asset = Instantiate<GameObject> (assetPrefab);
			// Change the position of the asset to match the position provided in the xy plane
			asset.transform.position = new Vector3 (position.x, position.y, asset.transform.position.z);
			asset.transform.parent = transform;
		} else if (assetPrefab && assetPrefab.name == "Spawner" && !spawner) {
			// A spawner has not yet been created, create one
			GameObject asset = Instantiate<GameObject> (assetPrefab);
			// Change the position of the asset to match the position provided in the xy plane
			asset.transform.position = new Vector3 (position.x, position.y, asset.transform.position.z);
			asset.transform.parent = transform;
			spawner = asset;
		}
	}

	// Moves the asset with the mouse
	void MoveAsset (GameObject selectedAsset) {
		Vector3 newPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		// Calculate the vector from the mouseDownPosition to the current position
		Vector3 difference = newPosition - mouseDownPosition;
		
		// Move the asset with the difference
		selectedAsset.transform.position = selectedAsset.transform.position + difference;
		// Update the mouseDownPosition for next frame
		mouseDownPosition = newPosition;
	}

	// Casts a ray forward from the position of the mouse.
	// Returns true if something was hit.
	bool RaycastFromMouse (out RaycastHit hit) {
		Ray ray = new Ray (mouseDownPosition, Vector3.forward);
		if (Physics.Raycast (ray, out hit, 10f)) {
			return true;
		} else {
			return false;
		}
	}

	// Changes the shader of the specified asset to the highlightedShader
	void HighlightSelectedAsset () {
		// First backup original shader of the asset
		Renderer rend = selectedAsset.GetComponent<Renderer> ();
		selectedAssetShader = rend.material.shader;
		// Change the shader to the highlighted one
		rend.material.shader = highlightedShader;
	}

	void UnhighlightSelectedAsset () {
		Renderer rend = selectedAsset.GetComponent<Renderer> ();
		rend.material.shader = selectedAssetShader;
	}
}
