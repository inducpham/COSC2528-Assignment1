using UnityEngine;
using System.Collections;

public class Face : Steering
{

		public float MaxAcceleration = 10f;
		public float TargetRadius = 1f;
		public float SlowRadius = 20;
		public float TimeToTarget = 1f;

		public void Start ()
		{
				if (TargetTransform == null)
						TargetPoint = transform.position + new Vector3 (0, 0, 1);
		}
	
		// Update is called once per frame
		public override SteeringOutput GetSteering ()
		{
				SteeringOutput output = new SteeringOutput ();
				float targetRotation, rotation, rotationSize;
		
				// Get the direction to the target point
				Vector3 faceDirection = GetTargetPoint() - transform.position;

				// If the direction is empty, can not determine target rotation, return nil
				if (faceDirection.sqrMagnitude <= 0)
						return output;

				// Get target rotation
				float newRotation = Mathf.Atan2 (-faceDirection.z, faceDirection.x) * Mathf.Rad2Deg;
				rotation = newRotation - transform.eulerAngles.y;
		
				// Map rotation size to -180 and 180
				rotation = Helpers.MapAngle (rotation);
		
				// Record the rotation size
				rotationSize = Mathf.Abs (rotation);
		
				// If we are there, try to stop the current rotation
				if (rotationSize < TargetRadius) {
						output.angular = -rigidbody.angularVelocity.y;
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