using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Obstacle : MonoBehaviour {

    [Flags] public enum E_ObsType
    {
        Normal  = 0,
        Slide   = 1,
        Climb   = 2

    }

    public E_ObsType obsType;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Lost");
            collision.gameObject.GetComponent<PlayerMovement>().canMove = false;
        }
    }
}
