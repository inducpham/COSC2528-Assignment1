using UnityEngine;
using System.Collections;

enum State
{
		Patrol,
		Chasing }
;

public class EnemyUnit : MonoBehaviour
{
		public float ChaseRadius = 10f;
		public float UnitScanRadius = 4f;
		public float PatrolRadius = 5;
		public float PatrolMinUpdateInterval = 2;
		public float PatrolMaxUpdateInterval = 5;
		private float _patrolDelta = 0f;
		public float PathfindingResetInterval = 0.3f;
		private float _pathfindingDelta = 0f;
		private Vector3 _home;
		private PFArrive _pathfinder;
		private Transform _chaseTarget;
		private State _state;

		// Use this for initialization
		void Start ()
		{
		Debug.Log(IronScheme.RuntimeExtensions.Eval("(+ 1 {0})",3).ToString());
				_home = transform.position;
				_pathfinder = GetComponent<PFArrive> ();
				_patrolDelta = Random.Range (0f, PatrolMaxUpdateInterval);
				_chaseTarget = null;
		}

		private void OnOutOfChaseRadius ()
		{
				if (_state == State.Chasing) {
						_state = State.Patrol;
						OnNewPatrolRouteRequest ();
				}
		}

		private void OnFollowerFound (Transform f)
		{
				if (_state == State.Patrol) {
						_state = State.Chasing;
						_chaseTarget = f;
				}
		}

		private void OnTargetNulled ()
		{
				if (_state == State.Chasing) {
						_state = State.Patrol;
				}
		}

		private void OnNewPatrolRouteRequest ()
		{
				if (_state == State.Patrol)
						UpdateNewPatrolDestination ();
		}

		private void OnRenewPathfindingRequest ()
		{
				if (_state == State.Chasing && _chaseTarget != null)
						_pathfinder.SetTargetPoint (_chaseTarget.position);
		}

		void OnCollisionEnter (Collision collision)
		{
				FollowerUnit unit = collision.gameObject.GetComponent<FollowerUnit> ();
		
				if (unit != null) { //HACK destroy the target unit
						Debug.Log ("Collision!");
						Destroy (unit.gameObject);
				}
		
		}
	
		// Update is called once per frame
		void Update ()
		{
				// Look surrounding for prospective prey
				//HACK look for prey interval share with pathfinding interval
				_pathfindingDelta -= Time.deltaTime;
				if (_pathfindingDelta < 0) {
						var hitColliders = Physics.OverlapSphere (transform.position, UnitScanRadius);
						Transform target = null;
						float minDistSqr = 0, distSqr;

						//Loop through all the game objects found on radius
						foreach (Collider c in hitColliders) {
								//Only consider the follower units
								if (c.GetComponent<FollowerUnit> () == null)
										continue;
								distSqr = (c.transform.position - transform.position).sqrMagnitude;

								//If it is a follower unit, check if it is closest to this enemy unit
								if (target == null || distSqr < minDistSqr) {
										target = c.transform;
										minDistSqr = distSqr;
								}
						}

						//If a target is found, notify event
						if (target != null)
								OnFollowerFound (target);
				}

				//HACK same reason as above, this time though, to check if the target is destroyed
				if (_pathfindingDelta < 0) {
						if (_chaseTarget == null)
								OnTargetNulled ();
				}
				
		
				// Check if we are too far from home
				if ((transform.position - _home).sqrMagnitude > ChaseRadius * ChaseRadius)
						OnOutOfChaseRadius ();

				// Update the patrol interval
				_patrolDelta -= Time.deltaTime;
				if (_patrolDelta < 0) {
						_patrolDelta = Random.Range (PatrolMinUpdateInterval, PatrolMaxUpdateInterval);
						OnNewPatrolRouteRequest ();
				}

				// Update the pathfinding interval
				_pathfindingDelta -= Time.deltaTime;
				if (_pathfindingDelta < 0) {
						_pathfindingDelta = PathfindingResetInterval;
						OnRenewPathfindingRequest ();
				}
		}

		private void UpdateNewPatrolDestination ()
		{
				Vector3 destination = _home;
				Vector2 offset = Random.insideUnitCircle * PatrolRadius;
				destination.x += offset.x;
				destination.z += offset.y;

				if (_pathfinder != null)
						_pathfinder.SetTargetPoint (destination);
		}
}
