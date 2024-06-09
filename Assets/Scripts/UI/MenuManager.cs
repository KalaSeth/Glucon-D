using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Unity.Netcode;

public class MenuManager : NetworkBehaviour
{
    [SerializeField] InputField PlayernameInput;
   
    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void LateUpdate()
    {
        
    }


   

    public void ExitGame()
    {
        Application.Quit();
    }
}
