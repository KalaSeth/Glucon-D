using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    [SerializeField] int Loadtime;
    [SerializeField] GameObject OfflinePanel;

    // Update is called once per frame
    void Update()
    {
        if (NetworkChecker.instance.isOnline == true)
        {
            OfflinePanel.SetActive(false);
            Invoke("SwitchScene", Loadtime);
        }else if (NetworkChecker.instance.isOnline == false)
        {
            if (OfflinePanel.activeInHierarchy == false)
            {
                OfflinePanel.SetActive(true);
            }
        }
    }

    void SwitchScene()
    {
        SceneManager.LoadScene(2);
    }
}
