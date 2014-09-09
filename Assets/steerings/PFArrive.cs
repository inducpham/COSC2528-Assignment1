using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PFArrive : Arrive {
	
	public Transform PathfinderHost = null;
	public float NodeArriveRadius = 1f;
	public bool PrintPFDebug = false;
	private PathfindingMap pathfinder = null;
	private LinkedList<Vector3> path = null;

	/* CAVEAT: make sure to set node arrive radius to be about greater than 1,
	 * otherwise arrive function would not be able to progress in the case of
	 * NodeArriveRadius < 0.2f for example */

	new public void Start() {
		base.Start();
		TargetIsPoint = true;
		if (PathfinderHost != null) {
			pathfinder = PathfinderHost.GetComponent<PathfindingMap>();
		}
	}

	public override void SetTargetPoint(Vector3 target)
	{
		if (pathfinder != null)
		{
			path = pathfinder.PlanPath(transform.position, target, PrintPFDebug);
			if (path.Count == 0) base.SetTargetPoint(target);
		}
		else
		{
			base.SetTargetPoint(target);
		}
	}
	
	public override Vector3 GetTargetPoint ()
	{
		if (path == null || path.Count == 0)
			return base.GetTargetPoint();

		//otherwise return the incoming node from path
		return path.First.Value;
	}

	public void Update()
	{
		base.Update();

		if (path != null && path.Count > 0) 
		{
			if (path.Count > 1)
				//remove the next node if arrive, and transition to the next node from the path)
				if ((path.First.Value - transform.position).sqrMagnitude < NodeArriveRadius*NodeArriveRadius)
					path.RemoveFirst();

			Vector3 curr, next;
			next = transform.position;
			foreach (Vector3 node in path)
			{
				curr = next; next = node;
				Debug.DrawLine(curr, next, Color.green);
			}
		}
	}
	
}
