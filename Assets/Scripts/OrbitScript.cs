using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class OrbitScript : MonoBehaviour
{
    public bool menu = true;
    public Transform target;
    public Vector3 shift = new Vector3(0, 0, 0);
    public float distance = 50.0f;
    private float xSpeed = 3.0f;
    private float ySpeed = 3.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = 0f;
    public float distanceMax = 1f;

    private Rigidbody rigidbody;

    private float x = 0.0f;
    private float y = 0.0f;

    private float _sensitivity = 3f;
    private float _translationSensitivity = 0.01f;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
    private Vector3 _translation;
    private bool _isRotating;

    public bool Menu
    {
        get
        {
            return menu;
        }

        set
        {
            x = 0;
            y = 0;

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + (target.position + shift);

            transform.rotation = rotation;
            transform.position = position;
            menu = value;
        }
    }

    // Use this for initialization
    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        rigidbody = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rigidbody != null)
        {
            rigidbody.freezeRotation = true;
        }
    }

    private bool moving = false;
    

    private void LateUpdate()
    {
        if (!Menu && target)
        {
            GameObject objectManager = GameObject.Find("ObjectManager");

            StudyManager studyManager = objectManager.GetComponent<StudyManager>();
            if (Input.GetMouseButton(0) && !studyManager.twodim)
            {

                // offset
                _mouseOffset = (Input.mousePosition - _mouseReference);
                // apply rotation
                _rotation.y = -(_mouseOffset.x) * _sensitivity;
                _rotation.x = (_mouseOffset.y) * _sensitivity;
                // rotate
                Vector3 rotatePoint = new Vector3(.5f, .5f, 0.5f);
                transform.RotateAround(rotatePoint, transform.right, -Input.GetAxis("Mouse Y") * _sensitivity);
                transform.RotateAround(rotatePoint, transform.up, Input.GetAxis("Mouse X") * _sensitivity);
                // store mouse
                _mouseReference = Input.mousePosition;

            } else if (Input.GetMouseButton(0) && studyManager.twodim)
            {
                _mouseOffset = (Input.mousePosition - _mouseReference);
                if (!moving)
                {
                    _mouseReference = Input.mousePosition;
                    _mouseOffset = new Vector3(0, 0, 0);
                }
                _translation.y = -(_mouseOffset.x) * _translationSensitivity;
                _translation.x = (_mouseOffset.y) * _translationSensitivity;

                Debug.Log("Translating: " + _translation);
                transform.Translate(_translation.y, -_translation.x, 0);
                _mouseReference = Input.mousePosition;
                moving = true;
            } else
            {
                moving = false;
            }

        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.position = new Vector3(0, 0, 1);
            shift = new Vector3(0, 0, 0);
        }

    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    void OnMouseDown()
    {
        Debug.Log("Mouse Down");
        // rotating flag
        _isRotating = true;

        // store mouse
        _mouseReference = Input.mousePosition;
    }

    void OnMouseUp()
    {
        // rotating flag
        _isRotating = false;
    }
}