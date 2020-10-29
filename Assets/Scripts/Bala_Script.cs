using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala_Script : MonoBehaviour {
	public float speed = 6f;
	public float lifetime = 0.2f;
	public float direction = 1;
	public float fuerza = 1;


	Vector2 vel = new Vector2(1,0);
	Vector2 moveVector;
	Rigidbody2D rgb;

	// Use this for initialization
	void Start () {
		moveVector = direction * speed * vel.normalized;
		rgb = GetComponent<Rigidbody2D> ();
		Invoke ("KillMe", 2);
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag ("Walls")) {
			KillMe ();
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		rgb.velocity = moveVector;
	}

	public void DisparoIzq(){
		direction *= -1;
		GetComponent<Transform> ().localScale *= -1;
		//print ("derecha bpu");
	}

	public void AtaqueRecibido(Slime_Script script){
		KillMe ();
		script.FinAtaque ();
	}

	void KillMe(){
		Destroy (gameObject);
	}
}
