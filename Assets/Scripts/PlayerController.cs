using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] Item[] items;

    [SerializeField] private GameObject playerCamera;

    [Header("Player stats")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float jumpForce;
    [SerializeField] private float smoothTime;

    private float verticalLookRotation;
    private Vector3 smoothMove;
    private Vector3 moveAmount;
    private Rigidbody rb;
    private PhotonView pnView;

    private bool isGrounded;

    private int itemIndex;
    private int prevItemIndex = -1;

    [SerializeField] GameObject cameraHolder;

    private void Awake()
    {
        pnView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if(!pnView.IsMine)
        {
            Destroy(playerCamera);
            Destroy(rb);
        }
        else
        {
            EquipItem(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!pnView.IsMine)
        {
            return;
        }
        Look();
        Movement();
        Jump();
        SelectWeapon();
    }

    private void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -80f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    private void Movement()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMove, smoothTime);
    }

    private void FixedUpdate()
    {
        if(!pnView.IsMine)
        {
            return;
        }
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void GroundState(bool isGrounded)
    {
        this.isGrounded = isGrounded;
    }

    private void CreateController()
    {
        controller = PhotonNetwork.Instantiate(Path.Combine("PlayerController"), Vector3.zero,Quaternion.identity,0, new object[] {pnView.ViewId});
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }

    private void SelectWeapon()
    {
        for(int i = 0; i < items.Length; i++)
        {
            if(Input.GetKeyDown((i+1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }
    }

    private void EquipItem(int index)
    {
        if(index == prevItemIndex) return;
        itemIndex = index;
        items[itemIndex].itemGameObject.SetActive(true);
        if(prevItemIndex != -1)
        {
            ifms[prevItemIndex].itemGameObject.SetActive(false);
        }
        prevItemIndex = itemIndex;

        if(pnView.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("index",itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targerPlayer, Hashtable changedProps)
    {
        if(!pnView.IsMine && targerPlayer == pnView.Owner)
        {
            EquipItem((int)changedProps["index"]);
        }
    }
}
