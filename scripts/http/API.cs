using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class API : MonoBehaviour {

	public string userControllerUrl = "/user";
	public string puzzlingGoodControllerUrl = "/puzzling-good";

	public Transform canvas;
	public Text requestStatus;
	public Text registerRequestStatus;
	public Text displayName;
	public InputField inputName;
	public InputField registerEmail;
	public InputField registerPassword;
	public InputField email;
	public InputField password;
	public GameObject signInButton;
	public GameObject signOutButton;
	public GameObject deleteButton;
	public Color creatorColor;
	public User user {get; private set;}

	// Use this for initialization
	void Start () {
		email.text = PlayerPrefs.GetString ("email", "");
		password.text = PlayerPrefs.GetString ("password", "");
		if (email.text != "")
			login (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#region http requests
	public void login(bool toMenu) {
		requestStatus.text = "login in...";
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("email", email.text);
		parameters.Add ("password", password.text);
		HttpClient.instance.login (userControllerUrl + "/login", parameters, delegate(string response) {
			try {
			user = JsonUtility.FromJson<User>(response);
			} catch {
				displayName.text = "User not found";
				requestStatus.text = response;
				return;
			}
			PlayerPrefs.SetString("email",email.text);
			PlayerPrefs.SetString("password",password.text);
			signOutButton.SetActive(true);
			signInButton.SetActive(false);
			if (Menu.instance == null)
				return;
			Menu.instance.closeUserMenu();
			if (toMenu && GameManager.instanse.gameMode != GameMode.levelEditor)
				Menu.instance.openMenu();
		});
	}

	public void logout() {
		user = null;
		HttpClient.instance.logout (userControllerUrl + "/logout");
		signOutButton.SetActive(false);
		signInButton.SetActive(true);
		requestStatus.text = "";
	}

	public void register() {
		registerRequestStatus.text = "registering...";
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("displayName", inputName.text);
		parameters.Add ("email", registerEmail.text);
		parameters.Add ("password", registerPassword.text);
		HttpClient.instance.login (userControllerUrl + "/register", parameters, delegate(string response) {
			if (response.Equals("registered")) {
				registerRequestStatus.text = "registered as " + inputName.name;
				email.text = registerEmail.text;
				password.text = registerPassword.text;
				login(true);
				Menu.instance.closeRegisterMenu();
			} else {
				registerRequestStatus.text = "faild to register";
			}
		});
	}

	public void submitLevel() {
		if (!HttpClient.instance.isSignedIn) {
			Menu.instance.openUserMenu();
			requestStatus.text = "can't submit when not logged in.";
			return;
		}
		PositionManager.instance.unshowSubmitPanel ();
		Level level = LevelEditor.instance.getLevel ();
		print (level);
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("levelName", level.name);
		parameters.Add ("content", level.ToString ());
		HttpClient.instance.request (puzzlingGoodControllerUrl + "/submitLevel", parameters, delegate(string response) {
			
		});
	}

	public void getAllLevels() {
		HttpClient.instance.request (puzzlingGoodControllerUrl + "/getAllLevels", delegate(string response) {
			renderLevels(response);
		});
	}

	public void testMethod() {
		getLevelsByParameters (0, -1, 0, 0, 0, "");
	}

	public void getLevelsByParameters(int page, int pageLength,int playesDirection,
		int likesDirection, float solvePrecentage, String searchWord) {
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("page", page + "");
		parameters.Add ("pageLength", pageLength + "");
		parameters.Add ("playesDirection", playesDirection + "");
		parameters.Add ("likesDirection", likesDirection + "");
		parameters.Add ("solvePrecentage", solvePrecentage + "");
		parameters.Add ("searchWord", searchWord);
		HttpClient.instance.request (puzzlingGoodControllerUrl + "/getLevelsByParameters", parameters, delegate(string response) {
			renderLevels(response);
		});
	}

	public void getAllLevelsSortedByLikes(int direction) {
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("direction", direction + "");
		HttpClient.instance.request (puzzlingGoodControllerUrl + "/getLevelsSortedByLikes",parameters, delegate(string response) {
			renderLevels(response);	
		});
	}

	public void getAllLevelsSortedByPlayes(int direction) {
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("direction", direction + "");
		HttpClient.instance.request (puzzlingGoodControllerUrl + "/getLevelsSortedByPlayes",parameters, delegate(string response) {
			renderLevels(response);	
		});
	}

	public void getLevelsBySolvePrecentage(float solvePrecentage) {
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("solvePercentage", solvePrecentage + "");
		HttpClient.instance.request (puzzlingGoodControllerUrl + "/getLevelsSortedBySolvePercentage", parameters, delegate(string response) {
			renderLevels(response);
		});
	}

	public void getLevelsByCreatorId(long id) {
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("creatorId", id + "");
		HttpClient.instance.request (puzzlingGoodControllerUrl + "/getLevelsByCreatorId", parameters, delegate(string response) {
			renderLevels(response);
		});
	}

	public void followCreatorById(long id) {
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("followedId", id + "");
		HttpClient.instance.request (userControllerUrl + "/followById", parameters, delegate(string response) {
			
		});
	}

	public void getLevelsBySearchWord(String searchWord) {
		if (searchWord.Equals ("")) {
			getAllLevels ();
			return;
		}
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("searchWord", searchWord);
		HttpClient.instance.request (puzzlingGoodControllerUrl + "/getLevelsBySearchWord", parameters, delegate(string response) {
			renderLevels(response);
		});
	}

	public void getFollowedCreatores() {
		if (!HttpClient.instance.isSignedIn)
			return;
		HttpClient.instance.request (userControllerUrl + "/getFollowingList", delegate(string response) {
			print (response);
			List<User> creators = JsonArraySpliter.splitJsonArray<User> (response);
			if (creators == null) {
				print ("no followed creators");
				return;
			}
			Menu.instance.clearCommunityLevelsMenu();
			foreach (User creator in creators) {
				Menu.instance.showCreatorButton (creator.id, creator.name);
			}
		});
	}

	public void playLevel(long levelId) {
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("levelId", levelId + "");
		HttpClient.instance.request (puzzlingGoodControllerUrl + "/playLevel", parameters, delegate(string response) {
			
		});
	}

	public void solveLevel(long levelId) {
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("levelId", levelId + "");
		HttpClient.instance.request (puzzlingGoodControllerUrl + "/solveLevel", parameters, delegate(string response) {
			
		});
	}

	public void likeLevel(long levelId) {
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("levelId", levelId + "");
		HttpClient.instance.request (puzzlingGoodControllerUrl + "/likeLevel", parameters, delegate(string response) {
			
		});
	}

	public void didILike(long levelId) {
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("levelId", levelId + "");
		HttpClient.instance.request (puzzlingGoodControllerUrl + "/didILike", parameters, delegate(string response) {
			if (response.Equals("no")) {
				Menu.instance.openLikeButton();
			}
		});
	}

	public void removeLevel(long levelId) {
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("levelId", levelId + "");
		HttpClient.instance.request (puzzlingGoodControllerUrl + "/removeLevel", parameters, delegate(string response) {
			print (response);	
		});
	}

	public void getHighestScore() {
		HttpClient.instance.request (puzzlingGoodControllerUrl + "/getHighestScore", delegate(string response) {
			try {
				HighScore h = JsonUtility.FromJson<HighScore>(response);
				ElementReferences.instance.getGameObject("highscore text").GetComponent<Text>().text = h.player.name + ": " + h.score;
			} catch {
			}
		});
	}

	public void submitHighScore() {
		Dictionary<string,string> parameters = new Dictionary<string, string> ();
		parameters.Add ("score", GameManager.instanse.score + "");
		HttpClient.instance.request (puzzlingGoodControllerUrl + "/submitHighScore",parameters,delegate(string response) {
			getHighestScore();	
		});
	}

	#endregion

	public void getMyLevels() {
		getLevelsByCreatorId (user.id);
	}

	public void getLevelsOfCurrentCreator() {
		getLevelsByCreatorId (PositionManager.instance.getCommunityLevel ().creatorID);
	}

	public void followCurrentCreator() {
		followCreatorById (PositionManager.instance.getCommunityLevel ().creatorID);
	}

	public void playCurrentLevel() {
		playLevel (PositionManager.instance.getCommunityLevel ().id);
	}

	public void solveLevelCurrentLevel() {
		solveLevel (PositionManager.instance.getCommunityLevel ().id);
	}

	public void likeLevelCurrentLevel() {
		likeLevel (PositionManager.instance.getCommunityLevel ().id);
	}

	public void didILikeCurrentLevel() {
		didILike (PositionManager.instance.getCommunityLevel ().id);
	}

	public void removeCurrentLevel() {
		removeLevel (PositionManager.instance.getCommunityLevel ().id);
	}

	private void renderLevels(string response) {
		List<JavaLevel> jlevels = JsonArraySpliter.splitJsonArray<JavaLevel>(response);
		foreach (JavaLevel jlevel in jlevels) {
			GameObject levelButton = (GameObject)Instantiate(ElementReferences.instance.getGameObject("button prefab"),ElementReferences.instance.getGameObject("community levels menu").transform.GetChild(0).GetChild(1));
			levelButton.GetComponentInChildren<Text>().text = 
				jlevel.name + " by " + jlevel.creator.name + "\nplayes: " +
				jlevel.playes + " solves: " + jlevel.solves + " likes: " + jlevel.likes;

			levelButton.GetComponent<Button>().onClick.AddListener(delegate {
				Level level = new Level(jlevel);
				PositionManager.instance.playCommunityLevel(level);
				Menu.instance.closeCommunityLevelsMenu();
				GameObject creatorButton = ElementReferences.instance.getGameObject("creator");
				creatorButton.SetActive(true);
				creatorButton.GetComponentInChildren<Text>().text = "Click here for more from " + level.creatorName;
				/*if (HttpClient.instance.isSignedIn && jlevel.creator.id == user.id) {
					GameObject delete = (GameObject)Instantiate(deleteButton,canvas);
					delete.GetComponent<Button>().onClick.AddListener(delegate {
						Menu.instance.openAreYouSureRemove();
					});
				}*/
			});
			if (HttpClient.instance.isSignedIn && jlevel.creator.id == user.id) {
				levelButton.GetComponentInChildren<Text> ().color = creatorColor;
			}
		}
	}
}
#region jsonators
[Serializable]
public class User {
	public long id;
	public string name;
	public int followers;
	public int following;
	public int solves;

	public override string ToString ()
	{
		return string.Format ("[User: id={0}, name={1}, followers={2}, following={3}, solves={4}]", id, name, followers, following, solves);
	}
}

public class JavaLevel {
	public long id;
	[SerializeField]
	public User creator;
	public string name;
	public string content;
	public int playes;
	public int solves;
	public int likes;

	public override string ToString ()
	{
		return string.Format ("[JavaLevel: id={0}, creator={1}, name={2}, content={3}, playes={4}, solves={5}, likes={6}]", id,creator,name,content,playes,solves,likes);
	}
}

public class HighScore {
	public User player;
	public int score;

	public override string ToString ()
	{
		return string.Format ("[HighScore: player={0}, score={1}]", player, score);
	}
}
#endregion