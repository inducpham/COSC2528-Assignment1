using UnityEngine;
using System.Collections;
using IronScheme;

public class Individual : MonoBehaviour {
	public float fitnessScore=0f;
	public object Gene{
		get{ 
			NN n=GetComponent<NN> ();
			if(n==null){
				Debug.LogError("Idividual.Gene: NN componnent doesn't exist at "+this.name);
				return null;
			}
			return n.NNet;}
		set{ 
			NN n=GetComponent<NN> ();
			if(n==null){
				Debug.LogError("Idividual.Gene: NN componnent doesn't exist at "+this.name);
				return;
			}
			n.NNet = value;
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
