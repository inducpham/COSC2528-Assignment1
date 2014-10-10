using UnityEngine;
using System.Collections;

public enum FollowerState
{
		FellBehind,
		CaughtUp,
		Engaging,
		Disengaging
}

[RequireComponent (typeof(Combat))]

public class FollowerUnit : MonoBehaviour
{
		public float FallbehindDistance = 10f;
		public float PathfindingResetTimer = 0.5f;
		public float AgroRange = 5f;
		private Transform _leader = null;
		private FollowerState _state = FollowerState.FellBehind;
		private float _delta = 0;
		private GameObject _currentEnemy = null;
		private Transform _pathfindingTarget = null;

		public void SetLeader (Transform leader)
		{
				_leader = leader;
				foreach (Steering s in GetComponents<Steering>()) {
						s.SetTargetPosition (_leader);
						s.TargetIsPoint = false;
				}

				//HACK normally set leader occurs when there it is in contact with the leader,
				//therefore we set the event to be on catching up
				OnCaughtUp ();

				PlayerUnit player = leader.GetComponent<PlayerUnit> ();
				if (player != null)
						player.AddToFlock (gameObject);
		}

		public bool AssignedLeader ()
		{
				return _leader != null;
		}
	
		void OnCollisionEnter (Collision collision)
		{
				FollowerUnit unit = collision.gameObject.GetComponent<FollowerUnit> ();
		
				if (unit != null && !unit.AssignedLeader ())
						unit.SetLeader (_leader);
				else
						TryAttackEnemy (collision);
		}

		void OnCollisionStay (Collision collision)
		{
				TryAttackEnemy (collision);
		}
		
		private void TryAttackEnemy (Collision collision)
		{
				if (_leader == null)
						return;

				EnemyUnit enemy = collision.gameObject.GetComponent<EnemyUnit> ();
				PlayerUnit player = _leader.GetComponent<PlayerUnit> ();
		
				if (enemy != null && player != null) {
						player.RecordThreat (enemy.gameObject);
			
						Combat unitCombat = null, enemyCombat = null;
						GetComponent<Combat> ().Attack (enemy.gameObject);
				}
		}
	
	
		//transition to flock state
		void OnCaughtUp ()
		{
				Steering s;
				_state = FollowerState.CaughtUp;

				_currentEnemy = null; //forget current enemy
				
				if ((s = GetComponent<Align> ()) != null)
						s.enabled = true;
				if ((s = GetComponent<VelocityMatch> ()) != null)
						s.enabled = true;
				if ((s = GetComponent<Separation> ()) != null)
						s.enabled = true;
				if ((s = GetComponent<Seek> ()) != null)
						s.enabled = true;
				if ((s = GetComponent<PFArrive> ()) != null)
						s.enabled = false;
		}

		//transition to pathfinding state
		void OnFellBehind ()
		{
				Steering s;
				_state = FollowerState.FellBehind;
				_pathfindingTarget = _leader;

				PFArrive pf1, pf2;

				_currentEnemy = null; //forget current enemy
				
				if ((s = GetComponent<Align> ()) != null)
						s.enabled = false;
				if ((s = GetComponent<VelocityMatch> ()) != null)
						s.enabled = false;
				if ((s = GetComponent<Separation> ()) != null)
						s.enabled = true;
				if ((s = GetComponent<Seek> ()) != null)
						s.enabled = false;
				if ((s = GetComponent<PFArrive> ()) != null)
						s.enabled = true;
		}

		//transition to defend state
		void OnEngaging (GameObject enemy)
		{
				Steering s;
				_pathfindingTarget = enemy.transform;
				_currentEnemy = enemy;
				_state = FollowerState.Engaging;
		
				if ((s = GetComponent<Align> ()) != null)
						s.enabled = false;
				if ((s = GetComponent<VelocityMatch> ()) != null)
						s.enabled = false;
				if ((s = GetComponent<Separation> ()) != null)
						s.enabled = false;
				if ((s = GetComponent<Seek> ()) != null)
						s.enabled = false;
				if ((s = GetComponent<PFArrive> ()) != null)
						s.enabled = true;
		}
	
		void Update ()
		{
				if (_leader == null)
						return;

				PlayerUnit player = _leader.GetComponent<PlayerUnit> ();
				float distance = (_leader.position - transform.position).magnitude;

				switch (_state) {
				case FollowerState.CaughtUp:
						if (distance > FallbehindDistance)
								OnFellBehind ();
						else if (player != null && _currentEnemy == null && 
								(_currentEnemy = player.GetNearestEnemyInRange (transform.position, AgroRange)) != null)
								OnEngaging (_currentEnemy);
						break;

				case FollowerState.Engaging:
						if (_currentEnemy == null) //in the ccase current enemy is not found
								OnFellBehind ();
						else if (distance > FallbehindDistance)
								OnFellBehind ();
						// in the case current enemy is out of range
						else if ((_currentEnemy.transform.position - transform.position).magnitude > AgroRange)
								OnFellBehind ();
						break;

				case FollowerState.FellBehind:
						if (distance < FallbehindDistance)
								OnCaughtUp ();
						break;
							
				}

				// calculate effective A* here
				PFArrive s = GetComponent<PFArrive> ();
				if (s.enabled) {
						_delta -= Time.deltaTime;
						if (_delta < 0) {
								_delta = PathfindingResetTimer;
								//HACk: should set keep target point as a gameobject reference
								s.SetTargetPoint (_pathfindingTarget.position);
						}						
				}
		}

		void OnDestroy ()
		{
				if (_leader == null)
						return;
				PlayerUnit player = _leader.GetComponent<PlayerUnit> ();
				if (player != null)
						player.RemoveFromFlock (gameObject);
		}
}