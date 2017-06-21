public class Gas : ObjectController {

	public override void Start () {
		base.Start();

	}

	// Triggers on collision with player
	void OnTriggerEnter2D() {
		Destroy(gameObject);
		GameController.Player.Collide(name);
	}
}
