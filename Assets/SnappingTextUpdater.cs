using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SnappingTextUpdater : MonoBehaviour {

	public string OnText = "Snapping On";
	public string OffText = "Snapping Off";

	Text text;

	void Start () {
		text = GetComponent<Text> ();
	}

	public void SetSnapping (bool isOn) {
		if (isOn) {
			text.text = OnText;
		} else {
			text.text = OffText;
		}
	}
}
