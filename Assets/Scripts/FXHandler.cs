using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class FXHandler : MonoBehaviour
{
    public static FXHandler instance;

    [SerializeField] Transform FxSpawnParent;

    [Header("WeaponFX")]
    [SerializeReference] Material ChainLineMat;
    [SerializeReference] Material ChainMat;
    [Tooltip("0: Hit, 1: HeavyHit, 2: Fire")] [SerializeField] GameObject[] BladeParticleSystems;
    [Tooltip("0: Hit, 1: Frost, 2: HandleSnow")] [SerializeField] GameObject[] AxeParticleSystems;
    [SerializeField] GameObject[] ShieldParticleSystems;    

    [Header("RageFX")]
    [SerializeField] GameObject RageFireFX;

    [Header("BloodFX")]
    [SerializeField] bool InfiniteDecal;
    [SerializeField] Light DirLight;
    [SerializeField] GameObject BloodAttach;
    [SerializeField] GameObject[] BloodFX;
    [SerializeField] Vector3 direction;
    int effectIdx;

    List<GameObject> tempFX = new List<GameObject>();
    Animator CameraAnimator;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CameraAnimator = PlayerController.instance.CameraAnimator;
    }

    public void ToggleFireBladeParticle(int status)
    {
        bool isEnabled = status == 1;

        for (int i = 2; i <= BladeParticleSystems.Length; i++)
        {
            if (isEnabled)
            {
                switch (i)
                {
                    case 2:
                        BladeParticleSystems[i].GetComponent<PSMeshRendererUpdater>().
                            UpdateMeshEffect(PlayerController.instance.Blade1.transform.GetChild(0).GetChild(2).gameObject);

                        PlayerController.instance.Blade1.transform.GetChild(2).gameObject.SetActive(true);
                        BladeParticleSystems[i].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                        BladeParticleSystems[i].transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                        BladeParticleSystems[i].transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>().Play();
                        BladeParticleSystems[i].transform.GetChild(2).GetComponent<ParticleSystem>().Play();
                        break;

                    case 3:
                        BladeParticleSystems[i].GetComponent<PSMeshRendererUpdater>().
                            UpdateMeshEffect(PlayerController.instance.Blade2.transform.GetChild(0).GetChild(2).gameObject);

                        PlayerController.instance.Blade2.transform.GetChild(2).gameObject.SetActive(true);
                        BladeParticleSystems[i].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                        BladeParticleSystems[i].transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                        BladeParticleSystems[i].transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>().Play();
                        BladeParticleSystems[i].transform.GetChild(2).GetComponent<ParticleSystem>().Play();
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 2:
                        BladeParticleSystems[i].transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                        BladeParticleSystems[i].transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
                        BladeParticleSystems[i].transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>().Stop();
                        BladeParticleSystems[i].transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
                        PlayerController.instance.Blade1.transform.GetChild(2).gameObject.SetActive(false);

                        RemoveAdditionalMaterial(PlayerController.instance.Blade1.transform.GetChild(0).GetChild(2).gameObject.GetComponent<Renderer>().materials,
                                        PlayerController.instance.Blade1.transform.GetChild(0).GetChild(2).gameObject.GetComponent<Renderer>());
                        break;

                    case 3:
                        BladeParticleSystems[i].transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                        BladeParticleSystems[i].transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
                        BladeParticleSystems[i].transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>().Stop();
                        BladeParticleSystems[i].transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
                        PlayerController.instance.Blade2.transform.GetChild(2).gameObject.SetActive(false);

                        RemoveAdditionalMaterial(PlayerController.instance.Blade2.transform.GetChild(0).GetChild(2).gameObject.GetComponent<Renderer>().materials,
                                        PlayerController.instance.Blade2.transform.GetChild(0).GetChild(2).gameObject.GetComponent<Renderer>());
                        break;
                }
            }
        }
    }

    public void ToggleFrostAxeParticle(int status)
    {
        bool isEnabled = status == 1;

        for (int i = 1; i <= AxeParticleSystems.Length; i++)
        {
            if (isEnabled)
            {
                switch (i)
                {
                    case 1:
                        AxeParticleSystems[i].GetComponent<PSMeshRendererUpdater>().
                            UpdateMeshEffect(PlayerController.instance.Axe.transform.GetChild(0).gameObject);

                        AxeParticleSystems[i].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                        AxeParticleSystems[i].transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play();
                        AxeParticleSystems[i].transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                        AxeParticleSystems[i].transform.GetChild(2).GetComponent<ParticleSystem>().Play();
                        AxeParticleSystems[i].transform.GetChild(3).GetComponent<ParticleSystem>().Play();
                        break;

                    case 2:
                        AxeParticleSystems[i].GetComponent<PSMeshRendererUpdater>().
                            UpdateMeshEffect(PlayerController.instance.Axe.transform.GetChild(1).gameObject);

                        AxeParticleSystems[i].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                        AxeParticleSystems[i].transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play();
                        AxeParticleSystems[i].transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                        AxeParticleSystems[i].transform.GetChild(2).GetComponent<ParticleSystem>().Play();
                        AxeParticleSystems[i].transform.GetChild(3).GetComponent<ParticleSystem>().Play();
                        break;

                    case 3:
                        if (!PlayerController.instance.isAiming)
                        {
                            AxeParticleSystems[i].GetComponent<PSMeshRendererUpdater>().
                                UpdateMeshEffect(PlayerController.instance.Axe.transform.GetChild(2).gameObject);

                            AxeParticleSystems[i].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                            AxeParticleSystems[i].transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                        }
                        break;

                    case 4:
                        if (!PlayerController.instance.isAiming)
                        {
                            AxeParticleSystems[i].GetComponent<PSMeshRendererUpdater>().
                            UpdateMeshEffect(PlayerController.instance.Axe.transform.GetChild(2).gameObject);

                            PlayerController.instance.Axe.transform.GetChild(7).gameObject.SetActive(true);
                            AxeParticleSystems[i].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                        }
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 1:
                        AxeParticleSystems[i].transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                        AxeParticleSystems[i].transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Stop();
                        AxeParticleSystems[i].transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
                        AxeParticleSystems[i].transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
                        AxeParticleSystems[i].transform.GetChild(3).GetComponent<ParticleSystem>().Stop();

                        RemoveAdditionalMaterial(PlayerController.instance.Axe.transform.GetChild(0).gameObject.GetComponent<Renderer>().materials,
                                        PlayerController.instance.Axe.transform.GetChild(0).gameObject.GetComponent<Renderer>());
                        break;

                    case 2:
                        AxeParticleSystems[i].transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                        AxeParticleSystems[i].transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Stop();
                        AxeParticleSystems[i].transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
                        AxeParticleSystems[i].transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
                        AxeParticleSystems[i].transform.GetChild(3).GetComponent<ParticleSystem>().Stop();

                        RemoveAdditionalMaterial(PlayerController.instance.Axe.transform.GetChild(1).gameObject.GetComponent<Renderer>().materials,
                                        PlayerController.instance.Axe.transform.GetChild(1).gameObject.GetComponent<Renderer>());
                        break;

                    case 3:
                        if (!PlayerController.instance.isAiming)
                        {
                            AxeParticleSystems[i].transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                            AxeParticleSystems[i].transform.GetChild(1).GetComponent<ParticleSystem>().Stop();

                            RemoveAdditionalMaterial(PlayerController.instance.Axe.transform.GetChild(2).gameObject.GetComponent<Renderer>().materials,
                                            PlayerController.instance.Axe.transform.GetChild(2).gameObject.GetComponent<Renderer>());
                        }
                        break;

                    case 4:
                        if (!PlayerController.instance.isAiming)
                        {
                            PlayerController.instance.Axe.transform.GetChild(7).gameObject.SetActive(false);
                            AxeParticleSystems[i].transform.GetChild(0).GetComponent<ParticleSystem>().Stop();

                            RemoveAdditionalMaterial(PlayerController.instance.Axe.transform.GetChild(2).gameObject.GetComponent<Renderer>().materials,
                                            PlayerController.instance.Axe.transform.GetChild(2).gameObject.GetComponent<Renderer>());
                        }
                        break;
                }
            }
        }
    }

    public void RemoveAdditionalMaterial(Material[] materials, Renderer renderer)
    {
        Material[] mat = new Material[1];
        mat[0] = materials[0];
        renderer.materials = mat;
    }

    public void PlayParticleSystem(int system, RaycastHit Hit)
    {
        var particle = new ParticleSystem();

        if(PlayerController.instance.isAxeEquipped)
            particle = Instantiate(AxeParticleSystems[system].GetComponent<ParticleSystem>(), Hit.point, Quaternion.LookRotation(Hit.normal));
        else if (PlayerController.instance.isBladesEquipped)
            particle = Instantiate(BladeParticleSystems[system].GetComponent<ParticleSystem>(), Hit.point, Quaternion.LookRotation(Hit.normal));

        particle.Play();
    }

    public void PlayShieldParticles(int type)
    {
        StartCoroutine(ShieldParticles(type));
    }

    IEnumerator ShieldParticles(int type)
    {
        yield return new WaitForSeconds(.2f);
        ShieldParticleSystems[type].transform.GetChild(2).GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(.5f);
        ShieldParticleSystems[type].transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
    }

    public void ChainColor(int status)
    {
        PlayerStats.instance.ChangeChainColor(status);
    }

    public void RageFX()
    {
        RageFireFX.GetComponent<ParticleSystem>().Play();
        UIManager.instance.ChangeTint(1, new Color(1.0f, 0.7f, 0.7f));
    }

    public void RageCamera()
    {
        StartCoroutine(RageCameraAnim());
    }

    public void BladeHeavyAttackCamera()
    {
        StartCoroutine(BladeHeavyAttackCameraAnim());
    }

    IEnumerator AxeCatchCameraShake()
    {
        CameraAnimator.SetBool("CatchShake", true);
        yield return new WaitForSeconds(.3f);
        CameraAnimator.SetBool("CatchShake", false);
    }

    IEnumerator RageCameraAnim()
    {
        CameraAnimator.SetBool("Rage", true);
        yield return new WaitForSeconds(1.0f);
        CameraAnimator.SetBool("Rage", false);
    }

    IEnumerator BladeHeavyAttackCameraAnim()
    {
        CameraAnimator.SetBool("BladeHeavyAttack", true);
        yield return new WaitForSeconds(.2f);
        CameraAnimator.SetBool("BladeHeavyAttack", false);
    }

    public IEnumerator ChangeColour(Color startColor, Color endColor)
    {
        Color colorToChange = new Color();
        float tick = 0f;
        while (colorToChange != endColor)
        {
            tick += Time.deltaTime * 1.0f;
            colorToChange = Color.Lerp(startColor, endColor, tick);
            UIManager.instance.ChangeTint(1, colorToChange);
            yield return null;
        }
    }

    public void SplashBlood(RaycastHit hit)
    {
        //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit))
        {
            // var randRotation = new Vector3(0, Random.value * 360f, 0);
            // var dir = CalculateAngle(Vector3.forward, hit.normal);
            float angle = Mathf.Atan2(hit.normal.x, hit.normal.z) * Mathf.Rad2Deg + 180;

            //var effectIdx = Random.Range(0, BloodFX.Length);
            if (effectIdx == BloodFX.Length) effectIdx = 0;

            var instance = Instantiate(BloodFX[effectIdx], hit.point, Quaternion.Euler(0, angle + 90, 0));
            effectIdx++;

            var settings = instance.GetComponent<BFX_BloodSettings>();
            //settings.FreezeDecalDisappearance = InfiniteDecal;
            settings.LightIntensityMultiplier = DirLight.intensity;

            var nearestBone = GetNearestObject(hit.transform.root, hit.point);
            if (nearestBone != null)
            {
                var attachBloodInstance = Instantiate(BloodAttach);
                var bloodT = attachBloodInstance.transform;
                bloodT.position = hit.point;
                bloodT.localRotation = Quaternion.identity;
                bloodT.localScale = Vector3.one * UnityEngine.Random.Range(0.75f, 1.2f);
                bloodT.LookAt(hit.point + hit.normal, direction);
                bloodT.Rotate(90, 0, 0);
                bloodT.transform.parent = nearestBone;
                Destroy(attachBloodInstance, 10);
            }

            if (!InfiniteDecal) Destroy(instance, 10);
        }
    }

    Transform GetNearestObject(Transform hit, Vector3 hitPos)
    {
        var closestPos = 100f;
        Transform closestBone = null;
        var childs = hit.GetComponentsInChildren<Transform>();

        foreach (var child in childs)
        {
            var dist = Vector3.Distance(child.position, hitPos);
            if (dist < closestPos)
            {
                closestPos = dist;
                closestBone = child;
            }
        }

        var distRoot = Vector3.Distance(hit.position, hitPos);
        if (distRoot < closestPos)
        {
            closestPos = distRoot;
            closestBone = hit;
        }
        return closestBone;
    }
}
