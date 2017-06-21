using UnityEngine;					// To inherit from MonoBehaviour

public class MenuObject : MonoBehaviour {

	void Update () {
		// Move objects up every frame
		gameObject.transform.localPosition += Vector3.up * Time.deltaTime;
		if(gameObject.transform.localPosition.y >= Camera.main.orthographicSize * 1.5f) {
			Destroy(gameObject);
		}
	}
}
