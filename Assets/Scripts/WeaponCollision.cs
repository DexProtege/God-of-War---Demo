using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollision : MonoBehaviour
{
    public static WeaponCollision instance;

    [SerializeField] Transform SPoint;
    [SerializeField] Transform EPoint;
    [SerializeField] LayerMask HitLayers;
    [SerializeField] Vector2 ContactSpeedTime = new Vector2(.1f, .1f);

    [Header("Settings")]
    [SerializeField] int MaxHitCount = 3;
    [SerializeField] float MaxRecoveryTime = .1f;

    float RecoveryTime = .1f;
    [SerializeField]int HitCount = 3;
    string CurrentWallType;
    bool isWeaponContact;

    private void Awake()
    {
        instance = this;
    }

    public void ResetSettings()
    {
        RecoveryTime = 0;
        HitCount = MaxHitCount;
    }

    public void Activate()
    {
        if(RecoveryTime > 0)
        {
            RecoveryTime -= Time.deltaTime;
            return;
        }

        ShootRay();
    }

    private void ShootRay()
    {
        RaycastHit Hit;
        if(Physics.Linecast(SPoint.position, EPoint.position, out Hit, HitLayers))
        //if(Physics.SphereCast(SPoint.position, 1.0f, EPoint.position, out Hit, 2.0f, HitLayers))
        {
            CurrentWallType = LayerMask.LayerToName(Hit.transform.gameObject.layer);
            //Debug.Log(Hit.transform.name);
            switch(CurrentWallType)
            {
                case "Enemy":
                    HitFX(1, Hit);
                    break;

                case "Map":
                    HitFX(0, Hit);
                    break;
            }
        }
    }

    void HitFX(int ParticleType, RaycastHit Hit)
    {
        if(HitCount > 0)
        {
            WeaponContact();

            if (ParticleType == 1)
            {
                //Audio
                if (PlayerController.instance.isBladesEquipped)
                {
                    if(gameObject.name == "Blade1")
                        SoundController.instance.PlayWeaponFXSound(7, "blade1");
                    else if (gameObject.name == "Blade2")
                        SoundController.instance.PlayWeaponFXSound(7, "blade2");
                }
                else if (PlayerController.instance.isAxeEquipped)
                {
                    SoundController.instance.PlayWeaponFXSound(7, "axe");
                }

                //Blood
                FXHandler.instance.SplashBlood(Hit);

                //Animation
                MonsterController.instance.HitReact();

                //Particle
                if (PlayerController.instance.BladesHeavyAttack)
                    FXHandler.instance.PlayParticleSystem(ParticleType, Hit);
            }
            else if (ParticleType == 0)
            {
                //Audio
                if (PlayerController.instance.isBladesEquipped)
                {
                    if (gameObject.name == "Blade1")
                        SoundController.instance.PlayWeaponFXSound(8, "blade1");
                    else if (gameObject.name == "Blade2")
                        SoundController.instance.PlayWeaponFXSound(8, "blade2");
                }
                else if (PlayerController.instance.isAxeEquipped)
                {
                    SoundController.instance.PlayWeaponFXSound(8, "axe");
                }

                //Particle
                if(PlayerController.instance.BladesHeavyAttack)
                    FXHandler.instance.PlayParticleSystem(ParticleType + 1, Hit);
                else
                    FXHandler.instance.PlayParticleSystem(ParticleType, Hit);
            }

            HitCount--;
            RecoveryTime = MaxRecoveryTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(SPoint.position, EPoint.position);
    }

    public void WeaponContact()
    {
        if (gameObject.name == "Blade1" && PlayerController.instance.Blade2.GetComponent<WeaponCollision>().isWeaponContact ||
            gameObject.name == "Blade2" && PlayerController.instance.Blade1.GetComponent<WeaponCollision>().isWeaponContact)
            return;

        if(!isWeaponContact)
        {
            StartCoroutine(SpeedRegain());
        }
    }

    IEnumerator SpeedRegain()
    {
        isWeaponContact = true;
        float initSpeed = PlayerController.instance.ThirdPersonAnimator.speed;
        PlayerController.instance.ThirdPersonAnimator.speed = ContactSpeedTime.x;
        yield return new WaitForSeconds(ContactSpeedTime.y);
        PlayerController.instance.ThirdPersonAnimator.speed = initSpeed;
        isWeaponContact = false;
    }
}
