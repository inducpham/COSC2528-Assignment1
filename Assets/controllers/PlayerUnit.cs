using UnityEngine;
using System.Collections.Generic;

public class PlayerUnit : MonoBehaviour {

	private HashSet<GameObject> _flock = new HashSet<GameObject>();
	private HashSet<GameObject> _recordedEnemies = new HashSet<GameObject>();

	public void Start() {
		_flock.Add(gameObject);
	}

	public void AddToFlock (GameObject followerUnit)
	{
		_flock.Add(followerUnit);
		//flock copy is used as a reference set for all the followers to refer to
		HashSet<GameObject> flockCopy = new HashSet<GameObject>(_flock);

		foreach (GameObject unit in _flock) {
			Separation sepSteer = unit.GetComponent<Separation>();
			if (sepSteer != null) sepSteer.SetTargets(flockCopy);
		}
	}

	public void RemoveFromFlock (GameObject followerUnit)
	{
		_flock.Remove(followerUnit);
		//flock copy is used as a reference set for all the followers to refer to
		HashSet<GameObject> flockCopy = new HashSet<GameObject>(_flock);
		
		foreach (GameObject unit in _flock) {
			Separation sepSteer = unit.GetComponent<Separation>();
			if (sepSteer != null) sepSteer.SetTargets(flockCopy);
		}
	}

	public void RecordThreat (GameObject enemy)
	{
		_recordedEnemies.Add(enemy);
	}

	public void NeutralizeThreat (GameObject enemy)
	{
		_recordedEnemies.Remove(enemy);
	}

	public GameObject GetNearestEnemyInRange (Vector3 loc,  float agroRange)
	{
		GameObject result = null;
		float rangeSqr = agroRange * agroRange;
		float tmp;

		foreach (GameObject enemy in _recordedEnemies) {
			if (enemy == null) continue;
			if ((tmp = (enemy.transform.position - loc).sqrMagnitude)
			    > rangeSqr) continue;

			result = enemy;
			rangeSqr = tmp;
		}

		return result;
	}
	
	void OnCollisionEnter(Collision collision) {
		FollowerUnit unit = collision.gameObject.GetComponent<FollowerUnit>();

		if (unit != null)
		{
			unit.SetLeader(this.transform);
		}
	}

}
