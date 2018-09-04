using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour {
	public const string DRAGGABLE_TAG = "dragable";
	public static DragHandler instance;

	public bool itemDroped;
	private bool dragging = false;
	private Transform objectToDrag;
	List<RaycastResult> hitObjects = new List<RaycastResult>();

	void Awake() {
		instance = this;
	}

	IEnumerator destroy() {
		yield return new WaitForEndOfFrame ();
		if (objectToDrag != null) {
			if (itemDroped) {
				itemDroped = false;
				if (GameManager.instanse.gameState != GameState.editing) {
					Destroy (objectToDrag.gameObject);
				}
			} else {
				if (GameManager.instanse.gameState != GameState.editing) {
					objectToDrag.localPosition = Vector3.zero;
				} else {
					Destroy (objectToDrag.gameObject);
				}
			}
		}
	}

	void Update() {
		if (Input.GetMouseButtonUp (0)) {
			dragging = false;
			StartCoroutine (destroy ());
			try {
				objectToDrag.GetComponentInChildren<Canvas>().overrideSorting = false;
			} catch {
			}
		}
		if (Input.GetMouseButtonDown (0)) {
			objectToDrag = getDraggableTransformUnderMouse ();

			if (objectToDrag != null) {
				dragging = true;

				objectToDrag.SetAsLastSibling ();
			}
 		}

		if (dragging) {
			objectToDrag.position = Input.mousePosition;
		}
	}

	private GameObject getObjectUnderMouse() {
		var pointer = new PointerEventData (EventSystem.current);

		pointer.position = Input.mousePosition;
		EventSystem.current.RaycastAll (pointer, hitObjects);
		if (hitObjects.Count <= 0)
			return null;
		return hitObjects [0].gameObject;
	}

	private GameObject getItemUnderMouse() {
		var pointer = new PointerEventData (EventSystem.current);

		pointer.position = Input.mousePosition;
		EventSystem.current.RaycastAll (pointer, hitObjects);
		if (hitObjects.Count <= 0) {
			return null;
		}
		GameObject item = null;
		hitObjects.ForEach (delegate(RaycastResult obj) {
			if (item == null) {
				if (obj.gameObject.transform.tag == "editDragable") {
					if (obj.gameObject.GetComponent<Item>().addable) {
						item = (GameObject) Instantiate(obj.gameObject,obj.gameObject.transform.parent);
					} else {
						item = obj.gameObject;
						item.GetComponent<Item>().addable = true;
					}
				}
				try {
					item.GetComponentInChildren<Canvas>().overrideSorting = true;
				} catch {
				}
			}

		});
		return item;
	}

	private Transform getDraggableTransformUnderMouse() {
		GameObject clickedObject = null;

		if (clickedObject == null && GameManager.instanse.gameState == GameState.editing) {
			GameObject item = getItemUnderMouse ();
			if (item != null) {
				return item.transform;
			} else {
				return null;
			}
		} else {
			clickedObject = getObjectUnderMouse ();
		}

		if (clickedObject != null && clickedObject.tag == DRAGGABLE_TAG) {
			return clickedObject.transform;
		}
		return null;
	}
}
