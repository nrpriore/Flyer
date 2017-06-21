using System.Collections.Generic;	// For lists
using UnityEngine;					// To inherit from MonoBehaviour

public class BackgroundController : MonoBehaviour {

	// Constants
	private const int HEIGHT = 3;	// Amount of sprites vertically in the initial background
	private const int WIDTH = 2;	// Amount of sprites horizontally in the initial background

	// Vars to control starting setup & movement
	private float _startX;			// Starting X position of the initial background
	private float _startY;			// Starting Y position of the initial background
	private float _bgWidth;			// Width of the background sprites
	private float _bgHeight;		// Height of the background sprites

	// Dynamically choose backgrounds and save a history to recreate as you move up and down
	private int _numChoices;
	private List<string> _historyMap;

	// Holds the background gameobjects
	private GameObject[] _background;


#region // Set/Get
	// The height index of the input background
	private int HeightIndex(GameObject _go) {
		return int.Parse(_go.name);
	}
#endregion

	void Start () {
		// Set background positions based on phone resolution
		_startY = -Camera.main.orthographicSize;
		_startX = _startY * Camera.main.aspect;
		// Dynamically build numChoices so adding backgrounds only requires the addition of the sprite, no code changes
		int cnt = 0;
		while(true) {
			if(Resources.Load("Sprites/Backgrounds/" + cnt) == null) {
				_numChoices = cnt-1;
				break;
			}
			cnt++;
		}
		// Init vars
		_historyMap = new List<string>();
		_background = new GameObject[HEIGHT * WIDTH];

		// Build the background
		SetInitialBackground();

		GameController.Background = this;
	}

	// Builds the initial background
	private void SetInitialBackground() {
		// Manually add the first to initialize vars
		GameObject _ground = new GameObject();
		_ground.name = "0";
		_ground.transform.parent = gameObject.transform;
		SpriteRenderer _sr = _ground.AddComponent<SpriteRenderer>();
		_historyMap.Add("0"); // Change this to alter the starting ground sprite
		_sr.sprite = Resources.Load<Sprite>("Sprites/Backgrounds/" + _historyMap[0]);
		_ground.transform.localPosition = new Vector3(_startX,_startY,0);
		_background[0] = _ground;
		// Set vars to make the next loop faster
		_bgWidth = _sr.bounds.size.x;
		_bgHeight = _sr.bounds.size.y;
		// Loop through and create remaining backgrounds
		for(int cnt = 1; cnt < _background.Length; cnt++) {
			GameObject _back = new GameObject();
			_back.name = (cnt/WIDTH).ToString();
			_back.transform.parent = gameObject.transform;
			_sr = _back.AddComponent<SpriteRenderer>();
			// If the start of a new row, choose a random background sprite and save history
			if(cnt%WIDTH == 0) {
				IncreaseHistoryMap();
			}
			_sr.sprite = Resources.Load<Sprite>("Sprites/Backgrounds/" + _historyMap[cnt/WIDTH]);
			_back.transform.localPosition = new Vector3(_startX + (_bgWidth * (cnt%WIDTH)),_startY + (_bgHeight * (cnt/WIDTH)),0);
			_background[cnt] = _back;
		}
	}

	void Update () {
		if(GameController.Playing) {
			// Translate background, move them when off screen, and set new sprites as you move up and down
			for(int cnt = 0; cnt < _background.Length; cnt++) {
				_background[cnt].transform.Translate(GameController.XVel, GameController.YVel, 0);
				if(_background[cnt].transform.localPosition.x < _startX - _bgWidth) {
					_background[cnt].transform.Translate(_bgWidth * WIDTH,0,0);
				}
				if(_background[cnt].transform.localPosition.y < (2 * _startY) - _bgHeight) {
					TranslateUp(_background[cnt]);
				}else if(_background[cnt].transform.localPosition.y > (2 * _startY) + (_bgHeight * (HEIGHT-1))) {
					TranslateDown(_background[cnt]);
				}
			}
			GameController.Player.GetComponent<PlayerController>().UpdatePlayer();
		}
	}

	// Move the background to the top when it goes too low
	private void TranslateUp(GameObject _go) {
		// If it's a new level, choose background and add to history, else set it to current history
		int _newInd = HeightIndex(_go) + HEIGHT;
		if(_historyMap.Count <= _newInd) {
			IncreaseHistoryMap();
		}
		_go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Backgrounds/" + _historyMap[_newInd]);
		_go.transform.Translate(0,_bgHeight * HEIGHT, 0);
		_go.name = _newInd.ToString();
	}
	// Move the bavkground to the bottom when it goes too high
	private void TranslateDown(GameObject _go) {
		// There will always be a history going down (since we came from there!) so just set it to that
		int _newInd = HeightIndex(_go) - HEIGHT;
		if(_newInd >= 0) {
			_go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Backgrounds/" + _historyMap[_newInd]);
			_go.transform.Translate(0,-_bgHeight * HEIGHT, 0);
			_go.name = _newInd.ToString();
		}
	}

	// Method to choose background. Currently is random
	private void IncreaseHistoryMap() {
		_historyMap.Add((1 + (int)(0.999 * Random.value * _numChoices)).ToString());
	}

}
