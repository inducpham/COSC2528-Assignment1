using UnityEngine;
using System.Collections;
using IronScheme;

//just attach this behaviour controller to an object and add "Pursue" and "Wandering" script
public class Enemy : MonoBehaviour {

	enum MovementStatus {Wandering,Pursuing};
	enum FireStatus{Fire,HoldOn};

	MovementStatus movementStatus = MovementStatus.Wandering;
	Transform target = null;
	// Use this for initialization
	public float visionRadium = 10f;
	void Start () {
	Debug.Log(IronScheme.RuntimeExtensions.Eval("(+ 1 {0})",3).ToString());
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
		    < visionRadium && movementStatus != MovementStatus.Pursuing) {
			movementStatus = MovementStatus.Pursuing;
			EnterSteering<Pursue> ();
		} else {
			EnterSteering<Wandering>();
			movementStatus = MovementStatus.Wandering;
		}
	}
}
