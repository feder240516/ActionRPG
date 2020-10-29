using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Analytics;

public class Player_Input : MonoBehaviour {
	public float MAXSPEED;
	public float life;
	public float jumpingforce;
	public GameObject copy_bala;
	public Text life_text;
	public Slider life_slider;
	public GameObject spawnObject;
	public Vector3 start_position;
	public AudioClip audio_disparar;
	public AudioClip audio_herida;

	//public int num_balas;
	float speed;
	bool derecha;
	bool fire;
	bool atacando = false;
	bool jumping = false;
	bool invulnerable = false;
	bool enabled = false;
	float maxlife;
	bool jump_button;


	Rigidbody2D rgb;
	Animator anim;
	GameObject this_bala;
	Canvas_Character canva;
	AudioSource aSource;
	GameObject pistola;


	Vector2 force;
	SpriteRenderer rend;
	Color col;
	Transform trans;
	//float verticalspeed;
	//public float MAXVERTICALSPEED = 10000f;
	//bool in_the_floor;

	// Use this for initialization
	void Start () {
		//life es publica y aparece en el inspector, inicializa a maxlife
		maxlife = life;
		rgb = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		speed = 0f;
		derecha = true;
		canva = new Canvas_Character (life, life_text, life_slider);
		force = new Vector2 (0, 0);
		rend = GetComponent<SpriteRenderer> ();
		col = rend.color;
		trans = GetComponent<Transform> ();
		aSource = GetComponent<AudioSource> ();
		pistola = GameObject.Find ("Pistola");
		//in_the_floor = true;
		//AnalyticsResult result = Analytics.CustomEvent("Test");
		// This should print "Ok" if the event was sent correctly.
		//Debug.Log(result);
	}

