﻿using UnityEngine;
using System.Collections.Generic;

public class CharacterGenerator : MonoBehaviour
{
		public bool DrawDebug = true;
		public float Padding = 1f;
		private List<Vector4> rooms = new List<Vector4> ();
		public Transform PathfinderHost = null;
		public GameObject PlayerPrefab = null;
		public GameObject FollowerPrefab = null;
		public GameObject EnemyPrefab = null;
		public GameObject TextPrefab = null;
		public GameObject HealingRoomPrefab = null;
		public GameObject HarmingRoomPrefab = null;

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
				//HACK, if character generator is enabled, destroy all the prototype gameobject
				foreach (GameObject o in GameObject.FindGameObjectsWithTag("Player"))
						Destroy (o);
				foreach (GameObject o in GameObject.FindGameObjectsWithTag("Follower"))
						Destroy (o);
				foreach (GameObject o in GameObject.FindGameObjectsWithTag("Enemy"))
						Destroy (o);
				foreach (GameObject o in GameObject.FindGameObjectsWithTag("Pane"))
						Destroy (o);
				// Modify this function to add characters into free rooms
				// Shuffle the freeroom list
				ShuffleRooms ();

				int count = rooms.Count;
				int paneCount = count / 5;
				count -= paneCount;

				// Let 1/10 be the healing / harming;
				int healingPaneCount = paneCount / 2;
				int harmingPaneCount = paneCount - healingPaneCount;
				
				// Get the number of rooms for player, follower and enemies
				int followerCount = (count - 1) / 2;
				int enemiesCount = count - 1 - followerCount;
				int index = 0;

				GameObject player = null;
				EnemyUnit enemyU = null;
				player = AddUnitToRoom (PlayerPrefab, rooms [index++], "player");
				for (int i = 0; i < followerCount; i++)
						AddUnitToRoom (FollowerPrefab, rooms [index++], "follower");
				for (int i = 0; i < enemiesCount; i++) {
						enemyU = AddUnitToRoom (EnemyPrefab, rooms [index++], "enemy")
									.GetComponent<EnemyUnit> ();
						//this section is very important, it allows the threat to neutralize itself
						if (enemyU != null)
								enemyU.Player = player;
				}
				//Apply healing/harming pane
				for (int i = 0; i < healingPaneCount; i++)
						AddPaneToRoom (HealingRoomPrefab, rooms [index++]);
				for (int i = 0; i < harmingPaneCount; i++)
						AddPaneToRoom (HarmingRoomPrefab, rooms [index++]);

				//Focus the game camera on the player
				Camera c = Camera.main;

				if (player != null && c.GetComponent<Arrive> () != null) {
						c.GetComponent<Arrive> ().SetTargetPosition (player.transform);
						Vector3 cp = c.transform.position;
						Vector3 pp = player.transform.position;
						cp.x = pp.x;
						cp.z = pp.z;
						c.transform.position = cp;
				}
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

		private void ShuffleRooms ()
		{  
				int n = rooms.Count;  
				while (n > 1) {  
						n--;  
						int k = Random.Range (0, n + 1);  
						Vector4 value = rooms [k];  
						rooms [k] = rooms [n];  
						rooms [n] = value;  
				}
		}

		private GameObject AddPaneToRoom (GameObject prefab, Vector4 v)
		{
				if (prefab == null)
						return null;

				GameObject p = Instantiate (prefab) as GameObject;
				Vector3 position = p.transform.position;
				Vector3 scale = p.transform.localScale;
				position.x = v.x + v.z / 2;
				position.y = 0.01f;
				position.z = v.y + v.w / 2;
				scale.x *= v.z;
				scale.z *= v.w;
				p.transform.position = position;
				p.transform.localScale = scale;
				p.transform.rotation = Quaternion.identity;

				return p;
		}

		private GameObject AddUnitToRoom (GameObject prefab, Vector4 v, string name)
		{
				if (prefab == null)
						return null;
				Vector3 position = new Vector3 (v.x + v.z / 2, 1f, v.y + v.w / 2);

				GameObject o = Instantiate (prefab, position, Quaternion.identity) as GameObject;
				o.name = name;
		
				//CRUDE HACK: get pfarrive, then manually set the pathfinder host :(
				PFArrive pathfinder = o.GetComponent<PFArrive> ();

				if (pathfinder != null && PathfinderHost != null) {
						pathfinder.PathfinderHost = PathfinderHost;
						pathfinder.Start ();
				}

				//Add the unit displayer
				if (TextPrefab) {
						GameObject text = Instantiate (TextPrefab, position, Quaternion.identity) as GameObject;
						UnitDisplayer d = text.GetComponent<UnitDisplayer> ();
						if (d != null)
								d.target = o.transform;
				}

				return o;
		}
}
