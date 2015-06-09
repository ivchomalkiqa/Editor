using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;


public class LevelSerializer : MonoBehaviour {

	public static void SaveLevel(LevelSegment level, string path) {
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream outStream = File.Create (path);
		// Write the level to the file
		bf.Serialize (outStream, level);
		outStream.Close ();
	}

	public static LevelSegment LoadLevel (string path) {
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream inStream = File.Open (path, FileMode.Open);

		LevelSegment level = (LevelSegment) bf.Deserialize (inStream);
		inStream.Close ();

		return level;
	}
}
