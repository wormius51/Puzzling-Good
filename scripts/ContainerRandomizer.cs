using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerRandomizer : MonoBehaviour {
	public int numberOfItems = 1;

	private GameObject containor = null;
	private GameObject[] randomables;
	// Use this for initialization
	void Start () {
		randomables = System.Array.ConvertAll(Resources.LoadAll ("prefabs/randomable"), item => (GameObject)item);
		placeItemsRandomly ();
	}
	/// <summary>
	/// Places the items randomly.
	/// </summary>
	/// <returns><c>true</c>, if the slot was available and items were placed, <c>false</c> otherwise.</returns>
	public bool placeItemsRandomly() {
		if (containor != null)
			return false;
		containor = (GameObject)Instantiate(Resources.Load("prefabs/item containor"),transform);
		for (int i = 0; i < numberOfItems; i++) {
			if (i == 0) {
				placeItemRandomly (0);
			} else {
				placeItemRandomly ();
			}
		}
		return true;
	}

	private void placeItemRandomly() {
		placeItem ((int)(Random.value * randomables.Length), (int)(Random.value * 9));

	}

	private void placeItemRandomly(int _itemNumber) {
		placeItem (_itemNumber, (int)(Random.value * 9));
	}

	private void placeItem(int _itemNumber, int _tileNumber) {
		Transform tile = containor.transform.GetChild (_tileNumber);
		if (tile.childCount > 0) {
			placeItemRandomly (_itemNumber);
			return;
		}
		GameObject randomable = Instantiate (randomables [_itemNumber],tile);
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
		randomable.GetComponent<Item> ().direction = randomDirection;

	}
}
