using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
	notPlaying,playing,victory,editing
}

public enum GameMode {
	levels,servival,levelEditor,communityLevels
}

public class GameManager {

	public static GameManager instanse = new GameManager ();

	private GameManager() {}

	public GameState gameState;

	public GameMode gameMode;

	public int score = 0;
}

