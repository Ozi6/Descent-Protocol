using System.Collections;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private Transform _weaponAnchor;
    [SerializeField] private float _aimSpeed = 10f;

    private Weapon _currentWeapon;

    public Weapon CurrentWeapon => _currentWeapon;

    private bool _isFiringContinuously;
    private Vector2 _currentFireDirection;

    public void StartFiring(Vector2 direction)
    {
        if(_currentWeapon.getHoldWep())
        {
            _isFiringContinuously = true;
            _currentFireDirection = direction;
            StartCoroutine(ContinuousFireRoutine());
        }
        else
            Fire(direction);
    }

    public void StopFiring()
    {
        _isFiringContinuously = false;
        _currentWeapon.setIsFiring(false);
    }

    private IEnumerator ContinuousFireRoutine()
    {
        while(_isFiringContinuously)
        {
            if(_currentWeapon != null)
                _currentWeapon.Fire(_currentFireDirection);
            yield return null;
        }
    }

    public void AttachWeapon(Weapon weapon)
    {
        if(_currentWeapon != null)
            _currentWeapon.DisableUI();

        _currentWeapon = weapon;
        weapon.transform.SetParent(_weaponAnchor);
        weapon.transform.localPosition = new Vector3(0, -0.15f, 0);

        weapon.EnableUI();
    }

    public void AimAt(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _weaponAnchor.rotation = Quaternion.Euler(0, 0, angle);

        bool shouldFlip = Mathf.Abs(angle) > 90;

        if(_currentWeapon != null)
        {
            _currentWeapon.transform.localScale = new Vector3(shouldFlip ? 1 : -1, shouldFlip ? -1 : 1, 1);
            _currentWeapon.transform.localPosition = new Vector3(0, shouldFlip ? 0.15f : -0.15f, 0);
        }
    }

    public void Fire(Vector2 direction)
    {
        if(_currentWeapon != null)
            _currentWeapon.Fire(direction);
    }
}