using UnityEngine;
using System.Collections;

public class VelocityMatch : Steering {

	public float MaxAcceleration = 10f;
	public float TimeToTarget = 0.1f;

	public override SteeringOutput GetSteering() {
		SteeringOutput output = new SteeringOutput();

		// If there is no target to match velocity, return output
		if (TargetTransform == null) {
			return output;
		}

		// Acceleration tries to match with the target velocity
		output.linear = GetTargetVelocity() - rigidbody.velocity;
		output.linear /= TimeToTarget;

		// Check if the acceleration is too fast
		if (output.linear.sqrMagnitude > (MaxAcceleration * MaxAcceleration)) {
			output.linear = output.linear.normalized * MaxAcceleration;
		}

		// Return the steering output
		return output;
	}
}
