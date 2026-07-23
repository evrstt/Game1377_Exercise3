using System.Collections;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    private const float defaultSpeedMultiplier = 1f;
    private const float defaultBulletSizeMultiplier = 1f;
    private float speedMultiplier = defaultSpeedMultiplier;
    private float bulletSizeMultiplier = defaultBulletSizeMultiplier;

    private Coroutine speedBoostCoroutine;
    private Coroutine bulletSizeCoroutine;

    public float SpeedMultiplier
    {
        get
        {
            return speedMultiplier;
        }
    }

    public float BulletSizeMultiplier
    {
        get
        {
            return bulletSizeMultiplier;
        }
    }

    public void ActivateSpeedBoost(float newSpeedMultiplier, float boostDuration)
    {
        if(newSpeedMultiplier <= defaultSpeedMultiplier || boostDuration <= 0f)
        {
            return;
        }

        if(speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }

        speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine(newSpeedMultiplier, boostDuration));
    }

    private IEnumerator SpeedBoostRoutine(float newSpeedMultiplier, float boostDuration)
    {
        speedMultiplier = newSpeedMultiplier;

        yield return new WaitForSeconds(boostDuration);
        speedMultiplier = defaultSpeedMultiplier;
        speedBoostCoroutine = null;
    }

    public void ActivateBulletSizeBoost(float newBulletSizeMultiplier, float boostDuration)
    {
        if(newBulletSizeMultiplier <= defaultBulletSizeMultiplier || boostDuration <= 0f)
        {
            return;
        }

        if(bulletSizeCoroutine != null)
        {
            StopCoroutine(bulletSizeCoroutine);
        }

        bulletSizeCoroutine = StartCoroutine(BulletSizeBoostRoutine(newBulletSizeMultiplier, boostDuration));
    }

    private IEnumerator BulletSizeBoostRoutine(float newBulletSizeMultiplier, float boostDuration)
    {
        bulletSizeMultiplier = newBulletSizeMultiplier;

        yield return new WaitForSeconds(boostDuration);
        bulletSizeMultiplier = defaultBulletSizeMultiplier;
        bulletSizeCoroutine = null;
    }

    private void ResetPowerUps()
    {
        if(speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
            speedBoostCoroutine = null;
        }

        if(bulletSizeCoroutine != null)
        {
            StopCoroutine(bulletSizeCoroutine);
            bulletSizeCoroutine = null;
        }

        speedMultiplier = defaultSpeedMultiplier;
        bulletSizeMultiplier = defaultSpeedMultiplier;
    }


}
