using System;
using UnityEngine;

public class InputHandler : MovementComponent
{
    [Header("Gesture Settings")]
    [SerializeField] private float _swipeThreshold = 20f;
    [SerializeField] private float _holdTimeThreshold = 0.3f; // Time in seconds before considering a hold
    [SerializeField] private float _maxSwipeTime = 0.5f; // Max time for a swipe to be considered valid

    [Header("Screen Regions")]
    [SerializeField] private bool _useScreenRegions = true;
    [SerializeField][Range(0, 1)] private float _firingRegionHeight = 0.3f; // Bottom % of screen for firing

    private Vector2 _startTouchPosition;
    private Vector2 _currentTouchPosition;
    private float _touchStartTime;
    private bool _isTouching;
    private bool _holdRegistered;
    private GestureState _currentGesture = GestureState.None;

    private Vector2 _screenDimensions;

    public event Action<Vector2> OnSwipe;
    public event Action<Vector2> OnTap;
    public event Action<Vector2> OnHoldStart;
    public event Action<Vector2> OnHold;
    public event Action OnHoldEnd;

    private enum GestureState
    {
        None,
        Possible,
        Holding,
        Swiping
    }

    private void Start()
    {
        _screenDimensions = new Vector2(Screen.width, Screen.height);
    }

    private void Update()
    {
        if(Application.isMobilePlatform)
            HandleTouchInput();
        else
            HandleMouseInput();
    }

    private bool IsInFiringRegion(Vector2 position)
    {
        if(!_useScreenRegions)
            return true;
        return position.y < _screenDimensions.y * _firingRegionHeight;
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartTouch(touch.position);
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    ContinueTouch(touch.position);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    EndTouch(touch.position);
                    break;
            }
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
            StartTouch(Input.mousePosition);
        else if (Input.GetMouseButton(0))
            ContinueTouch(Input.mousePosition);
        else if (Input.GetMouseButtonUp(0))
            EndTouch(Input.mousePosition);
    }

    private void StartTouch(Vector2 position)
    {
        _startTouchPosition = position;
        _currentTouchPosition = position;
        _touchStartTime = Time.time;
        _isTouching = true;
        _holdRegistered = false;
        _currentGesture = GestureState.Possible;
    }

    private void ContinueTouch(Vector2 position)
    {
        if(!_isTouching)
            return;

        _currentTouchPosition = position;
        float touchDuration = Time.time - _touchStartTime;
        float distance = Vector2.Distance(position, _startTouchPosition);

        switch(_currentGesture)
        {
            case GestureState.Possible:
                if(distance > _swipeThreshold && touchDuration < _maxSwipeTime)
                {
                    _currentGesture = GestureState.Swiping;
                    if(!IsInFiringRegion(_startTouchPosition))
                        OnSwipe?.Invoke(position - _startTouchPosition);
                }
                else if(touchDuration > _holdTimeThreshold && distance < _swipeThreshold)
                {
                    _currentGesture = GestureState.Holding;
                    _holdRegistered = true;
                    if(IsInFiringRegion(_startTouchPosition))
                        OnHoldStart?.Invoke(position);
                }
                break;

            case GestureState.Holding:
                if(_holdRegistered && IsInFiringRegion(_startTouchPosition))
                    OnHold?.Invoke(position);
                break;

            case GestureState.Swiping:

                break;
        }
    }

    private void EndTouch(Vector2 position)
    {
        if(!_isTouching)
            return;

        float touchDuration = Time.time - _touchStartTime;
        float distance = Vector2.Distance(position, _startTouchPosition);

        switch(_currentGesture)
        {
            case GestureState.Possible:
                if(touchDuration < _holdTimeThreshold && distance < _swipeThreshold)
                    OnTap?.Invoke(position);
                else if (distance > _swipeThreshold && touchDuration < _maxSwipeTime)
                {
                    if(!IsInFiringRegion(_startTouchPosition))
                        OnSwipe?.Invoke(position - _startTouchPosition);
                }
                break;

            case GestureState.Holding:
                if(_holdRegistered && IsInFiringRegion(_startTouchPosition))
                    OnHoldEnd?.Invoke();
                break;

            case GestureState.Swiping:

                break;
        }

        _isTouching = false;
        _holdRegistered = false;
        _currentGesture = GestureState.None;
    }
}