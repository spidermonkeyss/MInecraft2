using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject playerCamera;

    public Text fpsText;
    public Text lastmeshGenText;

    public float breakTimer;

    public float speed;
    public float topSpeed;
    private float unmodifiedSpeed;
    public float jumpForce = 7;
    public float groundDrag;
    public float miningSpeed = 1;
    public int inventorySize = 16;

    public float placeBlockDistance;

    public GameObject blockOutline;
    public bool hasSelectedBlock;
    public BlockUtils.BlockData selectedBlock;
    public float selectedBlockBreakPercent;
    public Vector3 selectedFaceNormal;
    public Chunk selectedChunk;

    bool doJump = false;
    bool onGround = false;
    public bool isFlying;

    public bool lockCursor;
    public bool isPlayerControlling;
    public bool isCreative;
    public bool isInventoryOpen;
    public Inventory inventory;
    public CraftingInventory craftingInventory;

    int chunkLayerMask;

    void Start()
    {
        inventory = new Inventory(inventorySize, isCreative);
        craftingInventory = new CraftingInventory(4, isCreative);
        GetComponent<Hotbar>().playerInventory = inventory;
        isInventoryOpen = false;
        breakTimer = 0;
        chunkLayerMask = 1 << LayerMask.NameToLayer("Chunk");
        unmodifiedSpeed = speed;

        hasSelectedBlock = false;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (playerCamera == null)
            playerCamera = FindObjectOfType<Camera>().gameObject;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.P))
            SaveLoad.SaveWorldData("worldName");

        if (Input.GetKeyDown(KeyCode.L))
        {
            inventory.AddItemToInventory(typeof(StoneSword));
            inventory.AddItemToInventory(typeof(StonePickaxe));
            inventory.AddItemToInventory(typeof(StoneAxe));
            inventory.AddItemToInventory(typeof(StoneShovel));
            inventory.AddItemToInventory(typeof(CraftingTableItem));
        }

        GroundDrag();
        
        if (!isPlayerControlling)
            return;

        if (Input.GetKeyDown(KeyCode.E))
            ToggleInventory(craftingInventory);

        if (isInventoryOpen)
            return;

        UpdateSelectedChunkAndBlock();

        CameraInput();

        MouseButtonInputs();

        HotbarInput();

        if (isFlying)
            FlyingInput();
        else
            MovementInput();

        if (hasSelectedBlock && selectedBlockBreakPercent >= 100)
            OnBlockBreak();

        fpsText.text = "Frame Time: " + (Time.deltaTime * 1000).ToString();
    }

    void FixedUpdate()
    {
        GetComponent<Rigidbody>().useGravity = !isFlying;

        if (doJump)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
            doJump = false;
        }
    }

    public void ToggleInventory(CraftingInventory _craftingInventory)
    {
        isInventoryOpen = !isInventoryOpen;
        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GetComponent<InventoryUIHandler>().ShowInventory(inventory, _craftingInventory);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GetComponent<InventoryUIHandler>().TurnOffInventory();
        }
    }

    void UpdateSelectedChunkAndBlock()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, placeBlockDistance, chunkLayerMask))
        {
            hasSelectedBlock = true;
            selectedChunk = hit.transform.GetComponent<Chunk>();
            selectedFaceNormal = hit.normal;
            Vector3 worldBlockPos = BlockUtils.GetBlockPosition(hit.point - (hit.normal / 2));
            Vector3 localBlockPos = BlockUtils.Get_LocalChunkBlockPosition(worldBlockPos);
            int index = Chunk.GetIndexOfBlockUsingLocalPosition(localBlockPos);
            
            //Check if selected block has changes
            if (BlockUtils.Get_WorldBlockPosition(selectedBlock.localChunkBlockPosition, selectedChunk.chunkPosition) != worldBlockPos)
            {
                selectedBlock = selectedChunk.blocks[index];
                selectedBlockBreakPercent = 0;
                breakTimer = 0;
            }

            blockOutline.transform.position = BlockUtils.Get_WorldBlockPosition(selectedBlock.localChunkBlockPosition, selectedChunk.chunkPosition);
        }
        else
        {
            hasSelectedBlock = false;
            selectedBlockBreakPercent = 0;
        }

        blockOutline.SetActive(hasSelectedBlock);
    }

    void MouseButtonInputs()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
            OnRightClick();
        if (Input.GetKeyDown(KeyCode.Mouse0))
            OnLeftClick();
        if (Input.GetKey(KeyCode.Mouse1) && !Input.GetKeyDown(KeyCode.Mouse1))
            OnRightClickHeld();
        if (Input.GetKey(KeyCode.Mouse0) && !Input.GetKeyDown(KeyCode.Mouse0))
            OnLeftClickHeld();
        if (Input.GetKeyUp(KeyCode.Mouse1))
            OnRightClickRelease();
        if (Input.GetKeyUp(KeyCode.Mouse0))
            OnLeftClickRelease();
    }

    void CameraInput()
    {
        //Player rotation
        float turnX = Input.GetAxis("Mouse X");
        transform.Rotate(0.0f, turnX, 0.0f);

        //Camera up and down
        float turnY = Input.GetAxis("Mouse Y");
        playerCamera.transform.Rotate(-turnY, 0.0f, 0.0f);
    }

    void GroundDrag()
    {
        //Ground Drag
        if (onGround)
        {
            Vector2 currentHorizontalVelocity = new Vector2(GetComponent<Rigidbody>().velocity.x, GetComponent<Rigidbody>().velocity.z);
            float newMag = currentHorizontalVelocity.magnitude - (groundDrag * Time.deltaTime);
            if (newMag < 0)
                newMag = 0;
            currentHorizontalVelocity = currentHorizontalVelocity.normalized * newMag;
            GetComponent<Rigidbody>().velocity = new Vector3(currentHorizontalVelocity.x, GetComponent<Rigidbody>().velocity.y, currentHorizontalVelocity.y);
        }
    }

    void MovementInput()
    {
        speed = unmodifiedSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= 2;

        //Apply movement forces
        Vector3 newVelocity = new Vector3();

        if (Input.GetKey(KeyCode.W))
            newVelocity += transform.forward;
        if (Input.GetKey(KeyCode.A))
            newVelocity -= transform.right;
        if (Input.GetKey(KeyCode.S))
            newVelocity -= transform.forward;
        if (Input.GetKey(KeyCode.D))
            newVelocity += transform.right;

        newVelocity = speed * Time.deltaTime * newVelocity.normalized;
        newVelocity = GetComponent<Rigidbody>().velocity + newVelocity;

        Vector2 newHorizontalVelocity = new Vector2(newVelocity.x, newVelocity.z);

        if (newHorizontalVelocity.magnitude > topSpeed)
            newHorizontalVelocity = newHorizontalVelocity.normalized * topSpeed;

        GetComponent<Rigidbody>().velocity = new Vector3(newHorizontalVelocity.x, GetComponent<Rigidbody>().velocity.y, newHorizontalVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && onGround)
            doJump = true;
    }

    void FlyingInput()
    {
        speed = unmodifiedSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= 2;

        if (Input.GetKey(KeyCode.W))
            transform.position += speed * Time.deltaTime * playerCamera.transform.forward;
        if (Input.GetKey(KeyCode.A))
            transform.position -= speed * Time.deltaTime * playerCamera.transform.right;
        if (Input.GetKey(KeyCode.S))
            transform.position -= speed * Time.deltaTime * playerCamera.transform.forward;
        if (Input.GetKey(KeyCode.D))
            transform.position += speed * Time.deltaTime * playerCamera.transform.right;
        if (Input.GetKey(KeyCode.Space))
            transform.position += speed * Time.deltaTime * transform.up;
        if (Input.GetKey(KeyCode.LeftControl))
            transform.position -= speed * Time.deltaTime * transform.up;
    }

    void HotbarInput()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            GetComponent<Hotbar>().MoveCurrentSelectionUp();
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            GetComponent<Hotbar>().MoveCurrentSelectionDown();
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                GetComponent<Hotbar>().SetInventorySlot(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                GetComponent<Hotbar>().SetInventorySlot(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                GetComponent<Hotbar>().SetInventorySlot(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                GetComponent<Hotbar>().SetInventorySlot(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                GetComponent<Hotbar>().SetInventorySlot(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                GetComponent<Hotbar>().SetInventorySlot(5);
            else if (Input.GetKeyDown(KeyCode.Alpha7))
                GetComponent<Hotbar>().SetInventorySlot(6);
            else if (Input.GetKeyDown(KeyCode.Alpha8))
                GetComponent<Hotbar>().SetInventorySlot(7);
            else if (Input.GetKeyDown(KeyCode.Alpha9))
                GetComponent<Hotbar>().SetInventorySlot(8);
        }
        
    }

    void OnRightClick()
    {
        //if hand is empty
        if (GetComponent<Hotbar>().GetCurrentSlot().isEmpty)
        {
            if (hasSelectedBlock)
                BlockUtils.GetBlockInstanceFromBlocktype(selectedBlock.blockType).OnRightClick(this.gameObject);
        }
        //item click on a block
        else
        {
            if (hasSelectedBlock)
                GetComponent<Hotbar>().GetCurrentSlot().item.OnRightClick(selectedBlock, selectedFaceNormal, selectedChunk, inventory, GetComponent<Hotbar>().currentSlotIndex);
        }
    }

    void OnLeftClick()
    {
        //if hand is empty
        if (GetComponent<Hotbar>().GetCurrentSlot().isEmpty)
        {

        }
        //item click on a block
        else
        {
            if (hasSelectedBlock)
                GetComponent<Hotbar>().GetCurrentSlot().item.OnLeftClick(selectedBlock, selectedFaceNormal, selectedChunk, inventory, GetComponent<Hotbar>().currentSlotIndex);
        }
    }

    void OnRightClickHeld()
    {
        //if hand is empty
        if (GetComponent<Hotbar>().GetCurrentSlot().isEmpty)
        {
        }
        //item click on a block
        else
        {
        }
    }

    void OnLeftClickHeld()
    {
        float damageAmount = 0;
        
        //if hand is empty
        if (GetComponent<Hotbar>().GetCurrentSlot().isEmpty)
        {
            if (hasSelectedBlock)
            {
                damageAmount = miningSpeed / BlockUtils.GetBlockInstanceFromBlocktype(selectedBlock.blockType).durabilty * 100 * Time.deltaTime;
                DamageSelectedBlock(damageAmount);
            }
        }
        //item click on a block
        else
        {
            if (hasSelectedBlock)
            {
                GetComponent<Hotbar>().GetCurrentSlot().item.OnLeftClickHeld(selectedBlock, selectedFaceNormal, selectedChunk, inventory, GetComponent<Hotbar>().currentSlotIndex);
                damageAmount = miningSpeed / BlockUtils.GetBlockInstanceFromBlocktype(selectedBlock.blockType).durabilty * 100 * Time.deltaTime;
                if (GetComponent<Hotbar>().GetCurrentSlot().item.GetType().IsSubclassOf(typeof(ToolItem)))
                {
                    if (BlockUtils.GetBlockInstanceFromBlocktype(selectedBlock.blockType).fastBreakTool == ((ToolItem)GetComponent<Hotbar>().GetCurrentSlot().item).toolType)
                        damageAmount = ((ToolItem)GetComponent<Hotbar>().GetCurrentSlot().item).miningSpeed / BlockUtils.GetBlockInstanceFromBlocktype(selectedBlock.blockType).durabilty * 100 * Time.deltaTime;
                }
                DamageSelectedBlock(damageAmount);
            }
        }
    }
    
    void OnRightClickRelease()
    {

    }

    void OnLeftClickRelease()
    {
        selectedBlockBreakPercent = 0;
        breakTimer = 0;
    }

    void DamageSelectedBlock(float amount)
    {
        selectedBlockBreakPercent += amount;
        breakTimer += Time.deltaTime;
    }

    void OnBlockBreak()
    {
        Debug.Log("Block break: " + breakTimer);
        breakTimer = 0;
        BlockUtils.BreakBlock(selectedChunk, Chunk.GetIndexOfBlockUsingLocalPosition(selectedBlock.localChunkBlockPosition), BlockUtils.BlockType.Air, GetComponent<Hotbar>().GetCurrentSlot().item);
    }

    void OnCollisionEnter(Collision other)
    {
        onGround = true;
    }

    void OnCollisionStay(Collision other)
    {
        onGround = true;
    }

    void OnCollisionExit(Collision other)
    {
        onGround = false;
    }
}
