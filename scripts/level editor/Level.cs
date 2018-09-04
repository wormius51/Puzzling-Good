using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Object representation of a level.
/// </summary>
public class Level {
	/// <summary>
	/// The tiles in the level.
	/// </summary>
	private DataTile[,] dataTiles = new DataTile[5,9];
	public long id;
	public string name = "My Fisrt Level!";
	public string creatorName = "mr.DidntSignIn";
	public long creatorID = 0;
	public int highestItem = 0;
	public int playes = 0;
	public int solves = 0;
	public int likes = 0;
	public bool solved = false;
	public bool like = false;

	public Level() {

	}

	public DataTile[,] getDataTiles() {
		return dataTiles;
	}

	private string cutEnds(string _data) {
		if (_data.Length > 1) {
			string data = _data.Substring (1, _data.Length - 2);
			return data;
		}
		return _data;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Level"/> class according to the given data.
	/// </summary>
	/// <param name="_data">Data.</param>
	public Level(string _data) {
		_data = cutEnds (_data);
		string[] containers = _data.Split ('c');
		for (int i = 0; i < containers.Length; i++) {
			string container = containers [i];
			container = cutEnds (container);
			string[] tiles = container.Split ('t');
			for (int j = 0; j < tiles.Length; j++) {
				string tile = tiles [j];
				tile = cutEnds (tile);
				string[] items = tile.Split ('i');
				DataItem[] dataItems = new DataItem[items.Length];
				for (int k = 0; k < items.Length; k++) {
					string item = items [k];
					if (item.Equals ("")) {
						dataItems[k] = null;
						continue;
					}
					item = cutEnds (item);
					try {
						int itemEnumInt = System.Int32.Parse(item.Split('x')[0]);
						if (highestItem < itemEnumInt) {
							highestItem = itemEnumInt;
						}
						int directionInt = System.Int32.Parse(item.Split('x')[1]);
						ItemEnum itemEnum;
						try {
							itemEnum = (ItemEnum)itemEnumInt;
						} catch {
							throw new PuzzleGameExeption("Missing item.");
						}
						if (itemEnumInt >= System.Enum.GetNames(typeof(ItemEnum)).Length) {
							throw new PuzzleGameExeption("Missing item.");
						}

						Direction direction = (Direction)directionInt;
						dataItems[k] = new DataItem(itemEnum,direction);
					} catch {
						dataItems[k] = null;
					}
				}
				dataTiles [i, j] = new DataTile (dataItems);
			}
		}
	}

	public Level(GameObject _inventory) {
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
				DataItem[] dataItems = new DataItem[tile.childCount];
				for (int k = 0; k < tile.childCount; k++) {
					Item item = tile.GetChild (k).gameObject.GetComponent<Item> ();
					dataItems [k] = new DataItem (item.itemEnum, item.direction);
				}
				dataTiles [i, j] = new DataTile (dataItems);
			}
		}
	}

	public Level(JavaLevel _javaLevel) : this(_javaLevel.content){
		id = _javaLevel.id;
		name = _javaLevel.name;
		creatorName = _javaLevel.creator.name;
		creatorID = _javaLevel.creator.id;
		playes = _javaLevel.playes;
		solves = _javaLevel.solves;
		likes = _javaLevel.likes;
	}

	public override string ToString ()
	{
		string data = "{";
		for (int i = 0; i < dataTiles.GetLength (0); i++) {
			data += "{";

			for (int j = 0; j < dataTiles.GetLength (1); j++) {
				data += dataTiles [i, j] + "t";
			}

			data = data.Substring (0, data.Length - 1);
			data += "}c";
		}
		data = data.Substring (0, data.Length - 1);
		data += "}";
		return data;
	}
}
/// <summary>
/// Item enumeration. New items are added to the end of the list so they have bigger numbers.
/// player = 0,
/// goal = 1,
/// arrow = 2,
/// rotator = 3,
/// enemy = 4,
/// skull = 5,
/// arrowKiller = 6,
/// locK = 7,
/// key = 8.
/// </summary>
public enum ItemEnum {
	player = 0,
	goal = 1,
	arrow = 2,
	rotatorClocwise = 3,
	rotatorCounterClocwise = 4,
	enemy = 5,
	skull = 6,
	arrowKiller = 7,
	locK = 8,
	key = 9,
	bomb = 10,
	mimic = 11
}
/// <summary>
/// Represents an item in the level.
/// </summary>
public class DataItem {
	public ItemEnum itemEnum;
	public Direction direction;

	public DataItem (ItemEnum _itemEnum,Direction _direction) {
		itemEnum = _itemEnum;
		direction = _direction;
	}

	public override string ToString ()
	{
		return "{" + (int)itemEnum + "x" + (int)direction + "}";
	}
}
/// <summary>
/// Represents a tile in the level.
/// </summary>
public class DataTile{
	public DataItem[] items;

	public DataTile(DataItem[] _items) {
		items = _items;
	}

	public override string ToString ()
	{
		string data = "{";
		foreach (DataItem dataItem in items) {
			data += dataItem + "i";
		}
		if (items.Length > 0)
			data = data.Substring (0, data.Length - 1);
		data += "}";
		return data;
	}
}