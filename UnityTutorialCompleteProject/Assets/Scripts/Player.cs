using UnityEngine;
using System.Collections;

// this class requires a MoveMotor to actually move
[RequireComponent(typeof(MoveMotor))]
public class Player : MonoBehaviour {

	MoveMotor characterController;
	
	// 
	public Transform aimTarget;
	public Transform head;

	public GameObject bulletPrefab;
	public float bulletLifetime = 1;
	public float firingRate = 0.5f;
	float lastFireTime = 0;
	
	bool headFollowsCursor = true;
	bool gameOver = false;
	bool playerAlive = true;
	public LayerMask enemyLayer;
	
	public GameObject gameWorld;
	
	void Start() {
		characterController = GetComponent<MoveMotor>();
	}
	
	void Update () {
		if (!playerAlive) 
			return;
		
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
		
		characterController.movementDirection = velocity.normalized;
		
		if (Input.GetMouseButton(0) &&
			(Time.time - lastFireTime) > firingRate) 
		{
			lastFireTime = Time.time;
			FireBullet();
		}
	}
	
	void FireBullet()
	{
		GameObject newBullet = Instantiate(bulletPrefab, transform.position,
			Quaternion.LookRotation(transform.forward)) as GameObject;
		newBullet.rigidbody.AddForce(transform.forward * 40, ForceMode.Impulse);
		Destroy(newBullet, bulletLifetime);
	}

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
		if (Utility.IsInLayerMask(collision.gameObject, enemyLayer))
		{
			collider.enabled = false;
			playerAlive= false;
			gameWorld.SendMessage("PlayerDied");
			lives -= 1;
			StartCoroutine(PlayerDeath());	
		}
	}
			
	IEnumerator PlayerDeath()
	{
		rigidbody.isKinematic = true;
		float elapsed = 0;
		//float spinDuration = 1.0f;
		headFollowsCursor = false;
		while (elapsed < 1.0) {
			float spin = Mathf.Lerp(0, Mathf.PI * 6 * Time.deltaTime, elapsed);
			transform.RotateAround(Vector3.up, spin);
			elapsed += Time.deltaTime;
			yield return null;
		}
		
		elapsed = 0;
		while (elapsed < 0.5f) {
			transform.Translate(-Vector3.up * 6.0f * Time.deltaTime);
			elapsed += Time.deltaTime;
			yield return null;
		}
		
		yield return new WaitForSeconds(1.0f);
		
		transform.position = new Vector3(0,0.5f,0);
		while (elapsed < 1.0) {
			float spin = Mathf.Lerp(Mathf.PI * 6 * Time.deltaTime, elapsed, 0);
			transform.RotateAround(Vector3.up, spin);
			elapsed += Time.deltaTime;
			yield return null;
		}
		
		collider.enabled = true;
		rigidbody.isKinematic = false;
		
		if (lives > 0) {
			playerAlive = true;
			headFollowsCursor = true;
			gameWorld.SendMessage("PlayerStart", gameObject);
		} else {
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
