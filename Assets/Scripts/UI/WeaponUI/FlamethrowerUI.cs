using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlamethrowerUI : WeaponUI
{
    [SerializeField] private Sprite tank;
    [SerializeField] private Color _activeColor = Color.white;
    [SerializeField] private Color _inactiveColor = new Color(1, 1, 1, 0.5f);

    private void Awake()
    {

    }

    public override void UpdateAmmoDisplay(int current, int max)
    {

    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }
}