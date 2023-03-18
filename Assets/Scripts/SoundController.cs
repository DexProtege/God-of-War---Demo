using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{
    public static SoundController instance;

    [Header("Audio Clips")]
    [SerializeField] AudioClip[] WalkingClips;    
    [SerializeField] AudioClip[] RunningClips;
    [SerializeField] AudioClip[] JumpStartClips;
    [SerializeField] AudioClip[] JumpEndClips;
    [SerializeField] AudioClip[] AxeClips;
    [SerializeField] AudioClip[] BladesClips;
    [SerializeField] AudioClip[] ShieldClips;
    [SerializeField] AudioClip[] GruntClips;
    [SerializeField] AudioClip LongGruntClip;
    [SerializeField] AudioClip RageClip;
    [SerializeField] AudioClip DieClip;

    [Header("Audio Sources")]
    [SerializeField] AudioSource PlayerAudioSource;
    [SerializeField] AudioSource PlayerFootAudioSource;
    [SerializeField] AudioSource AxeWeaponAudioSource;    
    [SerializeField] AudioSource ShieldWeaponAudioSource;
    [SerializeField] AudioSource Blade2WeaponAudioSource;
    [SerializeField] AudioSource Blade1WeaponAudioSource;

    [Header("Audio Mixer")]
    [SerializeField] AudioMixer MainAudioMixer;
    [SerializeField] AudioMixerGroup FootAudioMixerGroup;
    [SerializeField] AudioMixerGroup PlayerAudioMixerGroup;
    [SerializeField] AudioMixerGroup WeaponFXMixerGroup;
    [SerializeField] AudioMixerGroup AxeFXMixerGroup;
    [SerializeField] AudioMixerGroup Blade1FXMixerGroup;
    [SerializeField] AudioMixerGroup Blade2FXMixerGroup;
    [SerializeField] AudioMixerGroup ShieldFXMixerGroup;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {

    }

    public void PlayWalkSound()
    {
        PlayerFootAudioSource.outputAudioMixerGroup = FootAudioMixerGroup;
        PlayerFootAudioSource.PlayOneShot(WalkingClips[Random.Range(0, WalkingClips.Length)]);
    }

    public void PlayRunSound()
    {
        PlayerFootAudioSource.outputAudioMixerGroup = FootAudioMixerGroup;
        PlayerFootAudioSource.PlayOneShot(RunningClips[Random.Range(0, RunningClips.Length)]);
    }

    public void PlayJumpStartSound()
    {
        PlayerFootAudioSource.outputAudioMixerGroup = FootAudioMixerGroup;
        PlayerFootAudioSource.PlayOneShot(JumpStartClips[Random.Range(0, JumpStartClips.Length)]);
    }

    public void PlayJumpEndSound()
    {
        PlayerFootAudioSource.outputAudioMixerGroup = FootAudioMixerGroup;
        PlayerFootAudioSource.PlayOneShot(JumpEndClips[Random.Range(0, JumpEndClips.Length)]);
    }

    public void PlayAxeSound(int soundNumber)
    {
        AxeWeaponAudioSource.outputAudioMixerGroup = AxeFXMixerGroup;
        AxeWeaponAudioSource.PlayOneShot(AxeClips[soundNumber]);
    }

    public void PlayWeaponFXSound(int soundNumber, string weapon)
    {
        if (weapon == "axe")
        {
            AxeWeaponAudioSource.outputAudioMixerGroup = WeaponFXMixerGroup;
            AxeWeaponAudioSource.PlayOneShot(AxeClips[soundNumber]);
        }
        else if (weapon == "blade1")
        {
            Blade1WeaponAudioSource.outputAudioMixerGroup = WeaponFXMixerGroup;
            Blade1WeaponAudioSource.PlayOneShot(BladesClips[soundNumber]);
        }
        else if (weapon == "blade2")
        {
            Blade2WeaponAudioSource.outputAudioMixerGroup = WeaponFXMixerGroup;
            Blade2WeaponAudioSource.PlayOneShot(BladesClips[soundNumber]);
        }
    }

    public void PlayBladesSound(int soundNumber)
    {
        bool isBlade1 = soundNumber <= 2 || soundNumber == 4 || soundNumber == 6 || soundNumber >= 7;

        if (isBlade1)
        {
            Blade1WeaponAudioSource.outputAudioMixerGroup = Blade1FXMixerGroup;
            Blade1WeaponAudioSource.PlayOneShot(BladesClips[soundNumber]);
        }
        else
        { 
            Blade2WeaponAudioSource.outputAudioMixerGroup = Blade2FXMixerGroup;
            Blade2WeaponAudioSource.PlayOneShot(BladesClips[soundNumber]);
        }
    }

    public void PlayShieldSound(int isOpen)
    {
        ShieldWeaponAudioSource.outputAudioMixerGroup = ShieldFXMixerGroup;
        ShieldWeaponAudioSource.PlayOneShot(ShieldClips[isOpen]);
    }

    public void PlayGruntSound()
    {
        PlayerAudioSource.outputAudioMixerGroup = PlayerAudioMixerGroup;
        PlayerAudioSource.PlayOneShot(GruntClips[Random.Range(0, GruntClips.Length)]);
    }

    public void PlayLongGruntSound()
    {
        PlayerAudioSource.outputAudioMixerGroup = PlayerAudioMixerGroup;
        PlayerAudioSource.PlayOneShot(LongGruntClip);
    }

    public void PlayRageSound()
    {
        PlayerAudioSource.outputAudioMixerGroup = PlayerAudioMixerGroup;
        PlayerAudioSource.PlayOneShot(RageClip);
    }

    public void PlayDeathSound()
    {
        PlayerAudioSource.outputAudioMixerGroup = PlayerAudioMixerGroup;
        PlayerAudioSource.PlayOneShot(DieClip);
    }
}
