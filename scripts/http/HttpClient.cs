using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HttpClient : MonoBehaviour {
	public static HttpClient instance {get; private set;}
	public bool localhost = true;
	public string localUrl = "https://localhost";
	public string backendUrl;
	public bool isSignedIn {get; private set;}
	public string sessionId {get; private set;}

	/// <summary>
	/// A class used to set methods to be executed, useing delegates.
	/// </summary>
	public delegate void oparation(string response);
	// Use this for initialization
	void Awake () {
		if (instance != null)
			Destroy (gameObject);
		instance = this;
		DontDestroyOnLoad (gameObject);
		sessionId = "";
		isSignedIn = false;
	}

	public void logout(string _url) {
		request (_url, delegate(string response) {
			
		});
		sessionId = "0";
		isSignedIn = false;
	}
	
	public void request(string _url,Dictionary<string,string> _params,oparation _callback) {
		WWWForm form = new WWWForm ();
		Dictionary<string,string> headers = new Dictionary<string, string> ();
		headers.Add ("sessionId", sessionId);
		headers.Add ("apiKey", ServerData.apiKey);
		foreach (KeyValuePair<string,string> param in _params) {
			form.AddField (param.Key, param.Value);
		}
		string url = backendUrl;
		if (localhost)
			url = localUrl;
		WWW www = new WWW (url + _url, form.data,headers);
		StartCoroutine (sendRequest (www,_callback));
	}

	public void request(string _url,oparation _callback) {
		WWWForm form = new WWWForm ();
		form.AddField ("", "");
		Dictionary<string,string> headers = new Dictionary<string, string> ();
		headers.Add ("sessionId", sessionId);
		headers.Add ("apiKey", ServerData.apiKey);
		string url = backendUrl;
		if (localhost)
			url = localUrl;
		WWW www = new WWW (url + _url,form.data,headers);
		StartCoroutine (sendRequest (www,_callback));
	}

	private IEnumerator sendRequest(WWW _www,oparation _callback) {
		yield return _www;
		print (_www.text);
		_callback (_www.text);
		_www.Dispose();
	}

	public void login(string _url,Dictionary<string,string> _params,oparation _callback) {
		WWWForm form = new WWWForm ();
		foreach (KeyValuePair<string,string> param in _params) {
			form.AddField (param.Key, param.Value);
		}
		Dictionary<string,string> headers = new Dictionary<string, string> ();
		headers.Add ("sessionId", sessionId);
		headers.Add ("apiKey",ServerData.apiKey);
		string url = backendUrl;
		if (localhost)
			url = localUrl;
		WWW www = new WWW (url + _url, form.data,headers);
		StartCoroutine (sendLogin (www,_callback));
	}

	private IEnumerator sendLogin(WWW _www,oparation _callback) {
		yield return _www;
		if (_www.responseHeaders.ContainsKey ("sessionId") && (_www.error == null || _www.error.Equals(""))) {
			sessionId = _www.responseHeaders ["sessionId"];
			isSignedIn = true;
		}
		print (_www.text);
		_callback (_www.text);
		_www.Dispose();
	}
}