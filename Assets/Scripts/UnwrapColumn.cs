using UnityEngine;
using System.Collections;

// Creates a mesh of a plane that matches the 
// width of the column from the game when unwrapped on the
// y-axis
public class UnwrapColumn : MonoBehaviour {

	// This determines the size of a square of a given texture when applied to
	// the plane. Smaller values will produce a greater number of repetitions
	// of the texture, when applied to the plane.
	public float textureScale = 1f;
	// The radius of the column which is unwrapped in this editor.
	// This value must match the radius of the column inside the game,
	// otherwise levels will not translate well from the editor to the game.
	public static float columnRadius = 5f;
	// This is the coordinate on x-axis where the left edge of the unwrapped cylinder will be anchored
	public float leftX = -13f;
	// This is the coordinate on the y-axis where the bottom edge of the unwrapped cylinder will be anchored
	public float bottomY = -5f;
	// The material of the plane
	public Material material;
	// Contains the height of the plane. Default value is just enough to fill the camera view (i.e. 10);
	// Note: the plane moves up and down with the camera, so it does not need to be enormously tall.
	public float height = 20;
	// Contains the width of the plane
	public float width { get; protected set;}


	void Start () {
		width = Mathf.PI * 2 * columnRadius;

		// Construct a plane with the specified width. Height does not matter here, as the plane will
		// be moving with the camera.
		// 0------1
		// |      |
		// |      |
		// 3------2
		Vector3[] vertices = new Vector3[] {
			new Vector3 (leftX, 		bottomY + height, 	0),	//0
			new Vector3 (leftX + width, bottomY + height, 	0),	//1
			new Vector3 (leftX + width, bottomY, 			0),	//2
			new Vector3 (leftX, 		bottomY, 			0)	//3
		};

		int[] triangles = new int[] {
			0, 1, 2,
			2, 3, 0
		};

		float uvHeight = height / textureScale;
		float uvWidth = width / textureScale;

		Vector2[] uvs = new Vector2[] {
			new Vector2 (0, 		uvHeight),
			new Vector2 (uvWidth, 	uvHeight),
			new Vector2 (uvWidth, 	0		),
			new Vector2 (0, 		0		)
		};

		// Now that we have all vertices and triangles generated, make the mesh itself
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
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
