using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailScript : MonoBehaviour {

    float vel = -0.3f;

    MeshRenderer r;

	// Use this for initialization
	void Start () {
        r = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        r.material.mainTextureOffset = new Vector2(Time.timeSinceLevelLoad * vel, 0);
    }
}
