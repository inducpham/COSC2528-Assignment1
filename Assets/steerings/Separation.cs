using UnityEngine;
using System.Collections.Generic;

public class Separation : Steering {

	public float Threshold = 2f;
	public float MaxAcceleration = 15f;
	public float DecayCoefficient = 0.3f;
	public float MaxPredictionTime = 0.5f;
	private HashSet<Transform> _targets = new HashSet<Transform>();

	public void SetTargets(HashSet<Transform> targets)
	{
		_targets = new HashSet<Transform>(targets);
		_targets.Remove (transform);
	}

	public override SteeringOutput GetSteering ()
	{
		SteeringOutput output = new SteeringOutput();

		foreach (Transform target in _targets)
		{
			Vector3 direction = target.position - transform.position;
			float distance = direction.magnitude;

			if (distance < Threshold)
			{
				float strength = Mathf.Min(DecayCoefficient / (distance * distance),
											MaxAcceleration);

				output.linear -= strength * direction.normalized;
			}

		}

		return output;
	}
}
