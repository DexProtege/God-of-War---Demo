using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public static LocationManager instance;

    [SerializeField] GameObject LocationPrefab;
    [SerializeField] GameObject LocationObject;

    [SerializeField] List<string> areaNames = new List<string>();
    [SerializeField] List<Vector3> areaLocations = new List<Vector3>();
    [SerializeField] int areaCount = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LocationObject = Instantiate(LocationPrefab);

        LocationName();
    }

    public void LocationName()
    {
        if (areaCount < areaNames.Count)
        {
            UIManager.instance.LocationPop(areaNames[areaCount]);
            areaCount++;

            if (areaCount < areaLocations.Count)
                LocationObject.transform.localPosition = areaLocations[areaCount];
        }
    }
}
