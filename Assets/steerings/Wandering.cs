using UnityEngine;
using System.Collections;

public class Wandering : PFArrive {

	// Use this for initialization
	int _counter=0;
	public int ReplanEverySec = 1500;//replan every 30s
	float fromx=-40;
	float fromy=-40;
	float tox=40;
	float toy=40;
	void Start () {
		base.Start ();
			//tox = -(fromx = pathfinder.StartX);
			//toy = -(fromy = pathfinder.StartY);
			if (tox < 0) {
				tox=-tox;
				fromx=-fromx;
			}
			if (toy < 0) {
				toy = -toy;
				fromy=-fromy;
			}

		RePlanNewTarger ();
	}
	public void RePlanNewTarger(){
		_counter = 0;
		Vector3 targetPoint = Vector3.zero;
		targetPoint.x = Random.Range (fromx, tox);
		targetPoint.z = Random.Range (fromy, toy);
		SetTargetPoint (targetPoint);
	}
	// Update is called once per frame
	void Update () {
		base.Update ();
		if (_counter == 0) {
			RePlanNewTarger();	
		}
		_counter = (_counter + 1)%ReplanEverySec;
	}
}
