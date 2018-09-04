using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/**
 * Controlls the position on the board.
 * */
public class PositionManager : MonoBehaviour {
	public static PositionManager instance = null;
	public bool autoMove = false;
	public bool autoInitilize = true;
	public RectTransform board;
	public GameObject tilePrefab;
	public GameObject ghostTilePrefab;
	public GameObject canvase;
	public int boardSize = 16;
	public int level = 1;
	public GameObject victory;
	public GameObject communityVisctory;
	public GameObject defeat;
	public GameObject subnitLevel;
	public GameObject moveButtons;
	public GameObject editorButtons;
	public GameObject editButton;
	public Button goButton;
	public Button autoMoveButton;
	public Text levelNumber;
	public Text highScoreText;
	public Text scoreText;
	public Slider animationSpeedSlider;
	public API api;

	private ContainerRandomizer[] containers;
	private GameObject[,] tiles;
	public GameObject[,] ghostTiles;
	private List<Item> items;
	private GameObject inventory;
	private Level communityLevel;

	// Use this for initialization
	void Awake () {
		editButton.SetActive (false);
		Item.StepsPerSecond = PlayerPrefs.GetFloat ("steps per second", 6);
		if (Item.StepsPerSecond < 3) {
			Item.StepsPerSecond = 3;
		}
		GameManager.instanse.gameState = GameState.playing;
		animationSpeedSlider.value = Item.StepsPerSecond;
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
		items = new List<Item> ();
		tileTheBoard (boardSize);
		if (autoInitilize) {
			initializeLevel ();
		} else {
			startServival ();
		}
	}

	public GameObject getInventory() {
		return inventory;
	}

	public GameObject[,] getTiles() {
		return tiles;
	}

	public Level getCommunityLevel() {
		return communityLevel;
	}
	/// <summary>
	/// Sets up a given level for play.
	/// </summary>
	/// <param name="_level">Level.</param>
	public void playCommunityLevel (Level _level) {
		communityLevel = _level;
		GameManager.instanse.gameState = GameState.playing;
		GameManager.instanse.gameMode = GameMode.communityLevels;
		levelNumber.transform.parent.gameObject.SetActive (false);
		highScoreText.transform.parent.gameObject.SetActive (false);
		scoreText.transform.parent.gameObject.SetActive (false);
		moveButtons.SetActive (true);
		editorButtons.SetActive (false);
		editButton.SetActive (false);
		Menu.instance.closeCommunityLevelsMenu ();
		Menu.instance.gameObject.SetActive (false);
		if (inventory != null) {
			Destroy (inventory);
		}
		clearBoard ();
		inventory = (GameObject)Instantiate (Resources.Load ("inventories/inventory clear"), canvase.transform);
		inventory.transform.SetSiblingIndex (2);
		LevelEditor.instance.custLevelToInventory (_level, inventory);
		api.playCurrentLevel ();
		Menu.instance.closeLikeButton ();
		api.didILikeCurrentLevel ();
	}

	/// <summary>
	/// Starts the level editor.
	/// </summary>
	public void startLevelEditor() {
		clearBoard ();
		if (autoMove) {
			toggleAutoMove ();
		}
		GameManager.instanse.gameState = GameState.editing;
		GameManager.instanse.gameMode = GameMode.levelEditor;
		levelNumber.transform.parent.gameObject.SetActive (false);
		highScoreText.transform.parent.gameObject.SetActive (false);
		scoreText.transform.parent.gameObject.SetActive (false);
		moveButtons.SetActive (false);
		editorButtons.SetActive (true);
		editButton.SetActive (false);
		if (inventory != null) {
			Destroy (inventory);
		}
		inventory = (GameObject)Instantiate (Resources.Load ("inventories/inventory edit"), canvase.transform);
		inventory.transform.SetSiblingIndex (2);
		if (LevelEditor.instance.getLevel () != null) {
			LevelEditor.instance.custLevelToInventory (LevelEditor.instance.getLevel (), inventory);
		}
	}
	/// <summary>
	/// Clears the level in the level editor.
	/// </summary>
	public void clearEditorLevel() {
		if (inventory != null) {
			Destroy (inventory);
		}
		inventory = (GameObject)Instantiate (Resources.Load ("inventories/inventory edit"), canvase.transform);
		inventory.transform.SetSiblingIndex (2);
	}
	/// <summary>
	/// Start the level that was edited by the user.
	/// </summary>
	public void playEditorLevel() {
		if (GameManager.instanse.gameState == GameState.editing) {
			LevelEditor.instance.setLevel ();
		}
		GameManager.instanse.gameState = GameState.playing;
		moveButtons.SetActive (true);
		editorButtons.SetActive (false);
		editButton.SetActive (true);
		if (inventory != null) {
			Destroy (inventory);
		}
		inventory = (GameObject)Instantiate (Resources.Load ("inventories/inventory clear"), canvase.transform);
		inventory.transform.SetSiblingIndex (2);
		LevelEditor.instance.custLevelToInventory (LevelEditor.instance.getLevel (), inventory);
	}
	/// <summary>
	/// Shows the submit panel.
	/// </summary>
	public void showSubmitPanel() {
		GameManager.instanse.gameState = GameState.victory;
		subnitLevel.SetActive (true);
	}

