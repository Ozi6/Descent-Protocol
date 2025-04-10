using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected float shootCooldown = 0.5f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public const float shootCD = 0.5f;

    public virtual void Fire(Vector2 direction)
    {

    }
}
