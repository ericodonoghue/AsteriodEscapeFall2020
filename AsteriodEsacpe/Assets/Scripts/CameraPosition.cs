using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraPosition : MonoBehaviour
{
    GameObject player;
    Transform playerT;
    private PauseControl pauseControl;


    // The distance in the x-z plane to the target
    public float distance = 10.0f;
    // the height we want the camera to be above the target
    float height = 5.0f;
    float heightDamping = 2.0f;
    float rotationDamping = 3.0f;
    public float lookSensitivity = 3f;
    public float cameraHeightOffset = 2.0f;    

    private Vector3 CamRot;

    // Start is called before the first frame update
    void Start()
    {
        pauseControl = Camera.main.GetComponent<PauseControl>();
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerT = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // quits game when player presses Q
        if (Input.GetKeyDown(KeyCode.Backspace))  
        {
            SceneManager.LoadScene("CollisionTestingScene");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }

    
    private void LateUpdate()
    {
        playerT = player.transform;
        // Early out if we don't have a target
        if (!playerT)
            return;

        if (!pauseControl.isPaused)
        {
            //New camera rotation using a mouse orbit camera. The player should now orient themselves based on the camera.
            CamRot.y += Input.GetAxis("Mouse X") * lookSensitivity;
            CamRot.x -= Input.GetAxis("Mouse Y") * lookSensitivity;
            CamRot.x = Mathf.Clamp(CamRot.x, -90f, 90f);
            Quaternion newAngle = Quaternion.Euler(CamRot.x, CamRot.y, 0);

            transform.rotation = Quaternion.Lerp(transform.rotation, newAngle, Time.deltaTime * rotationDamping);
            transform.position = playerT.position - transform.rotation * Vector3.forward * distance;
            transform.position = new Vector3(transform.position.x, transform.position.y + cameraHeightOffset, transform.position.z);
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
