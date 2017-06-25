using UnityEngine; 					// To inherit from MonoBehaviour
using UnityEngine.UI;				// To access UI components
using System.Collections.Generic;	// For lists
using UnityEngine.EventSystems;		// To edit event system
using UnityEngine.SceneManagement;	// To switch scenes

public class MenuController : MonoBehaviour {

	private const float CLOUD_MAX 	= 4f;		// Max amount of time before spawning a cloud
	private const float CLOUD_INT 	= 1f;		// Interval to check for cloud spawn
	private const int XLIST_MAX		= 5;		// The limit of the _cloudXList count
	private const float POPUP_TIME 	= 0.2f;		// Time it takes to lerp open/close a popup

	private float _cloudTimer;			// Timer governing cloud spawns
	private float _cloudInterval;		// Timer governing interval to check for cloud spawn
	private float _prevCloudX;			// X position of previous cloud
	private List<float> _cloudXList;	// Average X positions of clouds to ensure even spawning
	private GameObject _cloudGO;		// Cloud game object to instantiate
	private float _rangeX;				// Possible range of X positions for cloud
	private float _xListAvg;			// The average which cloud spawning must abide by

	private RectTransform _settingsRT; 	// The transform of the settings screen
	private bool _settingsExpand;		// Is the settings screen expanding
	private bool _settingsCollapse;		// Is the settings screen collapsing
	private float _settingsTimer;		// Timer governing settings screen expansion

	private RectTransform _pregameRT;	// The transform of the pregame screen
	private RectTransform _playRT;		// The transform of the play button
	private bool _pregameExpand;		// Is the pregame screen expanding
	private bool _pregameCollapse;		// Is the pregame screen collapsing
	private float _pregameTimer;		// Timer governing pregame screen expansion
	public static bool InPreGame;		// Returns whether pregame screen is active

	private DataController Data;


#region // Set/Get
	// Returns whether a cloud should be spawned or not
	private bool DoSpawnCloud {
		get{return Random.value * CLOUD_MAX <= _cloudTimer;}
	}
	// Returns the average x values of the last specified number of clouds
	private float CloudXAvg {
		get{float sum = 0f;
			foreach(float x in _cloudXList){
				sum += x;
			}
			return sum / (float)_cloudXList.Count;
		}
	}
#endregion


	void Start () {
		// Init cloud variables
		_cloudTimer = 0f;
		_cloudInterval = _cloudTimer;
		_prevCloudX = 0f;
		_cloudXList = new List<float>();
		_cloudGO = Resources.Load("Prefabs/MainCloud") as GameObject;
		_rangeX = Camera.main.orthographicSize * Camera.main.aspect;
		_xListAvg = _rangeX / 3f;

		// Init button variables
		_settingsRT = (RectTransform)GameObject.Find("SettingsScreen").transform;
		_settingsExpand = false;
		_settingsCollapse = false;
		_pregameRT = (RectTransform)GameObject.Find("PregameScreen").transform;
		_pregameExpand = false;
		_pregameCollapse = false;
		_playRT = (RectTransform)GameObject.Find("Play").transform;

		// Init others
		InPreGame = false;
		Data = gameObject.GetComponent<DataController>();

		// Load & set player/screen data
		InitializeGame();
	}

	private void InitializeGame() {
		LoadStats();
		SetScreenData();
	}
	private void LoadStats() {
		if(!PlayerPrefs.HasKey("Initialized")) {
			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetString("Initialized", "true");
			PlayerPrefs.SetInt("Coins",100);
			PlayerPrefs.SetInt("Gems",100);
		}
	}
	private void SetScreenData() {
		Data.ShopCoins.GetComponent<Text>().text = PlayerPrefs.GetInt("Coins").ToString();
		Data.ShopGems.GetComponent<Text>().text = PlayerPrefs.GetInt("Gems").ToString();
	}

	public void ResetStats() {
		PlayerPrefs.DeleteAll();
		InitializeGame();
	}

