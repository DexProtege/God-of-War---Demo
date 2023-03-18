using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Profiling;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] AudioSource UIAudioSource;

    [Header("Post Processing")]
    public Volume PostProcessingVolume;

    [Header("Reticle")]
    [SerializeField] GameObject[] Reticles;    

    [Header("Compass")]
    [SerializeField] List<GameObject> Directions;
    [SerializeField] List<GameObject> Dividers;
    [SerializeField] GameObject ArrowLeft;
    [SerializeField] GameObject ArrowRight;
    public GameObject QuestWaypoint;

    [Header("Bottom HUD")]
    [SerializeField] Sprite[] WeaponIcons;
    [SerializeField] Image WeaponIcon;
    [SerializeField] Image HealthFill;
    [SerializeField] Image Rage1Fill;
    [SerializeField] GameObject Rage2;
    [SerializeField] Image Rage2Fill;
    [SerializeField] GameObject RageControls;

    [Header("Location")]
    [SerializeField] GameObject LocationUI;
    [SerializeField] TextMeshProUGUI LocationName;

    [Header("Quest Bar")]
    [SerializeField] AudioClip QuestPopSound;
    [SerializeField] GameObject QuestBar;
    [SerializeField] TextMeshProUGUI QuestTitle;
    [SerializeField] TextMeshProUGUI QuestSubtitle;
    [SerializeField] TextMeshProUGUI QuestDesc;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

    }

    private void Update()
    {
        if(PlayerController.instance.ControlType == "Gamepad")
            RageControls.SetActive(PlayerStats.instance.canRage);
        else
            RageControls.SetActive(false);

        CompassDirection();
    }

    public void ChangeTint(int index, Color color)
    {        
        if (PostProcessingVolume.profile.TryGet<ColorAdjustments>(out var colorAdj))
        {
            colorAdj.colorFilter.value = color;
        }            
    }

    public void UpdateHUDBars(float fillAmount, int index)
    {
        switch(index)
        {
            case 0:
                HealthFill.fillAmount = fillAmount;
                break;

            case 1:
                Rage1Fill.fillAmount = fillAmount;
                break;

            case 2:
                Rage2Fill.fillAmount = fillAmount;
                break;
        }
    }

    public void CompassDirection()
    {
        float pos = Camera.main.transform.localEulerAngles.y;

        if (pos > 180)
            pos -= 360;

        Directions[0].transform.localPosition = new Vector3(-pos * 15f, -30.0f, 0f);
        Directions[1].transform.localPosition = new Vector3(Directions[0].transform.localPosition.x + 1350.0f, -20.0f, 0f);
        Directions[2].transform.localPosition = new Vector3(Directions[0].transform.localPosition.x - 1350.0f, -20.0f, 0f);
        Directions[3].transform.localPosition = new Vector3(Directions[0].transform.localPosition.x + 2700.0f, -20.0f, 0f);
        Directions[4].transform.localPosition = new Vector3(Directions[0].transform.localPosition.x - 2700.0f, -20.0f, 0f);

        Dividers[0].transform.localPosition = new Vector3(Directions[0].transform.localPosition.x + 675.00f, 0f, 0f);
        Dividers[1].transform.localPosition = new Vector3(Directions[1].transform.localPosition.x + 675.00f, 0f, 0f);
        Dividers[2].transform.localPosition = new Vector3(Directions[2].transform.localPosition.x + 675.00f, 0f, 0f);
        Dividers[3].transform.localPosition = new Vector3(Directions[3].transform.localPosition.x + 675.00f, 0f, 0f);
        Dividers[4].transform.localPosition = new Vector3(Directions[4].transform.localPosition.x + 675.00f, 0f, 0f);

        float questPos = QuestManager.instance.QuestPosOnCompass();

        if (questPos > 400.0f)
        {
            questPos = 400.0f;
            ArrowLeft.SetActive(false);
            ArrowRight.SetActive(true);
        }
        else if (questPos < -400.0f)
        {
            questPos = -400.0f;
            ArrowLeft.SetActive(true);
            ArrowRight.SetActive(false);
        }
        else
        {
            ArrowLeft.SetActive(false);
            ArrowRight.SetActive(false);
        }

        QuestWaypoint.transform.localPosition = new Vector3(questPos, 0, 0);
        QuestWaypoint.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(QuestManager.instance.distance).ToString() + "m";

        foreach (var dir in Directions)
        {
            if (dir.transform.localPosition.x > 400.0f || dir.transform.localPosition.x < -400.0f)
            {
                dir.SetActive(false);
            }
            else
            {
                dir.SetActive(true);
            }
        }

        foreach (var dir in Dividers)
        {
            if (dir.transform.localPosition.x > 400.0f || dir.transform.localPosition.x < -400.0f)
            {
                dir.SetActive(false);
            }
            else
            {
                dir.SetActive(true);
            }
        }
    }

    public void SetWeaponIcon(int weapon)
    {
        if (weapon == 0)
        {
            WeaponIcon.enabled = false;
        }
        else
        {
            WeaponIcon.enabled = true;
            WeaponIcon.sprite = WeaponIcons[weapon - 1];
        }
    }

    public void ChangeReticleColor(bool status)
    {
        foreach (var reticle in Reticles)
        {
            if(status)
                reticle.GetComponent<Image>().color = Color.red;
            else
                reticle.GetComponent<Image>().color = Color.white;
        }
    }

    public void SetReticleType(int type, bool status)
    {
        Reticles[type].SetActive(status);
        
        for(int i = 0; i < Reticles.Length; i++)
        {
            if (i != type)
                Reticles[i].SetActive(false);
        }
    }

    public void QuestPop(string title, string subtitle, string desc)
    {
        QuestTitle.text = title;
        QuestSubtitle.text = subtitle;
        QuestDesc.text = desc;

        QuestBar.SetActive(false);
        QuestBar.SetActive(true);
        UIAudioSource.clip = QuestPopSound;
        UIAudioSource.Play();
    }

    public void LocationPop(string locationName)
    {
        LocationName.text = locationName;
        LocationUI.SetActive(false);
        LocationUI.SetActive(true);
    }
}
