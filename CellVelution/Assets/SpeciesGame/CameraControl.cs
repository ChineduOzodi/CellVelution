using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    //Camera Variables
    public float camMoveSpeed = 1f;
    public float zoomSpeed = 1;

    Camera mainCam;

    machineLearning2 gameController;

    // Use this for initialization
    void Start () {
        mainCam = GetComponent<Camera>();
        gameController = GetComponent<machineLearning2>();
    }
	
	// Update is called once per frame
	void Update () {

        //Camera Movement
        float moveModifier = camMoveSpeed * mainCam.orthographicSize;
        mainCam.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed * moveModifier;

        float transX = Input.GetAxis("Horizontal") * moveModifier * Time.deltaTime/Time.timeScale;
        float transY = Input.GetAxis("Vertical") * moveModifier * Time.deltaTime / Time.timeScale;

        Camera.main.transform.Translate(new Vector3(transX, transY));

        if (Input.GetKeyDown(KeyCode.Home))
        {
            Camera.main.transform.position = new Vector3(0, 0, 10);
        }

        if (transform.position.y < 0)
            transform.position = new Vector3(transform.position.x, gameController.gameArea.y,-10);
        else if (transform.position.y > gameController.gameArea.y)
            transform.position = new Vector3(transform.position.x, 0, -10);

        if (transform.position.x < 0)
            transform.position = new Vector3(gameController.gameArea.x, transform.position.y, -10);
        else if (transform.position.x > gameController.gameArea.x)
            transform.position = new Vector3(0, transform.position.y, -10);

    }
}
