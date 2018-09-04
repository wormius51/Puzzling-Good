using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorTile : MonoBehaviour {

	public int container;
	public int spot;

	// Use this for initialization
	void Start () {
		spot = transform.GetSiblingIndex ();
		foreach (Transform child in transform) {
			child.gameObject.GetComponent<Item> ().addable = false;
			child.gameObject.GetComponent<CanvasGroup> ().blocksRaycasts = true;
			child.tag = "editDragable";
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void placeItem (Item _item) {
		_item.gameObject.transform.SetParent (transform);
		_item.gameObject.transform.localPosition = Vector3.zero;
		DragHandler.instance.itemDroped = true;
	}
}
