using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Clase que permite tener a cada personaje su propio elemento de cambio de vida.
//Creo que en el futuro debería cambiarse por una interface.
public class Canvas_Character : ScriptableObject {
	public Text life_text;
	public Slider life_slider;

	public Canvas_Character(float life, Text text, Slider slider){
		life_text = text;
		life_slider = slider;
		life_text.text = life.ToString ();
		life_slider.maxValue = life;
		life_slider.value = life;
	}

	public void UpdateLife(float life){
		life_text.text = life.ToString ();
		life_slider.value = life;
	}

}
