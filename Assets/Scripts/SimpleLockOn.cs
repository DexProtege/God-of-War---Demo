using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class SimpleLockOn : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float RotationSpeed;
    float z;

    void OnEnable()
    {
        if(target == null) target = Camera.main.transform;
        StartCoroutine(LookAtTarget());
    }

    private IEnumerator LookAtTarget()
    {
        while(this.gameObject.activeInHierarchy)
        {
            Vector3 _dir = target.position - transform.position;            

            transform.rotation = Quaternion.LookRotation(_dir);

            z += RotationSpeed * Time.deltaTime;

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + z);

            yield return null;
        }

        z = 0;
    }
}
