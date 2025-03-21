using UnityEngine;
using System.Collections;

public class SmoothCameraFollow : MonoBehaviour
{
    #region Variables

    private Vector3 _offset;
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime;
    private Vector3 _currentVelocity = Vector3.zero;

    public bool isStageChanging = false;
    public bool isPlayerDying = false;
    private Vector3 eventTargetPosition;
    private float eventSmoothTime;

    #endregion

    #region Unity callbacks

    private void Awake() => _offset = transform.position - target.position;

    private void LateUpdate()
    {
        if (isStageChanging)
        {
            transform.position = Vector3.SmoothDamp(transform.position, eventTargetPosition, ref _currentVelocity, eventSmoothTime);
        }
        else if (isPlayerDying) 
        {
            Vector3 zoomTarget = target.position + _offset * 0.5f;
            transform.position = Vector3.SmoothDamp(transform.position, zoomTarget, ref _currentVelocity, smoothTime);
        }
        else
        {
            Vector3 targetPosition = target.position + _offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);
        }
    }

    #endregion

    #region Public Methods

    // Call this method to move the camera to a specific position for an event
    public void MoveCameraToEvent(Vector3 newPosition, float duration)
    {
        Debug.Log("moveTo is working!");
        eventTargetPosition = newPosition;
        eventSmoothTime = duration;
    }

    // Call this method to reset the camera back to following the player
    public void ResetCamera()
    {
        Debug.Log("Reset is working!");

        isStageChanging = false;
    }

    // Coroutine to move camera for a set duration and then return to normal
    public IEnumerator stageChangingEvent(Vector3 newPosition, float duration, float holdTime)
    {
        Debug.Log("IEnum is working!");
        isStageChanging = true;
        MoveCameraToEvent(newPosition, duration);
        yield return new WaitForSeconds(holdTime);
        ResetCamera();
    }

    #endregion
}