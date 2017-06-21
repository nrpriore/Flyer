using UnityEngine;					// To inherit from MonoBehaviour

public class ObjectController : MonoBehaviour {

	public virtual void Start () {
		// Remove "(clone)" from name... silly Unity
		name = name.Substring(0, name.Length-7);
	}

	void Update () {
		// Translate in same fashion as the background and destroy when out of view
		gameObject.transform.Translate(GameController.XVel, GameController.YVel, 0);
		if(gameObject.transform.localPosition.x <= -Camera.main.orthographicSize * Camera.main.aspect - 200) {
			Destroy(gameObject);
		}
	}

}
