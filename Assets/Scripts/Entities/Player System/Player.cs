using UnityEngine;

public class Player : Entity
{
    private InputHandler _input;
    private WeaponHolder _weaponHolder;
    private Animator _animator;

    protected override void Awake()
    {
        base.Awake();

        _input = GetComponent<InputHandler>();
        if(_input == null)
            _input = gameObject.AddComponent<InputHandler>();

        _weaponHolder = GetComponent<WeaponHolder>();
        if(_weaponHolder == null)
            _weaponHolder = gameObject.AddComponent<WeaponHolder>();

        _animator = GetComponent<Animator>();

        Weapon startingWeapon = GetComponentInChildren<Weapon>();
        if(startingWeapon != null)
            _weaponHolder.AttachWeapon(startingWeapon);

        _input.OnSwipe += HandleSwipe;
        _input.OnTap += HandleTap;
        _input.OnHoldStart += HandleHoldStart;
        _input.OnHold += HandleHold;
        _input.OnHoldEnd += HandleHoldEnd;
    }

    private void HandleHoldStart(Vector2 screenPosition)
    {
        if(_weaponHolder.CurrentWeapon.getHoldWep())
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
            Vector2 aimDirection = (worldPos - (Vector2)transform.position).normalized;

            transform.localScale = new Vector3(Mathf.Abs(Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg) > 90 ? 1 : -1, 1, 1);

            _weaponHolder.AimAt(aimDirection);
            _weaponHolder.StartFiring(aimDirection);
        }
    }

    private void HandleHold(Vector2 screenPosition)
    {
        if(_weaponHolder.CurrentWeapon.getHoldWep())
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
            Vector2 aimDirection = (worldPos - (Vector2)transform.position).normalized;

            _weaponHolder.AimAt(aimDirection);
        }
    }

    private void HandleHoldEnd()
    {
        if(_weaponHolder.CurrentWeapon.getHoldWep())
            _weaponHolder.StopFiring();
    }

    private void Update()
    {
        if(_animator != null)
            _animator.SetFloat("Speed", _movement.IsMoving ? 1 : 0);
    }

    private void HandleSwipe(Vector2 direction)
    {
        if(!_movement.IsMoving)
        {
            Vector2 moveDirection = GetPrimaryDirection(direction);
            _movement.MoveInDirection(moveDirection);
            if(moveDirection.x != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (moveDirection.x > 0 ? -1 : 1);
                transform.localScale = scale;
            }
        }
    }

    private void HandleTap(Vector2 screenPosition)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector2 aimDirection = (worldPos - (Vector2)transform.position).normalized;

        transform.localScale = new Vector3(Mathf.Abs(Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg) > 90 ? 1 : -1, 1, 1);

        _weaponHolder.AimAt(aimDirection);
        _weaponHolder.Fire(aimDirection);
        _weaponHolder.StopFiring();
    }

    private Vector2 GetPrimaryDirection(Vector2 rawDirection)
    {
        if(Mathf.Abs(rawDirection.x) > Mathf.Abs(rawDirection.y))
            return rawDirection.x > 0 ? Vector2.right : Vector2.left;
        return rawDirection.y > 0 ? Vector2.up : Vector2.down;
    }

    private void OnDestroy()
    {
        if (_input != null)
        {
            _input.OnSwipe -= HandleSwipe;
            _input.OnTap -= HandleTap;
            _input.OnHoldStart -= HandleHoldStart;
            _input.OnHold -= HandleHold;
            _input.OnHoldEnd -= HandleHoldEnd;
        }
    }
}