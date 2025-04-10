using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(MovementComponent))]
public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected HealthComponent _health;
    [SerializeField] protected MovementComponent _movement;

    protected virtual void Awake()
    {
        _health ??= GetComponent<HealthComponent>();
        _movement ??= GetComponent<MovementComponent>();
    }
}
