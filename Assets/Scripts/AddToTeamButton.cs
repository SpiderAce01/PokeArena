using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddToTeamButton : MonoBehaviour
{
    public PlayerScript player;
    public UnitScript pokemon;
    public bool pressed = false;
    public Text positionInPartyDisplay;

    public void OnSelectPokemon()
    {
        pressed = !pressed;
        positionInPartyDisplay.enabled = pressed;

        if (pressed)
        {
            player.teamSelection.Add(pokemon);
        }
        else
        {
            player.teamSelection.Remove(pokemon);
        }

    }
}
