using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Events;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerTouchMovement : MonoBehaviour
{
    #region Player Variables
    [SerializeField]
    private Transform grabPoint;
    [SerializeField]
    private int life = 3;
    [SerializeField]
    private NavMeshAgent player;
    [SerializeField]
    private float playerSpeed = 5f;
    #endregion

    #region Joystick variables(Tenho que tirar urgente!)
    [SerializeField]
    private Vector2 joystickSize = new Vector2(200, 200);
    [SerializeField]
    private FloatingJoystick joystick;
    #endregion

    #region Data & Control Variables
    [SerializeField]
    private UnityEvent collect;  
    [SerializeField]
    private GameObject pickItem;
    [SerializeField]
    private bool isItemPickUp = false;
    private bool canGrab = false;
    private Finger movementFinger;
    private Vector2 movementAmount, beginTouch;
    private bool firstMove = true;
    [SerializeField]
    private BulletType pickItemType;
    #endregion

    #region Manager Variable (Definitivamente tenho que tirar daqui!)
    [SerializeField]
    private BulletManager bulletManager;
    #endregion

    private void Update()
    {
        Movement();
        ItemIteractions();
    }

    #region Listeners Methods
    private void OnEnable()
    {
        ETouch.EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandlerFingerDown;
        ETouch.Touch.onFingerUp += HandlerLoserFinger;
        ETouch.Touch.onFingerMove += HandlerFingerMove;
    }

    private void OnDisable()
    {

        ETouch.Touch.onFingerDown -= HandlerFingerDown;
        ETouch.Touch.onFingerUp -= HandlerLoserFinger;
        ETouch.Touch.onFingerMove -= HandlerFingerMove;
        ETouch.EnhancedTouchSupport.Disable();
    }

    private void HandlerFingerMove(Finger movedFinger)
    {
        if (movedFinger == movementFinger)
        {
            if(beginTouch != movedFinger.screenPosition)
            {

                joystick.gameObject.SetActive(true);
                Vector2 knobPosition;
                float maxMovement = joystickSize.x / 2;
                ETouch.Touch currentTouch = movedFinger.currentTouch;

                if (Vector2.Distance(currentTouch.screenPosition, joystick.RectTransform.anchoredPosition) > maxMovement)
                {
                    knobPosition = (currentTouch.screenPosition - joystick.RectTransform.anchoredPosition).normalized * maxMovement;
                }
                else
                {
                    knobPosition = currentTouch.screenPosition - joystick.RectTransform.anchoredPosition;
                }
                joystick.knob.anchoredPosition = knobPosition;
                movementAmount = knobPosition / maxMovement;
            }
        }  
    }

    private void HandlerLoserFinger(Finger lostFinger)
    {
        if (lostFinger == movementFinger)
        {
            if(beginTouch == lostFinger.screenPosition)
            {
                PickUpItem();
                if(isItemPickUp)
                    pickItemType = pickItem.GetComponent<trashConfiguration>().ReturnType();
            }
            movementFinger = null;
            joystick.knob.anchoredPosition = Vector2.zero;
            joystick.gameObject.SetActive(false);
            movementAmount = Vector2.zero;
        }
    }

    private void HandlerFingerDown(Finger touchedFinger)
    {
        if (!Utils.ClickedOnUi(touchedFinger))
        {
            if (movementFinger == null && touchedFinger.screenPosition.x >= Screen.width / 2f)
            {
                beginTouch = touchedFinger.screenPosition;
                movementFinger = touchedFinger;
                movementAmount = Vector2.zero;
                if (firstMove)
                {
                    joystick.gameObject.SetActive(true);
                    firstMove = false;
                }
                joystick.RectTransform.sizeDelta = joystickSize;
                joystick.RectTransform.anchoredPosition = ClampStartPosition(touchedFinger.screenPosition);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "collectable":
                if (isItemPickUp != true && !canGrab && pickItem == null)
                {
                    canGrab = true;
                    pickItem = other.gameObject;
                }
                break;
            case "OrganicTrash":
                if (isItemPickUp && pickItemType == BulletType.ORGANIC)
                {
                    cleanPickItem();
                    bulletManager.AddMunition(pickItemType);
                }
                break;
            case "MetalTrash":
                if (isItemPickUp && pickItemType == BulletType.METAL)
                {
                    cleanPickItem();
                    bulletManager.AddMunition(pickItemType);
                }
                break;
            case "PlasticTrash":
                if (isItemPickUp && pickItemType == BulletType.PLASTIC)
                {
                    cleanPickItem();
                    bulletManager.AddMunition(pickItemType);
                }
                break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("collectable"))
        {
            if (isItemPickUp != true && canGrab == true && pickItem != null)
            {
                canGrab = false;
                pickItem = null;
            }
        }
    }

    #endregion

    #region Private Methods
    Vector2 ClampStartPosition(Vector2 startPosition)
    {
        if(startPosition.x < joystickSize.x / 2)
        {
            startPosition.x = joystickSize.x / 2;
        }
        if (startPosition.y < joystickSize.y / 2)
        {
            startPosition.y = joystickSize.y / 2;
        } else if (startPosition.y > Screen.height - joystickSize.y / 2)
        {
            startPosition.y = Screen.height - joystickSize.y / 2;
        }

        return startPosition;
    }
    void ItemIteractions()
    {
        if (pickItem != null && isItemPickUp == true)
        {
            pickItem.transform.position = grabPoint.position;
            pickItem.transform.rotation = grabPoint.transform.rotation;
        }
    }

    void Movement()
    {
        Vector3 scaledMovement = playerSpeed * Time.deltaTime * new Vector3(movementAmount.x, 0, movementAmount.y);
        player.transform.LookAt(player.transform.position + scaledMovement, Vector3.up);
        player.Move(scaledMovement);
    }

    void PickUpItem()
    {
        if (canGrab == true && pickItem != null && isItemPickUp == false)
        {
            isItemPickUp = true;
            pickItemType = pickItem.GetComponent<trashConfiguration>().ReturnType();
            pickItem.GetComponent<BoxCollider>().enabled = false;
        }
    }

    void cleanPickItem()
    {
        Destroy(pickItem);
        pickItem = null;
        isItemPickUp = false;
        canGrab = false;
    }
    #endregion

    public void TakeDamage()
    {
        life--;
        if (life <= 0)
        {
            Destroy(this.gameObject);
        }
    }

}
