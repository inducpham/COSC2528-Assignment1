using UnityEngine;
using System.Collections;

public class Pursue : Arrive
{

		public float MaxPredictionTime = 1;

		public override Vector3 GetTargetPoint ()
		{
				// Find the distance to target
				Vector3 direction = GetTargetPosition () - transform.position;
				float distance = direction.magnitude;
				float predictionTime;

				// Find our current speed
				float targetSpeed = GetTargetVelocity ().magnitude;

				// If target speed is too small to predict
				if (targetSpeed < distance / MaxPredictionTime)
						predictionTime = MaxPredictionTime;
				else
						predictionTime = distance / targetSpeed;

				// Return the predicted position as the target point
				return GetTargetPosition () + GetTargetVelocity () * predictionTime;
		}

}
