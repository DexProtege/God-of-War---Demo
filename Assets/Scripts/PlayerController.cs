using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;
using TMPro;
using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    CinemachineVirtualCamera cinemachineVirtualCamera;
    Cinemachine3rdPersonFollow thirdPersonFollow;

    public TextMeshProUGUI DebugText;

    [Header("Input")]
    public string ControlType;
    [SerializeField] InputActionAsset inputActionAsset;

    [Header("Gameobjects")]
    [SerializeField] GameObject PlayerCamera;
    [SerializeField] GameObject PlayerCameraRoot;
    [SerializeField] GameObject CurvePoint;

    [Header("Weapons")]
    [SerializeField] GameObject AxeLoc;
    public GameObject Axe;
    [SerializeField] GameObject AxeDisarmed;
    [SerializeField] GameObject BladeDisarmed;
    public GameObject Blade1Loc;
    public GameObject Blade2Loc;
    public GameObject Blade1;
    public GameObject Blade2;

    [Header("Animators")]
    public Animator CameraAnimator;
    public Animator LockCameraAnimator;
    public Animator ThirdPersonAnimator;
    public Animator ShieldAnimator;

    [Header("Animators")]
    [SerializeField] AnimationClip[] NormalClips;
    [SerializeField] AnimationClip[] CombatClips;

    [Header("Player Status")]
    public bool isAnim = false;
    public bool isAxeEquipped = true;
    public bool isBladesEquipped = true;
    public bool isBlocked = false;
    public bool isAiming = false;
    public bool isAimingButtonUp = false;
    public bool isAxeThrown = false;
    public bool isBladeThrown = false;
    public bool isPulling = false;
    public bool CombatMode = false;
    public bool isDamage = false;
    public bool isSlowMo = false;
    public bool isIKActive = true;
    public bool isFirstDodge = false;
    public bool isSecondDodge = false;
    public bool isEnemyLockOn = false;

    [Header("Attack Status")]
    public bool AxeAttack1 = false;
    public bool AxeAttack2 = false;
    public bool AxeHeavyAttack = false;
    public bool BladesAttack1 = false;
    public bool BladesAttack2 = false;
    public bool BladesHeavyAttack = false;
    public bool Rage = false;

    [Header("Axe Controls")]
    public bool isCollided = false;
    public float throwPower = 1.0f;
    public float rotationSpeed = 1.0f;
    private float returnTime;
    private Vector3 pullPos;
    private Rigidbody AxeRb;
    private Rigidbody Blade1Rb;
    private Rigidbody Blade2Rb;    

    private Ray ReticleDir;
    private float lookWeight;
    public bool isLeftStickPressed = false;
    public bool isRightStickPressed = false;

    private void Awake()
    {
        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;

        instance = this;

        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();

        AxeRb = Axe.GetComponent<Rigidbody>();
        Blade1Rb = Blade1.GetComponent<Rigidbody>();
        Blade2Rb = Blade2.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        cinemachineVirtualCamera = PlayerCamera.GetComponent<CinemachineVirtualCamera>();
        thirdPersonFollow = cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
    }

    void Update()
    {
        ReticleDir = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ReticleDir.origin, ReticleDir.direction * 100, Color.yellow);
        CheckForEnemy(ReticleDir);

        SetCameraPanAnims();
        Restrictions();
        CheckAxe();
        WeaponContact();
        SlowMotion();
        CheckLookAtWeight();
        ChangeCombatLayerWeight();
    }

    void CheckForEnemy(Ray ray)
    {
        UIManager.instance.ChangeReticleColor(Physics.SphereCast(ray, .1f, 100, EnemyLockOn.instance.targetLayers));
    }

    public void CheckRage()
    {
        float speed = 1.0f;
        if(Rage)
        {
            if(BladesAttack1 || BladesAttack2 || BladesHeavyAttack || 
                AxeAttack1 || AxeAttack2 || AxeHeavyAttack)
            {
                speed = 1.5f;                
            }
            else
            {
                speed = 1.0f;
            }
        }
        else
        {
            speed = 1.0f;
        }

        ThirdPersonAnimator.speed = speed;
        CameraAnimator.speed = speed;
        LockCameraAnimator.speed = speed;
        Blade1Loc.GetComponent<Animator>().speed = speed;
        Blade2Loc.GetComponent<Animator>().speed = speed;
    }

    void CheckLookAtWeight()
    {
        if (IsIK())
        {
            lookWeight = Mathf.Lerp(lookWeight, 0, Time.deltaTime * 2.5f);
        }
        else
        {
            lookWeight = Mathf.Lerp(lookWeight, 1, Time.deltaTime * 2.5f);
        }
    }

    void OnAnimatorIK()
    {
        if (isIKActive)
        {
            ThirdPersonAnimator.SetLookAtWeight(lookWeight);            
            ThirdPersonAnimator.SetLookAtPosition(ReticleDir.GetPoint(10));
        }
    }

    public void SetCameraPanAnims()
    {
        bool isWalking = false;

        if (StarterAssets.StarterAssetsInputs.instance.move != Vector2.zero)
        {
            if (StarterAssets.StarterAssetsInputs.instance.sprint && StarterAssets.ThirdPersonController.instance._speed > 0)
                isWalking = false;
            else
                isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        CameraAnimator.SetBool("Sprinting", StarterAssets.StarterAssetsInputs.instance.sprint && StarterAssets.ThirdPersonController.instance._speed > 0);
        LockCameraAnimator.SetBool("Sprinting", StarterAssets.StarterAssetsInputs.instance.sprint && StarterAssets.ThirdPersonController.instance._speed > 0);
        CameraAnimator.SetBool("Walking", isWalking);
        LockCameraAnimator.SetBool("Walking", isWalking);
    }

    public void SlowMotion()
    {
        if (isSlowMo)
        {
            Time.timeScale = 0.2f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void ToggleSlowMo(int status)
    {
        isSlowMo = status == 1;
    }

    public void WeaponContact()
    {
        if (isDamage)
        {
            if (isBladesEquipped && !isBladeThrown)
            {
                Blade1.GetComponent<WeaponCollision>().Activate();
                Blade2.GetComponent<WeaponCollision>().Activate();
            }
            else if (isAxeEquipped && !isAxeThrown)
            {
                Axe.GetComponent<WeaponCollision>().Activate();
            }
        }
    }

    public void WeaponContactResetSettings()
    {
        if (isBladesEquipped && !isBladeThrown)
        {
            Blade1.GetComponent<WeaponCollision>().ResetSettings();
            Blade2.GetComponent<WeaponCollision>().ResetSettings();
        }
        else if (isAxeEquipped && !isAxeThrown)
        {
            Axe.GetComponent<WeaponCollision>().ResetSettings();
        }
    }

    public void DamageStatus(int status)
    {
        isDamage = status == 1;
    }

    public void OnControlsChanged()
    {
        ControlType = PlayerInput.all[0].currentControlScheme;
    }

    public void Restrictions()
    {
        //isIKActive = !IsPrimaryAttackAnimating() && !IsSecondaryAttackAnimating() && !IsHeavyAttackAnimating() &&
                         //StarterAssets.ThirdPersonController.instance.Grounded && !ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Rage");

        StarterAssets.ThirdPersonController.instance.MoveSpeed = isAiming || isBlocked ? .75f : 3.0f;

        isAnim = ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Axe Attack 1") || 
                    ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Axe Attack 2") ||
                    ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Axe Heavy Attack") || 
                    ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Blades Attack 1") ||
                    ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Blades Attack 2") || 
                    ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Blades Heavy Attack") ||
                    ThirdPersonAnimator.GetCurrentAnimatorStateInfo(3).IsName("Throw Blades") ||
                    ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Rage") || 
                    ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("JumpStart") && ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("JumpLand");
;

        if (StarterAssets.ThirdPersonController.instance.Grounded)
        {
            //////
            /// ROTATE TOWARDS CAM LOOK
            //////
            if (isAiming || isBlocked || BladesAttack1 || BladesAttack2 || BladesHeavyAttack ||
                AxeAttack1 || AxeAttack2 || AxeHeavyAttack)
            {
                StarterAssets.ThirdPersonController.instance.Move(true);
            }
        }
    }

    public void ChangeCombatLayerWeight()
    {
        bool status = !isAiming && !BladesAttack1 && !BladesAttack2 && !BladesHeavyAttack && !AxeAttack1 && !AxeAttack2 && !AxeHeavyAttack &&
                         StarterAssets.ThirdPersonController.instance.Grounded && StarterAssets.ThirdPersonController.instance._speed == 0;

        float prevWeight = ThirdPersonAnimator.GetLayerWeight(1);
        ThirdPersonAnimator.SetLayerWeight(1, status ? Mathf.Lerp(prevWeight, 0.3f, Time.deltaTime * 3f) : Mathf.Lerp(prevWeight, 0, Time.deltaTime * 3f));
    }

    //////
    /// AXE|THROW|CATCH|TRAVEL...
    //////
    public void CheckAxe()
    {
        if (isAxeThrown && !isCollided && !isBladesEquipped || isPulling && !isBladesEquipped)
        {
            //Axe.GetComponent<TrailRenderer>().enabled = true;
            float axeBend = isPulling ? (1 - returnTime) * 100 : -50;

            Axe.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + axeBend, transform.localEulerAngles.y - 85f, transform.localEulerAngles.z);

            if (returnTime < .5)
                Axe.transform.localEulerAngles += Vector3.forward * 1000 * (isPulling ? rotationSpeed / 2 : rotationSpeed) * Time.deltaTime;
        }
        else
        {
            //Axe.GetComponent<TrailRenderer>().enabled = false;
        }

        if (isPulling && !isBladesEquipped)
        {
            if (returnTime < 1)
            {
                Axe.transform.position = GetQuadraticCurvePoint(returnTime, pullPos, CurvePoint.transform.position, AxeLoc.transform.position);
                returnTime += Time.deltaTime * 1.5f;

                if (returnTime < 0.1)
                    SoundController.instance.PlayAxeSound(2);

                if (returnTime > .8)
                {
                    ThirdPersonAnimator.SetBool("Pulling", false);
                    ThirdPersonAnimator.SetBool("Caught", true);
                }
            }
            else
            {
                StartCoroutine(CatchAxe());
            }
        }
    }

    public void ThrowAxe()
    {
        if (isAxeEquipped)
        {
            isAxeEquipped = false;
            SoundController.instance.PlayGruntSound();
            UIManager.instance.SetReticleType(0, true);
            SoundController.instance.PlayAxeSound(3);

            AxeRb.isKinematic = false;
            AxeRb.transform.parent = null;
            AxeRb.AddForce(ReticleDir.direction * throwPower, ForceMode.Impulse);

            isAxeThrown = true;
            //isAiming = false;
        }
    }


    public void ReturnAxe()
    {
        isCollided = false;

        pullPos = Axe.transform.position;

        AxeRb.isKinematic = true;
        
        isPulling = true;
    }

    IEnumerator CatchAxe()
    {
        isPulling = false;

        returnTime = 0;

        AxeRb.transform.parent = AxeLoc.transform;
        Axe.transform.localEulerAngles = Vector3.zero;
        Axe.transform.localPosition = Vector3.zero;

        isAxeThrown = false;
        isAxeEquipped = true;

        yield return new WaitForSeconds(.3f);
        ThirdPersonAnimator.SetBool("Caught", false);

        if (isAiming)
        {
            UIManager.instance.SetReticleType(1, true);
        }
        else
        {
            //yield return new WaitForSeconds(.5f);
            FXHandler.instance.ToggleFrostAxeParticle(0);
        }
    }

    public Vector3 GetQuadraticCurvePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return (uu * p0) + (2 * u * t * p1) + (tt * p2);
    }

    public void OnAxe()
    {
        //////
        /// EQUIP/UNEQUIP AXE
        //////
        if (!StarterAssets.ThirdPersonController.instance.Grounded) return;
        if (!isAnim && !isBlocked && !isAxeThrown && !IsEquipping())
        {
            if (isAiming)
            {
                BladesAim(false);
            }

            if (isBladesEquipped)
            {
                StopCoroutine(DisarmBlades(false));
                StartCoroutine(DisarmBlades(true));

                isBladesEquipped = false;
                isAxeEquipped = true;

                UIManager.instance.SetWeaponIcon(2);
            }
            else if (isAxeEquipped)
            {
                StopCoroutine(DisarmAxe(false));
                StartCoroutine(DisarmAxe(false));

                isAxeEquipped = false;

                UIManager.instance.SetWeaponIcon(0);
            }
            else
            {
                StopCoroutine(ArmAxe());
                StartCoroutine(ArmAxe());

                isAxeEquipped = true;

                UIManager.instance.SetWeaponIcon(2);
            }
        }
    }

    public void OnBlade()
    {
        //////
        /// EQUIP/UNEQUIP BLADES
        //////
        if (!StarterAssets.ThirdPersonController.instance.Grounded) return;
        if (!isAnim && !isBlocked && !IsEquipping())
        {
            if(isAiming)
            {
                AxeAim(false);
            }
            if (isAxeEquipped)
            {
                StopCoroutine(DisarmAxe(false));
                StartCoroutine(DisarmAxe(true));

                isAxeEquipped = false;
                isBladesEquipped = true;

                UIManager.instance.SetWeaponIcon(1);
            }
            else if (isBladesEquipped)
            {
                StopCoroutine(DisarmBlades(false));
                StartCoroutine(DisarmBlades(false));

                isBladesEquipped = false;

                UIManager.instance.SetWeaponIcon(0);
            }
            else
            {
                StopCoroutine(ArmBlades());
                StartCoroutine(ArmBlades());

                isBladesEquipped = true;

                UIManager.instance.SetWeaponIcon(1);
            }
        }
    }

    public void OnQuickTurn()
    {
        //////
        /// QUICK TURN
        //////
        if (!StarterAssets.ThirdPersonController.instance.Grounded) return;
        if (!isAnim && !isBlocked)
        {
            StopCoroutine(QuickTurn());
            StartCoroutine(QuickTurn());
        }
    }

    public void OnRightStick(InputValue value)
    {
        isRightStickPressed = value.Get<float>() == 1;
        DoRageOrEnemyLock();
    }
    public void OnLeftStick(InputValue value)
    {
        isLeftStickPressed = value.Get<float>() == 1;
        DoRageOrEnemyLock();
    }

    public void OnRage()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            isRightStickPressed = true;
            isLeftStickPressed = true;
            DoRageOrEnemyLock();
        }
    }

    public void DoRageOrEnemyLock()
    {
        //////
        /// SPECIAL ABILITY (RAGE)
        //////
        if (!StarterAssets.ThirdPersonController.instance.Grounded) return;

        if (!isAiming && PlayerStats.instance.canRage && isRightStickPressed && isLeftStickPressed)
        {
            if (isBladesEquipped)
            {
                StartCoroutine(BladeBlock(false));
            }

            StopCoroutine(ShieldAnim(false));
            StartCoroutine(ShieldAnim(false));
            StarterAssets.ThirdPersonController.instance.RotationSmoothTime = .2f;

            isBlocked = false;

            StopCoroutine(SpecialAbility());
            StartCoroutine(SpecialAbility());

            FXHandler.instance.RageCamera();
            PlayerStats.instance.UseRage(10);

            isLeftStickPressed = false;
            isRightStickPressed = false;
        }
        //////
        /// ENEMY LOCK-ON
        //////
        else if (isRightStickPressed && !isLeftStickPressed)
        {
            isEnemyLockOn = true;
        }
    }

    public void OnAxeRecall()
    {
        //////
        /// AXE CATCH
        //////
        if (!StarterAssets.ThirdPersonController.instance.Grounded) return;
        if (isAxeThrown && !isAnim && !isPulling && !isBlocked)
        {
            StopCoroutine(CatchAxeAnim());
            StartCoroutine(CatchAxeAnim());
        }
    }

    public void OnHeavyAttack(InputValue value)
    {
        //////
        /// AXE/BLADES HEAVY ATTACK
        //////
        if (!StarterAssets.ThirdPersonController.instance.Grounded) return;

        if (!isAnim && !isAiming && isAimingButtonUp)
        {
            string weapon = "";
            if (isAxeEquipped && !isAxeThrown &&
                !ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Axe Heavy Attack") &&
                !ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Axe Attack 1") &&
                !ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Axe Attack 2"))
            {
                weapon = "axe";
                AxeHeavyAttack = true;
            }
            else if (isBladesEquipped && 
                !ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Blades Heavy Attack") &&
                !ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Blades Attack 1") &&
                !ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Blades Attack 2"))
            {                
                weapon = "blades";
                BladesHeavyAttack = true;
            }

            StopCoroutine(HeavyAttack(weapon));
            StartCoroutine(HeavyAttack(weapon));
        }
    }

    public void OnAttack()
    {
        if (!StarterAssets.ThirdPersonController.instance.Grounded) return;

        if (isAiming)
        {
            //////
            /// AXE THROW
            //////
            if (!isAxeThrown && !isBlocked && isAxeEquipped && isAiming)
            {
                StopCoroutine(ThrowAxeAnim());
                StartCoroutine(ThrowAxeAnim());
            }

            //////
            /// BLADES THROW
            //////
            if (!isBladeThrown && !isBlocked && isBladesEquipped && isAiming)
            {
                StopCoroutine(ThrowBlades());
                StartCoroutine(ThrowBlades());
            }
        }
        else if(isAimingButtonUp)
        {
            //////
            /// AXE ATTACK
            //////
            if (isAxeEquipped && !isAxeThrown && !isBlocked)
            {
                bool attack1;
                if (!ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Axe Attack 1"))
                {
                    attack1 = true;
                    AxeAttack1 = true;
                    AxeAttack2 = false;
                }
                else
                {
                    attack1 = false;
                    AxeAttack1 = false;
                    AxeAttack2 = true;
                }

                StopCoroutine(AttackAxe(attack1));
                StartCoroutine(AttackAxe(attack1));
            }

            //////
            /// BLADES ATTACK
            //////
            if (isBladesEquipped && !isBlocked)
            {
                bool attack1;
                if (!ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Blades Attack 1"))
                {
                    attack1 = true;
                    BladesAttack1 = true;
                    BladesAttack2 = false;
                }
                else
                {
                    attack1 = false;
                    BladesAttack1 = false;
                    BladesAttack2 = true;
                }

                StopCoroutine(AttackBladesPlayer(attack1));
                StartCoroutine(AttackBladesPlayer(attack1));
            }
        }
    }

    public void OnAim(InputValue value)
    {
        if (!StarterAssets.ThirdPersonController.instance.Grounded) return;

        //////
        /// AXE AIM
        //////
        if (!isAnim && value.Get<float>() == 1 && !isBladesEquipped)
        {
            AxeAim(true);
            isAimingButtonUp = false;
        }
        else if(!isBladesEquipped || isAxeThrown)
        {
            AxeAim(false);
            isAimingButtonUp = true;
        }

        //////
        /// BLADES AIM
        //////
        if (!isAnim && value.Get<float>() == 1 && isBladesEquipped)
        {
            BladesAim(true);
            isAimingButtonUp = false;
        }
        else if(isBladesEquipped)
        {
            BladesAim(false);
            isAimingButtonUp = true;
        }
    }

    public void AxeAim(bool status)
    {
        if(status)
        {
            isAiming = true;

            if (isAxeEquipped)
            {
                FXHandler.instance.ToggleFrostAxeParticle(1);
                SoundController.instance.PlayWeaponFXSound(9, "axe");
            }

            UIManager.instance.SetReticleType(isAxeThrown ? 0 : 1, true);


            ThirdPersonAnimator.SetBool("AxeAiming", true);
            LockCameraAnimator.SetBool("Zoom", true);
            CameraAnimator.SetBool("Zoom", true);
            StarterAssets.ThirdPersonController.instance.RotationSmoothTime = .05f;
        }
        else
        {
            StarterAssets.ThirdPersonController.instance.RotationSmoothTime = .2f;
            CameraAnimator.SetBool("Zoom", false);
            LockCameraAnimator.SetBool("Zoom", false);

            UIManager.instance.SetReticleType(isAxeThrown ? 0 : 1, false);

            if (!isAxeThrown)
                FXHandler.instance.ToggleFrostAxeParticle(0);

            ThirdPersonAnimator.SetBool("AxeAiming", false);
            isAiming = false;
        }
    }

    public void BladesAim(bool status)
    {
        if(status)
        {
            isAiming = true;

            SoundController.instance.PlayBladesSound(5);
            FXHandler.instance.ToggleFireBladeParticle(1);
            UIManager.instance.SetReticleType(0, true);

            ThirdPersonAnimator.SetBool("BladesAiming", true);

            //Blade rotate like whip
            //Blade2Loc.GetComponent<Animator>().SetBool("Aim", true);

            CameraAnimator.SetBool("Zoom", true);
            LockCameraAnimator.SetBool("Zoom", true);
            StarterAssets.ThirdPersonController.instance.RotationSmoothTime = .05f;
        }
        else
        {
            Blade2.GetComponent<AudioSource>().Stop();
            StarterAssets.ThirdPersonController.instance.RotationSmoothTime = .2f;

            CameraAnimator.SetBool("Zoom", false);
            LockCameraAnimator.SetBool("Zoom", false);
            UIManager.instance.SetReticleType(0, false);

            //Blade rotate like whip
            //Blade2Loc.GetComponent<Animator>().SetBool("Aim", false);

            //if (!ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Throw Blades") && !isThrown)
            //{
            //    Blade2.transform.localPosition = Vector3.zero;
            //    Blade2.transform.localEulerAngles = Vector3.zero;
            //}

            if (!isBladeThrown)
                FXHandler.instance.ToggleFireBladeParticle(0);

            ThirdPersonAnimator.SetBool("BladesAiming", false);
            isAiming = false;
        }
    }

    public void OnBlock(InputValue value)
    {
        if (!StarterAssets.ThirdPersonController.instance.Grounded) return;

        //////
        ///EQUIP SHIELD (BLOCK)
        //////
        if (!isAnim && !isAiming && value.Get<float>() == 1)
        {
            isBlocked = true;

            StarterAssets.ThirdPersonController.instance.RotationSmoothTime = .05f;

            StopCoroutine(ShieldAnim(true));
            StartCoroutine(ShieldAnim(true));

            if (isBladesEquipped)
            {
                StartCoroutine(BladeBlock(true));
            }

            FXHandler.instance.PlayShieldParticles(0);
        }
        else if (!isAiming)
        {
            SoundController.instance.PlayShieldSound(1);

            if (isBladesEquipped)
            {
                StartCoroutine(BladeBlock(false));
            }

            StopCoroutine(ShieldAnim(false));
            StartCoroutine(ShieldAnim(false));

            StarterAssets.ThirdPersonController.instance.RotationSmoothTime = .2f;

            isBlocked = false;
        }
    }

    public void ChainSwitch(int chain)
    {
        if(chain == 0)
        {
            Blade2Loc.GetComponent<ChainWeapon>().chain.gameObject.SetActive(true);
            Blade2Loc.GetComponent<ChainWeapon>().chainLine.gameObject.SetActive(false);
        }
        else if (chain == 1)
        {
            Blade2Loc.GetComponent<ChainWeapon>().chainLine.gameObject.SetActive(true);
            Blade2Loc.GetComponent<ChainWeapon>().chain.gameObject.SetActive(false);
        }

        if (chain == 2)
        {
            Blade1Loc.GetComponent<ChainWeapon>().chain.gameObject.SetActive(true);
            Blade1Loc.GetComponent<ChainWeapon>().chainLine.gameObject.SetActive(false);
        }
        else if (chain == 3)
        {
            Blade1Loc.GetComponent<ChainWeapon>().chainLine.gameObject.SetActive(true);
            Blade1Loc.GetComponent<ChainWeapon>().chain.gameObject.SetActive(false);
        }
    }

    public bool IsEquipping()
    {
        return ThirdPersonAnimator.GetCurrentAnimatorStateInfo(2).IsName("Disarm Blades") ||
                ThirdPersonAnimator.GetCurrentAnimatorStateInfo(2).IsName("Disarm Axe") ||
                ThirdPersonAnimator.GetCurrentAnimatorStateInfo(2).IsName("Arm Axe") ||
                ThirdPersonAnimator.GetCurrentAnimatorStateInfo(2).IsName("Arm Blades");
    }

    public bool IsAttacking()
    {
        return AxeAttack1 || AxeAttack2 || AxeHeavyAttack ||
                BladesAttack1 || BladesAttack2 || BladesHeavyAttack;
    }

    public bool IsPrimaryAttackAnimating()
    {
        return ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Axe Attack 1") ||
                ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Blades Attack 1");
    }

    public bool IsSecondaryAttackAnimating()
    {
        return ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Axe Attack 2") ||
                ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Blades Attack 2");
    }

    public bool IsHeavyAttackAnimating()
    {
        return ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Axe Heavy Attack") ||
                ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Blades Heavy Attack");
    }

    public bool IsIK()
    {
        return Vector3.Angle(transform.forward, ReticleDir.GetPoint(10) - transform.position) > 75 ||
                IsPrimaryAttackAnimating() || IsSecondaryAttackAnimating() || IsHeavyAttackAnimating() ||
                !StarterAssets.ThirdPersonController.instance.Grounded || ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Rage");
    }


    public void DebugBreak()
    {
        Debug.Break();
    }

    #region ANIM IENUMERATORS

    IEnumerator QuickTurn()
    {
        ThirdPersonAnimator.SetBool("QuickTurn", true);
        yield return new WaitForSeconds(0.1f);
        StarterAssets.StarterAssetsInputs.instance.LookInput(new Vector2(StarterAssets.ThirdPersonController.instance.CameraSensitivity / 1.25f, 0.0f));
        SoundController.instance.PlayGruntSound();
        ThirdPersonAnimator.SetBool("QuickTurn", false);
        yield return new WaitForSeconds(0.1f);
        transform.localEulerAngles = Vector3.Slerp(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + 180.0f, transform.localEulerAngles.z),
                                                    new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + 180.0f, transform.localEulerAngles.z),
                                                    .2f);
        StarterAssets.StarterAssetsInputs.instance.LookInput(Vector2.zero);
    }

    IEnumerator ThrowBlades()
    {
        SoundController.instance.PlayGruntSound();
        Vector3 throwPos = Camera.main.transform.forward;

        isBladeThrown = true;
        SoundController.instance.PlayBladesSound(6);
        Blade2Loc.GetComponent<Animator>().SetBool("Aim", false);
        Blade2.transform.localPosition = Vector3.zero;

        ThirdPersonAnimator.SetBool("Thrown", true);
        yield return new WaitForSeconds(.2f);

        Blade2Loc.GetComponent<Animator>().enabled = false;
        Blade2Loc.GetComponent<ChainWeapon>().enabled = true;
        Blade2.transform.localEulerAngles = new Vector3(40f, 100f, -90f);        
        Blade2.transform.parent = null;
        Blade2Rb.isKinematic = false;
        Blade2Rb.AddForce(ReticleDir.direction * throwPower * 1.5f, ForceMode.Impulse);
        
        ThirdPersonAnimator.SetBool("Thrown", false);
        yield return new WaitForSeconds(0.5f);

        Blade2Rb.isKinematic = true;
        Blade2.transform.parent = Blade2Loc.transform;
        Blade2.transform.localEulerAngles = Vector3.zero;
        Blade2.transform.DOLocalMove(Vector3.zero, .4f);
        yield return new WaitUntil(() => Blade2.transform.localPosition == Vector3.zero);
        Blade2Loc.GetComponent<ChainWeapon>().enabled = false;
        Blade2Loc.GetComponent<Animator>().enabled = true;
        isBladeThrown = false;

        //if(isAiming)
        //    Blade2Loc.GetComponent<Animator>().SetBool("Aim", true);
    }

    IEnumerator HeavyAttack(string type)
    {
        if (type == "axe")
        {
            StarterAssets.ThirdPersonController.instance.RotationSmoothTime = .0f;
            StopCoroutine(AttackAxe(true));
            ThirdPersonAnimator.SetBool("AxeHeavyAttack", true);
            yield return new WaitForSeconds(1.5f);
            ThirdPersonAnimator.SetBool("AxeHeavyAttack", false);
            AxeHeavyAttack = false;
            StarterAssets.ThirdPersonController.instance.RotationSmoothTime = .2f;
        }
        else if (type == "blades")
        {
            StarterAssets.ThirdPersonController.instance.RotationSmoothTime = .0f;
            StopCoroutine(AttackBladesPlayer(true));
            ThirdPersonAnimator.SetBool("BladesHeavyAttack", true);
            yield return new WaitForSeconds(.1f);
            FXHandler.instance.BladeHeavyAttackCamera();
            ToggleSlowMo(1);
            yield return new WaitForSeconds(.2f);
            ToggleSlowMo(0);
            yield return new WaitForSeconds(.8f);
            Blade1.transform.localEulerAngles = Vector3.zero;
            Blade1.transform.DOLocalMove(Vector3.zero, 0.1f);
            Blade2.transform.DOLocalMove(Vector3.zero, 0.1f);
            ThirdPersonAnimator.SetBool("BladesHeavyAttack", false);
            BladesHeavyAttack = false;
            StarterAssets.ThirdPersonController.instance.RotationSmoothTime = .2f;
        }
    }

    IEnumerator AttackAxe(bool attack1)
    {
        StarterAssets.ThirdPersonController.instance.RotationSmoothTime = .0f;
        isAnim = true;
        if (attack1)
        {
            ThirdPersonAnimator.SetBool("AxeAttack1", true);
            yield return new WaitForSeconds(1f);
            ThirdPersonAnimator.SetBool("AxeAttack1", false);
            AxeAttack1 = false;
        }
        else
        {
            ThirdPersonAnimator.SetBool("AxeAttack2", true);
            yield return new WaitForSeconds(1f);
            ThirdPersonAnimator.SetBool("AxeAttack2", false);
            AxeAttack2 = false;
        }
        StarterAssets.ThirdPersonController.instance.RotationSmoothTime = .2f;
    }

    IEnumerator AttackBladesPlayer(bool attack1)
    {
        isAnim = true;
        StarterAssets.ThirdPersonController.instance.RotationSmoothTime = .0f;
        if (attack1)
        {
            ThirdPersonAnimator.SetBool("BladesAttack1", true);
            yield return new WaitForSeconds(.5f);
            ThirdPersonAnimator.SetBool("BladesAttack1", false);
            BladesAttack1 = false;
        }
        else
        {
            ThirdPersonAnimator.SetBool("BladesAttack2", true);
            yield return new WaitForSeconds(.5f);            
            ThirdPersonAnimator.SetBool("BladesAttack2", false);
            BladesAttack2 = false;        
        }
        StarterAssets.ThirdPersonController.instance.RotationSmoothTime = .2f;
    }

    IEnumerator BladeBlock(bool status)
    {
        Blade1Loc.GetComponent<Animator>().SetBool("BladeBlock", status);
        yield return new WaitForSeconds(.1f);        
    }

    IEnumerator Blade1Attack()
    {
        Blade2Loc.GetComponent<Animator>().SetBool("Attack1", true);
        yield return new WaitForSeconds(.1f);
        Blade2Loc.GetComponent<Animator>().SetBool("Attack1", false);
    }

    IEnumerator Blade2Attack()
    {
        Blade1Loc.GetComponent<Animator>().SetBool("Attack2", true);
        Blade2Loc.GetComponent<Animator>().SetBool("Attack2", true);
        yield return new WaitForSeconds(.1f);
        Blade1Loc.GetComponent<Animator>().SetBool("Attack2", false);
        Blade2Loc.GetComponent<Animator>().SetBool("Attack2", false);
    }

    IEnumerator BladeAttackHeavy()
    {
        Blade1Loc.GetComponent<Animator>().SetBool("HeavyAttack", true);
        Blade2Loc.GetComponent<Animator>().SetBool("HeavyAttack", true);
        yield return new WaitForSeconds(.1f);
        Blade1Loc.GetComponent<Animator>().SetBool("HeavyAttack", false);
        Blade2Loc.GetComponent<Animator>().SetBool("HeavyAttack", false);
    }

    IEnumerator Die()
    {
        ThirdPersonAnimator.SetBool("Die", true);
        yield return new WaitForSeconds(1f);
        ThirdPersonAnimator.SetBool("Die", false);
    }

    IEnumerator ShieldAnim(bool status)
    {
        ShieldAnimator.SetBool("isBlocked", status);
        ThirdPersonAnimator.SetBool("Blocked", status);
        yield return new WaitForSeconds(.35f);      
    }

    public void ArmAxeGameobject(int arm)
    {
        bool equip = arm == 1;

        Axe.SetActive(equip);
        AxeDisarmed.SetActive(!equip);
    }

    public void ArmBladesGameobject(int arm)
    {
        bool equip = arm == 1;

        Blade1.SetActive(equip);
        Blade2.SetActive(equip);
        BladeDisarmed.SetActive(!equip);
    }

    IEnumerator ArmAxe()
    {
        ThirdPersonAnimator.SetBool("ArmedAxe", true);
        yield return new WaitForSeconds(.3f);

        yield return new WaitForSeconds(.1f);
        ThirdPersonAnimator.SetBool("ArmedAxe", false);

        if (!isAimingButtonUp)
            AxeAim(true);
    }
    
    IEnumerator DisarmAxe(bool isBlade)
    {
        ThirdPersonAnimator.SetBool("DisarmedAxe", true);

        yield return new WaitForSeconds(.1f);
        SoundController.instance.PlayAxeSound(6);

        yield return new WaitForSeconds(.1f);

        if (isBlade)
        {
            StopCoroutine(ArmBlades());
            StartCoroutine(ArmBlades());
        }

        yield return new WaitForSeconds(.1f);

        ThirdPersonAnimator.SetBool("DisarmedAxe", false);

        yield return new WaitForSeconds(1.1f);
    }

    IEnumerator ArmBlades()
    {
        ThirdPersonAnimator.SetBool("ArmedBlades", true);
        yield return new WaitForSeconds(.3f);
        
        yield return new WaitForSeconds(.1f);
        ThirdPersonAnimator.SetBool("ArmedBlades", false);

        if (!isAimingButtonUp)
            BladesAim(true);
    }

    IEnumerator DisarmBlades(bool isAxe)
    {
        ThirdPersonAnimator.SetBool("DisarmedBlades", true);

        SoundController.instance.PlayBladesSound(1);

        yield return new WaitForSeconds(.1f);

        if (isAxe)
        {
            StopCoroutine(ArmAxe());
            StartCoroutine(ArmAxe());
        }

        yield return new WaitForSeconds(.1f);

        ThirdPersonAnimator.SetBool("DisarmedBlades", false);

        yield return new WaitForSeconds(1.1f);
    }

    IEnumerator ThrowAxeAnim()
    {
        ThirdPersonAnimator.SetBool("Thrown", true);
        yield return new WaitForSeconds(.5f);
        ThirdPersonAnimator.SetBool("Thrown", false);
    }

    IEnumerator SpecialAbility()
    {
        ThirdPersonAnimator.SetBool("Rage", true);
        yield return new WaitForSeconds(.5f);
        ThirdPersonAnimator.SetBool("Rage", false);
    }

    IEnumerator CatchAxeAnim()
    {
        if(isBladesEquipped)
        {
            StopCoroutine(DisarmBlades(false));
            StartCoroutine(DisarmBlades(false));
            yield return new WaitForSeconds(.5f);
            isBladesEquipped = false;

            UIManager.instance.SetWeaponIcon(0);
        }

        FXHandler.instance.ToggleFrostAxeParticle(1);
        ThirdPersonAnimator.SetBool("Pulling", true);
        yield return new WaitForSeconds(.2f);
        ReturnAxe();
    }

    #endregion
}
