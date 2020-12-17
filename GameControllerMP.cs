using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameStateMP
{
    FreeRoam,
    Battle,
    Other
}

public class GameControllerMP : Photon.MonoBehaviour
{
    [SerializeField] movement _movement,_movement1;
    [SerializeField] BattleSystemMP battleSystem;
    [SerializeField] TeamScreen teamScreen;
    [SerializeField] Camera worldCamera;
    [SerializeField] GameObject TeamUI,healbg,startbtn;
    [SerializeField] Top4 top4;
    [SerializeField] trainscreen _trainscreen;
    [SerializeField] touchmove _touchmove;
    [SerializeField] CritPartyMP playerParty,player2Party;
    LevelCrits wildCrit;
    GameStateMP state;
    public GameObject BattleSystemPrefab;
    [SerializeField] Animator img;
    public Text healtext;

    public void newScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    void StartAction()
    {
        _movement.OnEncountered+=StartBattle;
        _movement1.OnEncountered+=StartBattle;
        battleSystem.OnBattleOver+=EndBattle;
        state=GameStateMP.FreeRoam;
    }

    public void StartBattle()
    {  
        PhotonView photonView=PhotonView.Get(this);
        photonView.RPC("StartBtndisable",PhotonTargets.AllBuffered);
        StartCoroutine(battleanim());
        state=GameStateMP.Battle;
    }
    
    [PunRPC]
    void StartBtndisable()
    {
        StartAction();
        PhotonView.Find(3).gameObject.SetActive(false);
    }
 
    //CritPartyMP player2Party,playerParty;
    IEnumerator battleanim()
    {
        yield return new WaitForSeconds(0f);
        PhotonNetwork.Instantiate(BattleSystemPrefab.name, new Vector2(this.transform.position.x,this.transform.position.y),Quaternion.identity,0);
        PhotonView photonView=PhotonView.Get(this);
        
        photonView.RPC("BattleBegin",PhotonTargets.AllBuffered);
            
    }

    [PunRPC]
    void BattleBegin()
    {
        _movement=PhotonView.Find(1001).gameObject.GetComponent<movement>();
        _movement1=PhotonView.Find(2001).gameObject.GetComponent<movement>();
        playerParty=_movement.GetComponent<CritPartyMP>();
        player2Party=_movement1.GetComponent<CritPartyMP>();
        if(PhotonView.Find(1002)!=null)
            PhotonView.Find(1002).gameObject.GetComponent<BattleSystemMP>().StartBattle(playerParty,player2Party);
        else if(PhotonView.Find(2002)!=null)
            PhotonView.Find(2002).gameObject.GetComponent<BattleSystemMP>().StartBattle(playerParty,player2Party);
    }

    IEnumerator NoHealthy()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("NUP");
    }

    public void EnemyCrit(LevelCrits wild)
    {
        state=GameStateMP.Battle;
        wildCrit=wild;
        //StartBattle();
    }

    void EndBattle(bool won)
    {
        Top4SetData();
        state=GameStateMP.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        state=GameStateMP.FreeRoam;
    }

    public void TeamStats()
    {
        state=GameStateMP.Other;
        var playerParty=_movement.GetComponent<CritPartyMP>();
        teamScreen.SetPartyData(playerParty.crits);
        TeamUI.gameObject.SetActive(true);
        Top4SetData();
    }

    public void CloseTeam()
    {
        TeamUI.gameObject.SetActive(false);
        Top4SetData();
        state=GameStateMP.FreeRoam;
    }

    public void TeamStatsNew(List<LevelCrits> playerParty)
    {
        teamScreen.SetPartyData(playerParty);
        TeamUI.gameObject.SetActive(true);
        Top4SetData();
    }

    public void Top4SetData()
    {
        var playerParty=_movement.GetComponent<CritPartyMP>();
        top4.SetPartyData(playerParty.crits);
    }

    public void Heal()
    {
        var playerParty=_movement.GetComponent<CritPartyMP>();
        for(int i=0;i<playerParty.crits.Count;i++)
        {
            playerParty.crits[i].HP=playerParty.crits[i].MaxHP;
        }
    }

    public void TrainScreen()
    {
        state=GameStateMP.Other;
        var playerParty=_movement.GetComponent<CritPartyMP>();
        _trainscreen.SetPartyData(playerParty.crits);
    }

    public void CloseTrainScreen()
    {
        state=GameStateMP.FreeRoam;
    }

    public void SetBattleState()
    {
        state=GameStateMP.Battle;
    }
}
