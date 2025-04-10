using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public int health = 100;

    private void Update()
    {
        if(health <= 0)
        {
            health = 0;
        }
    }

    public void Hurt(int damage)
    {

    }
}