using UnityEngine;
using UnityEngine.UI;

public class CircularBarrelUI : WeaponUI
{
    [SerializeField] private Image[] _shellIcons;
    [SerializeField] private Sprite _fullShell;
    [SerializeField] private Sprite _emptyShell;
    [SerializeField] private Color _activeColor = Color.white;
    [SerializeField] private Color _inactiveColor = new Color(1, 1, 1, 0.5f);

    private void Awake()
    {
        for(int i = 0; i < _shellIcons.Length; i++)
            _shellIcons[i].gameObject.SetActive(true);
    }

    public override void UpdateAmmoDisplay(int current, int max)
    {
        for(int i = 0; i < _shellIcons.Length; i++)
        {
            bool hasShell = i < current;
            _shellIcons[i].sprite = hasShell ? _fullShell : _emptyShell;
            _shellIcons[i].color = i < max ? _activeColor : _inactiveColor;
        }
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