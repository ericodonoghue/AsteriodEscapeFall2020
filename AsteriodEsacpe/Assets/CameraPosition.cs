using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraPosition : MonoBehaviour
{
    GameObject player;
    Transform playerT;


    // The distance in the x-z plane to the target
    float distance = 10.0f;
    // the height we want the camera to be above the target
    float height = 5.0f;
    float heightDamping = 2.0f;
    float rotationDamping = 3.0f;

    // Start is called before the first frame update
    void Start()
    {

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

        //Vector3 pos = player.transform.position;
        //pos.y = player.transform.position.y + 1f;
        //pos.z = player.transform.position.z - 10f;
        //Camera.main.transform.position = pos;

        // TODO: figure out how to rotate camera with player
        //Camera.main.transform.rotation = player.transform.rotation;

        /*Vector3 p = new Vector3(0, 0, 0);

        p.x = Mathf.Sin(player.transform.rotation.y) * player.transform.position.x;
        p.y = player.transform.position.y + 1f;
        p.z = Mathf.Cos(player.transform.rotation.y) * (player.transform.position.z - 10f);

        Camera.main.transform.position = p;*/

    }

    // https://answers.unity.com/questions/38526/smooth-follow-camera.html
    private void LateUpdate()
    {
        playerT = player.transform;
        // Early out if we don't have a target
        if (!playerT)
            return;

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
    }
}
