using System.Collections;
using System.Collections.Generic;
using MgsMoudle;
using MgsMoudle.MgsDefine;
using UnityEngine;

public class SendTest : MonoBehaviour
{
    public int showCounter = 99;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.A))
	    {
            GMessenger.Broadcast<int>(MgsDef.TEST_MGS_SHOW , showCounter);
        }
    }
}
