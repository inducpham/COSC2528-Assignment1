using UnityEngine;
using System.Collections;

public class Align : Steering
{
		public float MaxAcceleration = 10f;
		public float TargetRadius = 1f;
		public float SlowRadius = 20;
		public float TimeToTarget = 0.1f;
	
		// Update is called once per frame
		public override SteeringOutput GetSteering ()
		{
				SteeringOutput output = new SteeringOutput ();
				float targetRotation, rotation, rotationSize;
				Vector3 targetEulerAngles;

				// If there is no target to align with, return the empty steering output
				if (TargetTransform == null)
						return output;

				// Fetch the euler angles of target
				targetEulerAngles = GetTargetEulerAngles ();

				// Get the orientation difference
				rotation = targetEulerAngles.y -
						transform.eulerAngles.y;

				// Map rotation size to -180 and 180
				rotation = Helpers.MapAngle (rotation);

				// Record the rotation size
				rotationSize = Mathf.Abs (rotation);

				// If we are there, try to stop the current rotation
				if (rotationSize < TargetRadius) {
						output.angular = -rigidbody.angularVelocity.y / TimeToTarget;
						return output;
				}

				// If we are outside the slow radius, use maximum rotation
				if (rotationSize > SlowRadius) {
						targetRotation = MaxAcceleration;
				}
				// Otherwise calculate a scaled rotation
				else {
						targetRotation = MaxAcceleration * rotationSize / SlowRadius;
				}

				// Final target rotation combines speed and direction
				targetRotation *= rotation / rotationSize;

				// Acceleration tries to get to the target rotation
				output.angular = targetRotation - rigidbody.angularVelocity.y;
				output.angular /= TimeToTarget;

				// Return the output
				return output;
		}
}
