using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }

        Vector3 pos = player.transform.position;
        pos.y = player.transform.position.y + 1f;
        pos.z = player.transform.position.z - 10f;
        Camera.main.transform.position = pos;;
    }
}
