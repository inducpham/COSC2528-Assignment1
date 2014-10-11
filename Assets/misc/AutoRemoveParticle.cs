using UnityEngine;
using System.Collections;

public class AutoRemoveParticle : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (GetComponent<ParticleSystem>() != null)
			StartCoroutine(Autodestroy());	
	}

	IEnumerator Autodestroy() {
		ParticleSystem s = GetComponent<ParticleSystem>();
		yield return new WaitForSeconds(s.duration + s.startLifetime);
		Destroy(this.gameObject);
	}
}
