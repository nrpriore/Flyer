using UnityEngine;					// To inherit from MonoBehaviour

public class SpawnController : MonoBehaviour {

	private float _timer;
	private float _interval;

	private bool AtGroundLevel {
		get{return GameController.Background.gameObject.transform.Find("0") != null;}
	}

	void Start () {
		_timer = 0;
		_interval = 2f;
	}

	void Update () {
		if(GameController.Playing) {
			_timer += GameController.AvgDeltaTime;
			if(_timer >= _interval) {
				_timer = 0;
				Spawn();
			}
		}
	}

	private void Spawn() {
		float _spawnY = GameController.Player.gameObject.transform.localPosition.y + (((Random.value - 0.5f) + (GameController.Player.VelRatio * GameController.Player.RotRatio)) * 400);
		float _spawnX = Camera.main.orthographicSize * Camera.main.aspect + 200;
		Instantiate(Resources.Load("Prefabs/Objects/Gas"),new Vector3(_spawnX,_spawnY,0),Quaternion.identity);
	}

}
