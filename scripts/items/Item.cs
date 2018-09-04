using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/**
 * A class for items that need to preform some action each turn.
 * */
public class Item : MonoBehaviour , IPointerClickHandler {
	/**
	 * How many steps do items take per second.
	 * */
	static protected float stepsPerSeconds { get; private set; }

	static public float StepsPerSecond{ 
		get {
			return Item.stepsPerSeconds; 
		}
		/**
		 * Only positive value allowed.
		 * */
		set {
			if (value > 0)
				Item.stepsPerSeconds = value;
			else
				throw new PuzzleGameExeption ("Only positive value is allowed for stepsPerSecond");
		}
	}
	protected Animator animator;
	public ItemState itemState { get; protected set; }
	public Direction direction = Direction.up;
	private Direction lastDirection = Direction.up;
	public int x;
	public int y;
	public bool addable = true;
	public ItemEnum itemEnum;
	public string description = "item";
	public bool locked = false;

	public GameObject lockingImage = null;

	protected bool counted = false;

	public bool isLocked() {
		return locked;
	}

	void Awake () {
		if (GameManager.instanse.gameState == GameState.editing) {
			GetComponent<CanvasGroup> ().blocksRaycasts = true;
			transform.tag = "editDragable";
		}
		GetComponent<Collider2D> ().enabled = false;
		animator = GetComponent<Animator> ();
		animator.speed = stepsPerSeconds;
		GetComponent<BoxCollider2D> ().size = new Vector2 (1, 1);


	}

	void Start() {
		start ();
	}

	public void refresh() {
		GetComponent<Collider2D> ().enabled = false;
		animator = GetComponent<Animator> ();
		animator.speed = stepsPerSeconds;
		GetComponent<BoxCollider2D> ().size = new Vector2 (1, 1);

		start ();
	}

	public void add() {
		print ("counted: " + counted);
		counted = true;
		if (counted)
			return;
		
		onAdd ();
	}

	protected virtual void start() {
	}

	void Update() {
		if (animator.speed != stepsPerSeconds) {
			animator.speed = stepsPerSeconds;
		}

		if (lastDirection != direction && !locked) {
			onRotate ();
		}

		lastDirection = direction;
			
		if (addable && Input.GetMouseButtonUp (0)) {
			
			List<RaycastResult> hitObjects = new List<RaycastResult> ();
			var pointer = new PointerEventData (EventSystem.current);
			pointer.position = transform.position;
			EventSystem.current.RaycastAll (pointer, hitObjects);
			hitObjects.ForEach (delegate(RaycastResult obj) {
				if (GameManager.instanse.gameState != GameState.editing && obj.gameObject.tag == "tile") {
					GetComponent<Collider2D> ().enabled = true;
					obj.gameObject.GetComponent<Tile> ().placeItem (this);
					onAdd ();
				} else if (GameManager.instanse.gameState == GameState.editing && obj.gameObject.tag == "editorTile") {
					addable = false;
					obj.gameObject.GetComponent<EditorTile> ().placeItem (this);
				}
			});
		}
		if (Input.GetMouseButtonUp (1) && GameManager.instanse.gameMode != GameMode.levels && GameManager.instanse.gameState != GameState.editing) {
			
			if ((transform.position - Input.mousePosition).magnitude <= 32) {
				Menu.instance.openItemDescription (description);
			}
		}

		animateOriantation ();
		update ();
	}

	public void OnPointerClick (PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Right) {
			if (GameManager.instanse.gameState == GameState.editing) {
				rotateDirection (true);
			} 
		}
	}

	public void destroy() {
		onDestroy ();
		Destroy (gameObject);
	}

	protected virtual void onDestroy() {
	}

	protected virtual void onAdd() {
	}

	protected virtual void update () {
	}

	public void rotate(bool _clockwise) {
		if (itemState == ItemState.inStep)
			return;
		itemState = ItemState.inStep;
		rotateDirection (_clockwise);
	}

	public void animateOriantation() {
		int dir = 0;
		switch (direction) {
		case Direction.up:
			dir = 1;
			break;
		case Direction.right:
			dir = 2;
			break;
		case Direction.down:
			dir = 3;
			break;
		case Direction.left:
			dir = 4;
			break;
		}
		animator.SetInteger ("direction", dir);
	}

	private IEnumerator rotateInum(bool _clockwise) {
		itemState = ItemState.inStep;
		yield return new WaitForSeconds (0);
		rotateDirection (_clockwise);
		itemState = ItemState.inMove;
	}

	protected void rotateDirection(bool _clockwise) {
		switch (direction) {
		case Direction.down:
			if (_clockwise) {
				direction = Direction.left;
			} else {
				direction = Direction.right;
			}
			break;
		case Direction.up:
			if (_clockwise) {
				direction = Direction.right;
			} else {
				direction = Direction.left;
			}
			break;
		case Direction.left:
			if (_clockwise) {
				direction = Direction.up;
			} else {
				direction = Direction.down;
			}
			break;
		case Direction.right:
			if (_clockwise) {
				direction = Direction.down;
			} else {
				direction = Direction.up;
			}
			break;
		}

	}
	/// <summary>
	/// Disables the fanctionality of this item.
	/// </summary>
	public void locK() {
		if (locked)
			return;
		locked = true;
		lockingImage = Instantiate ((GameObject)Resources.Load ("prefabs/locking"), transform);
	}
	/// <summary>
	/// Enables the fanctionality of this item.
	/// </summary>
	public void unlock() {
		if (!locked)
			return;
		locked = false;
		if (lockingImage != null)
			Destroy (lockingImage);
	}

	/// <summary>
	/// Makes a move if not locked.
	/// </summary>
	public void tryMove() {
		if (!locked)
			makeMove ();
	}

	/**
	 * <summary>
	 * makeMove is called once per turn.
	 * </summary>
	 * */
	protected virtual void makeMove(){
	}

	protected virtual void onRotate() {
	}
}
