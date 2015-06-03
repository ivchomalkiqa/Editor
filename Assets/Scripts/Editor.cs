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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.mousePosition.x < 150) {
			return;
		}
		if (Input.GetMouseButtonUp (0)) {
			// TODO Check if the mouse is on a platform, or above the board

			// If an asset has been selected, we have to spawn it in the position
			// of the mouse
			// First get the type of asset that has been selected
			GameObject assetPrefab = AssetButtonsManager.selectedPrefab;
			if (assetPrefab) {
				// We have an asset which is selected; instantiate it
				GameObject asset = Instantiate<GameObject> (assetPrefab);
				// Change the position of the asset to match the position of the mouse in the xy plane
				Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				asset.transform.position = new Vector3 (mousePos.x, mousePos.y, asset.transform.position.z);
				asset.transform.parent = transform;
			}
		}
	}
}
