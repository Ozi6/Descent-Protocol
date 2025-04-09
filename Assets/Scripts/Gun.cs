using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public const float shootCD = 0.5f;
    private float shootCooldown;

    void Start()
    {
        shootCooldown = 0;
    }

    void Update()
    {
        if(shootCooldown > 0)
            shootCooldown -= Time.deltaTime;
    }

    public void Shoot(Vector2 direction)
    {
        if(shootCooldown <= 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = transform.right * bulletSpeed;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle -= 90f;
            bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            shootCooldown = shootCD;
        }
    }
}