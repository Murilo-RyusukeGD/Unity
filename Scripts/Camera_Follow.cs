using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follow : MonoBehaviour {

    [SerializeField]
    private float smoothTime;
    private Vector2 velocidade;
    private bool offsetBool = false;

    [SerializeField]
    private Transform objFocused;

    [SerializeField]
    [Header("Câmeras")]
    private Transform mainCamera;
    //[SerializeField]
    //private Transform secondCamera;
    [SerializeField]
    private Vector3 offset = new Vector3(0, 0, -20);

    private void Awake()
    {

    }

    // Use this for initialization
    void Start ()
    {
        velocidade.x = mainCamera.position.x;
        velocidade.y = mainCamera.position.y;
    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    private void FixedUpdate()
    {
        if (offsetBool == false)
        {
            mainCamera.transform.position = smoothCameraFocus();
            //offset = new Vector3(0, 0, 0);
            offsetBool = true;
        }
        mainCamera.transform.position = smoothCameraFocus();
    }

    void mainCameraFocus()
    {
        print("Atualizando a câmera");
        mainCamera.position = objFocused.position + offset;
    }

    Vector3 smoothCameraFocus()
    {
        float posX = Mathf.SmoothDamp(mainCamera.position.x, objFocused.position.x, ref velocidade.x, smoothTime);
        float posY = Mathf.SmoothDamp(mainCamera.position.y, objFocused.position.y, ref velocidade.y, smoothTime);
        return new Vector3(posX,posY, 0) + offset;
        
    }
}
