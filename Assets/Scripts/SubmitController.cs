using UnityEngine;
using Zenject;

public class SubmitController : MonoBehaviour
{
    [Inject] private PlayerParams _playerParams;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private GameObject _holdPosition;
    [SerializeField] private float _minStrength = 1.5f;
    [SerializeField] private float _maxStrength = 15f;
    [SerializeField] private float _speed = 0.5f;
    [SerializeField] private float _takeBallDistance = 10f;
    [SerializeField] private float _throwTrajectoryCoefficient = 0.005f;
    private RaycastHit _hit;
    private Vector3 _vectorThrow;
    private float _timeElapsed;
    private Ray _ray;
    private HandledObject _handledObject;
    private float _strength;
    private Rigidbody _rb;

    private void Awake()
    {
        _strength = _minStrength;
        _handledObject = _holdPosition.GetComponent<HandledObject>();
    }

    private void Update()
    {
        // Check if there is an existing object has been taken
        if (_handledObject.IsHandled || _handledObject.IsDraging)
        {
            if (_playerParams.IsShrinked)
            {
                _handledObject.ReleaseObject();
            }
            else if (_handledObject.IsHandled && _rb != null)
            {
                _strength = Mathf.Lerp(_minStrength, _maxStrength, _timeElapsed);
                _vectorThrow = new Vector3(Camera.main.transform.forward.x, Camera.main.transform.forward.y + 0.5f, Camera.main.transform.forward.z) * _strength;
                Vector3 forceV = _vectorThrow * 50;

                DrawTrajectory.Instance.UpdateTrajectory(forceV, _rb, _holdPosition.transform.position);
            }
        }
        else
        {
            _ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Debug.DrawRay(_ray.origin, _ray.direction * 100, Color.red);
            bool sphereCast = Physics.SphereCast(_ray.origin, 0.3f, _ray.direction, out _hit, _takeBallDistance, _layerMask);
            _rb = null;
            if (_hit.transform != null)
            {
                Debug.Log($"____{_hit.transform.name}");
            }
            if (_hit.transform != null) _rb = _hit.transform.GetComponent<Rigidbody>();

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_rb != null && _hit.transform.tag == "Ball")
                {
                    _handledObject.IsDraging = true;
                    _handledObject.TakeObject(_hit.transform.gameObject);
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log($"Submit RB {_rb != null}");
                if (_rb != null && _hit.transform.tag == "Ball")
                {
                    _handledObject.IsDraging = true;
                    _handledObject.TakeObject(_hit.transform.gameObject);
                }
            }
        }

        // Get the scroll to change the trajectory of throw
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            ChangeForce(Input.GetAxis("Mouse ScrollWheel"));
        }

        //Get the F button to drop the object
        if (Input.GetKeyDown(KeyCode.F))
        {
            _handledObject.ReleaseObject();
        }

        //Get the right click hold to add force for the throw
        if (Input.GetMouseButton(1))
        {
            ChangeForce(_throwTrajectoryCoefficient);
        }

        //Get the right click to throw the object
        if (Input.GetMouseButtonUp(1))
        {
            Throw();
        }

        //Get the left click to throw the object
        if (Input.GetMouseButtonDown(0))
        {
            Throw();
        }
        CheckScaleControls();
    }


    private void CheckScaleControls()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            _playerParams.OnShrinking();
        }
        else
        {
            _playerParams.OnNormalizingScale();
        }
    }

    /// <summary>
    /// Throw the object by button
    /// </summary>
    private void Throw()
    {
        if (_handledObject.IsHandled)
        {
            _handledObject.ThrowObject(_vectorThrow);
        }
        _timeElapsed = 0;
        _strength = _minStrength;
    }

    /// <summary>
    /// Change the force of the throw
    /// </summary>
    /// <param name="coefficient"></param>
    private void ChangeForce(float coefficient)
    {
        _timeElapsed += (coefficient * _speed);
        if (_timeElapsed < 0) { _timeElapsed = 0; }
        if (_timeElapsed > 1) { _timeElapsed = 1; }
    }
}
