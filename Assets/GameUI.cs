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
    public GameObject settingMenu;
    public GameObject menuEsc;

    [Header("DeathAndKiller")]
    public GameObject contentKilledByScreen;
    public GameObject contentPrefab;
    public TextMeshProUGUI gameTimer;

    [Header("BuyScreen")]
    public GameObject buyScreen;

    private void Awake()
    {
         pw = GetComponent<PhotonView>();  
    }
    private void Update()
    {
        if(player != null)
        {

            GetPlayerDataOnUI();
            GetInputUI();
        }
    }
    public void GetInputUI()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            OpenAndCloseBuyScreen();

        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscMenu();
        }
    }
    public void EscMenu()
    {
        if(menuEsc.activeSelf == true)
        {
            settingMenu.SetActive(false);
            menuEsc.SetActive(false);

        }
        else
        {
            menuEsc.SetActive(true);

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

    public void OpenAndCloseBuyScreen()
    {
        if (buyScreen.activeSelf == false) 
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            buyScreen.SetActive(true);
        
        }
        else if(buyScreen.activeSelf == true )
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            buyScreen.SetActive(false);
        }
    }

    public void ExitGame()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(1);
    }
    public void OpenSettings()
    {

        if(settingMenu.activeSelf == false)
        {
            settingMenu.SetActive(true);
        }
        else if(settingMenu.activeSelf == true)
        {
            settingMenu.SetActive(false);
        }
    }
    

    public void GetWeaponMainBuyScreen(int main)
    {
        player.transform.GetChild(1).GetComponent<ActiveWeapon>().EquipMainWeapon(main);
    }
    public void GetWeaponSecondaryBuyScreen(int secondary)
    {
        player.transform.GetChild(1).GetComponent<ActiveWeapon>().EquipSecondaryWeapon(secondary);
    }
}