	void Update () {
		// Spawn clouds
		_cloudTimer += Time.deltaTime;
		_cloudInterval += Time.deltaTime;
		if(_cloudInterval >= CLOUD_INT) {
			if(DoSpawnCloud) {
				SpawnCloud();
				_cloudTimer = 0f;
			}
			_cloudInterval -= CLOUD_INT;
		}

		// Expand or Collapse settings screen
		if(_settingsTimer >= POPUP_TIME) {
			_settingsExpand = false;
			_settingsCollapse = false;
		}else if(_settingsExpand) {
			_settingsTimer += Time.deltaTime;
			_settingsRT.localScale = new Vector3(
				Mathf.Lerp(_settingsRT.localScale.x,1f,_settingsTimer/POPUP_TIME),
				Mathf.Lerp(_settingsRT.localScale.y,1f,_settingsTimer/POPUP_TIME),
				1f);
		}else if(_settingsCollapse) {
			_settingsTimer += Time.deltaTime;
			_settingsRT.localScale = new Vector3(
				Mathf.Lerp(_settingsRT.localScale.x,0f,_settingsTimer/POPUP_TIME),
				Mathf.Lerp(_settingsRT.localScale.y,0f,_settingsTimer/POPUP_TIME),
				1f);
		}

		// Expand or Coolapse pregame screen
		if(_pregameTimer >= POPUP_TIME) {
			_pregameExpand = false;
			_pregameCollapse = false;
		}else if(_pregameExpand) {
			_pregameTimer += Time.deltaTime;
			_pregameRT.localScale = new Vector3(
				Mathf.Lerp(_pregameRT.localScale.x,1f,_pregameTimer/POPUP_TIME),
				Mathf.Lerp(_pregameRT.localScale.y,1f,_pregameTimer/POPUP_TIME),
				1f);
			_playRT.localPosition = new Vector3(
				Mathf.Lerp(_playRT.localPosition.x,-640f,_pregameTimer/POPUP_TIME),
				Mathf.Lerp(_playRT.localPosition.y,-570f,_pregameTimer/POPUP_TIME),
				0f);
			_playRT.sizeDelta = new Vector2(
				Mathf.Lerp(_playRT.sizeDelta.x,1100,_pregameTimer/POPUP_TIME),
				Mathf.Lerp(_playRT.sizeDelta.y,250,_pregameTimer/POPUP_TIME));
		}else if(_pregameCollapse) {
			_pregameTimer += Time.deltaTime;
			_pregameRT.localScale = new Vector3(
				Mathf.Lerp(_pregameRT.localScale.x,0f,_pregameTimer/POPUP_TIME),
				Mathf.Lerp(_pregameRT.localScale.y,0f,_pregameTimer/POPUP_TIME),
				1f);
			_playRT.localPosition = new Vector3(
				Mathf.Lerp(_playRT.localPosition.x,0,_pregameTimer/POPUP_TIME),
				Mathf.Lerp(_playRT.localPosition.y,-100,_pregameTimer/POPUP_TIME),
				0f);
			_playRT.sizeDelta = new Vector2(
				Mathf.Lerp(_playRT.sizeDelta.x,1500,_pregameTimer/POPUP_TIME),
				Mathf.Lerp(_playRT.sizeDelta.y,300,_pregameTimer/POPUP_TIME));
		}
	}

	public void TogglePregameScreen(string action) {
		EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
		if(InPreGame && action == "open") {
			PlayButton();
			return;
		}
		_pregameTimer = 0f;
		if(action == "open") {
			InPreGame = true;
			_pregameCollapse = false;
			_pregameExpand = true;
		}else if(action == "close") {
			InPreGame = false;
			_pregameExpand = false;
			_pregameCollapse = true;
		}
	}

	public void ToggleSettingsScreen(string action) {
		EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
		_settingsTimer = 0f;
		if(action == "open") {
			_settingsCollapse = false;
			_settingsExpand = true;
		}else if(action == "close") {
			_settingsExpand = false;
			_settingsCollapse = true;
		}
	}
	private void PlayButton() {
		SceneManager.LoadScene("Game",LoadSceneMode.Single);
	}

	// Spawns a cloud
	private void SpawnCloud() {
		float _y = -Camera.main.orthographicSize * 1.5f;
		float _x = GetNextX();
		while(TooClose(_x) || !TempXAvgOK(_x)) {
			_x = GetNextX();
		}
		_prevCloudX = _x;
		CloudXAvgAdd(_x);
		Instantiate(_cloudGO,new Vector3(_x,_y,0),Quaternion.identity);
	}

	// Gets the next X coordinate for a cloud
	private float GetNextX() {
		return -_rangeX + (2 * _rangeX * Random.value);
	}
	// Adds to the list of cloud X coordinates
	private void CloudXAvgAdd(float x) {
		if(_cloudXList.Count == XLIST_MAX) {
			_cloudXList.RemoveAt(0);
		}
		_cloudXList.Add(x);
	}
	// Is the new cloud too close to the last?
	private bool TooClose(float x) {
		return Mathf.Abs(x - _prevCloudX) < _cloudGO.GetComponent<SpriteRenderer>().bounds.size.x / 2f;
	}
	// Are the clouds' X positions correctly averaged?
	private bool TempXAvgOK(float x) {
		List<float> tempList = new List<float>(_cloudXList);
		if(tempList.Count == XLIST_MAX) {
			tempList.RemoveAt(0);
		}
		tempList.Add(x);
		float sum = 0f;
		foreach(float _x in tempList) {
			sum += _x;
		}
		return Mathf.Abs(sum / (float)tempList.Count) <= _xListAvg;
	}

}
