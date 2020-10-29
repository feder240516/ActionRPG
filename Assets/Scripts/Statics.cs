using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statics : MonoBehaviour {
	public GameObject[] enemies;
	public Vector3[] spawn_points;
	public Vector3[] text_points;
	public Vector3[] slider_points;
	public static GameObject manager;
	public GameObject icon;
	public Canvas back_canvas;
	public GameObject player;
	public Dictionary<string,Camera> camaras = new Dictionary<string, Camera>();
	public Button boton_inicio;
	public Button boton_reinicio;
	public Slider slider_player;
	public Text life_player;
	public static int level;
	public static int tries;
	//GameObject[] active_enemies;
	//ArrayList enemigos_activos;
	Camera camara_activa;
	private static Statics priv_script;
	public static Statics script{
		get{
			if (priv_script == null) {
				priv_script = FindObjectOfType<Statics> ();
			}
			if (priv_script == null) {
				throw new UnityException ("statics exception");
			}
			return priv_script;
		}
	}

	void Start(){
		manager = GameObject.FindWithTag ("Manager");
		foreach (Camera i in FindObjectsOfType<Camera> ()) {
			camaras.Add (i.name, i);
			//print (i.name);
			if (i.isActiveAndEnabled) {
				//print ("camera activa");
				camara_activa = i;
			}
		}
		level = 1;
		tries = 0;
		//active_enemies = new GameObject[10];
		//enemigos_activos = new ArrayList();
	}

	void Update(){
		var key = Input.GetKeyDown ("l");
		if (key) {
			StartCoroutine( CreateEnemies ((int)(level * 1.5f)));
		}
		
	}

	public void StartButton(){
		SwitchCameras ("Camera_Levels");
		Player_Icon icon_script = icon.GetComponent<Player_Icon> ();
		icon_script.LevelFinished ();
		boton_inicio.gameObject.SetActive (false);
		slider_player.gameObject.SetActive (true);
		life_player.gameObject.SetActive (true);
		icon_script.enabled = true;
		tries++;
	}

	public void RestartButton(){
		
		foreach (Slime_Script i in FindObjectsOfType<Slime_Script>()) {
			i.DestruirThis ();
		}
		Slime_Script.enemies_in_scene = 0;
		icon.GetComponent<Player_Icon> ().Restart ();
		SwitchCameras ("Camera_Game");
		var player_script = player.GetComponent<Player_Input> ();
		player_script.Revivir ();
		level = 1;
		//enemigos_activos.Clear ();
	}

	/*public bool SacarGameObject(GameObject obj){
		if (enemigos_activos.Contains (obj)) {
			enemigos_activos.Remove (obj);
			return true;
		}
		throw new UnityException ("no hay elemento");
	}*/

	public IEnumerator CreateEnemies(int num){
		Vector3[] puntos_generados = new Vector3[num];
		puntos_generados [0] = spawn_points [0];
		Vector3[] puntos_texto = new Vector3[num];
		puntos_texto [0] = text_points [0];
		for (int j = 1; j < num; j++) {
			puntos_generados[j] = new Vector3 (puntos_generados[j-1].x + 2,puntos_generados[j-1].y);
			puntos_texto[j] = new Vector3 (puntos_texto[j-1].x,puntos_texto[j-1].y-18);
		}
		for(int i=0;i<num;i++){
			int enemy = Random.Range (0, enemies.Length);
			int point = Random.Range (0, spawn_points.Length);
			//print (text_points [i]);
			//print (point);
			//print (spawn_points [point]);
			//print (enemies [enemy]);
			yield return new WaitForSeconds(0.1f);
			Slime_Script.CreateEnemy (enemies [enemy], puntos_generados [i], Quaternion.identity,puntos_texto[i]);
		}
	}

	public void SwitchCameras(string namecamara){
		camaras [namecamara].enabled = true;
		camara_activa.enabled = false;
		camara_activa = camaras [namecamara];
	}

	public void StartBatalla(int num, string namecamera){
		StartCoroutine( CreateEnemies ((int)(level * 0.8f + 0.2f)));
		SwitchCameras (namecamera);
		player.GetComponent<Player_Input> ().StartBattle ();
	}

	public void FinBatalla(){
		icon.GetComponent<Player_Icon> ().LevelFinished ();
		if (level < 5) {
			SwitchCameras ("Camera_Levels");
		} else {
			boton_reinicio.gameObject.SetActive (true);
		}
		player.GetComponent<Player_Input> ().EndBattle ();
		//enemigos_activos.Clear ();
		level++;
	}
}
