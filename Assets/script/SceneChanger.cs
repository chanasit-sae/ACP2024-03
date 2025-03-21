using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private string[] sceneList = { "level1-new", "level2-new", "level3-new", "level4-new", "level5-new" };
    public string sceneToLoad;
    public Vector3 eventCameraPosition;
    public Vector3 offset;
    public float transitionTime = 1.5f;
    public float holdTime = 3f;
    private bool stageChanged = false;
    public int enemyCount;
    public int cellTriggerCnt;
    public float meshEnableTime;
    public counter_UI counter_UI;
    private System.Random random = new System.Random();
    [SerializeField] public gameController gameController;
    [SerializeField] private Transform loaderObject;


    void Start()
    {
        loaderObject.gameObject.SetActive(false);
        loaderObject.GetComponent<MeshRenderer>().enabled = false;
        DisableAllWithName("monster");
        
        loaderObject.GetComponent<MeshRenderer>().enabled = false;
        offset = new Vector3(-1, 3, -5.5f);
        eventCameraPosition = transform.position + offset;

        int index = random.Next(sceneList.Length); // Get random index
        sceneToLoad = sceneList[index];
    }

    void Update()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length + GameObject.FindGameObjectsWithTag("Dashing enemy").Length;
        cellTriggerCnt = GameObject.FindGameObjectsWithTag("cellTrigger").Length;
        if (enemyCount == 0 && cellTriggerCnt == 0 && !stageChanged)
        {
            stageChanged = true;
            Debug.Log("scene changer is working");
            StartCoroutine(EnableMesh());
            StartCoroutine(Camera.main.GetComponent<SmoothCameraFollow>().stageChangingEvent(eventCameraPosition, transitionTime, holdTime));
        }
    }

    public IEnumerator EnableMesh()
    {
        yield return new WaitForSeconds(meshEnableTime);
        loaderObject.gameObject.SetActive(true);
        loaderObject.GetComponent<MeshRenderer>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemyCount == 0 && other.CompareTag("Player"))
        {
            gameController.Instance.stage++;
            SceneManager.LoadScene(sceneToLoad);
            counter_UI.updateText();
        }
    }

    void DisableAllWithName(string name)
    {
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>(); // Find all objects in the scene

        foreach (GameObject obj in objects)
        {
            if (obj.name == name || obj.name == "preset")  // Check if the object's name matches
            {
                obj.SetActive(false);
            }
        }
    }

}
