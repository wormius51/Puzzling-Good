using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
	public static Menu instance;

	public API api;
	private int levelsPage = 0;
	private int levelsPageLength = 10;
	private ElementReferences references;
	void Awake() {
		instance = this;
	}

	void Start() {
		
	}

	public void setFirstPage() {
		levelsPage = 0;
	}

	public void showLevelsByParameters() {
		string searchWord = references.getGameObject ("searchWord").GetComponentInChildren<InputField> ().text;
		int playesDirection = (int)references.getGameObject ("playes").GetComponent<Slider> ().value;
		int likesDirection = (int)references.getGameObject ("likes").GetComponent<Slider> ().value;
		float solvePrecentage = references.getGameObject ("solvePrecentage").GetComponent<Slider> ().value;
		api.getLevelsByParameters (levelsPage, levelsPageLength, playesDirection, likesDirection, solvePrecentage, searchWord);
		levelsPage++;
	}

	public void quit() {
		Application.Quit ();
	}

	public void openMenu() {
		references = ElementReferences.instance;
		gameObject.SetActive (true);
		if (HttpClient.instance.isSignedIn) {
			references.getGameObject("user stats button").SetActive (true);
			references.getGameObject("user stats button").GetComponentInChildren<Text> ().text = api.user.name;
		} else {
			references.getGameObject("user stats button").SetActive (false);
		}
	}

	public void resume() {
		gameObject.SetActive (false);
	}

	public void openLevelsMenu() {
		references.getGameObject("levels menu").SetActive (true);
	}

	public void closeLevelsMenu() {
		references.getGameObject("levels menu").SetActive (false);
	}

	public void closeComunityVictiory() {
		references.getGameObject("community victory").SetActive (false);
	}

	public void closeUserStatsButton() {
		references.getGameObject("user stats button").SetActive (false);
	}

	public void openLikeButton() {
		references.getGameObject("like button").SetActive (true);
	}

	public void closeLikeButton() {
		references.getGameObject("like button").SetActive (false);
	}

	public void initializeLevel(int _level) {
		PositionManager.instance.initializeLevel (_level);
		closeLevelsMenu ();
		references.getGameObject("creator").SetActive (false);
		references.getGameObject("like button").SetActive (false);
		resume ();
	}

	public void startServival() {
		PositionManager.instance.startServival ();
		references.getGameObject("creator").SetActive (false);
		references.getGameObject("like button").SetActive (false);
		resume ();
	}

	public void startLevelEditor() {
		PositionManager.instance.startLevelEditor ();
		references.getGameObject("creator").SetActive (false);
		references.getGameObject("like button").SetActive (false);
		resume ();
	}

	public void openCommunityLevelsMenu() {
		references.getGameObject("community levels menu").SetActive (true);
		references.getGameObject("followed creators").SetActive (HttpClient.instance != null && HttpClient.instance.isSignedIn);
	}

	public void closeCommunityLevelsMenu() {
		references.getGameObject("community levels menu").SetActive (false);
		clearCommunityLevelsMenu ();
	}

	public void clearCommunityLevelsMenu() {
		foreach (Transform child in references.getGameObject("community levels menu").transform.GetChild(0).GetChild(1)) {
			Destroy (child.gameObject);
		}
	}

	public void openAreYouSureClear() {
		references.getGameObject("are you sure clear").SetActive (true);
	}

	public void closeAreYouSureClear() {
		references.getGameObject("are you sure clear").SetActive (false);
	}

	public void openAreYouSureRemove() {
		references.getGameObject("are you sure remove").SetActive (true);
	}

	public void closeAreYouSureRemove() {
		references.getGameObject("are you sure remove").SetActive (false);
	}

	public void openItemDescription(string _description) {
		references.getGameObject("item description").SetActive (true);
		references.getGameObject("item description").
		transform.GetChild(0).GetComponent<Text> ().text = _description;
	}

	public void closeItemDescription() {
		references.getGameObject("item description").SetActive (false);
	}

	public void showFollowedCreators() {
		api.getFollowedCreatores ();
	}

	public void showCreatorButton(long _creatorId, string _creatorName) {
		GameObject creatorButton = (GameObject)Instantiate(references.getGameObject("button prefab"),references.getGameObject("community levels menu").transform.GetChild(0).GetChild(1));
		creatorButton.GetComponent<Button> ().onClick.AddListener (delegate {
			clearCommunityLevelsMenu();
			api.getLevelsByCreatorId(_creatorId);
		});
		creatorButton.GetComponentInChildren<Text> ().text = _creatorName;
	}

	public void openUserStats() {
		if (!HttpClient.instance.isSignedIn)
			return;
		references.getGameObject("user stats").SetActive (true);
		references.getGameObject("display name").GetComponentInChildren<Text>().text = api.user.name;
		references.getGameObject("followers").GetComponentInChildren<Text>().text = "followers\n" + api.user.followers;
		references.getGameObject("following").GetComponentInChildren<Text>().text = "following\n" + api.user.following;
	}

	public void closeUserStats() {
		references.getGameObject("user stats").SetActive (false);
	}

	public void openUserMenu() {
		references.getGameObject("user menu").SetActive (true);
	}

	public void closeUserMenu() {
		references.getGameObject("user menu").SetActive (false);
	}

	public void openRegisterMenu() {
		references.getGameObject("register menu").SetActive (true);
	}

	public void closeRegisterMenu() {
		references.getGameObject("register menu").SetActive (false);
	}

	#region options

	public void changeStepsPerSecond(float _stepsPerSecond) {
		Item.StepsPerSecond = _stepsPerSecond;
		PlayerPrefs.SetFloat ("steps per second", Item.StepsPerSecond);
	}

	#endregion

}