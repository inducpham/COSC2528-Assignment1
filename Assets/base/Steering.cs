using UnityEngine;
using System.Collections;

public struct SteeringOutput
{
		public Vector3 linear;
		public float angular;
}

public abstract class Steering : MonoBehaviour
{
		public Vector3 TargetPoint;
		public Transform TargetTransform = null;
		public bool TargetIsPoint = true;
		public bool IsDrawDebug = true;

		public void Start ()
		{
				TargetPoint = transform.position;
		}

		public void Update ()
		{
				if (IsDrawDebug)
						DrawDebug ();
		}

		public virtual Vector3 GetTargetPoint ()
		{
				if (TargetPoint == null)
						return transform.position;
				return TargetPoint;
		}

		public virtual Vector3 GetTargetPosition ()
		{
				if (TargetTransform == null)
						return transform.position;

				return TargetTransform.position;
		}

		public virtual Vector3 GetTargetEulerAngles ()
		{
				if (TargetTransform == null)
						return Vector3.zero;

				return TargetTransform.eulerAngles;
		}

		public virtual Vector3 GetTargetVelocity ()
		{
				if (TargetTransform == null)
						return Vector3.zero;
		
				return TargetTransform.rigidbody.velocity;
		}

		public virtual void SetTargetPoint (Vector3 target)
		{
				TargetPoint = target;
		}

		public virtual void SetTargetPosition(Transform t)
		{
				TargetTransform = t;
		}

		public void DrawDebug ()
		{
				if (TargetIsPoint)
						Debug.DrawLine (transform.position, GetTargetPoint ());
				else if (TargetTransform != null)
						Debug.DrawLine (transform.position, GetTargetPosition ());
		}

		public abstract SteeringOutput GetSteering ();
}
