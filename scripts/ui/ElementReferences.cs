using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ElementReferences : MonoBehaviour {
	public static ElementReferences instance;

	void Awake() {
		instance = this;
	}

	[Serializable]
	public struct keyAndGameObject {
		public string key;
		public GameObject gameO;
	}

	public keyAndGameObject[] references;

	public GameObject getGameObject(string key) {
		GameObject g = null;
		for (int i = 0; i < references.Length; i++) {
			keyAndGameObject k = references [i];
			if (k.key.Equals (key)) {
				g = k.gameO;
				break;
			}
		}
		return g;
	}
}

