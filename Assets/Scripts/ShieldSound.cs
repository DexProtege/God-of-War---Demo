using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSound : MonoBehaviour
{
    public void PlayShieldSound(int isOpen)
    {
        SoundController.instance.PlayShieldSound(isOpen);
    }
}
