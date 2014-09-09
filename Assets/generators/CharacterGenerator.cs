using UnityEngine;
using System.Collections.Generic;

public class CharacterGenerator : MonoBehaviour
{
		public bool DrawDebug = true;
		public float Padding = 1f;
		private List<Vector4> rooms = new List<Vector4> ();

		public void AddFreeRoom (Vector4 room)
		{
				room.x += Padding;
				room.y += Padding;
				room.z -= Padding * 2;
				room.w -= Padding * 2;
				rooms.Add (room);
		}

		// Use this for initialization
		void Start ()
		{
			// Modify this function to add characters into free rooms
		}
	
		// Update is called once per frame
		void Update ()
		{
				Vector3 tl, tr, bl, br;
				tl = tr = bl = br = Vector3.zero;

				if (DrawDebug)
						foreach (Vector4 room in rooms) {
								tl.x = room.x;
								tl.z = room.y;
								tr.x = tl.x + room.z;
								tr.z = tl.z;
								bl.x = room.x;
								bl.z = tl.z + room.w;
								br.x = tr.x;
								br.z = bl.z;

								Debug.DrawLine (tl, tr, Color.yellow);
								Debug.DrawLine (tr, br, Color.yellow);
								Debug.DrawLine (br, bl, Color.yellow);
								Debug.DrawLine (tl, bl, Color.yellow);
						}
		}
}
