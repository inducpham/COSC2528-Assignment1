using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PriorityQueueDemo;

public class PathfindingMap : MonoBehaviour
{	
		public float BorderGrow = 0.1f;
		public float TileWidth = 1f, TileHeight = 1f;
		public float StartX = 0f, StartY = 0f;
		public bool DrawDebug = true;
		private bool[,] Grid = null;
		private Vector3[,] GridVectors = null;
		private int Width = 0, Height = 0;
		private Vector3 _offset;
	
		public void SetSize (float startX, float startY, int width, int height)
		{
				StartX = startX;
				StartY = startY;
				Width = width;
				Height = height;
				_offset = new Vector3 (TileWidth / 2, 0, TileHeight / 2);
				Grid = new bool[width, height];
				GridVectors = new Vector3[width + 1, height + 1];
				
				//init grid values
				for (int x = 0; x < Width; x++)
						for (int y = 0; y < Height; y++)
								Grid [x, y] = true;
		
				//setup grid of vectors for easy debug line
				for (int x = 0; x <= Width; x++)
						for (int y = 0; y <= Height; y++) {
								GridVectors [x, y] = new Vector3 (StartX + x * TileWidth, 0,
			                                  StartY + y * TileHeight);
						}
		}
	
		public void DisableSpace (float x, float y, float w, float h)
		{
				x += BorderGrow - StartX;
				y += BorderGrow - StartY;
				w -= 2 * BorderGrow;
				h -= 2 * BorderGrow;
		
				for (int ix = (int) x; ix < x + w; ix++)
						for (int iy = (int) y; iy < y + h; iy++)
								if (0 <= ix && ix < Width && 0 <= iy && iy < Height)
										Grid [ix, iy] = false;
		}

		private LinkedList<Vector3> ReconstructPath (IDictionary<Vector3, Vector3> cameFrom, Vector3 goal,
	                                            Vector3 originalGoal, bool printDebug)
		{
				LinkedList<Vector3> path = new LinkedList<Vector3> ();
				Vector3 current = goal;
				Vector3 toAdd;

				if (printDebug) Debug.Log ("Explored nodes count: " + cameFrom.Count);
		
				while (true) {
						toAdd = current;
						toAdd.y = originalGoal.y;
						path.AddFirst (toAdd + _offset);

						if (!cameFrom.ContainsKey (current))
								break;
						current = cameFrom [current];
				}

				path.AddLast (originalGoal);

				if (printDebug) Debug.Log ("Path count: " + path.Count);

				return path;
		}

		private LinkedList<Vector3> TryAddNode (int x, int y, LinkedList<Vector3> list)
		{
				if (x < 0 || x >= Width || y < 0 || y >= Height)
						return list;

				if (!Grid [x, y])
						return list;

				list.AddLast (GridVectors [x, y]);
				return list;
		}
	 
		private bool NodeIsValid (Vector3 node)
		{
				/*translate to map index*/
				int x = (int)((node.x - StartX) / TileWidth);
				int y = (int)((node.z - StartY) / TileHeight);
		
				if (x < 0 || x >= Width || y < 0 || y >= Height)
						return false;

				return !Grid [x, y];
		}

		private LinkedList<Vector3> GetNodeNeighbor (Vector3 node)
		{
				LinkedList<Vector3> neighbors = new LinkedList<Vector3> ();

				/*translate to map index*/
				int ix = (int)((node.x - StartX) / TileWidth);
				int iy = (int)((node.z - StartY) / TileHeight);

				neighbors = TryAddNode (ix - 1, iy - 1, neighbors);
				neighbors = TryAddNode (ix - 1, iy, neighbors);
				neighbors = TryAddNode (ix - 1, iy + 1, neighbors);
				neighbors = TryAddNode (ix, iy - 1, neighbors);
				neighbors = TryAddNode (ix, iy + 1, neighbors);
				neighbors = TryAddNode (ix + 1, iy - 1, neighbors);
				neighbors = TryAddNode (ix + 1, iy, neighbors);
				neighbors = TryAddNode (ix + 1, iy + 1, neighbors);

				return neighbors;
		}

		private float HeuristicFunction (Vector3 from, Vector3 target)
		{

				float dx, dz;
				dx = Mathf.Abs (from.x - target.x);
				dz = Mathf.Abs (from.z - target.z);
		
				//return Mathf.Max(dx, dz);
				//return dx * dx + dz * dz;
				return dx + dz;
		}

		public LinkedList<Vector3> PlanPath (Vector3 start, Vector3 goal, bool printDebug)
		{
				Vector3 oStart = start, oGoal = goal;
				//Normalize the start and goal to grid coordinate
				start.x = (int)Mathf.Floor (start.x);
				start.y = 0;
				start.z = (int)Mathf.Floor (start.z);
				goal.x = (int)Mathf.Floor (goal.x);
				goal.y = 0;
				goal.z = (int)Mathf.Floor (goal.z);

				if (NodeIsValid (goal))
						return new LinkedList<Vector3> ();

				Vector3 current;

				HashSet<Vector3> closedSet = new HashSet<Vector3> ();
				PriorityQueue<float, Vector3> openSetQueue = new PriorityQueue<float, Vector3> ();
				HashSet<Vector3> openSet = new HashSet<Vector3> ();
				openSetQueue.Enqueue (0, start);
				openSet.Add (start);
				Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3> ();
				Dictionary<Vector3, float> gDict = new Dictionary<Vector3, float> ();
				Dictionary<Vector3, float> fDict = new Dictionary<Vector3, float> ();
				gDict [start] = 0;
				fDict [start] = gDict [start] + HeuristicFunction (start, goal);

				while (openSetQueue.Count > 0) { // while the open set is not empty
						current = openSetQueue.Dequeue ().Value;
						if (current == goal)
								return ReconstructPath (cameFrom, goal, oGoal, printDebug);

						closedSet.Add (current);
						openSet.Remove (current);

						foreach (Vector3 neighbor in GetNodeNeighbor(current)) {
								if (closedSet.Contains (neighbor))
										continue;

								float totalG = gDict [current] + HeuristicFunction (current, neighbor);
								if (!openSet.Contains (neighbor) || totalG < gDict [neighbor]) {
										cameFrom [neighbor] = current;
										gDict [neighbor] = totalG;
										fDict [neighbor] = gDict [neighbor] + HeuristicFunction (neighbor, goal);

										if (!openSet.Contains (neighbor)) {
												openSetQueue.Enqueue (fDict [neighbor], neighbor);
												openSet.Add (neighbor);
										}
								}
						}
				}

				//return no path
				return new LinkedList<Vector3> ();
		}
	
		//draw debug
		public void Update ()
		{
				if (DrawDebug)
						for (int x = 0; x < Width; x++)
								for (int y = 0; y < Height; y++) {
										if (Grid [x, y]) {
												Debug.DrawLine (GridVectors [x, y], GridVectors [x + 1, y], Color.blue);
												Debug.DrawLine (GridVectors [x, y], GridVectors [x, y + 1], Color.blue);
												Debug.DrawLine (GridVectors [x + 1, y], GridVectors [x + 1, y + 1], Color.blue);
												Debug.DrawLine (GridVectors [x, y + 1], GridVectors [x + 1, y + 1], Color.blue);
										}
								}
		}
}
