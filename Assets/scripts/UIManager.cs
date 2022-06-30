/*UI manager holds all the ui related events which needs to be invoked when
 some UI related activities will happen , by the user  */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    //following are the utillity functions which gets invoked by the ui button press events
    //used for checking the top card value
    [SerializeField]
    TextMeshProUGUI Textfield;
    public void peek()
    {
        Gamemanager.Carddeck_classref.peek();

    }
    //used for suffling the cards so that we always get to choose a different top card 
    public void shuffle()
    {
        Gamemanager.Carddeck_classref.suffle_call();
    }
    //used to discard a card from the deck
    public void pop()
    {
        Gamemanager.Carddeck_classref.pop();
    }
    //used to create and insert the card in stack/deck
    public void push()
    {
        Gamemanager.Carddeck_classref.push();
    }
    public void exit()
    {
        Application.Quit();
    }
    private void Update()
    {
        Textfield.SetText( ((int)(1/Time.unscaledDeltaTime)).ToString());
    }
}
