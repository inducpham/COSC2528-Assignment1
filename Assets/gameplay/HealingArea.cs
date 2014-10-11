using UnityEngine;
using System.Collections.Generic;

public class HealingArea : MonoBehaviour
{
		public float Interval = 2;
		private HashSet<GameObject> _contactRecord = new HashSet<GameObject> ();
		private float _cooldown = 0f;
	
		void Start ()
		{
				GetComponent<Combat> ().Invulnerable = true;
				_cooldown = Interval;
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (_cooldown >= 0)
						_cooldown -= Time.deltaTime;
		
				if (_cooldown < 0) {
						_cooldown = Interval;
						Combat b, r;
						b = GetComponent<Combat> ();
			
						foreach (GameObject o in _contactRecord) {
								if ((r = o.GetComponent<Combat> ()) != null)
										r.Heal (b);
						}

						_contactRecord.Clear ();
				}
		}
	
		void OnCollisionStay (Collision collision)
		{
				_contactRecord.Add (collision.gameObject);
		}

		void OnCollisionExit (Collision collision)
		{
				_contactRecord.Remove (collision.gameObject);
		}
}
