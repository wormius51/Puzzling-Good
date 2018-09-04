using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Key : Item {
	void OnTriggerEnter2D(Collider2D other) {
		if (locked)
			return;
		if (!addable && other.transform.tag == "Player" && GameManager.instanse.gameState != GameState.victory) {
			PositionManager.instance.unlockAllItems ();
			PositionManager.instance.destroyItem(this);
		}
	}

	protected override void onAdd ()
	{
		List<RaycastResult> hitObjects = new List<RaycastResult>();
		var pointer = new PointerEventData (EventSystem.current);
		pointer.position = transform.position;
		EventSystem.current.RaycastAll (pointer, hitObjects);
		hitObjects.ForEach (delegate(RaycastResult obj) {
			if (obj.gameObject.tag == "Player") {
				PositionManager.instance.unlockAllItems();
				PositionManager.instance.destroyItem(this);
			}
		});
	}
}
