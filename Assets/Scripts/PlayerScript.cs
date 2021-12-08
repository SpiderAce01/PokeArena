using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Range(1, 2)]
    public int playerNumber = 1;

    public UnitScript[] party;
    public UnitScript currentPokemon;
}
