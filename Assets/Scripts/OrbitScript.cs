using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class OrbitScript : MonoBehaviour
{
    private bool menu = true;
    public Transform target;
    public Vector3 shift = new Vector3(0, 0, 0);
    public float distance = 50.0f;
    public float xSpeed = 240.0f;
    public float ySpeed = 480.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = 0f;
    public float distanceMax = 1f;

    private Rigidbody rigidbody;

    private float x = 0.0f;
    private float y = 0.0f;

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
                var mx = Input.mousePosition.x;//Input.GetAxis("Mouse X");
                var my = Input.mousePosition.y;//Input.GetAxis("Mouse Y");
                x += mx * xSpeed * distance * 0.02f;
                y -= my * ySpeed * 0.02f;

                Quaternion rotation = Quaternion.Euler(y, x, 0);

                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * negDistance + (target.position + shift);

                transform.rotation = rotation;
                transform.position = position;
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.position = new Vector3(0, 0, 1);
            shift = new Vector3(0, 0, 0);
        }
        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    if (shift == new Vector3(0, 0, 0))
        //    {
        //        shift = new Vector3(-1.5f, 0, 0);
        //        transform.position = transform.position + shift;
        //    }
        //    else if (shift == new Vector3(1.5f, 0, 0))
        //    {
        //        transform.position = transform.position - shift;
        //        shift = new Vector3(0, 0, 0);
        //    }
        //}
        //else if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    if (shift == new Vector3(0, 0, 0))
        //    {
        //        shift = new Vector3(1.5f, 0, 0);
        //        transform.position = transform.position + shift;
        //    }
        //    else if (shift == new Vector3(-1.5f, 0, 0))
        //    {
        //        transform.position = transform.position - shift;
        //        shift = new Vector3(0, 0, 0);
        //    }
        //}
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}