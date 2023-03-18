using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class PlayerAttackPush : MonoBehaviour
{
    public static PlayerAttackPush instance;

    public Transform currentTarget;
    public LayerMask targetLayers;

    [Header("Settings")]
    [SerializeField] bool zeroVert_Look;
    [SerializeField] float noticeZone;
    [Tooltip("Angle_Degree")][SerializeField] float maxNoticeAngle = 60;

    Transform cam;
    public bool enemyLocked;
    float currentYOffset;
    Vector3 pos;

    void Start()
    {
        instance = this;

        cam = Camera.main.transform;
    }

    void Update()
    {
        //Debug purpose
        //Vector3 startPos = GameObject.Find("Monster").transform.position;
        //Vector3 player = transform.position;
        //Vector3 endPos = startPos + ((player - startPos) * (.5f));
        //Debug.DrawLine(startPos, endPos, Color.black);

        if (PlayerController.instance.isAxeEquipped)
            noticeZone = 5;
        else if (PlayerController.instance.isBladesEquipped)
            noticeZone = 10;

        if (PlayerController.instance.IsAttacking())
        {
            if (currentTarget)
            {
                //If there is already a target, Reset.
                ResetTarget();
                return;
            }

            if (currentTarget = ScanNearBy()) FoundTarget(); else ResetTarget();
        }

        if (enemyLocked)
        {
            TranslateToTarget();
            if (!TargetOnRange()) ResetTarget();
        }

    }


    void FoundTarget()
    {
        enemyLocked = true;
        //Debug.Log("enemy target found");
    }

    void ResetTarget()
    {
        currentTarget = null;
        enemyLocked = false;
        //Debug.Log("enemy target gone");
    }


    private Transform ScanNearBy()
    {
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
        float closestAngle = maxNoticeAngle;
        Transform closestTarget = null;
        if (nearbyTargets.Length <= 0) return null;

        for (int i = 0; i < nearbyTargets.Length; i++)
        {
            //Debug.Log(nearbyTargets[i].name);
            Vector3 dir = nearbyTargets[i].transform.position - cam.position;
            dir.y = 0;
            float _angle = Vector3.Angle(cam.forward, dir);

            if (_angle < closestAngle)
            {
                closestTarget = nearbyTargets[i].transform;
                closestAngle = _angle;
            }
        }

        if (!closestTarget) return null;
        float h1 = closestTarget.GetComponent<CapsuleCollider>().height;
        float h2 = closestTarget.localScale.y;
        float h = h1 * h2;
        float half_h = (h / 2) / 2;
        currentYOffset = h - half_h;
        if (zeroVert_Look && currentYOffset > 1.6f && currentYOffset < 1.6f * 3) currentYOffset = 1.6f;
        Vector3 tarPos = closestTarget.position + new Vector3(0, currentYOffset, 0);
        if (Blocked(tarPos)) return null;
        return closestTarget;
    }

    bool Blocked(Vector3 t)
    {
        RaycastHit hit;
        if (Physics.Linecast(transform.position + Vector3.up * 0.5f, t, out hit))
        {
            if (!hit.transform.CompareTag("Monster")) return true;
        }
        return false;
    }

    bool TargetOnRange()
    {
        float dis = (transform.position - pos).magnitude;
        if (dis > noticeZone) return false; else return true;
    }


    private void TranslateToTarget()
    {
        if (currentTarget == null)
        {
            ResetTarget();
            return;
        }

        //Rotate Towards
        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);

        if(transform.rotation != rot)
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 5f);

        //MoveTowards
        float dis = Vector3.Distance(transform.position, currentTarget.position);
        if (noticeZone == 5 && dis >= 1.6f)
            transform.position = Vector3.Lerp(transform.position, currentTarget.position, Time.deltaTime * 5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, noticeZone);
    }
}
