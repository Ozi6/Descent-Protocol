using UnityEngine;

public abstract class WeaponUI : MonoBehaviour
{
    public abstract void UpdateAmmoDisplay(int current, int max);
    public abstract void Show();
    public abstract void Hide();
}