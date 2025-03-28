using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CarControler : MonoBehaviour
{

    [SerializeField]
    private string _hAxisInputName = "Horizontal", _accelerateInputName = "Accelerate";
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private Rigidbody _rb;

    private float _speed, _accelerationLerpInterpolator, _rotationInput; 
    [SerializeField]
    private float _speedMaxBasic = 3, _speedMaxTurbo = 20, _accelerationFactor, _rotationSpeed = 0.5f, _maxAngle =360;
    private bool _isAccelerating, _isBoosting;
    private float _groundSpeedVariator;

    [SerializeField]
    public Transform _carColliderAndMesh;
    [SerializeField]
    private AnimationCurve _accelerationCurve;


    public void Boost() //permet de boost si y a pas de boost en cours
    {
        if (!_isBoosting)
        {
            StartCoroutine(Boosting());
        }
    }

    private IEnumerator Boosting()
    {
        _isBoosting = true;
        yield return new WaitForSeconds(3);
        _isBoosting = false;
    }
    void Update()
    {


        _rotationInput = Input.GetAxis(_hAxisInputName);

        if (Input.GetButtonDown(_accelerateInputName))
        {
            _isAccelerating = true;
        }
        if(Input.GetButtonUp(_accelerateInputName))
        {
            _isAccelerating = false;
        }

        if (Physics.Raycast(transform.position, transform.up * -1, out var info, 1, _layerMask))
        {
            Ground groundBellow = info.transform.GetComponent<Ground>();
            if (groundBellow != null)
            {
                _groundSpeedVariator = groundBellow.speedVariator;
            }
            else
            {
                _groundSpeedVariator = 1;
            }
        }
        else
        {
            _groundSpeedVariator = 1;
        }
    }

    private bool IsOnSlope(out RaycastHit hit, out float angle, out float angleZ)
    {
        hit = new RaycastHit();
        angle = 0f;
        angleZ = 0f;
        if(Physics.Raycast(transform.position+transform.forward*.5f,Vector3.down,out hit, 0.8f))
        {
            angle = Vector3.Angle(Vector3.up, hit.normal);
            angleZ = Vector3.Angle(Vector3.right, hit.normal);
            return angle !=  0 && angle<=_maxAngle;
        }
        return false;
    }

    private void FixedUpdate()
    {
        
        if (_isAccelerating)
        {
            _accelerationLerpInterpolator += _accelerationFactor;
        }
        else
        {
            _accelerationLerpInterpolator -= _accelerationFactor*2;
        }

        _accelerationLerpInterpolator = Mathf.Clamp01(_accelerationLerpInterpolator);
        

        if(_isBoosting)
        {
            _speed = _speedMaxTurbo;
        }
        else
        {
            _speed = _accelerationCurve.Evaluate(_accelerationLerpInterpolator)*_speedMaxBasic*_groundSpeedVariator;
        }

        var forward = transform.forward;
        var angle = 0f;
        var angleZ = 0f;
        if(IsOnSlope(out var hit, out angle, out angleZ))
        {
            forward = Vector3.ProjectOnPlane(forward, hit.normal).normalized;

        }

        _carColliderAndMesh.eulerAngles =  new Vector3(-angle,_carColliderAndMesh.eulerAngles.y, angleZ);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + _rotationSpeed * Time.deltaTime * _rotationInput, 0);
        _rb.MovePosition(transform.position+forward*_speed*Time.fixedDeltaTime);
    }
    private void OnTriggerEnter(Collider other) //quand on touche le boost accelere
    {
        if (other.gameObject.CompareTag("BoostPad"))
            Boost();
    }
}
