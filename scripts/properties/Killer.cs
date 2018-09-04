using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Killer : MonoBehaviour {

	public string target;

	void Update() {
		if (Input.GetMouseButtonUp (0) && !GetComponent<Item> ().isLocked ()) {
			List<RaycastResult> hitObjects = new List<RaycastResult>();
			var pointer = new PointerEventData (EventSystem.current);
			pointer.position = transform.position;
			EventSystem.current.RaycastAll (pointer, hitObjects);
			hitObjects.ForEach (delegate(RaycastResult obj) {
				if (obj.gameObject.tag == target) {
					PositionManager.instance.destroyItem(obj.gameObject.GetComponent<Item>());
				}
			});
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (GetComponent<Item> ().isLocked ())
			return;
		try {
			if (!GetComponent<Item>().addable && other.transform.tag == target && GameManager.instanse.gameState != GameState.victory) {
				if (target == "arrow" && !other.gameObject.GetComponent<Item>().isLocked()) {
					if (other.GetComponent<Rotator>() != null) {
						bool clockwise = other.GetComponent<Rotator>().clockwise;
						StartCoroutine(rotateSelf(clockwise));
					} else {
						StartCoroutine(rotateSelf(other.GetComponent<Item>().direction));
					}
				}
				print("kill " + target);
				PositionManager.instance.destroyItem (other.gameObject.GetComponent<Item> ());
			}
		} catch {
		}
	}

	IEnumerator rotateSelf(Direction _direction) {
		yield return new WaitForSeconds (1 / Item.StepsPerSecond);
		GetComponent<Item> ().direction = _direction;
	}

	IEnumerator rotateSelf(bool _clockwise) {
		yield return new WaitForSeconds (1 / Item.StepsPerSecond);
		GetComponent<Item> ().rotate (_clockwise);
	}
}
