using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Arrow : Item {

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
			_other.gameObject.GetComponent<Item> ().direction = direction;
		} catch {
		}
	}

	protected override void onAdd ()
	{
		List<RaycastResult> hitObjects = new List<RaycastResult>();
		var pointer = new PointerEventData (EventSystem.current);
		pointer.position = transform.position;
		EventSystem.current.RaycastAll (pointer, hitObjects);
		hitObjects.ForEach (delegate(RaycastResult obj) {
			if (obj.gameObject.tag == "arrow" && obj.gameObject != gameObject) {
				PositionManager.instance.destroyItem(obj.gameObject.GetComponent<Item>());
			} else if (obj.gameObject.GetComponent<Item>() != null) {
				obj.gameObject.GetComponent<Item> ().direction = direction;
			}
		});
		transform.SetAsFirstSibling ();
	}

	protected override void update ()
	{
		
	}
}
