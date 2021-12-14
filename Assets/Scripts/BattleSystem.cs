using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYER1TURN, PLAYER2TURN, RESOLVETURN, PLAYER1WIN, PLAYER2WIN }

public class BattleSystem : MonoBehaviour
{
    #region Singleton
    public static BattleSystem instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of BattleSystem found");
            return;
        }
        instance = this;
    }
    #endregion

    #region Variables
    public BattleState state;

    public Text dialogueText;

    public GameObject player1Prefab;
    public GameObject player2Prefab;

    //public Transform player1Spawn;
    //public Transform player2Spawn;

    public PlayerScript player1Script;
    public PlayerScript player2Script;

    public UnitScript[] player1Party;
    public UnitScript[] player2Party;

    public BattleHUD player1HUD;
    public BattleHUD player2HUD;

    public GameObject playerAttacksPanel;
    public GameObject openAttacksButton;

    Move player1ChosenMove;
    Move player2ChosenMove;

    [HideInInspector]
    public bool player1MoveChosen = false;
    [HideInInspector]
    public bool player2MoveChosen = false;

    public GameObject winPanel;
    public GameObject player1WinsText;
    public GameObject player2WinsText;
    #endregion

    void Start()
    {
        player1MoveChosen = false;
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = player1Prefab;
        player1Party = playerGO.GetComponent<PlayerScript>().party;

        GameObject enemyGO = player2Prefab;
        player2Party = enemyGO.GetComponent<PlayerScript>().party;

        dialogueText.text = "The battle begins!";

        player1HUD.SetHUD(player1Party[0]);
        player2HUD.SetHUD(player2Party[0]);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYER1TURN;
        Player1Turn();
    }

    private void Player1Turn()
    {
        player1MoveChosen = false;
        dialogueText.text = "Choose a move, Player 1";
    }

    public void OpenAttacksButton()
    {
        if (state == BattleState.PLAYER1TURN)
        {
            //Add names to moves here
            playerAttacksPanel.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = player1Script.currentPokemon.moves[0].moveName;
            playerAttacksPanel.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = player1Script.currentPokemon.moves[1].moveName;
            playerAttacksPanel.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = player1Script.currentPokemon.moves[2].moveName;
            playerAttacksPanel.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = player1Script.currentPokemon.moves[3].moveName;
            playerAttacksPanel.SetActive(true);
            openAttacksButton.SetActive(false);
        }
        else if (state == BattleState.PLAYER2TURN)
        {
            //Add names to moves here
            playerAttacksPanel.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = player2Script.currentPokemon.moves[0].moveName;
            playerAttacksPanel.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = player2Script.currentPokemon.moves[1].moveName;
            playerAttacksPanel.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = player2Script.currentPokemon.moves[2].moveName;
            playerAttacksPanel.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = player2Script.currentPokemon.moves[3].moveName;
            playerAttacksPanel.SetActive(true);
            openAttacksButton.SetActive(false);
        }
    }

    public void CloseAttacksButton()
    {
        playerAttacksPanel.SetActive(false);
        openAttacksButton.SetActive(true);
    }

    public void OnAttackButton(int button)
    {
        if (state == BattleState.PLAYER1TURN && player1MoveChosen == false)
        {
            player1MoveChosen = true;

            CloseAttacksButton();
            player1ChosenMove = player1Script.currentPokemon.moves[button];

            //player 1 has chosen a move, move on to player 2
            state = BattleState.PLAYER2TURN;
            Player2Turn();
            //IF THINGS ARE FUCKING UP IT MIGHT BE HERE
        }
        else if (state == BattleState.PLAYER2TURN && player2MoveChosen == false)
        {
            player2MoveChosen = true;

            CloseAttacksButton();
            player2ChosenMove = player2Script.currentPokemon.moves[button];

            //player 2 has chosen a move, lets resolve the attacks!
            state = BattleState.RESOLVETURN;
            StartCoroutine(ResolveAttacks());
        }

        //HERE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //StartCoroutine(PlayerAttack(player1Script.currentPokemon.moves[button]));
    }

    private void Player2Turn()
    {
        player2MoveChosen = false;
        dialogueText.text = "Choose a move, Player 2";
    }

    //private void PlayerHeal(Move move)
    //{
    //    player1Party.Heal(move.heal);
    //    player1HUD.SetHP(player1Party.stats.currHP);
    //    dialogueText.text = player1Party.unitName + " heals " + move.heal + " health";
    //}

    IEnumerator ResolveAttacks()
    {
        state = BattleState.RESOLVETURN;

        //if either player swapped pokemon, do that here.

        //choose which pokemon goes first.
        if (player1Script.currentPokemon.stats.speed > player2Script.currentPokemon.stats.speed)
        {
            //if player 1 is faster
            StartCoroutine(Player1Attack(player1ChosenMove));
            yield return new WaitForSeconds(2f);
            if (state == BattleState.RESOLVETURN)
            {
                StartCoroutine(Player2Attack(player2ChosenMove));
                yield return new WaitForSeconds(2f);
            }
        }
        else if (player1Script.currentPokemon.stats.speed > player2Script.currentPokemon.stats.speed)
        {
            //if player 2 is faster
            StartCoroutine(Player2Attack(player2ChosenMove));
            yield return new WaitForSeconds(2f);
            if (state == BattleState.RESOLVETURN)
            {
                StartCoroutine(Player1Attack(player1ChosenMove));
                yield return new WaitForSeconds(2f);
            }
        }
        else
        {
            //if they are tied in speed choose a random one
            int rand = UnityEngine.Random.Range(1, 3);
            if (rand == 1)
            {
                StartCoroutine(Player1Attack(player1ChosenMove));
                yield return new WaitForSeconds(2f);
                if (state == BattleState.RESOLVETURN)
                {
                    StartCoroutine(Player2Attack(player2ChosenMove));
                    yield return new WaitForSeconds(2f);
                }
            }
            else
            {
                StartCoroutine(Player2Attack(player2ChosenMove));
                yield return new WaitForSeconds(2f);
                if (state == BattleState.RESOLVETURN)
                {
                    StartCoroutine(Player1Attack(player1ChosenMove));
                    yield return new WaitForSeconds(2f);
                }
            }
        }

        player1ChosenMove = null;
        player2ChosenMove = null;

        //set the state back to player 1 turn

        yield return new WaitForSeconds(2f);

        if (state != BattleState.PLAYER1WIN || state != BattleState.PLAYER2WIN)
        {
            state = BattleState.PLAYER1TURN;
            Player1Turn();
        }
        else
        {
            print("a");
        }
    }

    IEnumerator Player1Attack(Move move)
    {
        // do attack anim
        bool isDead = false;


        if (move.heal > 0)
        {
            //PlayerHeal(move);
            yield return new WaitForSeconds(2f);
        }

        if (move.damage > 0)
        {
            isDead = player2Script.currentPokemon.TakeDamage(move.damage, move.damageType);
            player2HUD.SetHP(player2Script.currentPokemon.stats.currHP);
            dialogueText.text = player1Script.currentPokemon.unitName + " uses " + move.moveName;
        }

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.PLAYER1WIN;
            dialogueText.text = "Player 1 wins!";
            winPanel.SetActive(true);
            player1WinsText.SetActive(true);
        }
    }

    IEnumerator Player2Attack(Move move)
    {
        // do attack anim
        bool isDead = false;


        if (move.heal > 0)
        {
            //PlayerHeal(move);
            yield return new WaitForSeconds(2f);
        }

        if (move.damage > 0)
        {
            isDead = player1Script.currentPokemon.TakeDamage(move.damage, move.damageType);
            player1HUD.SetHP(player1Script.currentPokemon.stats.currHP);
            dialogueText.text = player2Script.currentPokemon.unitName + " uses " + move.moveName;
        }

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.PLAYER2WIN;
            dialogueText.text = "Player 2 wins!";
            winPanel.SetActive(true);
            player2WinsText.SetActive(true);
        }
    }

    public int CalculateDamageFromPower(Move move, UnitScript attackingUnit, UnitScript attackeeUnit)
    {
        int damage;

        if (move.isPhysical)
        {
            // damage = move.power * attackingUnit
        }
        else
        {

        }
        return 0;
    }

    public IEnumerator ShowSuperEffective()
    {
        yield return new WaitForSeconds(2);
        dialogueText.text = "It's super effective!";
    }

    public IEnumerator ShowNotVeryEffective()
    {
        yield return new WaitForSeconds(2);
        dialogueText.text = "It's not very effective...";
    }

    IEnumerator EnemyAttackAnim(Move move)
    {
        //player2Party.anim.SetBool("isAttacking", true);
        //yield return new WaitForSeconds(0.2f);
        //player2Party.anim.SetBool("isAttacking", false);

        yield return new WaitForSeconds(1f);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}