	void Update(){
		fire = Input.GetButtonDown ("Fire1");
		jump_button = Input.GetButtonDown ("Jump");
		if (fire && enabled && life > 0) {
			
			Disparar ();
		}
		if (jump_button && enabled && life > 0) {
			if (!jumping || rgb.velocity.y == 0) {
				jumping = true;
				force.x = 0;
				force.y = jumpingforce;
				rgb.AddForce (force);
			}
		}

	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.CompareTag ("Floor")) {
			jumping = false;
		}
//		if (other.gameObject.CompareTag ("Enemy")) {
//			Debug.Log ("golpe");
//			invulnerable = true;
//			//life -= other.gameObject.GetComponent<Slime_Script> ().fuerza;
//			CambioVida (other.gameObject.GetComponent<Slime_Script> ().fuerza * -1);
//			print (life);
//			Golpeado (Mathf.Sign(other.gameObject.transform.position.x - gameObject.transform.position.x));
//		}
	}

	void OnTriggerExit2d(Collider2D other){
		if (other.gameObject.CompareTag ("Floor")) {
			jumping = true;
		}
	}

	void OnTriggerStay2D(Collider2D other){
		if (other.gameObject.CompareTag ("Enemy")) {
			if (!invulnerable && life > 0) {
				invulnerable = true;
				Slime_Script other_script = other.gameObject.GetComponent<Slime_Script> ();
				CambioVida (other_script.fuerza * -0.5f * Statics.level - 0.5f);
				//print (life);
				Golpeado (Mathf.Sign(other.gameObject.transform.position.x - gameObject.transform.position.x));
				other_script.AtaqueHecho ();
			}
		}
	}

	void RecuperarVida(float recuperacion){
		life = (life + recuperacion > maxlife) ? maxlife : life + recuperacion;
	}

	void CambioVida(float cambio){
		life += cambio;
		InstanciarLifeText (cambio);
	}

	void InstanciarLifeText(float cambio){
		GameObject letrero_vida = null;
		letrero_vida = (GameObject)Instantiate (spawnObject, trans.position,trans.rotation);

		//print (letrero_vida.ToString ());
		letrero_vida.GetComponent<RetroalimentacionEnergia> ().cantidadCambiodeEnergia = cambio;
	}

	void Restore(){
		rend.color = col;
		invulnerable = false;
	}

	void Golpeado(float direction){
		if(direction == 0) direction = 1;

		force.x = -3900f * direction;
		force.y = 0;
		rgb.AddForce (force);
		aSource.PlayOneShot (audio_herida);

		rend.color = new Color (80,0,0);
		Invoke ("Restore", 0.7f);
		if (life <= 0) {
			life = 0;
			anim.SetTrigger ("morir");
			/*Analytics.CustomEvent ("GameOver", new Dictionary<string, object> {
				{"levels", Statics.level},
				{"tries", Statics.tries}
			});*/
			/*print("evento: "+GetComponent<AnalyticsTracker> ().eventName);
			
			GetComponent<AnalyticsTracker> ().TriggerEvent ();*/
			Statics.script.boton_reinicio.gameObject.SetActive (true);

		}
		canva.UpdateLife (life);
		jumping = false;
	}

	void FixedUpdate () {
		if (life > 0) {
			if (enabled) {
				if (!atacando) {//comprueba que no esté atacando
					speed = Input.GetAxis ("Horizontal");

			
					var oldv = rgb.velocity;
			
					//verticalspeed = Input.GetAxis ("Vertical") * MAXVERTICALSPEED;
			
					Vector2 v = new Vector2 (speed * MAXSPEED, oldv.y);
					rgb.velocity = v;
					if (speed < 0 & derecha) {
						Flip ();
					} else if (speed > 0 & !derecha) {
						Flip ();
					}
					/*if(verticalspeed > 0.1f){
				in_the_floor = false;
			}*/
					anim.SetFloat ("velocidad", Mathf.Abs (v.x));
				} else {
					atacando = !anim.GetBool ("moviendose");
				}
				/*if (Input.GetAxis ("Jump") > 0.1f) {
				if (!jumping || rgb.velocity.y == 0) {
					jumping = true;
					force.x = 0;
					force.y = jumpingforce;
					rgb.AddForce (force);
				}
			}*/
			} else {
				rgb.velocity = new Vector2 (0,0);
			}
		}
	}

	void Flip(){
		speed *= -1;
		var s = transform.localScale;
		s.x *= -1;
		transform.localScale = s;
		derecha = !derecha;
	}

	void Disparar(){
		if (!atacando && !this_bala && enabled && life > 0) {
			atacando = true;
			anim.SetBool ("moviendose", false);
			var oldv = rgb.velocity;
			Vector2 v = new Vector2 (0, oldv.y);
			rgb.velocity = v;
			anim.SetFloat ("velocidad", 0);
			anim.SetTrigger ("atacar");
			Invoke ("CrearBala", 2f/7);
		}
	}

	void DestruirBala(){
		Destroy (this_bala);
	}

	void CrearBala(){
		if (!this_bala) {
			this_bala = Instantiate (copy_bala);
			atacando = false;
			Transform tr, balatr;
			if (pistola != null) {
				print ("from pistola");
				var pistr = pistola.transform.localPosition;
				var humtr = GetComponent<Transform> ().position;
				this_bala.transform.localPosition = new Vector3 (pistr.x + humtr.x, pistr.y + humtr.y, pistr.z + humtr.z);
			} else {
				print ("from women");
				var pistr = GetComponent<Transform> ().position;
				this_bala.transform.position = new Vector3 (pistr.x, pistr.y, pistr.z);
			}
			//print (pistola.transform.position);
			if (!derecha) {
				this_bala.GetComponent<Bala_Script> ().DisparoIzq ();
			}
			aSource.PlayOneShot (audio_disparar);
		}
	}

	public void StartBattle(){
		enabled = true;
		rend.enabled = true;
		if (!derecha) {
			Flip ();
		}
	}

	public void EndBattle(){
		enabled = false;
		rgb.transform.position = start_position;
		rend.enabled = false;
		life += Statics.level;
		canva.UpdateLife (life);
	}

	public void Revivir(){
		EndBattle ();
		life = maxlife;
		anim.SetTrigger ("revivir");
		life_text.gameObject.SetActive (false);
		life_slider.gameObject.SetActive (false);
		canva.UpdateLife (maxlife);
	}
}
