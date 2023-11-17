using UnityEngine;
using UnityEngine.Serialization;

public class LookAtObject : MonoBehaviour
{
    private Transform m_objectToLookAt;            // The object to look at.
    private Transform m_mainCameraTransform;       // The transform of the main camera.
    [FormerlySerializedAs("arrow")] [SerializeField] private GameObject m_arrow;                    // The arrow GameObject to display.
    private float m_degreeTolerance;               // The degree tolerance for considering the object as looked at.

    public Transform objectToLookAt
    {
        get => m_objectToLookAt;
        set { m_objectToLookAt = value; }
    }

    public Transform mainCameraTransform
    {
        get => m_mainCameraTransform;
        set { m_mainCameraTransform = value; }
    }

    public float degreeTolerance
    {
        get => m_degreeTolerance;
        set { m_degreeTolerance = value; }
    }

    /// <summary>
    /// Updates the rotation and arrow visibility based on the target object.
    /// </summary>
    /// <param>No inputs required.</param>
    /// <returns>No expected outputs.</returns>
    void Update()
    {
        if (objectToLookAt != null)
        {
            var _levelObject = new Vector3(objectToLookAt.position.x, transform.position.y, objectToLookAt.position.z);

            var _angle = Vector3.Angle(mainCameraTransform.forward, _levelObject - mainCameraTransform.position);

            transform.LookAt(_levelObject);

            if (_angle < degreeTolerance)
                m_arrow.SetActive(false);
            else
            {
                m_arrow.SetActive(true);
                
                if (_angle > 180 - (degreeTolerance + 10))
                {
                    transform.Rotate(Vector3.down * 60f);
                }
            }
        }
    }
}
