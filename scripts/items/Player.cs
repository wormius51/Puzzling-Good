using System.Collections;
using UnityEngine;
/**
 * Controlls the players movement.
 * */
public class Player : Item {

	public bool stepEnd = false;


	/**
	 * <summary>
	 * makeMove is called once per turn.
	 * </summary>
	 * */
	protected override void makeMove() {
		if (GameManager.instanse.gameState == GameState.playing)
			GameManager.instanse.score++;
		itemState = ItemState.inMove;
		GetComponent<Collider2D> ().enabled = false;
		transform.SetParent (PositionManager.instance.ghostTiles [x, y].transform, false);
		animator.SetBool ("step", true);
	}

	protected override void onDestroy ()
	{
		if (transform.tag == "Player")
			PositionManager.instance.lose ();
		print (transform.name + " death");
	}

	/**
     * <summary>
	 * The player takes a step to the direction its pointed.
	 * </summary>
	 * */
	public void step() {
		/*if (itemState == ItemState.rest)
			return;*/
		animator.SetBool ("step", false);
		if (GetComponent<Killer> () != null) {
			GetComponent<Killer> ().enabled = true;
		}
		GetComponent<Collider2D> ().enabled = true;
		itemState = ItemState.rest;
		switch (direction) {
		case Direction.up:
			y--;
			break;
		case Direction.right:
			x++;
			break;
		case Direction.down:
			y++;
			break;
		case Direction.left:
			x--;
			break;
		}
		PositionManager.instance.changeParent (this, x, y);
	}
}
