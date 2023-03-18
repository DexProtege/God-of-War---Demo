using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            switch (tag)
            {
                case "QuestLocation":
                    QuestManager.instance.CheckpointNextLocation();
                    break;

                case "AreaLocation":
                    LocationManager.instance.LocationName();
                    break;
            }          
        }
    }
}
