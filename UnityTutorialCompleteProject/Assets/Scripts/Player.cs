using UnityEngine;
using System.Collections;

// this class requires a MoveMotor to actually move
[RequireComponent(typeof(MoveMotor))]
public class Player : MonoBehaviour {

	MoveMotor characterController;
	
	// 
	public Transform aimTarget;
	public Transform head;
	
	// the template from wich all bullets will be created
	// assigned in the editor to a prefab in the project tab.
	public GameObject bulletPrefab;
	
	// bullets will be automatically destroyed after this amount of time
	public float bulletLifetime = 1;
	
	// how long between another bullet can be fired
	public float firingRate = 0.5f;
	// keep track of how long since last bullet
	float lastFireTime = 0;
	
	// lots and lots of flags to keep track of player's game state
	bool headFollowsCursor = true;
	bool gameOver = false;
	bool playerAlive = true;
	
	// all enemies are on this layer, used to check for player collision again enemies
	public LayerMask enemyLayer;
	
	// reference to the game world root, used to send messages when the player dies or
	// starts a new life
	public GameObject gameWorld;
	
	void Start() {
		// get reference to the MoveMotor component once
		characterController = GetComponent<MoveMotor>();
	}
	
	void Update () 
	{
		if (!playerAlive) // can't move the player when dead
			return;
		
		// start off with zero velocity, and accumulate forces based on key presses
		Vector3 velocity = Vector2.zero;
		if (Input.GetKey(KeyCode.A)) {
			velocity.z += 1.0f;
		}
		if (Input.GetKey(KeyCode.D)) {
			velocity.z -= 1.0f;
		}
		if (Input.GetKey(KeyCode.W)) {
			velocity.x += 1.0f;
		}
		if (Input.GetKey (KeyCode.S)) {
			velocity.x -= 1.0f;
		}
		
		// set the motor movement direction to make it move
		// fun fact: some (early) games did not normalize their player's velocity vector
		// so you could actually move faster when travelling diagonally
		// http://tvtropes.org/pmwiki/pmwiki.php/Main/DiagonalSpeedBoost
		characterController.movementDirection = velocity.normalized;
		
		// if mouse button down, and enough time passed from last bullet fire, fire another one
		if (Input.GetMouseButton(0) &&
			(Time.time - lastFireTime) > firingRate) 
		{
			lastFireTime = Time.time;
			FireBullet();
		}
	}
	
	void FireBullet()
	{
		// instantiate the bullet prefab template the the player's current position,
		// and facing along the forward vector. 
		// note: creating and destroying bullets very quickly like this can lead to 
		// performance problems and garbage collector hitches. a better way would be to 
		// maintain a pool of bullet objects and re-use them here. (like UITableView of firearms)
		GameObject newBullet = Instantiate(bulletPrefab, transform.position,
			Quaternion.LookRotation(transform.forward)) as GameObject;
		
		// give the bullet an impulse force like a gun explosion to make it fly
		newBullet.rigidbody.AddForce(transform.forward * 40, ForceMode.Impulse);
		
		// bullets needs to be removed after a while or they'll start accumulating, slowing the game down
		Destroy(newBullet, bulletLifetime);
	}
	
	// rotate the player towards the cursor
	// actually half of this can be repalced by adding a LookAt script to the player
	void LateUpdate() 
	{
		if (headFollowsCursor) 
		{
			Vector3 toAim = (aimTarget.position - head.position);
			toAim.y = 0;
			
			head.LookAt( aimTarget, Vector3.up );
			head.transform.rotation *= Quaternion.AngleAxis(-90, Vector3.right);
			
			characterController.facingDirection = toAim;
		}
	}
	
	void OnCollisionEnter(Collision collision) 
	{
		// check if the player hit an enemy, if so start the death sequence
		if (Utility.IsInLayerMask(collision.gameObject, enemyLayer))
		{
			// disable collision and player controls when dying
			collider.enabled = false;
			playerAlive= false;
			// lets the world to remove all enemies and stop them spawning
			gameWorld.SendMessage("PlayerDied"); 
			lives -= 1;
			StartCoroutine(PlayerDeath());	
		}
	}
			
	// this is coroutine. it animates the player's death animation over time
	// when this function returns with with a "yield return", its local variables are 
	// saved, and it will resume execution after the yield statement
	IEnumerator PlayerDeath()
	{
		rigidbody.isKinematic = true;
		float elapsed = 0;
		
		headFollowsCursor = false;
		// spins really fast for 1 second
		while (elapsed < 1.0) {
			float spin = Mathf.Lerp(0, Mathf.PI * 6 * Time.deltaTime, elapsed);
			transform.RotateAround(Vector3.up, spin);
			elapsed += Time.deltaTime;
			yield return null; // yields function execution until next frame
		}
		
		// moves down 6 units over 0.5 seconds
		elapsed = 0;
		while (elapsed < 0.5f) {
			transform.Translate(-Vector3.up * 6.0f * Time.deltaTime);
			elapsed += Time.deltaTime;
			yield return null;
		}
		
		// wait for 1 second, while we catch a breath
		yield return new WaitForSeconds(1.0f);
		
		// return in glory for a new life
		transform.position = new Vector3(0,0.5f,0);
		while (elapsed < 1.0) {
			float spin = Mathf.Lerp(Mathf.PI * 6 * Time.deltaTime, elapsed, 0);
			transform.RotateAround(Vector3.up, spin);
			elapsed += Time.deltaTime;
			yield return null;
		}
		
		
		// re-enable collision, and return the rididbody to physics simulation
		collider.enabled = true;
		rigidbody.isKinematic = false;
		
		if (lives > 0) {
			playerAlive = true;
			headFollowsCursor = true;
			gameWorld.SendMessage("PlayerStart", gameObject);
		} else {
			// if no more lives left, game over
			Debug.Log ("Game Over!");
			gameOver = true;
			//gameObject.SetActive(false);
		}
	}		
	
	public int lives = 3;
	public int score = 10;
	
	public GUIStyle guiStyle;
	public GUIStyle gameOverStyle;
	
	void OnGUI() {
		GUI.Label(new Rect(10,10,150,50),
			string.Format ("Lives:{0}", lives), guiStyle);
		GUI.Label(new Rect(10,40,150,50),
			string.Format ("Score:{0}", score), guiStyle);
		
		if (gameOver) {
			GUI.Label(new Rect(0,0,Screen.width,Screen.height),
				"Game Over", gameOverStyle);
		}
	}
}
