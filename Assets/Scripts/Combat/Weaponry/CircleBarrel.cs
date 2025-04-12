using UnityEngine;

public class CircleBarrel : Weapon
{
    [Header("UI Settings")]
    [SerializeField] private CircularBarrelUI _uiPrefab;
    private CircularBarrelUI _activeUI;
    [SerializeField] private float _bulletSpreadAngle = 5f;

    void Start()
    {
        shootCooldown = 0;
        _currentAmmo = 8;
        _maxAmmo = 8;
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

        if(_reloadTime > 0)
            _reloadTime -= Time.deltaTime;
        else if(_reloadTime < 0)
        {
            _activeUI.UpdateAmmoDisplay(_currentAmmo, _maxAmmo);
            _reloadTime = 0;
        }
    }

    public override void Fire(Vector2 direction)
    {
        if(shootCooldown <= 0 && _reloadTime <= 0 && _currentAmmo >= 2)
        {
            GameObject bullet1 = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet1.transform.Rotate(0, 0, _bulletSpreadAngle / 2f);
            Rigidbody2D rb1 = bullet1.GetComponent<Rigidbody2D>();
            rb1.linearVelocity = bullet1.transform.right * bulletSpeed;
            bullet1.transform.Rotate(0, 0, bullet1.transform.rotation[2] + 90f);

            GameObject bullet2 = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet2.transform.Rotate(0, 0, -_bulletSpreadAngle / 2f);
            Rigidbody2D rb2 = bullet2.GetComponent<Rigidbody2D>();
            rb2.linearVelocity = bullet2.transform.right * bulletSpeed;
            bullet2.transform.Rotate(0, 0, bullet2.transform.rotation[2] + 90f);

            shootCooldown = shootCD;
            _currentAmmo -= 2;
            _activeUI.UpdateAmmoDisplay(_currentAmmo, _maxAmmo);

            if(_currentAmmo <= 0)
                Reload();
        }
    }
}