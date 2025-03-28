using System.Collections;
using UnityEngine;
using TMPro; // Nécessaire pour le texte du compte à rebours

public class CarControler : MonoBehaviour
{
    [SerializeField] private string _hAxisInputName = "Horizontal", _accelerateInputName = "Accelerate";
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private ParticleSystem _driftParticles;
    [SerializeField] private TextMeshProUGUI _countdownText; // Texte pour le compte à rebours

    private float _speed, _accelerationLerpInterpolator, _rotationInput;
    [SerializeField] private float _speedMaxBasic = 30, _speedMaxTurbo = 40, _accelerationFactor, _rotationSpeed = 100f, _maxAngle = 360;
    private bool _isAccelerating, _isBoosting, _isDrifting, _raceStarted, _boostReady;
    private float _groundSpeedVariator;
    private float _speedModifier = 1f; // Ajouté pour gérer le multiplicateur de vitesse

    [SerializeField] public Transform _carColliderAndMesh;
    [SerializeField] private AnimationCurve _accelerationCurve;

    private float _driftCharge;
    private int _driftDirection;
    private string _driftKey;
    private ParticleSystem.MainModule _driftParticleSettings;

    private void Start()
    {
        _driftKey = gameObject.name == "P1" ? "f" : "right ctrl";

        if (_driftParticles != null)
        {
            _driftParticleSettings = _driftParticles.main;
            _driftParticles.Stop();
        }

        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        int countdown = 3;
        _countdownText.gameObject.SetActive(true);
        _boostReady = false;
        _raceStarted = false;

        while (countdown > 0)
        {
            _countdownText.text = countdown.ToString();

            // Lorsque le compte à rebours atteint 1, vérifie si le joueur garde la touche d'accélérer
            if (countdown == 1)
            {
                if (Input.GetButton(_accelerateInputName)) // Vérifie si la touche d'accélérer est maintenue
                {
                    _boostReady = true;  // Le boost sera prêt
                }
            }

            yield return new WaitForSeconds(1);
            countdown--;
        }

        _countdownText.text = "GO!";
        _raceStarted = true;

        yield return new WaitForSeconds(0.5f);
        _countdownText.gameObject.SetActive(false);
    }

    public void Boost()
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
        if (!_raceStarted) return;

        _rotationInput = Input.GetAxis(_hAxisInputName);

        if (Input.GetButtonDown(_accelerateInputName))
        {
            _isAccelerating = true;
        }
        if (Input.GetButtonUp(_accelerateInputName))
        {
            _isAccelerating = false;
        }

        if (Input.GetKeyDown(_driftKey))
        {
            StartDrift();
        }
        if (Input.GetKeyUp(_driftKey))
        {
            EndDrift();
        }

        if (Physics.Raycast(transform.position, -transform.up, out var info, 1, _layerMask))
        {
            Ground groundBellow = info.transform.GetComponent<Ground>();
            _groundSpeedVariator = groundBellow != null ? groundBellow.speedVariator : 1;
        }
        else
        {
            _groundSpeedVariator = 1;
        }

        // Le boost peut être activé si le joueur garde la touche d'accélérer après que le compte à rebours soit à 1
        if (_boostReady && Input.GetButton(_accelerateInputName)) // Vérifie si la touche d'accélérer est toujours maintenue
        {
            Boost();
            _boostReady = false;  // Le boost est activé une seule fois
        }
    }

    private void StartDrift()
    {
        if (!_isDrifting && Mathf.Abs(_rotationInput) > 0.1f)
        {
            _isDrifting = true;
            _driftDirection = _rotationInput > 0 ? 1 : -1;
            _driftCharge = 0;

            if (_driftParticles != null)
            {
                _driftParticles.Play();
            }
        }
    }

    private void EndDrift()
    {
        if (_isDrifting)
        {
            _isDrifting = false;
            if (_driftCharge >= 1)
            {
                Boost();
            }

            if (_driftParticles != null)
            {
                _driftParticles.Stop();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_raceStarted) return;

        if (_isAccelerating)
        {
            _accelerationLerpInterpolator += _accelerationFactor;
        }
        else
        {
            _accelerationLerpInterpolator -= _accelerationFactor * 2;
        }

        _accelerationLerpInterpolator = Mathf.Clamp01(_accelerationLerpInterpolator);

        if (_isBoosting)
        {
            _speed = _speedMaxTurbo;
        }
        else
        {
            _speed = _accelerationCurve.Evaluate(_accelerationLerpInterpolator) * _speedMaxBasic * _groundSpeedVariator * _speedModifier;
        }

        var forward = transform.forward;

        if (_isDrifting)
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + _rotationSpeed * Time.deltaTime * _rotationInput * 1.5f, 0);
            _driftCharge = Mathf.Clamp(_driftCharge + Time.deltaTime, 0, 1.5f);

            if (_driftParticles != null)
            {
                if (_driftCharge < 0.5f)
                    _driftParticleSettings.startColor = Color.blue;
                else if (_driftCharge < 1f)
                    _driftParticleSettings.startColor = Color.yellow;
                else
                    _driftParticleSettings.startColor = Color.red;
            }
        }
        else
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + _rotationSpeed * Time.deltaTime * _rotationInput, 0);
        }

        _rb.MovePosition(transform.position + forward * _speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BoostPad"))
            Boost();
    }

    // Méthode pour modifier la vitesse du joueur (en fonction de la taille)
    public void SetSpeedModifier(float modifier)
    {
        _speedModifier = modifier;
    }
}
