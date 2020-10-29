using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Icon : MonoBehaviour {
	public GameObject next_standing;
	bool moverse;
	//public GameObject player;

	Summon_Point summon;
	Rigidbody2D rgb;
	Transform summontrans;
	Transform icontrans;
	Animator anim;
	Vector2 velocidad;

	/*private static GameObject priv_icon;
	public static GameObject public_icon{
		get{
			if (!priv_icon) {
				priv_icon = GameObject.FindGameObjectWithTag ("icon");
			}
			if (!priv_icon) {
				throw new UnityException ("icono no encontrado");
			}
			return priv_icon;
		}
	}*/

	// Use this for initialization
	void Start () {
		next_standing = GameObject.Find ("Health_1");
		summon = next_standing.GetComponent<Summon_Point> ();
		rgb = GetComponent<Rigidbody2D> ();
		icontrans = GetComponent<Transform> ();
		summontrans = summon.GetComponent <Transform> ();
		anim = GetComponent<Animator> ();
		velocidad = rgb.velocity;
		moverse = false;
	}

	void FixedUpdate () {
		if (moverse) {
			float distx = summontrans.position.x - rgb.position.x;
			float disty = summontrans.position.y - rgb.position.y;
			if (new Vector2 (distx, disty).magnitude > 0.1) {
				velocidad = new Vector2 (distx, disty).normalized;

			} else {
				velocidad = new Vector2 (0, 0);
				SummonReached ();
			}
			rgb.velocity = velocidad;
			anim.SetFloat ("velocidad", velocidad.magnitude);
		} else {
			rgb.velocity = new Vector2 (0, 0);
		}
	}

	void SummonReached(){
		moverse = false;
		Statics.script.StartBatalla (2, "Camera_Game");
		if (summon.next_health) {
			next_standing = summon.next_health;
			summon = next_standing.GetComponent<Summon_Point> ();
			summontrans = summon.GetComponent <Transform> ();
		} else {
			enabled = false;
		}
	}

	public void LevelFinished(){
		moverse = true;
	}

	public void Restart(){
		next_standing = GameObject.Find ("Health_1");
		summon = next_standing.GetComponent<Summon_Point> ();
		summontrans = summon.GetComponent <Transform> ();
		icontrans.position = new Vector3 (27.81f, -1f);
		moverse = false;
	}
}
