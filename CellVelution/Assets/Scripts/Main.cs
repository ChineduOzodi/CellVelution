using UnityEngine;
using System.Collections;
using CodeControl;

public class Main : MonoBehaviour {

    //Game Variables
    internal GameObject cellEmpty;

    //Camera Variables
    public float camMoveSpeed = 1f;
    public float zoomSpeed = 1;

    Camera mainCam;
	// Use this for initialization
	void Start () {

        mainCam = GetComponent<Camera>();
        cellEmpty = new GameObject("CellEmpty");

	
	}
	
	// Update is called once per frame
	void Update () {
        //Object Clicking
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.CircleCast(rayPos, .1f, Vector2.up,.1f);

            if (hit.collider != null)
            {
                Debug.Log(hit.transform.gameObject.name);
                                
            }
            else
            {
                CellModel model = new CellModel();

                model.food = .5f;
                model.foodToGrow = 1;

                model.mutationChance = .01f;

                model.duplicationAngle = 45;

                
                int randomNum = Random.Range(0, 2);
                
                if (randomNum == 0)
                {
                    model.color = Color.red;
                    model.color.a = .8f;
                    var modController = Controller.Instantiate<PhagocyteController>("phagocyte", model, cellEmpty.transform);

                    modController.transform.position = rayPos;
                }
                else
                {
                    model.color = Color.green;
                    model.color.a = .8f;
                    var modController = Controller.Instantiate<PhotocyteController>("photocyte", model, cellEmpty.transform);

                    modController.transform.position = rayPos;
                }

                
            }
        }

        //Camera Movement
        float moveModifier = camMoveSpeed * mainCam.orthographicSize;
        mainCam.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed * moveModifier;

        float transX = Input.GetAxis("Horizontal") * moveModifier * Time.deltaTime;
        float transY = Input.GetAxis("Vertical") * moveModifier * Time.deltaTime;

        Camera.main.transform.Translate(new Vector3(transX, transY));
    }
}
