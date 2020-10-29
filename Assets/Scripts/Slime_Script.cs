using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slime_Script : MonoBehaviour {
	public static int enemies_in_scene = 0;
	public float MAXSPEED;
	public float velocidad;
	bool apagado = false;
	float oldvel = 0f;
	public float life;
	public float fuerza;
	public float die_time;
	bool derecha = false;
	bool tocando_otro = false;
	float distancia_x_al_jugador{
		get{
			return tran.position.x - Statics.script.player.GetComponent<Transform> ().position.x;
		}
	}


	Rigidbody2D rgb;
	Vector2 vec;
	SpriteRenderer spr;
	Animator anim;
	GameObject atacante;
	SpriteRenderer rend;
	Color col;
	Canvas_Character canva;
	BoxCollider2D[] colliders;
	BoxCollider2D coll_trigger;
	Transform tran;
	AudioSource audio_source;
	Vector3 text_location;



	public Text life_text;
	public Slider life_slider;
	public GameObject spawnObject;
	public AudioClip audio_die;
	public AudioClip audio_attack;


	private static Canvas priv_canvas_back;
	public static Canvas canvas_back{
		get{
			if (priv_canvas_back == null) {
				priv_canvas_back = FindObjectOfType<Canvas> ();
			}
			if (priv_canvas_back == null) {
				throw new UnityException ("Canvas not found");
			}
			return priv_canvas_back;
		}
	}


	// Use this for initialization
	void Start () {
		
		rgb = GetComponent<Rigidbody2D> ();
		//print ("rgb=" + rgb + " obj=" + this.name);
		anim = GetComponent<Animator> ();
		rend = GetComponent<SpriteRenderer> ();
		col = rend.color;
		audio_source = GetComponent<AudioSource> ();
		life_text = Instantiate (life_text,canvas_back.transform);
		life_slider = Instantiate (life_slider,canvas_back.transform);
		canva = new Canvas_Character (life, life_text, life_slider);
		life_text = canva.life_text;
		life_slider = canva.life_slider;
		tran = GetComponent<Transform> ();
		colliders = GetComponents<BoxCollider2D> ();
		foreach(BoxCollider2D colli in colliders){
			if (colli.isTrigger) {
				coll_trigger = colli;
				break;
			}
		}
		//print ("started\n" +
		//"rgb = " + rgb.GetInstanceID ());
		//print ("started\n" +
		//	"rigid = " + GetComponent<Rigidbody2D>().GetInstanceID ());
		//print (gameObject.name);
		/*if (gameObject.name == "skeleton"){
			velocidad = 0f;
			print ("hello");
		}*/
	}


	void FixedUpdate () {
		//print (velocidad);
		if (!anim.GetBool ("moviendose")) {
			if (!apagado) {
				velocidad = 0;
				apagado = true;
			}
		} else {
			if (life > 0) {
				apagado = false;
				int x = (derecha) ? 1 : -1;
				velocidad = (velocidad == 0 ? MAXSPEED * x * Statics.level : velocidad);

				vec = new Vector2 (velocidad, 0);
				rgb.velocity = vec;
			}
			//oldvel = 0;
		}
		if (tocando_otro) {
			if (Random.value < 0.02f) {
				Flip ();
			}
			tocando_otro = false;
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		GameObject go = other.gameObject;
		if (!go.CompareTag ("Gun") && !go.CompareTag ("Player")&& !go.CompareTag("Enemy") && !go.CompareTag("Floor")) {
			Flip ();
		// else if (go.CompareTag ("Player")) {
		//	anim.SetTrigger ("atacar");
		//	audio_source.PlayOneShot (audio_attack);
		} else if (go.CompareTag ("Gun")) {

			Atacado (go.GetComponent<Bala_Script> ().fuerza,go);
		}
	}

	void OnTriggerStay2D(Collider2D other){
		GameObject go = other.gameObject;
		if (go.CompareTag ("Walls")) {
			if ((go.name == "Pared izq" && !derecha) || (go.name == "Pared der" && derecha)) {
				Flip ();
			}
		} else if (go.CompareTag ("Enemy") || go.CompareTag("Player")) {
			//Slime_Script go_script = go.GetComponent<Slime_Script> ();
			if (Random.value > 0.3 && ((distancia_x_al_jugador < 0 && !derecha)||(distancia_x_al_jugador > 0 && derecha))) {
				tocando_otro = true;
			}
		}

	}

	public void AtaqueHecho(){
		anim.SetTrigger ("atacar");
		audio_source.PlayOneShot (audio_attack);
	}

	void CambioVida(float cambio){
		life += cambio;
		InstanciarLifeText (cambio);
	}

	void InstanciarLifeText(float cambio){
		GameObject letrero_vida = null;
		letrero_vida = (GameObject)Instantiate (spawnObject,tran.position + new Vector3(Random.value-0.5f,Random.value-0.5f),tran.rotation);

		//print (letrero_vida.ToString ());
		letrero_vida.GetComponent<RetroalimentacionEnergia> ().cantidadCambiodeEnergia = (int)cambio;
	}

	void Flip(){
		velocidad *= -1;
		//var s = transform.localScale;
		//s.x *= -1;
		//transform.localScale = s;
		derecha = !derecha;
		rend.flipX = !rend.flipX;
	}

	//Código a ejecutar después de ser atacado
	public void Atacado(float fuerza, GameObject ataco){
		if(!atacante && life>0){
			atacante = ataco;
			CambioVida(Mathf.Min (fuerza,life)*-1);
			if (life <= 0) {
				rgb.velocity = new Vector2 (0, 0);
				anim.SetTrigger ("morir");
				audio_source.PlayOneShot (audio_die);
				rgb.Sleep ();
				foreach (Collider2D coll in colliders) {
					coll.enabled = false;
				}
				Invoke ("morir", die_time);

			}
			canva.UpdateLife (life);
			rend.color = new Color (80,0,0);
			Invoke ("Restore",0.2f);
			atacante.GetComponent<Bala_Script>().AtaqueRecibido (this);
		}
	}

	//Restaurar colores
	void Restore(){
		rend.color = col;
	}

	public void FinAtaque(){
		atacante = null;
	}

	public void morir(){
		enemies_in_scene -= 1;
		//print (enemies_in_scene);
		if (enemies_in_scene == 0) {
			Statics.script.FinBatalla ();
		}
		//Statics.script.SacarGameObject (gameObject);
		DestruirThis ();

	}

	public void DestruirThis(){
		Destroy (life_slider.gameObject);
		Destroy (life_text.gameObject);
		Destroy (this.gameObject);
	}

	public static GameObject CreateEnemy(GameObject enemy, Vector3 spawn_point,Quaternion quaternion, Vector3 text_point){
		enemies_in_scene += 1;
		GameObject obj = Instantiate (enemy, spawn_point, quaternion);
		Slime_Script script = obj.GetComponent<Slime_Script> ();
		/*script.life_text = Instantiate (script.life_text,canvas_back.transform);
		script.life_slider = Instantiate (script.life_slider,canvas_back.transform);
		script.canva = new Canvas_Character (script.life, script.life_text, script.life_slider);*/
		script.life_text.transform.position = text_point;
		script.life_slider.transform.position = text_point + new Vector3(10,5,0);
		return obj;
	}

}
