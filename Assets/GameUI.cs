using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class GameUI : MonoBehaviour
{

    private static GameUI instance;
    public static GameUI Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<GameUI>();
            }
            return instance;
        }
    }

    PhotonView pw;


    [Header("PlayerInfo")]
    public GameObject player;
    public TextMeshProUGUI pmSpeed;
    public TextMeshProUGUI pmHealth;
    public TextMeshProUGUI pmArmor;
    public GameObject pmAmmoMain;
    public GameObject pmAmmoSecondary;

    [Header("Canvas")]
    public GameObject waitingPlayer;
    public GameObject gameElements;
    public GameObject timer;
    public GameObject crosshair;

    [Header("DeathAndKiller")]
    public GameObject contentKilledByScreen;
    public GameObject contentPrefab;

    public TextMeshProUGUI gameTimer;

    private void Awake()
    {
         pw = GetComponent<PhotonView>();  
    }
    private void Update()
    {
        if(player != null)
        {

            GetPlayerDataOnUI();
        }
    }

    public void GameWaitingPlayers()
    {
        waitingPlayer.SetActive(true);
        gameElements.SetActive(false);
        timer.SetActive(false);
        crosshair.SetActive(false);

    }
    public void GameOnUI()
    {
        waitingPlayer.SetActive(false);
        gameElements.SetActive(true);
        timer.SetActive(true);
        crosshair.SetActive(true);
    }
    private void GetPlayerDataOnUI()
    {

        pmSpeed.text = player.transform.GetChild(1).GetComponent<Rigidbody>().velocity.magnitude.ToString();
        pmHealth.text = player.transform.GetChild(1).GetComponent<PlayerManager>().currentHP.ToString();
        pmArmor.text = player.transform.GetChild(1).GetComponent<PlayerManager>().currentArmor.ToString();
        if (player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainIsActive)
        {
            pmAmmoMain.GetComponent<TextMeshProUGUI>().text = player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainWeaponObject.weapon.currentAmmo.ToString() + " / " + player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainWeaponObject.weapon.totalAmmo.ToString();
        }
        else if (player.transform.GetChild(1).GetComponent<ActiveWeapon>().secondaryIsActive)
        {
            pmAmmoMain.GetComponent<TextMeshProUGUI>().text = player.transform.GetChild(1).GetComponent<ActiveWeapon>().secondaryWeaponObject.weapon.currentAmmo.ToString() + " / " + player.transform.GetChild(1).GetComponent<ActiveWeapon>().secondaryWeaponObject.weapon.totalAmmo.ToString();
        }
    }
    public void KilledByScreenRPC(string killer,string victim)
    {
        pw.RPC("KilledByScreen",RpcTarget.All,killer,victim);
    }

    [PunRPC]
    public void KilledByScreen(string killer,string victim)
    {
        var killedBy = Instantiate(contentPrefab, contentKilledByScreen.transform);

        killedBy.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = killer;
        killedBy.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = victim;
        Destroy(killedBy, 3f);
        
    }
    public void GetPlayerData(GameObject player)
    {
        this.player = player;
    }
}
