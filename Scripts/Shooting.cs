using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

public class Shooting : MonoBehaviour
{
    private Camera playerCamera;
    [Header("Aiming")]
    private Animator animator;
    public RotationConstraint rotationConstraint;
    public float smoothSpeedRotConst = 5f; 
    public float targetWeightRotConst = 0f;
    public float currentLayerWeight = 0f;
    private float targetLayerWeight = 1f;
    private bool isRecoiling = false;
    private float recoilReturnDuration = 0.2f;

    [Header("Inventory")]
    public List<GameObject> allWeapons;
    private int invIndex;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        invIndex = 0;
        UpdateWeaponActivation();
    }

    void Update()
    {
        HandleAimToggle();
        SmoothWeightTransition();
        InventoryIndexer();
        
        
    }

    void InventoryIndexer()
    {
        if (animator.GetBool("isAiming")) return;
        
        int previousIndex = invIndex;

        if (Input.GetKeyDown(KeyCode.E))
        {
            invIndex = (invIndex + 1) % allWeapons.Count;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            invIndex = (invIndex - 1 + allWeapons.Count) % allWeapons.Count;
        }

        // Only update if index changed
        if (invIndex != previousIndex)
        {
            UpdateWeaponActivation();
        }
    }

    void UpdateWeaponActivation()
    {
        for (int i = 0; i < allWeapons.Count; i++)
        {
            allWeapons[i].SetActive(i == invIndex);
        }
    }


    void Shoot()
    {
        if (isRecoiling) return;

        Debug.Log("Shoot");
        allWeapons[invIndex].gameObject.GetComponent<Weapon>().TriggerMuzzleEffects();
        StartCoroutine(RecoilRoutine());

        // hint: allWeapons[invIndex].gameObject = the weapon itself
    }

    void  HandleAimToggle()
    {
        bool isAiming = animator.GetBool("isAiming");
        animator.SetBool("isTwoHanded", allWeapons[invIndex].gameObject.GetComponent<Weapon>().twoHanded);

        if (isAiming)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();

            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (!isAiming)
            {
                targetWeightRotConst = 1f;
                targetLayerWeight = 0f; 
                rotationConstraint.rotationOffset = new Vector3(0, 45, 0);
                animator.SetBool("isAiming", true);
                animator.SetTrigger("Aiming");
            }
            else
            {
                targetWeightRotConst = 0f;
                targetLayerWeight = 1f; 
                animator.SetBool("isAiming", false);
            }
        }
    }

    void SmoothWeightTransition()
    {
        rotationConstraint.weight = Mathf.Lerp(rotationConstraint.weight, targetWeightRotConst, Time.deltaTime * smoothSpeedRotConst);

        if (animator.GetBool("isTwoHanded"))
        {
            currentLayerWeight = Mathf.Lerp(currentLayerWeight, targetLayerWeight, Time.deltaTime * smoothSpeedRotConst * 2);
        }
        else
        {
            currentLayerWeight = Mathf.Lerp(currentLayerWeight, 0f, Time.deltaTime * smoothSpeedRotConst * 2);
        }

        animator.SetLayerWeight(3, currentLayerWeight);
    }

    IEnumerator RecoilRoutine()
    {
        isRecoiling = true;
        rotationConstraint.rotationOffset = new Vector3(-7.5f, 45f, -7.5f); // Recoil offset, in X and Z values.

        yield return new WaitForSeconds(0.01f); // kicked back state duration

        float elapsed = 0f;
        Vector3 startOffset = rotationConstraint.rotationOffset;
        Vector3 targetOffset = new Vector3(0f, 45f, 0f);

        while (elapsed < recoilReturnDuration)
        {
            rotationConstraint.rotationOffset = Vector3.Lerp(startOffset, targetOffset, elapsed / recoilReturnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rotationConstraint.rotationOffset = targetOffset;
        isRecoiling = false;
    }

}
