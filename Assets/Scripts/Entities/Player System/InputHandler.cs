using System;
using UnityEngine;

public class InputHandler : MovementComponent
{
    [SerializeField] private float _swipeThreshold = 20f;

    private Vector2 _startTouchPosition;
    private bool _isSwiping;
    private bool _isHolding;
    private Vector2 _currentHoldPosition;

    public event Action<Vector2> OnSwipe;
    public event Action<Vector2> OnTap;
    public event Action<Vector2> OnHoldStart;
    public event Action<Vector2> OnHold;
    public event Action OnHoldEnd;

    private void Update()
    {
        HandleTouchInput();
        HandleMouseInput();
    }

    private void HandleTouchInput()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch(touch.phase)
            {
                case TouchPhase.Began:
                    _startTouchPosition = touch.position;
                    _isSwiping = true;
                    _isHolding = true;
                    _currentHoldPosition = touch.position;
                    OnHoldStart?.Invoke(touch.position);
                    break;

                case TouchPhase.Moved when _isSwiping:
                    if(Vector2.Distance(touch.position, _startTouchPosition) > _swipeThreshold)
                    {
                        OnSwipe?.Invoke(touch.position - _startTouchPosition);
                        _isSwiping = false;
                    }
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    _currentHoldPosition = touch.position;
                    OnHold?.Invoke(touch.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if(_isHolding)
                    {
                        OnHoldEnd?.Invoke();
                        _isHolding = false;
                    }

                    if(_isSwiping)
                    {
                        Vector2 endPos = touch.position;
                        if (Vector2.Distance(endPos, _startTouchPosition) < _swipeThreshold)
                            OnTap?.Invoke(endPos);
                        else
                            OnSwipe?.Invoke(endPos - _startTouchPosition);
                        _isSwiping = false;
                    }
                    break;
            }
        }
        else if(_isHolding)
        {
            OnHoldEnd?.Invoke();
            _isHolding = false;
        }
    }

    private void HandleMouseInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _startTouchPosition = Input.mousePosition;
            _isSwiping = true;
            _isHolding = true;
            _currentHoldPosition = Input.mousePosition;
            OnHoldStart?.Invoke(Input.mousePosition);
        }
        else if(Input.GetMouseButton(0))
        {
            _currentHoldPosition = Input.mousePosition;
            OnHold?.Invoke(Input.mousePosition);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if(_isHolding)
            {
                OnHoldEnd?.Invoke();
                _isHolding = false;
            }

            if(_isSwiping)
            {
                Vector2 endPos = Input.mousePosition;
                if(Vector2.Distance(endPos, _startTouchPosition) < _swipeThreshold)
                    OnTap?.Invoke(endPos);
                else
                    OnSwipe?.Invoke(endPos - _startTouchPosition);
                _isSwiping = false;
            }
        }
    }
}