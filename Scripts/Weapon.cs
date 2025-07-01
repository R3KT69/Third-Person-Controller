using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("Attributes")]
    public GameObject wep_light;
    public ParticleSystem wep_flash;
    public WeaponData wep_data;
    public Transform shoot_point;
    public float lightDuration = 0.1f; // Flash duration
    public bool twoHanded = false;

    private Coroutine lightRoutine;

    
    public void TriggerMuzzleEffects()
    {
        if (wep_flash != null)
        {
            wep_flash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            wep_flash.Play();
        }

        if (wep_light != null)
        {
            wep_light.SetActive(true);

            // Restart light toggle coroutine
            if (lightRoutine != null)
                StopCoroutine(lightRoutine);

            lightRoutine = StartCoroutine(AutoDisableLight());
        }
    }

    private IEnumerator AutoDisableLight()
    {
        yield return new WaitForSeconds(lightDuration);
        if (wep_light != null)
        {
            wep_light.SetActive(false);
        }
    }
}
