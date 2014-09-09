using UnityEngine;
using System.Collections;

public class Seek : Steering
{
		public float Acceleration = 10f;
		public bool MoveAway = false;

		public void Start ()
		{
				TargetPoint = transform.position;
		}

		public override SteeringOutput GetSteering ()
		{
				SteeringOutput output = new SteeringOutput ();
				output.linear = GetTargetPosition () - transform.position;
				output.linear = output.linear.normalized * Acceleration;

				if (MoveAway)
						output.linear = -output.linear;

				return output;
		}
}
