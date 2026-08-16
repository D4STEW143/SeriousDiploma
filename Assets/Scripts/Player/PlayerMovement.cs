using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement attributes")]
    [SerializeField]private float _moveSpeed = 5f;
    [SerializeField]private float _jumpHeight = 1f;

    [Header("Look attributes")]
    [SerializeField, Range(0.1f, 10f)]private float _sensativity = 10f;
    [SerializeField, Range(-90f, -50f)]private float _minVertical = -65f;
    [SerializeField, Range(50f, 90f)]private float _maxVertical = 65f;

    [Header("Weapon sway and bobbing")]
    [SerializeField]private GameObject _weaponHolder;
    [SerializeField]private float _swayMultipler;
    [SerializeField]private float _smooth;
    private float _mouseSwayX;
    private float _mouseSwayY;

    private CharacterController _characterController;

    private Vector2 _inputMovement;
    private Vector3 _velocity;
    private Vector2 _inputMouse;
    private float _xRot = 0f;
    private float _yRot = 0f;
    private float _gravity = -9.8f;

    public static event Action<float> PlayerSpeed;
    private float _speedForAnimator;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMouse();
        HandleMovement();

        UpdatePlayerSpeedEvent();
    }

    private void HandleMovement()
    {
        Vector3 _charForward = transform.forward;
        Vector3 _charRight = transform.right;

        _charForward.y = 0f;
        _charRight.y = 0f;

        _charForward.Normalize();
        _charRight.Normalize();

        Vector3 _horizontalMove = (_charForward * _inputMovement.y) + (_charRight * _inputMovement.x);
        _velocity.y += _gravity * Time.deltaTime;
        Vector3 _totalMove = _horizontalMove * _moveSpeed + new Vector3(0f, _velocity.y, 0f);
        _characterController.Move(_totalMove * Time.deltaTime);
    }

    private void UpdatePlayerSpeedEvent()
    {
        Vector3 horizontalVelocity = _characterController.velocity;
        horizontalVelocity.y = 0f; 

        float currentSpeed = horizontalVelocity.magnitude;
        float normalizedSpeed = currentSpeed / _moveSpeed;
        normalizedSpeed = Mathf.Clamp01(normalizedSpeed);
        _speedForAnimator = normalizedSpeed;
        PlayerSpeed?.Invoke(normalizedSpeed);
    }

    private void HandleMouse()
    {
        float mouseX = _inputMouse.x * _sensativity * Time.deltaTime;
        float mouseY = _inputMouse.y * _sensativity * Time.deltaTime;
        _xRot -= mouseY;
        _xRot = Mathf.Clamp(_xRot, _minVertical, _maxVertical);
        _yRot += mouseX;
        transform.localEulerAngles = new Vector3(_xRot, _yRot,0f);
    }

    private void WeaponSway()
    {
        _mouseSwayX = _inputMouse.x * _swayMultipler;
        _mouseSwayY = _inputMouse.y * _swayMultipler;

        Quaternion _rotX = Quaternion.AngleAxis(-_mouseSwayY, Vector3.right);
        Quaternion _rotY = Quaternion.AngleAxis(_mouseSwayX, Vector3.up);

        Quaternion targetRot = _rotX * _rotY;

        _weaponHolder.transform.localRotation = Quaternion.Slerp(_weaponHolder.transform.localRotation, targetRot, _smooth * Time.deltaTime);
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        _inputMovement = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _inputMouse = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (_characterController.isGrounded)
        {
            _velocity.y = _jumpHeight;
        }
    }
}
