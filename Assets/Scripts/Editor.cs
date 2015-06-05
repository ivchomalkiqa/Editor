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
	// By how much is the size of assets changed during every resize step
	public float sizeChangeStep = 1f;

	// If the user clicks on an existing asset, it becomes selected
	GameObject selectedAsset;
	// A backup of the original shader used by the selected asset
	Shader selectedAssetShader;
	// The position where the mouse was when the LMB (left mouse button) was pressed down
	Vector3 mouseDownPosition;
	// Since there should be only one spawner per level, I will keep a reference to the instance
	// of the spawner to make things easier
	GameObject spawner;

	float minSize, maxSize;

	// Use this for initialization
	void Start () {
		minSize = 1;
		maxSize = UnwrapColumn.columnRadius;
	}
	
	// Update is called once per frame
	void Update () {
		// On LMB mouse down, figure out whether the user is clicking on an asset,
		// deselecting an already selected asset or creating a new one, by calling HandleSelection ()
		if (Input.GetMouseButtonDown (0)) {
			HandleSelection ();
		}

		// If the mouse is being held down while an asset is selected, then move the asset as
		// much as the mouse is moved
		if (Input.GetMouseButton (0) && selectedAsset) {
			MoveAsset (selectedAsset);
		}

		// If the scroll wheel is scrolled, resize asset between min and max values, if there is
		// an asset already selected
		if (Input.GetAxis ("Mouse ScrollWheel") == 0) {
			// Skip next two checks after having done only one check
			// This statement is unnecessary, but it reduces the number of
			// checks by one the majority of time, when we are not scrolling.
			// The tradeoff is that when we do scroll, it increases the number of checks by one.
		} else if (Input.GetAxis ("Mouse ScrollWheel") < 0 && selectedAsset) {
			// Resize asset down
			ResizeSelectedAsset (-sizeChangeStep);
		} else if (Input.GetAxis ("Mouse ScrollWheel") > 0 && selectedAsset) {
			// Resize asset up
			ResizeSelectedAsset (sizeChangeStep);
		}
	}

	// A helper method aimed at reducing the complexity of the
	// Update method. What it does is take care of selecting and
	// deselecting assets when the mouse is clicked over them or
	// away from them, and also creating new asset when the mouse is
	// clicked over empty space with no other asset selected.
	private void HandleSelection () {
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

	// Scales the selected asset on its local x-axis, so that 
	// it changes size with the desired amount, if there is an 
	// asset selected, and if its size remains within the minSize
	// and maxSize bounds (inclusively) after the change.
	void ResizeSelectedAsset (float amount) {
		// TODO Restrict resizing to assets different than the spawner, but that is not too important really
		if (selectedAsset) {
			float scaleX = selectedAsset.transform.localScale.x;
			scaleX += amount;
			if (scaleX < minSize) {
				scaleX = minSize;
			} else if (scaleX > maxSize) {
				scaleX = maxSize;
			}
			// Apply new scale
			Vector3 tmp = selectedAsset.transform.localScale;
			tmp.x = scaleX;
			selectedAsset.transform.localScale = tmp;
		}
	}


	// Returns true if an asset is selected and 
	// highlighted, otherwise returns false.
	public bool HasSelectedAsset () {
		if (selectedAsset) {
			return true;
		} else {
			return false;
		}
	}
}
