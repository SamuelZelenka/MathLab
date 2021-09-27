using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float sensitivity;
    public GameObject holder;
    RaycastHit hit;
    LineRenderer lineRendedrer;
    Vector3 previousPos;

    Vector3[] corners = new Vector3[]{
 	// bottom 4 positions:
	new Vector3( 1, 0, 1 ),
    new Vector3( -1, 0, 1 ),
    new Vector3( -1, 0, -1 ),
    new Vector3( 1, 0, -1 ),
	// top 4 positions:
	new Vector3( 1, 2, 1 ),
    new Vector3( -1, 2, 1 ),
    new Vector3( -1, 2, -1 ),
    new Vector3( 1, 2, -1 )
};


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        lineRendedrer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetAxis("Mouse Y") != 0)
        {
            Camera.main.transform.rotation *= Quaternion.Euler(Input.GetAxis("Mouse Y") * -sensitivity * Time.deltaTime, 0, 0 ) ;
        }
        if (Input.GetAxis("Mouse X") != 0)
        {
            holder.transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime, 0);
        }

        if (Physics.Raycast(transform.position, transform.position = Camera.main.transform.forward, out hit, Mathf.Infinity))
        {
            Matrix4x4 matrix = Matrix4x4.TRS(hit.point, Quaternion.LookRotation(hit.normal, transform.forward), Vector3.one * 0.5f);
            Vector3 placementPoint = hit.point + new Vector3(0, 1.2f, 0);
            Vector3[] cubePoints = new Vector3[19];
            cubePoints[0] = placementPoint + matrix.MultiplyVector(corners[0]);
            cubePoints[1] = placementPoint + matrix.MultiplyVector(corners[1]);
            cubePoints[2] = placementPoint + matrix.MultiplyVector(corners[2]);

            cubePoints[3] = placementPoint + matrix.MultiplyVector(corners[0]);
            cubePoints[4] = placementPoint + matrix.MultiplyVector(corners[3]);
            cubePoints[5] = placementPoint + matrix.MultiplyVector(corners[2]);

            cubePoints[6] = placementPoint + matrix.MultiplyVector(corners[6]);
            cubePoints[7] = placementPoint + matrix.MultiplyVector(corners[3]);
            cubePoints[8] = placementPoint + matrix.MultiplyVector(corners[4]);

            cubePoints[9] = placementPoint + matrix.MultiplyVector(corners[6]);
            cubePoints[10] = placementPoint + matrix.MultiplyVector(corners[1]);
            cubePoints[11] = placementPoint + matrix.MultiplyVector(corners[7]);

            cubePoints[12] = placementPoint + matrix.MultiplyVector(corners[6]);
            cubePoints[13] = placementPoint + matrix.MultiplyVector(corners[5]);
            cubePoints[14] = placementPoint + matrix.MultiplyVector(corners[7]);

            cubePoints[15] = placementPoint + matrix.MultiplyVector(corners[0]);
            cubePoints[16] = placementPoint + matrix.MultiplyVector(corners[5]);
            cubePoints[17] = placementPoint + matrix.MultiplyVector(corners[4]);

            cubePoints[18] = placementPoint + matrix.MultiplyVector(corners[0]);

            lineRendedrer.positionCount = cubePoints.Length;
            lineRendedrer.SetPositions(cubePoints);
        }
    }
}
