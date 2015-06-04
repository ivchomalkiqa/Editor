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

	// If the user clicks on an existing asset, it becomes selected
	GameObject selectedAsset;
	// The position where the mouse was when the LMB (left mouse button) was pressed down
	Vector3 mouseDownPosition;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			// Ignore clicks inside the assets panel
			if (Input.mousePosition.x < 150) {
				return;
			}
			// Save the mouse down position
			mouseDownPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			// Now check whether we clicked on the board or on an existing asset
			// To do that, create a ray from the mouse position pointing forward...
			Ray ray = new Ray (mouseDownPosition, Vector3.forward);
			RaycastHit hit;
			// ... and see if that ray hits an asset.
			if (Physics.Raycast (ray, out hit, 10f)) {
				// Select the asset under the cursor
				selectedAsset = hit.collider.gameObject;
			} else {
				// Create a new asset at the provided position
				CreateAsset(mouseDownPosition);
			}
		}
		// If the left mouse button was released, deselect any selected asset TODO revise this behaviour
		if (Input.GetMouseButtonUp (0)) {
			selectedAsset = null;
		}
		// If the mouse is being held down while an asset is selected, then move the asset as
		// much as the mouse is moved
		if (Input.GetMouseButton (0) && selectedAsset) {
			Vector3 newPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			// Calculate the vector from the mouseDownPosition to the current position
			Vector3 difference = newPosition - mouseDownPosition;

			// Move the asset with the difference
			selectedAsset.transform.position = selectedAsset.transform.position + difference;
			// Update the mouseDownPosition for next frame
			mouseDownPosition = newPosition;
		}
	}

	// Creates an asset at the specified position, but ignoring
	// the z-coordinate. The asset created corresponds to the selectedPrefab
	// from the AssetButtonManager script. If no prefab is selected, then 
	// this method does nothing.
	protected void CreateAsset(Vector3 position) {
		// First get the type of asset that has been selected
		GameObject assetPrefab = AssetButtonsManager.selectedPrefab;
		if (assetPrefab) {
			// We have an asset which is selected; instantiate it
			GameObject asset = Instantiate<GameObject> (assetPrefab);
			// Change the position of the asset to match the position provided in the xy plane
			asset.transform.position = new Vector3 (position.x, position.y, asset.transform.position.z);
			asset.transform.parent = transform;
		}
	}
}
