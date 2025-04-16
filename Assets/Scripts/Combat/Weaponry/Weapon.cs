using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] protected Sprite _weaponIcon;
    [SerializeField] protected WeaponUI _weaponUI;
    protected int _currentAmmo;
    protected int _maxAmmo;

    protected const float RELOAD_TIME = 1.5f;
    protected float _reloadTime = 0f;
    protected float shootCooldown = 0.5f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public const float shootCD = 0.5f;
    protected bool _holdWeapon = false;

    public virtual void Fire(Vector2 direction)
    {

    }

    public virtual void EnableUI()
    {
        if(_weaponUI != null)
        {
            _weaponUI.Show();
            UpdateUI();
        }
    }

    protected virtual void UpdateUI()
    {
        _weaponUI?.UpdateAmmoDisplay(_currentAmmo, _maxAmmo);
    }

    public virtual void DisableUI()
    {
        if(_weaponUI != null)
        {
            _weaponUI.Hide();
            UpdateUI();
        }
    }

    public virtual void Reload()
    {
        if (_currentAmmo <= 0)
        {
            _currentAmmo = _maxAmmo;
            _reloadTime = RELOAD_TIME;
        }
    }

    public bool getHoldWep()
        { return _holdWeapon; }

    public void setHoldWep(bool value)
        { _holdWeapon = value; }
}