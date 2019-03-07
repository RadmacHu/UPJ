using System;
using System.Collections;
using System.Collections.Generic;
using MgsMoudle;
using MgsMoudle.MgsDefine;
using UnityEngine;

public class GetterTest : MonoBehaviour {

	// Use this for initialization
    void Start()
    {
        GMessenger.AddListener<int>(MgsDef.TEST_MGS_SHOW, OnGetMgs);
    }

    private void OnGetMgs(int arg1)
    {
        Debug.LogError("Show in Getter Test . And arg1 = "+ arg1);
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
