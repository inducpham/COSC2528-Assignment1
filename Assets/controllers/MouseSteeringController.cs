using UnityEngine;
using System.Collections;

public class MouseSteeringController : MonoBehaviour {

	private Plane _ground;

	// Use this for initialization
	void Start () {
		_ground = new Plane (Vector3.up, Vector3.zero);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (1)) {
			/* calculate target point from the camera */
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float distance;
			if (_ground.Raycast (ray, out distance)) {
				foreach (Steering s in GetComponents<Steering>())
					s.SetTargetPoint(ray.GetPoint (distance));
			}
		}
	}
}
