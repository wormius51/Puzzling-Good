using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonArraySpliter {

	private static string cutEnds(string _s) {
		return _s.Substring (1, _s.Length - 2);
	}

	public static List<string> splitJsonArray(string _json) {
		List<string> jsons = new List<string> ();
		string json = cutEnds (_json);
		int startIndex = 0;
		int parenthesesCount = 0;
		int jsonLength = 1;
		for (int i = 0; i < json.Length; i++) {
			if (json [i].Equals ('{'))
				parenthesesCount++;
			else if (json [i].Equals ('}'))
				parenthesesCount--;
			if (parenthesesCount == 0) {
				jsons.Add (json.Substring (startIndex, jsonLength));
				jsonLength = 1;
				i ++;
				startIndex = i + 1;
			} else {
				jsonLength++;
			}
		}

		return jsons;
	}

	public static List<T> splitJsonArray<T>(string _json) {
		List<string> jsons = splitJsonArray (_json);
		List<T> objects = new List<T> ();
		foreach (string json in jsons) {
			objects.Add (JsonUtility.FromJson<T> (json));
		}
		return objects;
	}
}
