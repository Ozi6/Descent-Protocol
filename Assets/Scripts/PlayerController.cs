using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float tileSize = 16f;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private bool isSwiping = false;
    public GameObject gun;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float swipeThreshold = 20f;
    private Vector3 gunLocalPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        targetPosition = transform.position;
        if(gun != null)
        {
            gunLocalPosition = gun.transform.position - transform.position;
            gun.transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    void Update()
    {
        HandleTouchInput();
        MoveToTarget();
        if(gun != null && !gun.transform.IsChildOf(transform))
        {
            Vector3 gunOffset = new Vector3(gunLocalPosition.x * transform.localScale.x, gunLocalPosition.y, gunLocalPosition.z);
            gun.transform.position = transform.position + gunOffset;
        }
    }

    private void HandleTouchInput()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                float currentSwipeDistance = Vector2.Distance(touch.position, startTouchPosition);
                if (currentSwipeDistance > swipeThreshold)
                {
                    Vector2 swipeDirection = touch.position - startTouchPosition;
                    HandleSwipe(swipeDirection);
                    isSwiping = false;
                }
            }
            else if(touch.phase == TouchPhase.Ended && isSwiping)
            {
                endTouchPosition = touch.position;
                float swipeDistance = Vector2.Distance(endTouchPosition, startTouchPosition);

                if (swipeDistance < swipeThreshold)
                {
                    HandleTap(touch.position);
                }
                else
                {
                    Vector2 swipeDirection = endTouchPosition - startTouchPosition;
                    HandleSwipe(swipeDirection);
                }
                isSwiping = false;
            }
        }

        if(Input.GetMouseButtonDown(0))
            startTouchPosition = Input.mousePosition;
        else if(Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            float clickDistance = Vector2.Distance(endTouchPosition, startTouchPosition);
            if(clickDistance < swipeThreshold)
                HandleTap(Input.mousePosition);
            else
                HandleSwipe(endTouchPosition - startTouchPosition);
        }
    }

    private void HandleSwipe(Vector2 swipeDirection)
    {
        if(!isMoving)
        {
            if(Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
            {
                if(swipeDirection.x > 0)
                    MoveInDirection(Vector2.right);
                else
                    MoveInDirection(Vector2.left);
            }
            else
            {
                if(swipeDirection.y > 0)
                    MoveInDirection(Vector2.up);
                else
                    MoveInDirection(Vector2.down);
            }
        }
    }

    private void HandleTap(Vector2 tapPosition)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(tapPosition);
        worldPos.z = 0;
        Vector2 direction = (worldPos - transform.position).normalized;
        AimGunAt(direction);
        Gun gunScript = gun.GetComponent<Gun>();
        if(gunScript != null)
            gunScript.Shoot(direction);
        else
            Debug.LogError("Gun script not found on the gunObject!");
    }

    private void AimGunAt(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bool shouldFlip = Mathf.Abs(angle) > 90;
        transform.localScale = new Vector3(shouldFlip ? 1 : -1, 1, 1);
        gun.transform.rotation = Quaternion.Euler(0, 0, angle);
        if(shouldFlip)
            gun.transform.localScale = new Vector3(1, -1, 1);
        else
            gun.transform.localScale = new Vector3(1, 1, 1);
    }

    private void MoveInDirection(Vector2 direction)
    {
        targetPosition = transform.position + new Vector3(direction.x, direction.y, 0) * (tileSize / 10f);
        animator.SetFloat("Speed", 1);
        isMoving = true;
        if(direction.x != 0)
        {
            transform.localScale = new Vector3(direction.x > 0 ? -1 : 1, 1, 1);
            UpdateGunOrientation();
        }
    }

    private void MoveToTarget()
    {
        if(isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5f);

            if(Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                rb.linearVelocity = Vector2.zero;
                animator.SetFloat("Speed", 0);
                isMoving = false;
            }
        }
    }

    private void UpdateGunOrientation()
    {
        Vector3 localPos = gun.transform.localPosition;
        gun.transform.localPosition = new Vector3(Mathf.Abs(localPos.x) * transform.localScale.x, localPos.y, localPos.z);
        gun.transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
