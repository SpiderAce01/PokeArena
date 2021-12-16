using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public List<UnitScript> party;
    public List<UnitScript> availablePokemon;
    public UnitScript currentPokemon;
    
    public void SelectTeam()
    {
        for (int i = 0; i < party.Count; i++)
        {
            int ran = Random.Range(0, availablePokemon.Count);

            party[i] = availablePokemon[ran];
            availablePokemon.Remove(availablePokemon[ran]);
        }
    }

    //public bool CheckIfAllAreFainted()
    //{
    //    for (int i = 0; i < party.Count; i++)
    //    {
    //        if(party[i].stats.currHP <= 0)
    //        {
    //            return 
    //        }
    //    }
    //}

}
