using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] protected Sprite _weaponIcon;
    [SerializeField] protected WeaponUI _weaponUI;
    protected int _currentAmmo;
    protected int _maxAmmo;

    protected float shootCooldown = 0.5f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public const float shootCD = 0.5f;

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
}