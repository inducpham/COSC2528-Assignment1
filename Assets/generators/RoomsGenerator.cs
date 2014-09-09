using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomsGenerator : MonoBehaviour
{
		public int Iteration = 4;
		public float X = -30,
				Y = -30,
				Width = 60,
				Height = 60;
		public float DoorSize = 2f;
		public float WallSize = 4f;
		public float WallHeight = 2f;
		public GameObject PathfindingMapHost = null;
		public GameObject CharacterGeneratorHost = null;
	
		// Use this for initialization
		void Awake ()
		{
				ArrayList walls = new ArrayList ((int)Mathf.Pow (2, Iteration));
				ArrayList freeRooms = new ArrayList ((int)Mathf.Pow (2, Iteration));
				ArrayList roomsToRemove = new ArrayList ();
		
				Room baseRoom = new Room (X, Y, Width, Height);
				baseRoom.Split (Iteration, walls, freeRooms, WallSize);
		
				for (int i = 0; i < walls.Count; i++) {
						(walls [i] as Wall).CreatePrimitives ("Wall", DoorSize, WallSize, WallHeight);
				}
		
				// Create the bounding wall
				GameObject top = GameObject.CreatePrimitive (PrimitiveType.Cube);
				top.transform.position = new Vector3 (0, 0.5f, Y - 0.5f);
				top.transform.localScale = new Vector3 (Width, 1, 1);
				GameObject bottom = GameObject.CreatePrimitive (PrimitiveType.Cube);
				bottom.transform.position = new Vector3 (0, 0.5f, Y + Height + 0.5f);
				bottom.transform.localScale = new Vector3 (Width, 1, 1);
				GameObject left = GameObject.CreatePrimitive (PrimitiveType.Cube);
				left.transform.position = new Vector3 (X - 0.5f, 0.5f, 0);
				left.transform.localScale = new Vector3 (1, 1, Height);
				GameObject right = GameObject.CreatePrimitive (PrimitiveType.Cube);
				right.transform.position = new Vector3 (X + Width + 0.5f, 0.5f, 0);
				right.transform.localScale = new Vector3 (1, 1, Height);
				top.tag = "Border";
				bottom.tag = "Border";
				left.tag = "Border";
				right.tag = "Border";
		
				// Init the PathfindingMapHost
				PathfindingMap map = PathfindingMapHost.GetComponent<PathfindingMap> ();
				if (map == null)
						return;
				map.SetSize (X, Y, (int)Width, (int)Height);
		
				// Gather all the walls this crash the game though
				GameObject[] wallObjects = GameObject.FindGameObjectsWithTag ("Wall");
				foreach (GameObject wallObject in wallObjects) {
			
						// Determine the x, y, w, h of the wall object
						float cx, cz, x, z, w, h, sw, sh;
						cx = wallObject.transform.position.x;
						cz = wallObject.transform.position.z;
						sw = wallObject.transform.localScale.x;
						sh = wallObject.transform.localScale.z;
						if (wallObject.transform.eulerAngles.y > 0) {
								x = cx - sh / 2;
								z = cz - sw / 2;
								w = sh;
								h = sw;
						} else {
								x = cx - sw / 2;
								z = cz - sh / 2;
								w = sw;
								h = sh;
						}
			
						map.DisableSpace (x, z, w, h);
						//wallObject.renderer.enabled = false;
				}

				// Filter the room list, only allow room that has enough spaces:
				Room room;
				foreach (object roomObject in freeRooms) {
						room = roomObject as Room;
						if (!room.CanContain (8, 8)) //HACK: constants
								roomsToRemove.Add (room);
				}
				foreach (object roomObject in roomsToRemove) {
						freeRooms.Remove (roomObject);
				}

				CharacterGenerator chargen = null;
				if (CharacterGeneratorHost != null) 
						chargen = CharacterGeneratorHost.GetComponent<CharacterGenerator> ();
				if (chargen != null)
						foreach (object roomObject in freeRooms) {
								room = roomObject as Room;
								chargen.AddFreeRoom (room.ToVector4 ());
						}
		}
	
		// Update is called once per frame
		void Update ()
		{
		
		}
}

class Wall
{
	
		private float _x, _y, _length;
		private bool _horizontal;
	
		public bool IsHorizontal { get { return _horizontal; } }
	
		public float X { get { return _x; } }
	
		public float Y { get { return _y; } }
	
		private ArrayList _doors = new ArrayList ();
	
		public Wall (float x, float y, float length, bool horizontal)
		{
				// need a reference to parent wall to randomly create door, anyhow
				_x = x;
				_y = y;
				_length = length;
				_horizontal = horizontal;
		}
	
		public void CreateDoor (float from, float to)
		{
				from = from < 0 ? from : 0f;
				to = to > _length ? to : _length;
				int r = (int)(from + CurvedRandom (to - from, 1));
				_doors.Add (r);
		}
	
