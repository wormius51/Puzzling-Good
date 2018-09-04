using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Rotator : Item {
	public bool clockwise = true;

	protected override void start () {
		if (!clockwise) {
			RawImage rawImage = transform.GetChild (0).GetComponent<RawImage> ();
			rawImage.uvRect = new Rect(1, 0, -1, 1);
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (locked)
			return;
		if (!addable && GameManager.instanse.gameState != GameState.victory) {
			StartCoroutine (rotateOther (other));
		}
	}

	private IEnumerator rotateOther(Collider2D _other) {
		yield return new WaitForSeconds (1 / stepsPerSeconds);
		try {
			_other.gameObject.GetComponent<Item> ().rotate (clockwise);
		} catch {
		}
	}

	protected override void onAdd () {
		List<RaycastResult> hitObjects = new List<RaycastResult>();
		var pointer = new PointerEventData (EventSystem.current);
		pointer.position = transform.position;
		EventSystem.current.RaycastAll (pointer, hitObjects);
		hitObjects.ForEach (delegate(RaycastResult obj) {
			if (obj.gameObject.tag == "arrow" && obj.gameObject != gameObject) {
				Destroy(obj.gameObject);
			} else if (obj.gameObject.GetComponent<Item>() != null) {
				obj.gameObject.GetComponent<Item> ().rotate (clockwise);
			}
		});
		transform.SetAsFirstSibling ();
	}
}
