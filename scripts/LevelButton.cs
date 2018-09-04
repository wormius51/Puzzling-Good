using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {
	public int level = 1;

	// Use this for initialization
	void Start () {
		GetComponent<Button> ().onClick.AddListener (() => Menu.instance.initializeLevel (level));
	}
	
	// Update is called once per frame
	void Update () {
		if (PlayerPrefs.GetInt ("reached level", 1) >= level) {
			GetComponent<Button>().interactable = true;
		}
	}
}
