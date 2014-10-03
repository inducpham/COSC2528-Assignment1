using UnityEngine;
using System.Collections;

//just attach this behaviour controller to an object and add "Pursue" and "Wandering" script
public class Enemy : MonoBehaviour {

	enum MovementStatus {Wandering,Pursuing};
	enum FireStatus{Fire,HoldOn};

	MovementStatus movementStatus = movementStatus.Wandering;
	Transform target = null;
	// Use this for initialization
	public float visionRadium = 10f;
	void Start () {
		if (target == null)
			Debug.LogWarning ("Enemy: targer player is not given in object: "+name);
	}

	void DisableSteerings(){
		foreach (var a in GetComponents<Steering>()) {
			a.enabled = false;
		}
	}
	void EnableSteering<T>()where T : Steering{
		T a = GetComponent<T> ();
		if (a != null)
			a.enabled = true;
	}
	void EnterSteering<T>()where T : Steering{
		DisableSteerings ();
		EnableSteering<T>();
	}
	// Update is called once per frame
	void Update () {
		if ((transform.position - target.position).sqrMagnitude
					< visionRadium && movementStatus != MovementmovementStatus.Pursuing) {
			movementStatus = MovementmovementStatus.Pursuing;
			EnterSteering<Pursue> ();
		} else {
			EnterSteering<Wandering>();
			movementStatus = MovementmovementStatus.Wandering;
		}
	}
}
