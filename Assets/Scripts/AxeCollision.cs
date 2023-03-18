using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name + "|" + collision.gameObject.tag);
        //if (collision.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        if (collision.gameObject.tag != "MainCamera" || collision.gameObject.tag != "Player" || collision.gameObject.tag != "Weapon")
        {
            PlayerController.instance.isCollided = true;
            GetComponent<Rigidbody>().isKinematic = true;
            FXHandler.instance.ToggleFrostAxeParticle(0);
        }
    }
}
