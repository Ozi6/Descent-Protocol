using System.Collections;
using UnityEngine;

public class Flamethrower : Weapon
{
    [Header("UI Settings")]
    [SerializeField] private FlamethrowerUI _uiPrefab;
    private FlamethrowerUI _activeUI;
    [SerializeField] private float _bulletSpreadAngle = 5f;

    [Header("Flamethrower Settings")]
    [SerializeField] private float _fireRate = 0.1f;

    void Start()
    {
        shootCooldown = 0;
        _currentAmmo = 28;
        _holdWeapon = true;
        _maxAmmo = 28;
        InitializeUI();
    }

    public override void Fire(Vector2 direction)
    {
        if(!_isFiring && _reloadTime <= 0 && _currentAmmo > 0)
            StartCoroutine(ContinuousFire(direction));
    }

    private IEnumerator ContinuousFire(Vector2 direction)
    {
        _isFiring = true;

        while(_isFiring && _currentAmmo > 0 && _reloadTime <= 0)
        {
            if(shootCooldown <= 0)
            {
                GameObject bullet1 = Instantiate(bulletPrefab, transform.position, transform.rotation);
                bullet1.transform.Rotate(0, 0, Random.Range(-_bulletSpreadAngle, _bulletSpreadAngle));
                Rigidbody2D rb1 = bullet1.GetComponent<Rigidbody2D>();
                rb1.linearVelocity = bullet1.transform.right * bulletSpeed;
                bullet1.transform.Rotate(0, 0, bullet1.transform.rotation.z - 90f);

                GameObject bullet2 = Instantiate(bulletPrefab, transform.position, transform.rotation);
                bullet2.transform.Rotate(0, 0, Random.Range(-_bulletSpreadAngle, _bulletSpreadAngle));
                Rigidbody2D rb2 = bullet2.GetComponent<Rigidbody2D>();
                rb2.linearVelocity = bullet2.transform.right * bulletSpeed;
                bullet2.transform.Rotate(0, 0, bullet2.transform.rotation.z - 90f);


                shootCooldown = _fireRate;
                _currentAmmo--;
                _activeUI.UpdateAmmoDisplay(_currentAmmo, _maxAmmo);

                if(_currentAmmo <= 0)
                {
                    Reload();
                    break;
                }
            }
            else
                shootCooldown -= Time.deltaTime;

            yield return null;
        }

        _isFiring = false;
    }

    public void StopFiring()
    {
        _isFiring = false;
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
        if(shootCooldown > 0)
            shootCooldown -= Time.deltaTime;

        if(_reloadTime > 0)
            _reloadTime -= Time.deltaTime;
        else if(_reloadTime < 0)
        {
            _activeUI.UpdateAmmoDisplay(_currentAmmo, _maxAmmo);
            _reloadTime = 0;
        }
    }
}