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
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
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

    private void LateUpdate()
    {
        if (!Menu && target)
        {
            if (Input.GetMouseButton(0))
            {
                //var mx = Input.mousePosition.x;//Input.GetAxis("Mouse X");
                //var my = Input.mousePosition.y;//Input.GetAxis("Mouse Y");
                //x += mx * xSpeed * distance * 0.02f;
                //y -= my * ySpeed * 0.02f;

                //Quaternion rotation = Quaternion.Euler(y, x, 0);

                //distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
                //Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                //Vector3 position = rotation * negDistance + (target.position + shift);

                //transform.rotation = rotation;
                //transform.position = position;

                GameObject objectManager = GameObject.Find("ObjectManager");

                // offset
                _mouseOffset = (Input.mousePosition - _mouseReference);
                // apply rotation
                //_rotation.y = -(_mouseOffset.x + _mouseOffset.y) * _sensitivity;
                _rotation.y = -(_mouseOffset.x) * _sensitivity;
                _rotation.x = (_mouseOffset.y) * _sensitivity;
                // rotate
                //]
                //transform.Rotate(_rotation);
                // transform.eulerAngles += _rotation;
                transform.RotateAround(objectManager.transform.position, transform.right, -Input.GetAxis("Mouse Y") * _sensitivity);
                transform.RotateAround(objectManager.transform.position, transform.up, Input.GetAxis("Mouse X") * _sensitivity);
                // store mouse
                //transform.rotation = _mouseOffset;
                _mouseReference = Input.mousePosition;

                var mx = Input.mousePosition.x - Screen.width / 2;// Input.GetAxis("Mouse X") ;
                var my = Input.mousePosition.y - Screen.height / 2;// Input.GetAxis("Mouse Y");

                x += mx * xSpeed * distance * 0.02f;
                y -= my * ySpeed * 0.02f;
                // Debug.Log("x " + x + "," + my);


                y = ClampAngle(y, yMinLimit, yMaxLimit);

                Quaternion rotation = Quaternion.Euler(_rotation.y, -_rotation.x, 0);
                // Debug.Log("..." + Input.GetAxis("Mouse ScrollWheel"));
                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
                //Debug.Log(distance + "," + Input.GetAxis("Mouse ScrollWheel"));
                RaycastHit hit;
                if (Physics.Linecast(target.position, transform.position, out hit))
                {
                    distance -= hit.distance;
                }
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * negDistance + target.position;
                //Debug.Log("Rotation: " + _rotation.x);
                //transform.rotation = rotation;
                //transform.position = position;
            }

        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.position = new Vector3(0, 0, 1);
            shift = new Vector3(0, 0, 0);
        }

        /*
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (shift == new Vector3(0, 0, 0))
            {
                shift = new Vector3(-1.5f, 0, 0);
                transform.position = transform.position + shift;
            }
            else if (shift == new Vector3(1.5f, 0, 0))
            {
                transform.position = transform.position - shift;
                shift = new Vector3(0, 0, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (shift == new Vector3(0, 0, 0))
            {
                shift = new Vector3(1.5f, 0, 0);
                transform.position = transform.position + shift;
            }
            else if (shift == new Vector3(-1.5f, 0, 0))
            {
                transform.position = transform.position - shift;
                shift = new Vector3(0, 0, 0);
            }
         
        }
        */

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