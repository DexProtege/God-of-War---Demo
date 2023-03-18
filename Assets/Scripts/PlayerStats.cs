using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    public float PlayerHealth = 100;
    public float PlayerRage = 100;
    public float MaxPlayerRage = 100;
    public float MaxPlayerHealth = 100;

    public bool canRage = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        canRage = MaxPlayerRage == PlayerRage;
    }

    public void ChangeChainColor(int status)
    {
        if (status == 0)
            StartCoroutine(ColorLerp(Color.red, Color.black, .1f));
        else
            StartCoroutine(ColorLerp(Color.black, Color.red, .2f));
    }

    public void UseRage(float duration)
    {
        canRage = false;        
        StartCoroutine(RageLerpFunction(1.0f, 0.0f, duration));
    }

    IEnumerator RageLerpFunction(float startValue, float endValue, float duration)
    {
        yield return new WaitForSeconds(2.5f);
        float time = 0;
        PlayerController.instance.Rage = true;
        while (time <= duration)
        {
            float value = Mathf.Lerp(startValue, endValue, time / duration);
            UIManager.instance.UpdateHUDBars(value, 1);
            PlayerController.instance.CheckRage();
            PlayerRage = value * 100;
            time += Time.deltaTime;
            yield return null;
        }
        PlayerController.instance.Rage = false;
        PlayerController.instance.CheckRage();
        UIManager.instance.UpdateHUDBars(endValue, 1);
        StartCoroutine(FXHandler.instance.ChangeColour(new Color(1.0f, 0.7f, 0.7f), Color.white));
    }

    IEnumerator ColorLerp(Color startColor, Color endColor, float duration)
    {
        float time = 0;
        while (time <= duration)
        {
            Color value = Color.Lerp(startColor, endColor, time / duration);
            PlayerController.instance.Blade1Loc.GetComponent<ChainWeapon>().chainLine.GetComponent<LineRenderer>().material.SetColor("_EmissiveColor", value * 50);
            PlayerController.instance.Blade2Loc.GetComponent<ChainWeapon>().chainLine.GetComponent<LineRenderer>().material.SetColor("_EmissiveColor", value * 50);
            time += Time.fixedDeltaTime;
            yield return null;
        }
    }
}
