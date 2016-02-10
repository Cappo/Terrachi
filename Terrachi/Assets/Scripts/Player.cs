using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))] //automically adds Controller2D upon attaching script to gameObject
public class Player : MonoBehaviour {


    Controller2D controller; //reference to the controller
	// Use this for initialization
	void Start () {
        controller = GetComponent<Controller2D>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
