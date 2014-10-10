using UnityEngine;
using System.Collections.Generic;

public class Separation : Steering {

	public float Threshold = 2f;
	public float MaxAcceleration = 15f;
	public float DecayCoefficient = 0.3f;
	public float MaxPredictionTime = 0.5f;
	private HashSet<GameObject> _targets = new HashSet<GameObject>();

	public void SetTargets(HashSet<GameObject> targets)
	{
		_targets = new HashSet<GameObject>(targets);
		_targets.Remove (gameObject);
	}

	public override SteeringOutput GetSteering ()
	{
		SteeringOutput output = new SteeringOutput();

		foreach (GameObject target in _targets)
		{
			Vector3 direction = target.transform.position - transform.position;
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