	public void unshowSubmitPanel() {
		subnitLevel.SetActive (false);
	}
	/// <summary>
	/// Starts the servival mode.
	/// </summary>
	public void startServival() {
		clearBoard ();
		if (autoMove) {
			toggleAutoMove ();
		}
		GameManager.instanse.gameState = GameState.playing;
		GameManager.instanse.gameMode = GameMode.servival;
		GameManager.instanse.score = 0;
		levelNumber.transform.parent.gameObject.SetActive (false);
		highScoreText.transform.parent.gameObject.SetActive (true);
		scoreText.transform.parent.gameObject.SetActive (true);
		moveButtons.SetActive (true);
		editorButtons.SetActive (false);
		editButton.SetActive (false);
		scoreText.text = "0";
		if (inventory != null) {
			Destroy (inventory);
		}
		inventory = (GameObject)Instantiate (Resources.Load ("inventories/inventory random"), canvase.transform);
		inventory.transform.SetSiblingIndex (2);
		containers = FindObjectsOfType<ContainerRandomizer> ();
		GameObject player = (GameObject)Instantiate (Resources.Load ("prefabs/player"), ghostTiles [0, 0].transform);
		addItem (player, 1, 3);
		api.getHighestScore ();
	}

	public void restart() {
		clearBoard ();
		defeat.SetActive (false);
		switch (GameManager.instanse.gameMode) {
		case GameMode.levels:
			initializeLevel ();
			break;
		case GameMode.servival:
			startServival ();
			break;
		case GameMode.levelEditor:
			if (GameManager.instanse.gameState == GameState.editing) {
				startLevelEditor ();
			} else {
				playEditorLevel ();
			}
			break;
		case GameMode.communityLevels:
			playCommunityLevel (communityLevel);
			break;
		}
	}
	// Update is called once per frame
	void Update () {
		if (autoMove && GameManager.instanse.gameState == GameState.playing && goButton.interactable) {
			takeTurn ();
		}

		if (Input.GetButton ("space"))
			takeTurn ();
	}

