/*
 A resource manager chaches all the resources which will be frequently get used the game ,
 which is a singleton class and only have one globla/static instance for secure access ,  and unintended
 creation of objects.
 All classes which require resources can rely on this class which ensures singular dependency ,hirarchy.
 
 */



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resourcemanager
{

    // Start is called before the first frame update
    //A dictionary which will cache all the Texture2ds required frequently by our card meshes
    private Dictionary<string, Texture2D> m_textureMaps=new Dictionary<string, Texture2D>();

    //A singleton class will have a private constructor which ensures the singularity of an instance

    private Resourcemanager() { }

    //A global static instance 

    private static Resourcemanager Instance = null;

    //function  which enables us to access the singular instance
    public static Resourcemanager getInstance()
    {
        if (Instance == null)
        {
            Instance = new Resourcemanager();
        }
        return Instance;
    }
    //An utility function which loads the textures into our cache (dictionary)
    //refferenced by Gamemanagers awake function
    public void LoadTexturesfromfolder(string path)
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>(path);
        foreach (Texture2D texture in textures)
        {
            //why lower case key?
            //Complete lower case/Upper case key ensures we stay case insensitive during access 
            m_textureMaps[texture.name.ToLower()] = texture;
        }
    }
    //utility function returns a Texture2D from dictionary
    public Texture2D getTexture(string cardname)
    {
        Texture2D defaultValue = null;
        if (m_textureMaps.TryGetValue(cardname.ToLower(), out defaultValue))
        {
            return defaultValue;
        }
        Debug.Log("Invalid card name");
        return null;
    }

}
