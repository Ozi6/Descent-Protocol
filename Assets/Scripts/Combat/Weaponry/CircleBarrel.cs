using UnityEngine;

public class CircleBarrel : Weapon
{
    void Start()
    {
        shootCooldown = 0;
    }

    void Update()
    {
        if (shootCooldown > 0)
            shootCooldown -= Time.deltaTime;
    }

    public override void Fire(Vector2 direction)
    {
        if(shootCooldown <= 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = transform.right * bulletSpeed;
            shootCooldown = shootCD;
        }
    }
}
