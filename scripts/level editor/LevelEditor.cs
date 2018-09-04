using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour {

	public static LevelEditor instance;

	private Level currectLevel;

	// Use this for initialization
	void Awake () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void clear() {
		currectLevel = new Level ();
	}

	public Level getLevel() {
		return currectLevel;
	}

	public void setLevelName(string _levelName) {
		if (currectLevel == null)
			return;
		currectLevel.name = _levelName;
	}

	/// <summary>
	/// Sets the level according to the current inventory.
	/// </summary>
	public void setLevel() {
		currectLevel = new Level (PositionManager.instance.getInventory ());
	}
	/// <summary>
	/// Custs the level into inventory.
	/// </summary>
	/// <returns>The level inventory as a GameObject.</returns>
	/// <param name="_level">Level.</param>
	/// <param name="_inventory">The inventory to cast the level to.</param>
	public void custLevelToInventory(Level _level, GameObject _inventory) {
		Transform main = _inventory.transform.GetChild (0);
		Transform secondery = _inventory.transform.GetChild (1);
		Transform third = _inventory.transform.GetChild (2);
		Transform[] containers = new Transform[5];
		containers [0] = main.GetChild (0).GetChild (0);
		containers [1] = main.GetChild (1).GetChild (0);
		containers [2] = main.GetChild (2).GetChild (0);
		containers [3] = secondery.GetChild (0);
		containers [4] = third.GetChild (0);

		for (int i = 0; i < containers.Length; i++) {
			Transform container = containers [i];
			for (int j = 0; j < container.childCount; j++) {
				Transform tile = container.GetChild (j);
				DataTile dataTile = _level.getDataTiles () [i, j];
				if (dataTile == null)
					continue;
				if (dataTile.items == null)
					continue;
				foreach (DataItem dataItem in dataTile.items) {
					if (dataItem == null)
						continue;
					GameObject itemPrefab = null;
					switch (dataItem.itemEnum) {
					case ItemEnum.arrow:
						itemPrefab = (GameObject)Resources.Load ("prefabs/randomable/arrow");
						break;
					case ItemEnum.arrowKiller:
						itemPrefab = (GameObject)Resources.Load ("prefabs/randomable/arrow_killer");
						break;
					case ItemEnum.enemy:
						itemPrefab = (GameObject)Resources.Load ("prefabs/randomable/enemy");
						break;
					case ItemEnum.goal:
						itemPrefab = (GameObject)Resources.Load ("prefabs/goal");
						break;
					case ItemEnum.key:
						itemPrefab = (GameObject)Resources.Load ("prefabs/randomable/key");
						break;
					case ItemEnum.locK:
						itemPrefab = (GameObject)Resources.Load ("prefabs/randomable/lock");
						break;
					case ItemEnum.player:
						itemPrefab = (GameObject)Resources.Load ("prefabs/player");
						break;
					case ItemEnum.rotatorClocwise:
						itemPrefab = (GameObject)Resources.Load ("prefabs/randomable/rotator_clockwise");
						break;
					case ItemEnum.rotatorCounterClocwise:
						itemPrefab = (GameObject)Resources.Load ("prefabs/randomable/rotator_counter_clockwise");
						break;
					case ItemEnum.skull:
						itemPrefab = (GameObject)Resources.Load ("prefabs/randomable/skull");
						break;
					case ItemEnum.bomb:
						itemPrefab = (GameObject)Resources.Load ("prefabs/randomable/bomb");
						break;
					case ItemEnum.mimic:
						itemPrefab = (GameObject)Resources.Load ("prefabs/randomable/mimic");
						break;
					}
					if (itemPrefab != null) {
						GameObject item = (GameObject) Instantiate (itemPrefab, tile);
						item.GetComponent<Item> ().direction = dataItem.direction;
					}
				}
			}
		}
	}
}
