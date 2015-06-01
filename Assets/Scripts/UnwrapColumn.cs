using UnityEngine;
using System.Collections;

// Creates a mesh of a plane that matches the 
// width of the column from the game when unwrapped on the
// y-axis
public class UnwrapColumn : MonoBehaviour {

	// The radius of the column which is unwrapped in this editor.
	// This value must match the radius of the column inside the game,
	// otherwise levels will not translate well from the editor to the game.
	public float columnRadius = 1f;
	// This is the coordinate on x-axis where the left edge of the unwrapped cylinder will be anchored
	public float leftX;
	// The material of the plane
	public Material material;
	// Contains the height of the plane. Default value is just enough to fill the camera view (i.e. 10);
	// Note: the plane moves up and down with the camera, so it does not need to be enormously tall.
	public float height = 10;
	// Contains the width of the plane
	public float width { get; protected set;}


	void Start () {
		width = Mathf.PI * 2 * columnRadius;

		float halfHeight = height / 2;
		// Construct a plane with the specified width. Height does not matter here, as the plane will
		// be moving with the camera.
		// 0------1
		// |      |
		// |      |
		// 3------2
		Vector3[] vertices = new Vector3[] {
			new Vector3 (leftX, 		 halfHeight, 0),	//0
			new Vector3 (leftX + width,  halfHeight, 0),	//1
			new Vector3 (leftX + width, -halfHeight, 0),	//2
			new Vector3 (leftX, 		-halfHeight, 0)		//3
		};

		int[] triangles = new int[] {
			0, 1, 2,
			2, 3, 0
		};


		// Now that we have all vertices and triangles generated, make the mesh itself
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;

		mesh.RecalculateNormals ();

		// Add some additional stuff needed for rendering the plane
		if (!GetComponent<MeshFilter> ()) {
			gameObject.AddComponent<MeshFilter> ();
		}

		if (!GetComponent<MeshRenderer> ()) {
			gameObject.AddComponent<MeshRenderer> ();
		}
		// Assign the newly created mesh to the mesh filter
		gameObject.GetComponent<MeshFilter> ().mesh = mesh;
		gameObject.GetComponent<MeshRenderer> ().material = material;
	}
}
