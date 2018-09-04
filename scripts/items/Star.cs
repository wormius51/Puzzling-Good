using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : Item {

	void OnTriggerEnter2D(Collider2D other) {
		if (locked)
			return;
		if (!addable && other.transform.tag == "Player" && GameManager.instanse.gameState != GameState.victory) {
			StartCoroutine (doubleScore (other));
		}
	}

	IEnumerator doubleScore(Collider2D _other) {
		yield return new WaitForSeconds (1 / stepsPerSeconds);
		if (_other != null) {
			GameManager.instanse.score *= 2;
			PositionManager.instance.refreshScore ();
			PositionManager.instance.destroyItem (this);
		}
	}

}
