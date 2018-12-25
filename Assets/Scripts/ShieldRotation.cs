using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRotation : MonoBehaviour {

    [SerializeField] public int[] rotationSpeed;
    [SerializeField] public GameObject[] plane;
    [SerializeField] public Color shieldColor;
    [SerializeField] public Color timeChangerColor;
    private Material[] mat;

	// Use this for initialization
	void Start () {
        int len = plane.Length;
        mat = new Material[len];

        for (int i = 0; i < len; i++)
        {
            Renderer rend = plane[i].GetComponent<Renderer>();
            mat[i] = rend.material;
            mat[i].SetColor("_TintColor", shieldColor);
        }
	}
	
	// Update is called once per frame
	void Update () {
        int len = plane.Length;
        for (int i = 0; i < len; i++)
        {
            
            plane[i].transform.Rotate(Vector3.up * rotationSpeed[i] * Time.deltaTime, Space.Self);

            Color col = mat[i].GetColor("_TintColor");
            col.a += Random.Range(-1f, 1f) / 30;
            
            /* Hack adjustments */
            if (col.a > 0.80f) col.a -= Random.Range(0.5f, 1f) / 20;
            if (col.a < 0.3f) col.a += Random.Range(0.5f, 1f) / 20;
            
            mat[i].SetColor("_TintColor", col);
        }
        
    }

    public void changeColor (Color col)
    {
        int len = plane.Length;
        for (int i = 0; i < len; i++)
        {
            /* 
             * TODO: color lerp 
             */
            mat[i].SetColor("_TintColor", col);
        }
    }

   
}
