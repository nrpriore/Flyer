using UnityEngine;					// To inherit from MonoBehaviour
using UnityEngine.UI;				// To display Canvas elements
using System.Collections.Generic;	// For lists

public class GameController : MonoBehaviour {

	public static PlayerController Player;
	public static BackgroundController Background;
	public static float Gravity;
	public static float Wind;

#region // AvgDeltaTime
	private const int NUM_FRAMES = 5;
	private List<float> _timeArray;
	private int _timeArrayIndex;
	private float _sumDeltaTime;
	public static float AvgDeltaTime;
#endregion

	// Test UI/misc vars
	public Text _gas;
	public Text _height;
	public Text _vel;
	public Text _fps;
	public Text _dist;


#region // Set/Get
	public static bool Playing {
		get{return (Player == null)? false : Player.enabled;}
	}
	// How much to translate the X position of moving objects each frame
	public static float XVel {
		get{return (Player != null)? -Player.XVel * AvgDeltaTime : 0f;}
	}
	// How much to translate the Y position of moving objects each frame
	public static float YVel {
		get{return (Player != null)? -Player.YVel * AvgDeltaTime : 0f;}
	}
#endregion

	void Start () {
	#region // AvgDeltaTime
		_timeArray = new List<float>();
		_timeArrayIndex = 0;
		_sumDeltaTime = 0;
	#endregion

		Gravity = 10;
		Wind = 0.8f;
	}

	void Update () {
	#region // AvgDeltaTime
		if(_timeArray.Count < NUM_FRAMES) {
			_sumDeltaTime += Time.deltaTime;
			_timeArray.Add(Time.deltaTime);
		}else{
			if(_timeArrayIndex == NUM_FRAMES) {
				_timeArrayIndex = 0;
			}
			_sumDeltaTime += Time.deltaTime - _timeArray[_timeArrayIndex];
			_timeArray[_timeArrayIndex] = Time.deltaTime;
			_timeArrayIndex++;
		}
		AvgDeltaTime = _sumDeltaTime / _timeArray.Count;
	#endregion

		// For development - update screen values and enable restart
		if(Playing) {
			float _UIGas 	= Player.CurGas;
			float _UIHeight = Player.Height;
			float _UIVel	= Player.Vel / 100f;
			float _UIDist 	= Player.Distance;

			_gas.text 		= "Gas: " + _UIGas;
			_height.text 	= "Height: " + _UIHeight;
			_vel.text 		= "Vel: " + _UIVel;
			_fps.text 		= "FPS: " + (1/AvgDeltaTime);
			_dist.text 		= ((int)_UIDist).ToString();
		}else {
			if(Input.touchCount > 0 || Input.GetKey("r")) {
				RestartGame();
			}
		}
	}

	public static void UpdateStatData() {
		PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + (int)(Player.Distance/50f));
	}

	// Initiate anything required to start the game
	private void InitGameObjects() {
		if(Player != null) {
			Destroy(Player.gameObject);
			Destroy(Background.gameObject);
			Destroy(gameObject.GetComponent<SpawnController>());
		}
		Instantiate(Resources.Load("Prefabs/Player"));
		Instantiate(Resources.Load("Prefabs/Background"));
		gameObject.AddComponent<SpawnController>();
	}

	// Restarts game straight from the crash screen
	private void RestartGame() {
		InitGameObjects();
	}

}