		public void CreatePrimitives (string tag, float fDoorSize, float wallSize, float wallHeight)
		{
				//sort the doors order
				_doors.Sort ();
		
				int halfDoorSize = (int)fDoorSize / 2 + 1;
				int startPos, length, endPos = -halfDoorSize;
		
				//iterate through the doors
				for (int i = 0; i < _doors.Count; i++) {
						endPos = (int)_doors [i] - halfDoorSize;
			
						if (i == 0)
								startPos = 0;
						else
								startPos = endPos + halfDoorSize * 2;
			
						length = Mathf.Abs (endPos - startPos);
						if (length < halfDoorSize * 2)
								continue;
			
						CreateLine (tag, startPos, length, wallSize, wallHeight);
				}
		
				CreateLine (tag, endPos + halfDoorSize * 2, _length - halfDoorSize * 2 - endPos, wallSize, wallHeight);
		}
	
		private void CreateLine (string tag, float start, float length, float wallSize, float wallHeight)
		{
				if (length < 1)
						return;
				GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
				float halfLength = Mathf.Floor (length / 2);
		
				cube.transform.localScale = new Vector3 (wallSize, wallHeight, halfLength * 2);
				if (_horizontal) {
						cube.transform.position = new Vector3 (_x + start + halfLength, wallHeight / 2, _y);
						cube.transform.eulerAngles = new Vector3 (0, 90, 0);
				} else
						cube.transform.position = new Vector3 (_x, wallHeight / 2, _y + start + halfLength);
		
				cube.tag = tag;
		}
	
		static float CurvedRandom (float range, int curveHeight)
		{
				float randValue = 1f / curveHeight;
		
				float rand = 0;
				for (int i = 0; i < curveHeight; i++) {
						rand += Random.Range (0f, randValue);
				}
				return rand * range;
		}
}

class Room
{	
		private Room child1, child2;
		private float _x, _y, _width, _height;
		private Wall _top = null;
		private Wall _bottom = null;
		private Wall _left = null;
		private Wall _right = null;
	
		public Room (float x, float y, float width, float height)
		{
				_x = x;
				_y = y;
				_width = width;
				_height = height;
		}
	
		public void SetWall (Wall top, Wall bottom, Wall left, Wall right)
		{
				_top = top;
				_bottom = bottom;
				_left = left;
				_right = right;
		
				// bust open a door koolaid style
				Wall[] walls = { top, bottom, left, right };
				int i;
		
				for (int j = 0; j < 1; j++) {
						i = Random.Range (0, 4);
						Wall wall = walls [i];
						if (wall != null) {
								if (wall.IsHorizontal) 
										wall.CreateDoor (wall.X - _x, wall.X - _x + _width);
								else
										wall.CreateDoor (wall.Y - _y, wall.X - _x + _height);
						} else
								j--;
				}
		}

		public bool CanContain (float width, float height)
		{
				return (_width >= width && _height >= height);
		}

		public Vector4 ToVector4 ()
		{
				return new Vector4 (_x, _y, _width, _height);
		}
	
		public void Split (int iteration, ArrayList wallCollector, ArrayList roomCollector, float wallSize)
		{
				bool splitBias = Random.Range (0f, 1f) > 0.5f;
				Split (iteration, wallCollector, roomCollector, wallSize, splitBias);
		}
	
		public void Split (int iteration, ArrayList wallCollector, ArrayList roomCollector, float wallSize, bool splitBias)
		{
				if (iteration == -1) {
						roomCollector.Add (this);
						return;
				}
		
				if (iteration-- == 0)
				if (!(_width / _height > 2f || _height / _width > 2f)) {
						roomCollector.Add (this);
						return;
				}
		
				int halfWallSize = (int)wallSize / 2;
		
				bool horizontal = splitBias;
				splitBias = !splitBias;
				//		horizontal = Random.Range(0f, 1f) > 0.5f;
				//		if (horizontal != splitBias) {
				//			horizontal = Random.Range(0f, 1f) > 0.5f;
				//			splitBias = !splitBias;
				//		}
		
				Wall newWall;
				float newDoorRange;
		
				if (horizontal) {
						int r = (int)CurvedRandom (_height, 10);
						newWall = new Wall (_x, _y + r, _width, true);
						child1 = new Room (_x, _y, _width, r - halfWallSize);
						child1.SetWall (_top, newWall, _left, _right);
						child2 = new Room (_x, _y + r, _width, _height - r + halfWallSize);
						child2.SetWall (newWall, _bottom, _left, _right);
						newDoorRange = _width;
				} else { //vertical split
						int r = (int)CurvedRandom (_width, 10);
						newWall = new Wall (_x + r, _y, _height, false);
						child1 = new Room (_x, _y, r - halfWallSize, _height);
						child1.SetWall (_top, _bottom, _left, newWall);
						child2 = new Room (_x + r, _y, _width - r + halfWallSize, _height);
						child2.SetWall (_top, _bottom, newWall, _right);
						newDoorRange = _height;
			
				}
		
				// wall with early iteration should have many exit, in case later iterations block it accidentally
				for (int i = 0; i < iteration + 1; i++)
						newWall.CreateDoor (halfWallSize, newDoorRange - halfWallSize);
		
				child1.Split (iteration, wallCollector, roomCollector, wallSize, splitBias);
				child2.Split (iteration, wallCollector, roomCollector, wallSize, splitBias);
		
				wallCollector.Add (newWall);
		}
	
		static float CurvedRandom (float range, int curveHeight)
		{
				float randValue = 1f / curveHeight;
		
				float rand = 0;
				for (int i = 0; i < curveHeight; i++) {
						rand += Random.Range (0f, randValue);
				}
				return rand * range;
		}
}