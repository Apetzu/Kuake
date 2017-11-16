using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testShooting : MonoBehaviour {
	
    playerController pc; 

    void Awake()
    {
        pc = GetComponent<playerController>();
    }

	void Update () 
    {
        pc.CmdFire();
	}
}
