/*A game manager class is the main entry  point of the application which handles
 initialization and access provider for instances and resources to other classes
 for simplicity sake every other classes will have access to key resources via gamemanager
 So programmer donot have to worry about any other class function details */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Gamemanager : MonoBehaviour
{
    //As the carddeck class gets dynamically attached to the Gamemanager 
    //we needed to have an access point to the   carddeck
    //As it holds UI functionalities which needs to be get called by the Uimanager class.
    public static Carddeck Carddeck_classref = null;
    long frameCount=0;

    public static float FPS;
    // Start is called before the first frame update
    private void Awake()
    {
        //Loading the textures  
        Resourcemanager.getInstance().LoadTexturesfromfolder("Textures");
        //attaching card deck class as component to the gameobject 
        gameObject.AddComponent<Carddeck>();
        //setting refference of  the carddeck class for external refference .e.g. UI manager
        Carddeck_classref = GetComponent<Carddeck>();
    }
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
    }
}
