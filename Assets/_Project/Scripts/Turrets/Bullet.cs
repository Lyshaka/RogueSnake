using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] Rigidbody rb;

	private float _damage;
	private Turret _origin;

	public void Setup(float damage, float speed, Turret origin)
	{
		rb.linearVelocity = transform.forward * speed;
		_damage = damage;
		_origin = origin;
		Destroy(gameObject, 10f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 10) // Enemy
		{
			other.GetComponent<Enemy>().Damage(_damage, _origin);
			Destroy(gameObject);
		}
	}
}
