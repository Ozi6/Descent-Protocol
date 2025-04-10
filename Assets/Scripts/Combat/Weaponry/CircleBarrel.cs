using UnityEngine;

public class CircleBarrel : Weapon
{
    [Header("UI Settings")]
    [SerializeField] private CircularBarrelUI _uiPrefab;
    private CircularBarrelUI _activeUI;

    void Start()
    {
        shootCooldown = 0;
        _currentAmmo = 8;
        InitializeUI();
    }

    private void InitializeUI()
    {
        if(_uiPrefab != null)
        {
            _activeUI = Instantiate(_uiPrefab, FindFirstObjectByType<Canvas>().transform);
            _activeUI.UpdateAmmoDisplay(_currentAmmo, _maxAmmo);
        }
    }

    void Update()
    {
        if (shootCooldown > 0)
            shootCooldown -= Time.deltaTime;
    }

    public override void Fire(Vector2 direction)
    {
        if(shootCooldown <= 0 && _currentAmmo > 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = transform.right * bulletSpeed;
            shootCooldown = shootCD;
            _currentAmmo--;
            _activeUI.UpdateAmmoDisplay(_currentAmmo, _maxAmmo);
        }
    }
}