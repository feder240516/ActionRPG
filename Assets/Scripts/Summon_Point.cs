using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Summon_Point : MonoBehaviour {
	private static GameObject[] priv_healths;
	public static GameObject[] healths {
		get {
			if (priv_healths == null) {
				priv_healths = GameObject.FindGameObjectsWithTag ("Summon");
			}
			if (priv_healths == null) {
				throw new MissingReferenceException ("Summons not found");
			}
			return priv_healths;
		}
	}
	public GameObject next_health;

	// Use this for initialization
	void Start () {
		string myname = gameObject.name;
		int number = (int.Parse(myname.Substring (7, 1)))+1;
		//Crea el puntero al siguiente nivel
		next_health = GameObject.Find ("Health_" + number.ToString ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
