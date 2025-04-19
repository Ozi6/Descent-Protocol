using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlamethrowerUI : WeaponUI
{
    [SerializeField] private Image tank; // The tank itself
    [SerializeField] private Sprite[] tankFrames; // Array of 14 images, each with a sprite for the animation frame

    private void Awake()
    {
        if(tank != null)
            tank.enabled = true;
    }

    public override void UpdateAmmoDisplay(int current, int max)
    {
        if(tankFrames.Length != 14 || current < 0 || current > max || max != 28)
        {
            Debug.LogWarning("Invalid tank array length or ammo values.");
            return;
        }

        int ammoStep = current / 2;
        int maxSteps = max / 2;
        int frameIndex = 13 - (ammoStep * 13 / maxSteps);

        tank.sprite = tankFrames[frameIndex];
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