	public void toggleAutoMove() {
		autoMove = !autoMove;
		autoMoveButton.GetComponentInChildren<Text> ().text = autoMove ? "Stop" : "Auto";
	}
	/// <summary>
	/// Adds the item to the list and to the board.
	/// </summary>
	/// <param name="_item">Item to be addwd.</param>
	/// <param name="_x">The x coordinate of the tile.</param>
	/// <param name="_y">The y coordinate of the tile.</param>
	public void addItem(GameObject _item, int _x, int _y) {
		_item.transform.SetParent (tiles [_x, _y].transform);
		try {
			_item.GetComponent<CanvasGroup>().blocksRaycasts = true;
		} catch {
		}
		_item.GetComponent<Item> ().addable = false;
		_item.GetComponent<Item> ().x = _x;
		_item.GetComponent<Item> ().y = _y;
		_item.GetComponent<BoxCollider2D> ().enabled = true;
		items.Add(_item.GetComponent<Item> ());
		//_item.GetComponent<Item> ().add ();
	}
	/**
	 * <summary>Putts the tiles in the board and saves reference to each tile.</summary>
	 * <param name = "_size">The number of tiles in each row and columen.</param>
	 * 
	 * */
	private void tileTheBoard(int _size) {
		tiles = new GameObject[_size, _size];
		ghostTiles = new GameObject[_size, _size];
		float tileSize = 4000 / _size;
		RectTransform tileRect = tilePrefab.GetComponent<RectTransform> ();
		tileRect.position = Vector3.zero;
		tileRect.sizeDelta = new Vector2 (tileSize, tileSize);
		RectTransform ghostTileRect = tilePrefab.GetComponent<RectTransform> ();
		ghostTileRect.position = Vector3.zero;
		ghostTileRect.sizeDelta = new Vector2 (tileSize, tileSize);
		Vector3 currentPosition = Vector3.zero;
		for (int y = 0; y < _size; y++) {
			for (int x = 0; x < _size; x++) {
				tiles[x,y] = Instantiate (tilePrefab, board.transform);
				tiles [x, y].GetComponent<RectTransform>().anchoredPosition = currentPosition;
				tiles [x, y].GetComponent<Tile> ().x = x;
				tiles [x, y].GetComponent<Tile> ().y = y;
				currentPosition += new Vector3 (board.sizeDelta.x / boardSize,0);
			}
			currentPosition -= new Vector3 (0, board.sizeDelta.y / boardSize);
			currentPosition.x = 0;
		}
		currentPosition = Vector3.zero;
		for (int y = 0; y < _size; y++) {
			for (int x = 0; x < _size; x++) {
				ghostTiles [x,y] = Instantiate (ghostTilePrefab, board.transform);
				ghostTiles [x, y].GetComponent<RectTransform>().anchoredPosition = currentPosition;
				currentPosition += new Vector3 (board.sizeDelta.x / boardSize,0);
			}
			currentPosition -= new Vector3 (0, board.sizeDelta.y / boardSize);
			currentPosition.x = 0;
		}
	}

	IEnumerator reactivateGoButton() {
		yield return new WaitForSeconds (1 / Item.StepsPerSecond + 0.5f);
		goButton.interactable = true;
	}

	public void unlockAllItems() {
		foreach (Item item in items) {
			item.unlock();
		}
	}

	/**
	 * <summary>
	 * Invokes all the items on the board to make a move.
	 * </summary>
	 * */
	public void takeTurn() {
		if (!goButton.interactable)
			return;
		StartCoroutine (reactivateGoButton ());
		servival ();
		goButton.interactable = false;
		foreach (Item item in items) {
			try {
				item.tryMove ();
			} catch {
			}
		}
		foreach (Item item in items) {
			try {
				if (item.GetComponent<Arrow>() != null || (item.GetComponent<Rotator>() != null)) {
					continue;
				}

				if (item.gameObject.GetComponent<Player> () != null && item.gameObject.GetComponent<Killer> () != null && item.gameObject.GetComponent<Killer> ().target.Equals("Player")) {
					item.gameObject.GetComponent<Collider2D> ().enabled = false;
					item.gameObject.GetComponent<Killer> ().enabled = false;
				}
			} catch {
			}
		}


	}

	private void servival() {
		if (GameManager.instanse.gameMode == GameMode.servival && GameManager.instanse.gameState == GameState.playing) {
			refreshScore ();
			if (GameManager.instanse.score % 5 == 0) {
				GameObject arrowKiller = (GameObject)Instantiate (Resources.Load ("prefabs/randomable/arrow_killer"), ghostTiles [0, 0].transform);
				Direction randomDirection = Direction.up;
				switch ((int)(Random.value * 4)) {
				case 0:
					randomDirection = Direction.up;
					break;
				case 1:
					randomDirection = Direction.down;
					break;
				case 2:
					randomDirection = Direction.left;
					break;
				case 3:
					randomDirection = Direction.right;
					break;
				}
				arrowKiller.GetComponent<Item> ().direction = randomDirection;
				int arX = (int)(Random.value * boardSize);
				int arY = (int)(Random.value * boardSize);
				foreach (Transform child in tiles [arX, arY].transform) {
					if (child.tag == "Player") {
						arX = (int)(Random.value * boardSize);
						arY = (int)(Random.value * boardSize);
						break;
					}
				}
				addItem (arrowKiller, (int)(Random.value * boardSize), (int)(Random.value * boardSize));
			}
			if (GameManager.instanse.score % 13 == 0) {
				GameObject star = (GameObject)Instantiate (Resources.Load ("prefabs/star"), ghostTiles [0, 0].transform);
				Direction randomDirection = Direction.up;
				switch ((int)(Random.value * 4)) {
				case 0:
					randomDirection = Direction.up;
					break;
				case 1:
					randomDirection = Direction.down;
					break;
				case 2:
					randomDirection = Direction.left;
					break;
				case 3:
					randomDirection = Direction.right;
					break;
				}
				star.GetComponent<Item> ().direction = randomDirection;
				addItem (star, (int)(Random.value * boardSize), (int)(Random.value * boardSize));
			}
			foreach (ContainerRandomizer c in containers) {
				if (c != null && c.placeItemsRandomly ())
					break;
			}
		}
	}

