using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoPlayerAnimations : MonoBehaviour
{
    public WeaponSystem weaponSystem;
    public PlayerMovement playerMovement;
    public Animator playerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        if (weaponSystem != null)
        {
            weaponSystem.WeaponShootingStarted += OnPlayerWeaponShootingStarted;
            weaponSystem.WeaponShootingFinished += OnPlayerWeaponShootingFinished;
        }
    }

    public void OnPlayerWeaponShootingStarted()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsShooting", true);
        }
    }

    public void OnPlayerWeaponShootingFinished()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsShooting", false);
        }
    }

    private void OnDestroy()
    {
        if (weaponSystem != null)
        {
            weaponSystem.WeaponShootingStarted -= OnPlayerWeaponShootingStarted;
            weaponSystem.WeaponShootingFinished -= OnPlayerWeaponShootingFinished;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement != null)
        {
            if (playerMovement.IsMoving)
            {
                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("IsMoving", true);
                }
            }
            else
            {
                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("IsMoving", false);
                }
            }
        }
    }
}
