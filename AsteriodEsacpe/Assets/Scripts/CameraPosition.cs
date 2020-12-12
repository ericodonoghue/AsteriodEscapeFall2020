using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class CameraPosition : MonoBehaviour
{
    GameObject player;
    Transform playerT;
    private PauseControl pauseControl;
    private AvatarAccounting avatarAccounting;


    // The distance in the x-z plane to the target
    public float cameraBackOffset = 10.0f;
    // the height we want the camera to be above the target
    //float height = 5.0f;
    float heightDamping = 2.0f;
    float rotationDamping = 5.0f;
    public float lookSensitivity = 1f;
    public float cameraHeightOffset = 4.0f;
    public float wallBuffer = .1f;

    private Vector3 CamRot;

    // Start is called before the first frame update
    void Start()
    {
        pauseControl = Camera.main.GetComponent<PauseControl>();
        avatarAccounting = Camera.main.GetComponent<AvatarAccounting>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerT = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    private void LateUpdate()
    {
        playerT = player.transform;
        // Early out if we don't have a target
        if (!playerT)
            return;

        if (!pauseControl.isPaused)
        {
            // Old camera code for reference
            //Quaternion newAngle = Quaternion.Euler(CamRot.x, CamRot.y, CamRot.z);
            //transform.rotation = Quaternion.Lerp(transform.rotation, newAngle, Time.deltaTime * rotationDamping);

            //Calculate and apply local rotation
            transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * lookSensitivity, Space.Self);
            transform.Rotate(Vector3.left, Input.GetAxis("Mouse Y") * lookSensitivity, Space.Self);

            // Set the camera position
            float camdist = cameraBackOffset;
            float camup = cameraHeightOffset;
            RaycastHit[] backhits = Physics.RaycastAll(playerT.position, transform.rotation * Vector3.back, cameraBackOffset * (1 + wallBuffer));
            int backindex = 0;
            while (backindex < backhits.Length && backhits[backindex].transform.tag == "Player")
            {
                backindex++;
            }
            if (backindex < backhits.Length)
            {
                camdist = backhits[backindex].distance / (1 + wallBuffer);
            }
            if (Physics.Raycast(playerT.position + transform.rotation * Vector3.back * camdist, transform.rotation * Vector3.up, out RaycastHit uphit, cameraHeightOffset * (1 + wallBuffer)))
            {
                camup = uphit.distance / (1 + wallBuffer);
            }
            transform.position = playerT.position + transform.rotation * Vector3.back * camdist + transform.rotation * Vector3.up * camup;
        }

        /*
        // https://answers.unity.com/questions/38526/smooth-follow-camera.html
        // Calculate the current rotation angles
        float wantedRotationAngle = playerT.eulerAngles.y;
        float wantedHeight = playerT.position.y + height;
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;
        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);
        // Convert the angle into a rotation
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        transform.position = playerT.position;
        transform.position -= currentRotation * Vector3.forward * distance;
        // Set the height of the camera
        transform.position = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
        // Always look at the target
        transform.LookAt(playerT);
        */
    }
}
