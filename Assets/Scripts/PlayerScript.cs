using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public List<UnitScript> party;
    public List<UnitScript> teamSelection;
    public UnitScript currentPokemon;
    
    public Button confirmButton;
    public AddToTeamButton[] selectButtons;

    private void Update()
    {
        for (int i = 0; i < selectButtons.Length; i++)
        {
            selectButtons[i].positionInPartyDisplay.text = (teamSelection.IndexOf(selectButtons[i].pokemon) + 1).ToString();
        }

            if (teamSelection.Count >= 3)
        {
            for (int i = 0; i < selectButtons.Length; i++)
            {
                if (selectButtons[i].pressed)
                    continue;
                else
                    selectButtons[i].GetComponent<Button>().interactable = false;
            }
            confirmButton.interactable = true;
        }
        else
        {
            for (int i = 0; i < selectButtons.Length; i++)
            {
                selectButtons[i].GetComponent<Button>().interactable = true;
            }
                confirmButton.interactable = false;
        }
    }
}
