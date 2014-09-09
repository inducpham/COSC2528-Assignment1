using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
		public float MaxVelocity = 10;
		public float MaxLinearAcceleration = 10;
		public float MaxRotation = 10f;
		public float MaxAngularAcceleration = 10f;
		private Vector3 _linearSteering;
		private float _angularSteering;

		/* During awake, set the rigid body constrain to freeze on y axis */
		public void Awake ()
		{
				rigidbody.constraints = RigidbodyConstraints.FreezePositionY |
						RigidbodyConstraints.FreezeRotationX | 
						RigidbodyConstraints.FreezeRotationZ;
		}

		/* Update steering combine all the steering behaviour and update the velocity and rotation of the
		 * kinematic.
		 * Update steering also limit how fast can the steering be, based on the maximum value of maximum
		 * acceleration.
		 * Param is dt, which is the delta time between updates */

		public void UpdateSteering (float dt)
		{
				_linearSteering = Vector3.zero;
				_angularSteering = 0f;

				Steering[] steerings = GetComponents<Steering> ();
				int count = 0;

				foreach (Steering steering in steerings) {
						if (steering.enabled) {
								count++;
								SteeringOutput o = steering.GetSteering ();
								_linearSteering += o.linear;
								_angularSteering += o.angular;
						}
				}

				_linearSteering /= count;
				_angularSteering /= count;
				_linearSteering = Helpers.CapVector3 (_linearSteering * dt, MaxLinearAcceleration);
				_angularSteering = Helpers.CapFloat (_angularSteering * dt, MaxAngularAcceleration);
		}

		/* During the update, the movement update its velocity with steering behaviours, then cull
		 * the velocity from 3d into 2d, with the starting height as the reference.
		 * The velocity is also capped by the maxSpeed, which limit what is the maximum velocity
		 * that can be achieved.
		 * Also, depends on the implementation that the update may manually set orientation based
		 * on the current velocity
		 * TODO: properly implement rotation and orientation */
		public void Update ()
		{
				UpdateSteering (Time.deltaTime);

				Vector3 v = rigidbody.velocity;
				v += _linearSteering;
				v.y = 0;
				rigidbody.velocity = Helpers.CapVector3 (v, MaxVelocity);
			
				Vector3 r = rigidbody.angularVelocity;
				r.y += _angularSteering;
				r.y = Helpers.CapFloat (r.y, MaxRotation);
				rigidbody.angularVelocity = r;

				/* No need to use get new orientation, we are using steering-provided rotation */
//				transform.rotation = _GetNewOrientation ();
		}

		private Quaternion _GetNewOrientation ()
		{
				return Quaternion.LookRotation (rigidbody.velocity, Vector3.up);
		}
}
