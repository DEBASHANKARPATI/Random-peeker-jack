/*Most of the core fuctionality of the game is implemented here*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Carddeck : MonoBehaviour
{
    GameObject Cardprefab = null;
    //why array ?
    //As we have a limited number of cards which are countable , it will be better to store them in contigious data
    //structure like array to benifite faster access as accessing from an array have Big-O complexity of O(1)
    //Hence i have implemented the stack with array instead of a linked list . which would have extra overhead
    // and linear access time
    GameObject[] Cards;
    //Top is a pointer which points to the top elemet of the stack
    short Top = -1;
    //this is a empty gameobject used for sorting the transform hirarchy in the editor 
    Transform Deck = null;
    //we use a hashset to avoid duplicate instantiation of element 
    //by checking the stack which takes O(log n) time for checking and faster then any linear data structure as list
    //best suited for checking and avoid duplicate instatiation.
    HashSet<int> spawnedCardsno;
    //We have used enum here for easier access of the keys from the dictionary as the dictionary keys are
    //nothing but the enumurated strings , we can access the dictionary using the enum->string convertion
    //
    //
    enum CardNames : short { ACE, SECOND, THIRD, FOURTH, FIFTH, SIXTH, SEVENTH, EIGTH, NINTH, TENTH, JACK, QUEEN, KING };
    CardNames m_cardnames;
    void Awake()
    {
        Cards = new GameObject[13];
        spawnedCardsno = new HashSet<int>();
        Cardprefab = Resources.Load<GameObject>("Prefab/Card") as GameObject;
        Deck = GameObject.Find("Deck").transform;
        Debug.Log(Cardprefab.name);
    }
    //private void Update()
    //{
    //    //if (Input.GetKeyUp(KeyCode.A))
    //    //    push();
    //    //if (Input.GetKeyUp(KeyCode.S)) 
    //    //    pop();
    //    //if (Input.GetKeyUp(KeyCode.D))
    //    //    peek();
    //    //if (Input.GetKeyUp(KeyCode.W))
    //    //    StartCoroutine("shuffle");
    //}
    public void push()
    {
        
        if (Top < 12)
        {
            Top += 1;
            //instantiate the card and give appropriate offset
            GameObject card = Instantiate(Cardprefab, Deck, false) as GameObject;
            card.transform.localRotation = Quaternion.Euler(0.0f,0.0f,180.0f);
            card.transform.position = card.transform.position + new Vector3(0.75f*Top, Top*1.0f,-Top*0.75f);


            string cardname_number = getRandomcardNo();
            //here we are separating the texture name and numeric representation
            // do that we can check for duplication using the number in pop method,
            // we are setting this as our object name for ease, ,  and apply the respective texture from 
            // the name as a dictionary key
            string texture_name = cardname_number.Substring(0,cardname_number.IndexOf("/"));
            string texture_number= cardname_number.Substring(cardname_number.IndexOf("/")+1,
                cardname_number.Length- cardname_number.IndexOf("/")-1);


            //Debug.Log("texture name:" + texture_name);
            //Debug.Log("texture number:" + texture_number);
            Texture2D front_texture = getTextureForCard(texture_name);
            card.name = texture_number;
            //adding textures to the card game object
            addTexturesTofrontAndBack(card, front_texture);
            //finally adding the card to the top of the stack
            Cards[Top] = card;
        }
        else
        {
            Debug.Log("Deck is filled");
        }
    }
    //used to discard a card from the deck
    public void pop()
    {
        
        if(Top>=0)
        {
            //Debug.Log(Cards[Top].name);
            int index=int.Parse(Cards[Top].name);
            spawnedCardsno.Remove(index);
            Destroy(Cards[Top]);
            Top -= 1;
        }
        else
        {
            Debug.Log("Deck is empty");
        }
    }
    //following are the utillity functions which gets invoked by the ui button press events
    //used for checking the top card value
    public void peek()
    {

    
        Debug.Log("Peek top");
        if (Top > -1)
        {
            GameObject card = Cards[Top];
            card.transform.Rotate(new Vector3(0.0f,0.0f, 180.0f));
        }
        else
        {
            Debug.Log("Please Add Cards to the deck");
        }
    }
    private IEnumerator shuffle()
    {//Why corutine?
     //As we have multiple cards and we want to show an animating suffling 
     //which can take more than 1 frame to complete , we are prefering corutine 
     //it enables  us to distribute the task accross frames while not haulting the frame update

        for(int i=0;i<Cards.Length;i++)
        {
            //for each iteration of card array we are going to 
            //generate two random indexes and swap the positions
            int endrange = Mathf.Min(Top, 12);
            int randomIndex1=Random.Range(0, endrange+1);
            int randomIndex2 = Random.Range(0, endrange+1);
            if (randomIndex1 != randomIndex2)
            {
                //if the two random indexes are not simmilar lets linearly interpolate the 
                //first card with seconds positions

                //resetting the rotation to show back of the array while suffling
                Cards[randomIndex1].transform.localRotation = Quaternion.AngleAxis(180.0f, Vector3.forward);
                Cards[randomIndex2].transform.localRotation = Quaternion.AngleAxis(180.0f, Vector3.forward);
                //interpolating between the two random card positions
                float alpha = 0.01f;
                Vector3 position=Cards[randomIndex1].transform.localPosition;
                while (Vector3.Distance(Cards[randomIndex1].transform.localPosition,
                    Cards[randomIndex2].transform.localPosition)>=0.01f)
                {
                    Cards[randomIndex1].transform.localPosition =
                        Vector3.Lerp(Cards[randomIndex1].transform.localPosition,
                        Cards[randomIndex2].transform.localPosition, alpha);
                    alpha += 0.01f;
                    yield return null;
                }
                //finally setting the 2card's position to the first cards position 
                //we can also implement this  using a linear interpolation but we don't want to eat up too much 
                //of time for suffling
                //hence i am directly setting the position of 2nd random card to previously stored 
                //first cards position
                Cards[randomIndex2].transform.localPosition = position;
                //Also do not forget to swap the cards in the acttual stack as we have to use the stack data structure
                //if we donot swap them it will cause the indexes of the cards stay same
                // which can result unordered deletion /pop operation
                GameObject temp = Cards[randomIndex1];
                Cards[randomIndex1] = Cards[randomIndex2];
                Cards[randomIndex2] = temp;
            }
        }
    }
    private Texture2D getTextureForCard(string Name)
    {
        //accessing a texture2d by a string key generated from the enumareted string
        return Resourcemanager.getInstance().getTexture(Name);
    }
    private void addTexturesTofrontAndBack(GameObject card, Texture2D texture)
    {
        //we are dynamically adding the textures to our own shader created materials 
        //as we have different kinds of textures to pass and functionalities to show 
        //such as creating a glowing  red outline
        card.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
        card.transform.GetChild(1).GetComponent<MeshRenderer>().material.
            SetTexture("_MainTex", getTextureForCard("back"));
        card.transform.GetChild(1).GetComponent<MeshRenderer>().material.
            SetTexture("_OutlineTex", getTextureForCard("outline"));
      
    }
    private string getRandomcardNo()
    {
        //we can create the keys for the texture resource cache using the random numbers 
        //But you see they are just integres  then how do we get appropriate texture using an integre where the 
        // dictionary key is a string
        // Now as the Enum has a functionality to get converted to a string 
        // we leverage that power to generate string keys from random numbers
        //and also the nomencleture of our card textures are enumerated Like{Ace,Second,third.....,queen,,jack,king}
        //we are able to covert the integres into string keys and using those we are accessing the appropriate 
        // textures.
        string m_cardnames;
        int card_no = Random.Range(0, 13);
        //It's possible that we get same random numbers multiple times , as our implementation 
        //focuses on unique cards in the deck we only create unique numbers
        //hence we use a hash map named spawncardsno
        while(spawnedCardsno.Contains(card_no))
        {
            card_no = Random.Range(0, 13);
        }
        spawnedCardsno.Add(card_no);
        m_cardnames =((CardNames)card_no).ToString();
        return  m_cardnames+"/"+card_no.ToString();
        //why i am returning a string+integr part?
        // the integre part is used to give names to the game objects via which we can know 
        // which card is holding what texture front in it
        //this helps us while poping the cards and adding them back 
        //as we donot want any duplicates we need to remove the indexes from hashset too
        //which is only possible if we have a integre part 
        //this can be done using a little string manipulation
        //on our end
    }
    //used for suffling the cards so that we always get to choose a different top card 
    //As a coroutine can not directly be invoked by a UI event 
    //we made a wrapper function instead which is refferenced by the UImanager which intern listens to the button 
    //click event.
    public void suffle_call()
    {
        StartCoroutine("shuffle");
    }
    
}