	public void refreshScore() {
		scoreText.text = GameManager.instanse.score + "";
		if (GameManager.instanse.score > PlayerPrefs.GetInt ("high score", 0)) {
			PlayerPrefs.SetInt ("high score", GameManager.instanse.score);
			highScoreText.text = PlayerPrefs.GetInt ("high score", 0) + "";
		}
	}
	/**
	 * <summary>
	 * Changes the parent of the given item to a tile with the given coordinates.
	 * </summary>
	 * <param name = "_item">The item to move.<param>
	 * <param name = "_x">The tile x coordinate.<param>
	 * <param name = "_y">The tile y coordinate.<param>
	 * */
	public void changeParent(Item _item, int _x, int _y) {
		if (_x >= 0 && _x < boardSize && _y >= 0 && _y < boardSize) {
			_item.transform.SetParent (tiles [_x, _y].transform, false);
		} else {
			destroyItem (_item);
		}
	}
	/// <summary>
	/// Destroies the item and removes it from the list.
	/// </summary>
	/// <param name="_item">The item to be removed.</param>
	public void destroyItem(Item _item) {
		print ("destroy " + _item.transform.name);
		if (items.Contains (_item)) {
			items.Remove (_item);
		}
		_item.destroy ();
	}
	/// <summary>
	/// Clears the board.
	/// </summary>
	private void clearBoard() {
		items.ForEach (delegate(Item obj) {
			try {
			Destroy (obj.gameObject);
			} catch {
			}
		});
		items = new List<Item> ();
	}
	/// <summary>
	/// Initializes the level.
	/// </summary>
	public void initializeLevel() {
		if (autoMove) {
			toggleAutoMove ();
		}
		GameManager.instanse.gameState = GameState.playing;
		GameManager.instanse.gameMode = GameMode.levels;
		levelNumber.transform.parent.gameObject.SetActive (true);
		highScoreText.transform.parent.gameObject.SetActive (false);
		scoreText.transform.parent.gameObject.SetActive (false);
		moveButtons.SetActive (true);
		editorButtons.SetActive (false);
		editButton.SetActive (false);
		victory.SetActive (false);
		clearBoard ();
		if (inventory != null) {
			Destroy (inventory);
		}
		try {
			inventory = Instantiate ((GameObject)Resources.Load ("inventories/inventory " + level),canvase.transform);
		} catch {
			inventory = Instantiate ((GameObject)Resources.Load ("inventories/inventory end"),canvase.transform);
		}
		inventory.transform.SetSiblingIndex (2);
		levelNumber.text = level + "";
	}
	/// <summary>
	/// Initializes the level.
	/// </summary>
	/// <param name="_level">Level to initialize.</param>
	public void initializeLevel(int _level) {
		level = _level;
		initializeLevel ();
	}
	/// <summary>
	/// Loads the level.
	/// </summary>
	public void nextLevel() {
		level++;
		initializeLevel ();
	}

	public void win() {
		print ("victory");

		GameManager.instanse.gameState = GameState.victory;
		if (GameManager.instanse.gameMode == GameMode.levels) {
			PlayerPrefs.SetInt("reached level", Mathf.Max(PlayerPrefs.GetInt("reached level", 1),level + 1));
			victory.SetActive (true);
		} else if (GameManager.instanse.gameMode == GameMode.communityLevels) {
			api.solveLevelCurrentLevel ();
			communityVisctory.SetActive (true);
		}
	}

	public void lose() {
		if (GameManager.instanse.gameMode != GameMode.servival) {
			defeat.SetActive (true);
			GameManager.instanse.gameState = GameState.notPlaying;
		} else {
			int players = 0;
			foreach (Item i in items) {
				if (i.transform.tag.Equals ("Player"))
					players++;
			}
			print ("players: " + players);
			if (players <= 0) {
				GameManager.instanse.gameState = GameState.notPlaying;
			}
		}
	}
}
