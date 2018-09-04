using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other) {
		
		Item item = null;
		try {
			item = other.gameObject.GetComponent<Item>();
		} catch {
			
		}

		if (item != null) {
			print (item.transform.name);
			PositionManager.instance.destroyItem (item);
		} else {
			print ("item not found in " + other.transform.name);
		}
	}

	// Use this for initialization
	void Start () {
		GetComponent<Animator> ().speed = Item.StepsPerSecond;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void selfDestruct() {
		Destroy (gameObject);
	}
}
