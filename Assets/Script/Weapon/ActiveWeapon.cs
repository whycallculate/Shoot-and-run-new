using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActiveWeapon : MonoBehaviour,IPunObservable
{
    [SerializeField] Weapons[] mainWeapon;
    public Weapons mainWeaponObject;
    int setMainIndex;
    public bool mainIsActive;

    [SerializeField] Weapons[] secondaryWeapon;
    public Weapons secondaryWeaponObject;
    int setSecondaryIndex;
    public bool secondaryIsActive;

    [SerializeField] Weapons[] meeleWeapon;

    int[] setIndex;
    int[] getIndex;



    [SerializeField] Transform crossHairTarget;
    [SerializeField] Animator playerAnim;
    [SerializeField] public Transform weaponLeftGrip;
    [SerializeField] public Transform weaponRightGrip;
    public Transform weaponParent;
    public AimState activeAimState;

    public int mainWeaponInt;
    public int lastMainWeaponInt;

    public int secondaryWeaponInt;
    public int lastSecondaryWeaponInt;

    public Animator anim;
    public Animator rigController;



    public PhotonView pw;
    // Start is called before the first frame update
    void Awake()
    {
        pw = GetComponent<PhotonView>();
        setIndex = new int[2];
        getIndex = new int[2];
        if (pw.IsMine)
        {

            anim = GetComponent<Animator>();

            EquipMainWeapon(mainWeaponInt);
            EquipSecondaryWeapon(secondaryWeaponInt);
        }


    }
    private void Update()
    {
        Debug.Log(secondaryIsActive);
        Debug.Log(mainIsActive);


        if (pw.IsMine)
        {

            
            if (mainWeaponObject && secondaryWeaponObject)
            {
                if (mainIsActive && !secondaryIsActive)
                {
                    if (!mainWeaponObject.notShooting)
                    {
                        StartCoroutine(mainWeaponObject.ShootMain());
                    }
                    if (Input.GetKeyDown(KeyCode.R))
                    {

                        StartCoroutine(mainWeaponObject.ReloadOnGame());
                    }
                }
                if (secondaryIsActive && !mainIsActive)
                {
                    if (!secondaryWeaponObject.notShooting)
                    {
                        StartCoroutine(secondaryWeaponObject.ShootSecondary());



                    }
                    if (Input.GetKeyDown(KeyCode.R))
                    {

                        StartCoroutine(secondaryWeaponObject.ReloadOnGame());
                    }
                }
            }

            GetWeaponOnHand();


        }

    }

    public void GetWeaponOnHand()
    {
        if (pw.IsMine)
        {
            if (mainWeaponObject)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) && !mainIsActive && !secondaryIsActive)
                {

                    mainIsActive = true;
                    secondaryIsActive = false;
                    mainWeaponObject.CheckWeapon();
                    rigController.Play("equip" + mainWeaponObject.weaponName);
                    string equipName = "equip" + mainWeaponObject.weaponName;
                    pw.RPC("PlayEquipMainWeapon" ,RpcTarget.OthersBuffered, equipName);

                }
                else if (Input.GetKeyDown(KeyCode.Alpha1) && mainIsActive && !secondaryIsActive)
                {
                    mainIsActive = false;
                    secondaryIsActive = false;
                    mainWeaponObject.CheckWeapon();
                }
            }
            if (secondaryWeaponObject)
            {
                if (Input.GetKeyDown(KeyCode.Alpha2) && !secondaryIsActive && !mainIsActive)
                {
                    mainIsActive = false;
                    secondaryIsActive = true;
                    secondaryWeaponObject.CheckWeapon();

                    rigController.Play("equip" + secondaryWeaponObject.weaponName);
                    string equipName = "equip" + secondaryWeaponObject.weaponName;
                    pw.RPC("PlayEquipSecondaryWeapon", RpcTarget.OthersBuffered, equipName);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2) && secondaryIsActive && !mainIsActive)
                {
                    mainIsActive = false;
                    secondaryIsActive = false;
                    secondaryWeaponObject.CheckWeapon();
                }
            }


        }
        else
        {
            return;
        }
    }
    [PunRPC]
    public void PlayEquipMainWeapon(string i)
    {
        if(!pw.IsMine)
        {
            rigController.Play(i);
        }
    }
    [PunRPC]
    public void PlayEquipSecondaryWeapon(string i)
    {
        if (!pw.IsMine)
        {
            rigController.Play(i);
        }
    }

    public void EquipMainWeapon(int mainWeaponIndex = 0)
    {
        if (pw.IsMine)
        {

            mainWeapon[lastMainWeaponInt].gameObject.SetActive(false);
            mainWeapon[mainWeaponIndex].gameObject.SetActive(true);

            mainWeaponObject = mainWeapon[mainWeaponIndex];
            setMainIndex++;
            rigController.Play("equip" + mainWeapon[mainWeaponIndex].weaponName);
            pw.RPC("EquipMainWeaponOtherPlayer",RpcTarget.OthersBuffered, mainWeaponIndex, lastMainWeaponInt);
            lastMainWeaponInt = mainWeaponIndex;
        }




    }
    [PunRPC]
    public void EquipMainWeaponOtherPlayer(int mainWeaponIndex , int lastindex)
    {
        if (!pw.IsMine)
        {
            mainWeapon[lastindex].gameObject.SetActive(false);
            mainWeapon[mainWeaponIndex].gameObject.SetActive(true);

            rigController.Play("equip" + mainWeapon[mainWeaponIndex].weaponName);
        }



    }
    public void EquipSecondaryWeapon(int secondaryWeaponIndex = 0)
    {
        if (pw.IsMine)
        {

            secondaryWeapon[lastSecondaryWeaponInt].gameObject.SetActive(false);
            secondaryWeapon[secondaryWeaponIndex].gameObject.SetActive(true);

            secondaryWeaponObject = secondaryWeapon[secondaryWeaponIndex];
            setSecondaryIndex++;
            rigController.Play("equip" + secondaryWeapon[secondaryWeaponIndex].weaponName);
            
            pw.RPC("EquipSecondaryWeaponOtherPlayer",RpcTarget.OthersBuffered, secondaryWeaponIndex, lastSecondaryWeaponInt);

        }

    }
    [PunRPC]
    public void EquipSecondaryWeaponOtherPlayer(int secondaryWeaponIndex,int lastIndex)

    {
        if (!pw.IsMine)
        {
            secondaryWeapon[lastIndex].gameObject.SetActive(false);
            secondaryWeapon[secondaryWeaponIndex].gameObject.SetActive(true);

            rigController.Play("equip" + secondaryWeapon[secondaryWeaponIndex].weaponName);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
