using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Move", menuName = "ScriptableObjects/Move", order = 1)]
public class Move : ScriptableObject
{
    public string moveName;

    
    public Types.Type damageType;

    [Multiline]
    public string description;

    public bool isPhysical;

    [Tooltip("Power is the base damage of the move; the number displayed to the player.")]
    public int power;
    [Tooltip("Damage is the actual damage the attack deals when used. This is randomised every time it is used.")]
    public int damage;
    public int heal;

    public string GetInfoText()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(description).AppendLine();

        if (damage > 0)
        {
            builder.Append("Deals " + damage + " damage").AppendLine();
        }

        if (heal > 0)
        {
            builder.Append("Heals " + heal + " damage").AppendLine();
        }

        return builder.ToString();
    }
}
