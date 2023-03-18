using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [SerializeField] GameObject MainQuestPrefab;
    [SerializeField] List<Vector3> QuestPosList = new List<Vector3>();
    [SerializeField] int PosCounter = 0;

    public GameObject QuestObject;
    public bool isQuestActive = false;

    public float distance = 0f;
    public float QuestUIScale= 0f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        QuestObject = Instantiate(MainQuestPrefab);
        isQuestActive = true;

        CheckpointNextLocation();
    }

    private void Update()
    {
        if (isQuestActive)
        {
            distance = Vector3.Distance(QuestObject.transform.position, PlayerController.instance.transform.position);

            if(distance > 20)
            {
                QuestObject.SetActive(false);
                return;
            }
            else
            {
                QuestObject.SetActive(true);
            }

            QuestObject.transform.LookAt(Camera.main.transform);
            QuestObject.transform.localScale = Vector3.one * (Camera.main.transform.position - QuestObject.transform.position).magnitude * QuestUIScale;
        }
        else
        {
            QuestObject.SetActive(false);
            UIManager.instance.QuestWaypoint.SetActive(false);
        }
    }

    public float QuestPosOnCompass()
    {
        float angle = Vector3.SignedAngle(QuestObject.transform.position - Camera.main.transform.position, Camera.main.transform.forward, Vector3.up);
        return -angle * 5f;
    }

    public void CheckpointNextLocation()
    {
        switch(PosCounter)
        {
            case 0:
                UIManager.instance.QuestPop("The Journey", "The snow swamps", "Walk further");                
                break;

            case 1:
                UIManager.instance.QuestPop("The Journey", "Path to the top", "Find Realm Gate");
                break;

            case 2:
                break;
        }

        if (PosCounter < QuestPosList.Count)
            QuestObject.transform.localPosition = QuestPosList[PosCounter];

        PosCounter++;        
    }
}
