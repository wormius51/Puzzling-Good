using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : Item {

	void OnTriggerEnter2D(Collider2D other) {
		if (locked)
			return;
		if (!addable && other.transform.tag == "Player" && GameManager.instanse.gameState != GameState.victory) {
			StartCoroutine (victory (other));
		}
	}

	IEnumerator victory(Collider2D _other) {
		yield return new WaitForSeconds (1 / stepsPerSeconds);
		if (_other != null && GameManager.instanse.gameState == GameState.playing) {
			if (GameManager.instanse.gameMode != GameMode.levelEditor) {
				PositionManager.instance.win ();
			} else {
				PositionManager.instance.showSubmitPanel ();
			}
		}
	}
}
