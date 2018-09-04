using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bomb : Item {


	public GameObject explosionPrefab;
	public int countdown = 3;

	protected override void makeMove ()
	{
		countdown--;
		transform.GetChild (0).GetChild(0).GetComponent<Text> ().text = countdown + "";
		if (countdown <= 0) {
			StartCoroutine (selfDestruct ());
		}
	}

	IEnumerator selfDestruct() {
		yield return new WaitForEndOfFrame ();
		PositionManager.instance.destroyItem (this);
	}

	protected override void start ()
	{
		transform.GetChild (0).GetChild(0).GetComponent<Text> ().text = countdown + "";
	}

	protected override void onDestroy ()
	{
		if (!isLocked())
			Instantiate (explosionPrefab, transform.parent);
	}
}
