using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class TurretFunctions : MonoBehaviour
{
    #region Turret variables
    [SerializeField]
    private float speedRotate = 1f;
    [SerializeField]
    private float minRotate = 45f;
    [SerializeField]
    private float maxRotate = 155f;
    [SerializeField]
    private UnityEvent bulletSpawn;
    [SerializeField]
    private GameObject ray;
    #endregion

    private Vector2 beginTouchPosition, endTouchPosition, rotateAmount;
    private ETouch.Finger rotateFinger;

    #region Listeners Methods
    private void OnEnable()
    {
        ETouch.EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandlerFingerDown;
        ETouch.Touch.onFingerUp += HandlerFingerUp;
        ETouch.Touch.onFingerMove += HandlerFingerMove;
    }

    private void OnDisable()
    {

        ETouch.Touch.onFingerDown -= HandlerFingerDown;
        ETouch.Touch.onFingerUp -= HandlerFingerUp;
        ETouch.Touch.onFingerMove -= HandlerFingerMove;
        ETouch.EnhancedTouchSupport.Disable();
        
    }

    private void HandlerFingerMove(ETouch.Finger finger)
    {
        if(finger == rotateFinger)
        {
            ray.SetActive(true);
            ETouch.Touch currentTouch = finger.currentTouch;
            rotateAmount = currentTouch.screenPosition - currentTouch.startScreenPosition;
        }
    }

    private void HandlerFingerUp(ETouch.Finger finger)
    {
        if(finger == rotateFinger)
        {
            endTouchPosition = finger.screenPosition;
            if (beginTouchPosition == endTouchPosition)
            {
                bulletSpawn.Invoke();
            }
            beginTouchPosition = Vector2.zero;
            rotateAmount = Vector2.zero;
            rotateFinger = null;
            ray.SetActive(false);
        }
    }

    private void HandlerFingerDown(ETouch.Finger finger)
    {
        if (!Utils.ClickedOnUi(finger))
        {
            if (rotateFinger == null && finger.screenPosition.x <= Screen.width / 2f)
            {
                beginTouchPosition = finger.screenPosition;
                rotateAmount = Vector2.zero;
                rotateFinger = finger;
            }
        }
        
    }
    #endregion

    private void Update()
    {
        Quaternion rotationY = Quaternion.Euler(0f, -rotateAmount.y * speedRotate * Time.deltaTime, 0f);
        transform.rotation = rotationY * transform.rotation;
        RotationLimit();
    }

    void RotationLimit()
    {
        Vector3 clampRotation = transform.localRotation.eulerAngles;
        clampRotation.y = Mathf.Clamp(clampRotation.y, minRotate, maxRotate);
        transform.rotation = Quaternion.Euler(clampRotation);
    }

}
