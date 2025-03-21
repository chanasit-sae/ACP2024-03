using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainmenu : MonoBehaviour{
     public void Gotoscene(string scenename){
        SceneManager.LoadScene(scenename);
     }
     public void QuitApp(){
        Application.Quit();
        Debug.Log("Application has quit.");
     }
}
