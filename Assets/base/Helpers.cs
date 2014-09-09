using UnityEngine;
using System.Collections;

public class Helpers
{
		/* Cap the length of a vector, if its size is greater than the limit, resize its length to
		 * the limit while keeping the direction.
		 * Otherwise return the vector */
		public static Vector3 CapVector3 (Vector3 v, float lim)
		{
				if (v.sqrMagnitude > (lim * lim))
						return v.normalized * lim;
				else
						return v;
		}

		/* Cap the size of a float, if its absolute value is greater than the limit, resize it to the limit */
		public static float CapFloat (float input, float lim)
		{
				float absInput = Mathf.Abs (input);
				lim = Mathf.Abs (lim);

				if (absInput > lim)
					return input / absInput * lim;
				return input;
		}

		/* Map angle range from -180 to 180 */
		public static float MapAngle (float input)
		{
				while (input < -180)
						input += 360;
				while (input > 180)
						input -= 360;

				return input;
		}
}