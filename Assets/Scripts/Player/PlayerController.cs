using UnityEngine;					// To inherit from MonoBehaviour

public class PlayerController : MonoBehaviour {

	public const float MAX_ROT = 60;			// Max rotation when the player is going up
	public const float MIN_ROT = -50;			// Min rotation when the player is going down
	private const float MAX_VEL = 2000;			// Max velocity

	private const float ROT_ACCEL_UP = 2f;		// Strength of rotation upwards when accelerating
	private const float ROT_CONST_DOWN = 0.7f;	// Strength of constant roatation downwards
	private const float ROT_FALL_DOWN = 1f;		// Strength of rotation when too slow and forced to curve down

	private const float INIT_ROT = 30;			// Initial rotation
	private const float INIT_VEL = 1500;		// Initial velocity
	private const float INIT_HEIGHT = 5;		// Initial height

	private float _power;			// Strength of increase in velocity when accelerating
	private float _maxGas;			// Maximum amount of gas player can hold
	private float _curGas;			// Current gas player has

	private float _vel;				// Current velocity of player
	private float _rot;				// Current rotation of player
	private float _height;			// Current height of player
	private float _distance;		// Current distance player has moved

#region // Set/Get
	// X portion of total velocity
	public float XVel {
		get{return Mathf.Cos(_rot * Mathf.PI / 180f) * _vel * 0.8f;}
	}
	// Y portion of total velocity
	public float YVel {
		get{return Mathf.Sin(_rot * Mathf.PI / 180f) * _vel;}
	}
	// Current velocity
	public float Vel {
		get{return _vel;}
	}
	// Ratio of current vel to max vel with modifier for object generation
	public float VelRatio {
		get{float mod = 1f; return (mod * Vel / MAX_VEL) + 1;}
	}
	// Change in speed based on gravity and current rotation
	private float YAccel {
		get{return YVel * GameController.Gravity / _vel;}
	}
	// Current rotation
	public float Rot {
		get{return _rot;}
	}
	// Ratio of current rot to max rot with modifier for object generation
	public float RotRatio {
		get{float mod = 1f; return mod * Rot / ((Rot > 0)? MAX_ROT : -MIN_ROT);}
	}
	// Current height
	public float Height {
		get{return _height;}
		set{_height = value;}
	}
	// Current Distance
	public float Distance {
		get{return _distance;}
		set{_distance = value;}
	}
	// Current Gas
	public float CurGas {
		get{return _curGas;}
		set{_curGas = value;}
	}

	// Determines if currently accelerating (supports keyboard and mobile)
	public bool Accelerating {
		get{return !TooSlow && HasGas && (Input.touchCount > 0 || Input.GetKey("space"));}
	}
	// Determines if player is currently crashed
	private bool Crashed {
		get{return _vel <= 0 || Height <= 0;}
	}
	// Determines if player is currently too slow
	private bool TooSlow {
		get{return _vel < 200;}
	}
	// Determines if player currently has gas
	private bool HasGas {
		get{return _curGas > 0;}
	}
#endregion


	void Start () {
		_power = 4f;
		_maxGas = 300;

		// Init vars
		_curGas = _maxGas;
		_rot = INIT_ROT;
		_vel = INIT_VEL;
		_height = INIT_HEIGHT;

		GameController.Player = this;
	}

	void Update () {
		if(!Crashed) {
			// Account for the constant forces
			gameObject.transform.eulerAngles = Vector3.forward * _rot;
			_vel -= YAccel;
			_rot -= ROT_CONST_DOWN;
			// If touching the screen (accelerating)
			if(Accelerating) {
				_rot += ROT_ACCEL_UP;
				_vel += _power;
				_curGas--;
			// If too slow, force rotation down
			}else if(TooSlow) {
				_rot -= ROT_FALL_DOWN;
			}else {
				_vel -= GameController.Wind;
			}
			// Clamp these 2 values to their valid ranges
			_vel = Mathf.Clamp(_vel, 100, MAX_VEL);
			_rot = Mathf.Clamp(_rot, MIN_ROT, MAX_ROT);
		}else{
			// Happens once on crash
			GameController.PostGame();
			_vel = 0;
			enabled = false;
		}
	}

	// Update player values after background moves (called from there)
	public void UpdatePlayer() {
		Height += YVel / 1000f;
		Distance += XVel / 1000f;
	}

	// Update player values after colliding with an object (called from respective object and passes in name)
	public void Collide(string _object) {
		switch(_object) {
			case "Gas":
				CurGas = Mathf.Clamp(CurGas + 50, 0, _maxGas);
				break;
		}
	}

}
