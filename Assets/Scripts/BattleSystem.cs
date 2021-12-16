using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleState { TEAMSELECTION, SETUPBATTLE, PLAYERTURN, ENEMYTURN, RESOLVETURN, PLAYERWIN, ENEMYWIN }

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

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerSpawn;
    public Transform enemySpawn;

    public PlayerScript playerScript;
    public EnemyScript enemyScript;

    public List<UnitScript> playerParty;
    public List<UnitScript> enemyParty;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public GameObject playerAttacksPanel;
    public GameObject openAttacksButton;
    public GameObject openSwapPokemonButton;

    Move playerChosenMove;
    Move enemyChosenMove;

    [HideInInspector]
    public bool playerMoveChosen = false;
    [HideInInspector]
    public bool enemyMoveChosen = false;
    [HideInInspector]
    public bool teamChosen = false;

    public GameObject winPanel;
    public GameObject playerWinsText;
    public GameObject enemyWinsText;

    public GameObject teamSelectPanel;
    public GameObject swapPokemonPanel;
    public Button[] swapPokemonButtons;
    #endregion

    void Start()
    {
        playerMoveChosen = false;
        state = BattleState.TEAMSELECTION;
        PlayerTeamSelect();
    }

    private void PlayerTeamSelect()
    {
        teamSelectPanel.SetActive(true);
    }

    public void ConfirmTeam()
    {
        playerScript.party = playerScript.teamSelection;
        playerScript.currentPokemon = playerScript.party[0];
        ChooseEnemyTeam();
    }

    public void ChooseEnemyTeam()
    {
        enemyScript.SelectTeam();
        enemyScript.currentPokemon = enemyScript.party[0];
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        state = BattleState.SETUPBATTLE;
        teamSelectPanel.SetActive(false);
        GameObject playerGO = playerPrefab;
        playerParty = playerGO.GetComponent<PlayerScript>().party;

        GameObject enemyGO = enemyPrefab;
        enemyParty = enemyGO.GetComponent<EnemyScript>().party;

        dialogueText.text = "The battle begins!";

        SendOutPlayerPokemon();
        SendOutEnemyPokemon();

        foreach (UnitScript unitScript in playerParty)
        {
            unitScript.stats.currHP = unitScript.stats.maxHP;
        }

        foreach (UnitScript unitScript in enemyParty)
        {
            unitScript.stats.currHP = unitScript.stats.maxHP;
        }

        playerHUD.SetHUD(playerParty[0]);
        enemyHUD.SetHUD(enemyParty[0]);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    public void SendOutPlayerPokemon()
    {
        for (int i = 1; i < playerScript.party.Count; i++)
        {
            playerScript.party[i] = Instantiate(playerScript.party[i], playerSpawn.transform.position, Quaternion.identity, playerSpawn.transform);
            playerScript.party[i].gameObject.SetActive(false);
        }
        playerScript.party[0] = Instantiate(playerScript.currentPokemon, playerSpawn.transform.position, Quaternion.identity, playerSpawn.transform);
        playerScript.currentPokemon = playerScript.party[0];
    }

    public void SendOutEnemyPokemon()
    {
        for (int i = 1; i < enemyScript.party.Count; i++)
        {
            enemyScript.party[i] = Instantiate(enemyScript.party[i], enemySpawn.transform.position, Quaternion.identity, enemySpawn.transform);
            enemyScript.party[i].gameObject.SetActive(false);
        }
        enemyScript.currentPokemon = Instantiate(enemyScript.currentPokemon, enemySpawn.transform.position, Quaternion.identity, enemySpawn.transform);
    }

    private void PlayerTurn()
    {
        playerMoveChosen = false;
        dialogueText.text = "Choose a move, Player 1";
        openAttacksButton.GetComponent<Button>().interactable = true;
        openSwapPokemonButton.GetComponent<Button>().interactable = true;
    }

    public void OpenAttacksButton()
    {
        if (state == BattleState.PLAYERTURN)
        {
            //Add names to moves here
            playerAttacksPanel.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = playerScript.currentPokemon.moves[0].moveName;
            playerAttacksPanel.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = playerScript.currentPokemon.moves[1].moveName;
            playerAttacksPanel.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = playerScript.currentPokemon.moves[2].moveName;
            playerAttacksPanel.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = playerScript.currentPokemon.moves[3].moveName;
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
        if (state == BattleState.PLAYERTURN && playerMoveChosen == false)
        {
            playerMoveChosen = true;

            CloseAttacksButton();
            playerChosenMove = playerScript.currentPokemon.moves[button];

            //player 1 has chosen a move, move on to enemy
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }
    }

    private void EnemyTurn()
    {
        enemyMoveChosen = false;
        //CHOOSE A MOVE
        ChooseEnemyMove();
        state = BattleState.RESOLVETURN;
        StartCoroutine(ResolveAttacks());
    }

    private void ChooseEnemyMove()
    {
        int ran = Random.Range(0, 4);
        enemyChosenMove = enemyScript.currentPokemon.moves[ran];
        enemyMoveChosen = true;
    }

    IEnumerator ResolveAttacks()
    {
        state = BattleState.RESOLVETURN;
        openAttacksButton.GetComponent<Button>().interactable = false;
        openSwapPokemonButton.GetComponent<Button>().interactable = false;
        //if either player swapped pokemon, do that here.

        #region choose which pokemon goes first.
        if (playerScript.currentPokemon.stats.speed > enemyScript.currentPokemon.stats.speed)
        {
            //if player 1 is faster
            StartCoroutine(PlayerAttack(playerChosenMove));
            yield return new WaitForSeconds(2f);
            if (state == BattleState.RESOLVETURN)
            {
                StartCoroutine(EnemyAttack(enemyChosenMove));
                yield return new WaitForSeconds(2f);
            }
        }
        else if (playerScript.currentPokemon.stats.speed > enemyScript.currentPokemon.stats.speed)
        {
            //if player 2 is faster
            StartCoroutine(EnemyAttack(enemyChosenMove));
            yield return new WaitForSeconds(2f);
            if (state == BattleState.RESOLVETURN)
            {
                StartCoroutine(PlayerAttack(playerChosenMove));
                yield return new WaitForSeconds(2f);
            }
        }
        else
        {
            //if they are tied in speed choose a random one
            int rand = UnityEngine.Random.Range(1, 3);
            if (rand == 1)
            {
                StartCoroutine(PlayerAttack(playerChosenMove));
                yield return new WaitForSeconds(2f);
                if (state == BattleState.RESOLVETURN)
                {
                    StartCoroutine(EnemyAttack(enemyChosenMove));
                    yield return new WaitForSeconds(2f);
                }
            }
            else
            {
                StartCoroutine(EnemyAttack(enemyChosenMove));
                yield return new WaitForSeconds(2f);
                if (state == BattleState.RESOLVETURN)
                {
                    StartCoroutine(PlayerAttack(playerChosenMove));
                    yield return new WaitForSeconds(2f);
                }
            }
        }
        #endregion

        playerChosenMove = null;
        enemyChosenMove = null;

        //set the state back to player 1 turn

        yield return new WaitForSeconds(2f);

        if (state != BattleState.PLAYERWIN || state != BattleState.ENEMYWIN)
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    IEnumerator PlayerAttack(Move move)
    {
        // do attack anim
        bool isDead = false;
        
        if (move.damage > 0)
        {
            dialogueText.text = playerScript.currentPokemon.unitName + " uses " + move.moveName;
            yield return new WaitForSeconds(2f);
            isDead = enemyScript.currentPokemon.TakeDamage(move.damage, move.damageType);
            enemyHUD.SetHP(enemyScript.currentPokemon.stats.currHP);
        }

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.PLAYERWIN;
            dialogueText.text = "Player 1 wins!";
            winPanel.SetActive(true);
            playerWinsText.SetActive(true);
        }
    }

    IEnumerator EnemyAttack(Move move)
    {
        // do attack anim
        bool isDead = false;
        
        if (move.damage > 0)
        {
            dialogueText.text = "The opposing " + enemyScript.currentPokemon.unitName + " uses " + move.moveName;
            yield return new WaitForSeconds(2f);
            isDead = playerScript.currentPokemon.TakeDamage(move.damage, move.damageType);
            playerHUD.SetHP(playerScript.currentPokemon.stats.currHP);
        }

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            playerScript.faintedPokemon.Add(playerScript.currentPokemon);
            if (playerScript.faintedPokemon.Count >= 3)
            {
                state = BattleState.ENEMYWIN;
                dialogueText.text = "Player 2 wins!";
                winPanel.SetActive(true);
                enemyWinsText.SetActive(true);
            }
            else
            {
                playerScript.currentPokemon.gameObject.SetActive(false);
                OnSwapPokemon();
            }
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

    public void OnSwapPokemon()
    {
        for (int i = 0; i < swapPokemonButtons.Length; i++)
        {
            swapPokemonButtons[i].GetComponent<Image>().sprite = playerScript.party[i].displayImg;
            if(playerScript.party[i].stats.currHP <=0)
                swapPokemonButtons[i].interactable = false;
        }
        swapPokemonPanel.SetActive(true);
    }

    public void OnCloseSwapPanel()
    {
        swapPokemonPanel.SetActive(false);
    }

    public void SelectPokemonButton(int pokemonToSendOut)
    {
        playerScript.currentPokemon.gameObject.SetActive(false);
        playerScript.currentPokemon = playerScript.party[pokemonToSendOut];
        playerScript.currentPokemon.gameObject.SetActive(true);
        swapPokemonPanel.SetActive(false);
        playerHUD.SetHUD(playerScript.currentPokemon);
    }
}