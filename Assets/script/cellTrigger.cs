//using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using static System.TimeZoneInfo;
using static UnityEngine.CullingGroup;

public class cellTrigger : MonoBehaviour
{
    private bool playerEntered = false;
    public bool cellCleared = false;
    [SerializeField] public cellManager cellManager;
    [SerializeField] public SceneChanger changer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerEntered)
        {
            Debug.Log("sss");
            foreach (Transform child in transform)
            {
                if (child.name == "monster" || child.name == "doorIn" || child.name == "doorOut")
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
        StartCoroutine(SetPlayerEnter());
    }

    private IEnumerator SetPlayerEnter() {
        cellManager.enableDoor();
        yield return new WaitForSeconds(0.1f);
        playerEntered = true;
    }

    void Update()
    {
        if (playerEntered && changer.enemyCount == 0)
        {
            gameObject.SetActive(false);
            cellManager.disableDoor();
        }

    }
}
