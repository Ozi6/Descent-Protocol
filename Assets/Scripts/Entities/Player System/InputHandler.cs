using System;
using UnityEngine;

public class InputHandler : MovementComponent
{
    [SerializeField] private float _swipeThreshold = 20f;

    private Vector2 _startTouchPosition;
    private bool _isSwiping;

    public event Action<Vector2> OnSwipe;
    public event Action<Vector2> OnTap;

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
                    break;

                case TouchPhase.Moved when _isSwiping:
                    if(Vector2.Distance(touch.position, _startTouchPosition) > _swipeThreshold)
                    {
                        OnSwipe?.Invoke(touch.position - _startTouchPosition);
                        _isSwiping = false;
                    }
                    break;

                case TouchPhase.Ended when _isSwiping:
                    Vector2 endPos = touch.position;
                    if (Vector2.Distance(endPos, _startTouchPosition) < _swipeThreshold)
                        OnTap?.Invoke(endPos);
                    else
                        OnSwipe?.Invoke(endPos - _startTouchPosition);
                    _isSwiping = false;
                    break;
            }
        }
    }

    private void HandleMouseInput()
    {
        
    }
}