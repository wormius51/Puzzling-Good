using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mimic : Item {

	protected override void makeMove ()
	{
		rotateDirection (true);
	}

	protected override void onRotate ()
	{
		StartCoroutine (mimic ());
	}

	protected override void onAdd ()
	{
		StartCoroutine (mimic ());
	}

	private IEnumerator mimic() {
		yield return new WaitForSeconds (2 / stepsPerSeconds);
		GameObject[,] tiles = PositionManager.instance.getTiles ();
		Tile tile = null;
		try {
			int x;
			int y;
		switch (direction) {
			case Direction.up:
				x = GetComponentInParent<Tile>().x;
				y = GetComponentInParent<Tile>().y - 1;
				tile = tiles[x,y].GetComponent<Tile>();
				break;
			case Direction.down:
				x = GetComponentInParent<Tile>().x;
				y = GetComponentInParent<Tile>().y + 1;
				tile = tiles[x,y].GetComponent<Tile>();
				break;
			case Direction.left:
				x = GetComponentInParent<Tile>().x - 1;
				y = GetComponentInParent<Tile>().y;
				tile = tiles[x,y].GetComponent<Tile>();
				break;
			case Direction.right:
				x = GetComponentInParent<Tile>().x + 1;
				y = GetComponentInParent<Tile>().y;
				tile = tiles[x,y].GetComponent<Tile>();
				break;
			}
		} catch {
			
		}
		if (tile != null) {
			if (tile.transform.childCount > 0) {
				if (tile.transform.GetChild (tile.transform.childCount - 1).tag.Equals ("arrow")) {
					List<RaycastResult> hitObjects = new List<RaycastResult>();
					var pointer = new PointerEventData (EventSystem.current);
					pointer.position = transform.position;
					EventSystem.current.RaycastAll (pointer, hitObjects);
					hitObjects.ForEach (delegate(RaycastResult obj) {
						if (obj.gameObject.tag == "arrow" && obj.gameObject != gameObject) {
							PositionManager.instance.destroyItem(obj.gameObject.GetComponent<Item>());
						}
					});
				}
				GameObject reflection = (GameObject)Instantiate (tile.transform.GetChild (tile.transform.childCount - 1).gameObject,transform.parent);
				if (tile.transform.GetChild (tile.transform.childCount - 1).GetComponent<Item> ().isLocked ()) {
					reflection.GetComponent<Item> ().lockingImage = reflection.transform.GetChild (reflection.transform.childCount - 1).gameObject;
				}
				GetComponentInParent<Tile> ().placeItem (reflection.GetComponent<Item> ());
				PositionManager.instance.destroyItem (this);
				DragHandler.instance.itemDroped = false;
				if (tile.transform.GetChild (tile.transform.childCount - 1).tag.Equals ("arrow")) {
					reflection.transform.SetAsFirstSibling ();
				}
			}
		}
	}
}
