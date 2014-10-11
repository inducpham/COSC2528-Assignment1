using UnityEngine;
using System.Collections;

public class Combat : MonoBehaviour {

	public int MaxHp = 5;
	public int Hp;
	public bool Invulnerable = false;
	public float AttackCooldown = 2;
	public GameObject BleedPrefab = null;
	public GameObject HealingPrefab = null;
	private Combat _lastDamage = null;
	private Combat _lastHeal = null;
	private float _cooldown = 0f;

	// Use this for initialization
	void Start () {
		Hp = MaxHp;
	}
	
	// Update is called once per frame
	void Update () {
		if (_cooldown > 0)
			_cooldown -= Time.deltaTime;
	}

	public void Attack (GameObject target)
	{
		Combat targetCombat = null;

		if (_cooldown > 0) return;
		targetCombat = target.GetComponent<Combat>();
		if (targetCombat == null) return;

		targetCombat.Receive(this);
		_cooldown = AttackCooldown;
	}

	public void Receive(Combat combat)
	{
		if (Invulnerable) return;

		if (BleedPrefab != null)
			Instantiate(BleedPrefab, transform.position, Quaternion.LookRotation(Vector3.up));
		Hp -= 1;
		_lastDamage = combat;
		if (Hp < 1) Destroy(gameObject);
	}
	
	public void Heal(Combat combat)
	{
		if (HealingPrefab != null)
			Instantiate(HealingPrefab, transform.position, Quaternion.LookRotation(Vector3.up));
		if (Hp < MaxHp)
			Hp += 1;
		_lastHeal = combat;
	}

	public string PromptString ()
	{
		return Hp + "/" + MaxHp;
	}
}
