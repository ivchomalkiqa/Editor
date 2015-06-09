using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**
 * This class contains the description of a level consisting
 * of name, statistics about the level and the platforms constituting
 * the level itself.
 */ 
[Serializable]
public class LevelSegment {
	// Name of the level, can be accessed or set publically
	public string SegmentName { get; protected set;}
	// A positive integer indicating the difficulty of the level
	private int segmentDifficulty;
	public int SegmentDifficulty {
		get {
			return segmentDifficulty;
		}
		set {
			if (value < 0) {
				segmentDifficulty = 0;
			} else {
				segmentDifficulty = value;
			}
		}
	}

	// List of platforms constituting the level
	public List<Platform> platforms;
	// The single spawn point, where the user is spawned. Can be null if
	// no spawner exists in the level
	public Platform spawner;

	// CONSTRUCTOR
	// Create a LevelSegment which.
	// string name: the name of the segment, which will also determine the name of its filename
	// int difficulty: the difficulty associated with this segment. 
	// A higher integer indicates higher difficulty. If a negative integer is passed, difficulty will default to 0.
	// IEnumerable<GameObject> level: Any sort of collection of GameObjects representing platforms part of the level.
	public LevelSegment (string name, int difficulty, IEnumerable<GameObject> level) {
		this.SegmentName = name;
		this.SegmentDifficulty = difficulty;

		// Initialize and set the list of platforms constituting the level
		this.platforms = new List<Platform> ();
		foreach (GameObject obj in level) {
			if (obj.name.Contains ("Spawner")) {
				// We found a spawner, this should not be added
				// to the level, but rather saved as the spawn
				spawner = new Platform(obj);
			} else {
				// This is a normal level platform
				this.platforms.Add (new Platform(obj));
			}
		}

		//TODO Generate statistics about the level

	}

	// Return true if the level contains a 
	// spawner where the player can start, false otherwise.
	public bool HasSpawner () {
		return spawner != null;
	}

	public int NumberOfPlatforms () {
		return platforms.Count;
	}
}

[Serializable]
public class Platform {
	/* Since neither of the Unity specific classes
	 * is serializable, I will need to use primitive
	 * types to save the information about each platform */
	public float positionX;
	public float positionY;

	public string type;

	public float scaleX;
	public float scaleY;

	public Platform (GameObject original) {
		positionX = original.transform.position.x;
		positionY = original.transform.position.y;

		type = original.name;

		scaleX = original.transform.localScale.x;
		scaleY = original.transform.localScale.y;
	}
}