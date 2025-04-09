using UnityEngine;

public class Bullet : MonoBehaviour
{
    public void Start()
    {
        Destroy(gameObject, 5f);
    }
}
