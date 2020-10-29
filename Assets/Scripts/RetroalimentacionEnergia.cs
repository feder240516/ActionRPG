using UnityEngine;
using System.Collections;

public class RetroalimentacionEnergia : MonoBehaviour {
    
    public float tiempoDeVida;
    public float velocidad;
    public float cantidadCambiodeEnergia;

    public GUIStyle estiloDeTexto;
    
    private Transform myTransform;

	// Use this for initialization
	void Start () {
	    myTransform = transform;
        Destroy(gameObject, tiempoDeVida);
	}
	
	// Update is called once per frame
	void Update () {
	    ActualizarMovimiento();
	}

    private void OnGUI() {
        Rect textRect = CalcularRectanguloMensaje();
        string mensaje = ObjenerMensaje();
        CambiarColorDeEstilo();
        GUI.TextField(textRect, mensaje, estiloDeTexto);
    }

    private void ActualizarMovimiento() {
        Vector3 paso = new Vector3(0,1,0) * Time.deltaTime * velocidad;
        myTransform.Translate(paso);
    }

    private Rect CalcularRectanguloMensaje() {
		Vector2 position = Camera.main.WorldToScreenPoint(myTransform.position);
        Rect rectanguloMensaje = new Rect(position.x - 50, Screen.height - position.y, 100, 30);
        return rectanguloMensaje;
    }

    private string ObjenerMensaje() {
        string mensaje = "" + cantidadCambiodeEnergia;
        if (cantidadCambiodeEnergia > 0)
            mensaje = "+" + mensaje;
        return mensaje;
    }

    private void CambiarColorDeEstilo() {
        if (cantidadCambiodeEnergia < 0)
            estiloDeTexto.normal.textColor = Color.red;
        else
            estiloDeTexto.normal.textColor = Color.green;
    }
}
