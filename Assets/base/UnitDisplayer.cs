using UnityEngine;
using System.Collections;

public class UnitDisplayer : MonoBehaviour
{

		public Transform target = null;
		public float height = 2f;
		private GUIText _text = null;
		private Combat _combat = null;

		void Start ()
		{
				_text = GetComponent<GUIText> ();
				if (_text != null)
						_text.text = "";

				if (target != null)
						_combat = target.GetComponent<Combat> ();
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (target == null) {
						Destroy (this.gameObject);
						return;
				}

				if (_combat != null) {
						//update based on combat
						_text.text = _combat.PromptString ();
				}

				Vector3 newPos = Camera.mainCamera.WorldToViewportPoint (target.position);
				newPos.z = height;

				transform.position = newPos;
		}
}
