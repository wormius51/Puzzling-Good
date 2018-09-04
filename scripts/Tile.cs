using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour {
	public int x;
	public int y;

	public void placeItem(Item _item) {
		PositionManager.instance.addItem (_item.gameObject, x, y);
		_item.addable = false;
		DragHandler.instance.itemDroped = true;
	}
}
