using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapModels : MonoBehaviour {

	void Start () {
        uint i = 1;
        string bits = System.Convert.ToString(i, 2).PadLeft(4,'0');
        Debug.Log(bits);
        //Debug.Log();

        i = 8;
        bits = System.Convert.ToString(i, 2).PadLeft(4, '0'); ;
        Debug.Log(bits);
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown("space"))
        {

        }

    }




}
