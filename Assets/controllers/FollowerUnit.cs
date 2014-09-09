using UnityEngine;
using System.Collections;

public class FollowerUnit : MonoBehaviour
{
		public float FallbehindDistance = 10f;
		public float PathfindingResetTimer = 0.5f;
		private Transform _leader = null;
		private bool _caughtUp = false;
		private float _delta = 0;

		public void SetLeader (Transform leader)
		{
				_leader = leader;
				foreach (Steering s in GetComponents<Steering>()) {
						s.SetTargetPosition (_leader);
						s.TargetIsPoint = false;
				}
				//HACK normally set leader occurs when there it is in contact with the leader,
				//therefore we set the event to be on catching up
				OnCatchingUp ();

				PlayerUnit player = leader.GetComponent<PlayerUnit> ();
				if (player != null)
						player.AddToFlock (this);
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
		}

		//transition to flock state
		void OnCatchingUp ()
		{
				Steering s;
				
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

				_caughtUp = true;
		}

		//transition to pathfinding state
		void OnFallBehind ()
		{
				Steering s;
				PFArrive pf1, pf2;
				
				if ((s = GetComponent<Align> ()) != null)
						s.enabled = false;
				if ((s = GetComponent<VelocityMatch> ()) != null)
						s.enabled = false;
				if ((s = GetComponent<Separation> ()) != null)
						s.enabled = false;
				if ((s = GetComponent<Seek> ()) != null)
						s.enabled = false;
				if ((pf1 = GetComponent<PFArrive> ()) != null &&
						(pf2 = _leader.GetComponent<PFArrive> ()) != null) {
						pf1.PathfinderHost = pf2.PathfinderHost;
						pf1.Start(); //reset the steering to register the pathfinder
						pf1.enabled = true;

				}

				_caughtUp = false;
		}

		void Update ()
		{
				//HACK, further states should include predatorss
				if (_leader == null)
						return;

				float distance = (_leader.position - transform.position).magnitude;

				if (distance > FallbehindDistance && _caughtUp)
						OnFallBehind ();

				if (distance < FallbehindDistance && !_caughtUp)
						OnCatchingUp ();

				//while not catching up, try to A* toward the unit
				if (!_caughtUp) {
						_delta -= Time.deltaTime;
						if (_delta < 0) {
								_delta = PathfindingResetTimer;
								PFArrive s = GetComponent<PFArrive> ();
								if (s != null) {
										s.SetTargetPoint (_leader.position);
								}
						}						
				}
		}			
}