using UnityEngine;
using System.Collections.Generic;

public class PlayerUnit : MonoBehaviour {

	private HashSet<Transform> _flock = new HashSet<Transform>();

	public void Start() {
		_flock.Add(transform);
	}

	public void AddToFlock (Component followerUnit)
	{
		_flock.Add(followerUnit.transform);
		HashSet<Transform> flockCopy = new HashSet<Transform>(_flock);

		foreach (Transform unit in _flock) {
			if (unit == transform) continue;
			Separation sepSteer = unit.GetComponent<Separation>();
			if (sepSteer != null) sepSteer.SetTargets(flockCopy);
		}
	}
	
	void OnCollisionEnter(Collision collision) {
		FollowerUnit unit = collision.gameObject.GetComponent<FollowerUnit>();

		if (unit != null)
		{
			unit.SetLeader(this.transform);
		}
		
	}

}
