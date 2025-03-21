using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cellManager : MonoBehaviour
{
    private Transform doorIn, doorOut;
    [SerializeField]public SceneChanger sceneChanger;

    void Start()
    {
        doorIn = transform.Find("doorIn");
        doorOut = transform.Find("doorOut");
    }

    public void disableDoor() { 
        doorIn.gameObject.SetActive(false);
        doorOut.gameObject.SetActive(false);
    }

    public void enableDoor()
    {
        doorIn.gameObject.SetActive(true);
        doorOut.gameObject.SetActive(true);
    }
}
