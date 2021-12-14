using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScript : MonoBehaviour
{

    public string unitName;

    public Types.Type type;
    public Types.Type[] typesWeakTo;
    public Types.Type[] typesStrongAgainst;

    [System.Serializable]
    public struct Stats
    {
        public int maxHP, currHP;
        public int atk, spAtk;
        public int def, spDef;
        public int speed;
    }

    public Stats stats;
    public Move[] moves;

    public int damage;

    private void Start()
    {
        //anim = transform.GetChild(0).GetComponent<Animator>();
    }

    public bool TakeDamage(int dmg, Types.Type type) //returns true if the target dies from the damage
    {
        
       // StartCoroutine(Pause());
        

        if (CheckIfIncomingAttackIsStrong(type))
        {
            //the incoming attack is super effective
            BattleSystem.instance.dialogueText.text = "It's Super Effective!";
            stats.currHP -= dmg * 2;
            StartCoroutine(BattleSystem.instance.ShowSuperEffective());
        }
        else if (CheckIfIncomingAttackIsWeak(type))
        {
            //the incoming attack is not very effective
            stats.currHP -= dmg /2;
            StartCoroutine(BattleSystem.instance.ShowNotVeryEffective());
        }
        else
        {
            //the incoming attack is neutral
            stats.currHP -= dmg;
        }

        if (stats.currHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Heal(int amount)
    {
        stats.currHP += amount;
        if (stats.currHP > stats.maxHP)
        {
            stats.currHP = stats.maxHP;
        }
    }

    IEnumerator Pause()
    {
        yield return new WaitForSeconds(0.2f);
        
    }

    public bool CheckIfIncomingAttackIsStrong(Types.Type type)
    {
        for (int i = 0; i < typesWeakTo.Length; i++)
        {
            if (typesWeakTo[i] == type)
                return true;
        }
        return false;
    }

    public bool CheckIfIncomingAttackIsWeak(Types.Type type)
    {
        for (int i = 0; i < typesStrongAgainst.Length; i++)
        {
            if (typesStrongAgainst[i] == type)
                return true;
        }
        return false;
    }
}


