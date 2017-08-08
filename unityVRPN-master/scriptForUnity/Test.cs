using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRPNNamespace;
//using UDPReceive;

public class Test : MonoBehaviour{
//	void Start(){
//	}
//
	// Use this for initialization
	void Update () {
		//Debug.Log ("Butter");
		//Debug.Log (VRPNNamespace.VRPN.vrpnButton ("192.168.10.1", 51001));
		Debug.Log (VRPNNamespace.VRPN.vrpnTrackerPos ("192.168.10.1", 51001));
		//Debug.Log ("Hey");
		//Debug.Log(UDPReceive.ReceiveData());
	}
}
