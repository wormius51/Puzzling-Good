using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Lock : Item {

	protected override void onAdd ()
	{
		List<RaycastResult> hitObjects = new List<RaycastResult>();
		var pointer = new PointerEventData (EventSystem.current);
		pointer.position = transform.position;
		EventSystem.current.RaycastAll (pointer, hitObjects);
		hitObjects.ForEach (delegate(RaycastResult obj) {
			try {
				obj.gameObject.GetComponent<Item>().locK();
			} catch {
			}
		});
		PositionManager.instance.destroyItem (this);
	}
}
