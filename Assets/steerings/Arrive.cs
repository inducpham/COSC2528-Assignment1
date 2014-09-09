using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Movement))]
public class Arrive : Steering
{
		public float MaxSpeed = 4;
		public float TargetRadius = .1f;
		public float SlowRadius = 1f;
		public float TimeToTarget = 0.1f;
	
		// Update is called once per frame
		public override SteeringOutput GetSteering ()
		{
				// Create the steering output to hold the steering
				SteeringOutput output = new SteeringOutput ();

				// Get the direction to the target
				Vector3 direction = GetTargetPoint() - transform.position;
				Vector3 targetVelocity;
				float distance = direction.magnitude;
				float targetSpeed = 0f;

				// If we are there, negate all the velocity
				if (distance < TargetRadius) {
						output.linear = - rigidbody.velocity;			
						return output;
				}

				// If we are outside the slow radius, go max speed
				if (distance > SlowRadius)
						targetSpeed = MaxSpeed;

				// Otherwise calculate a scaled speed
				else
						targetSpeed = MaxSpeed * distance / SlowRadius;

				// The target velocity combines speed and direction
				targetVelocity = direction.normalized * targetSpeed;

				// Acceleration tries to match the target velocity
				output.linear = targetVelocity - rigidbody.velocity;
				output.linear /= TimeToTarget;
				
				return output;
		}
}
