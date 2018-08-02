using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPlayer : MonoBehaviour
{
    
    [SerializeField] GameObject cam;
    [SerializeField] GameObject body;
    
    [SerializeField] float mouseSensitivity = 4.0f;
    [SerializeField] float walkSpeed = 4.0f;

    float rotUD = 0.0f;
    float rotLR = 0.0f;

	void Awake ()
    {
	    rotUD = cam.transform.eulerAngles.x;
	    rotLR = body.transform.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
	}
	
	void FixedUpdate ()
    {
		rotLR += mouseSensitivity * Input.GetAxis("Mouse X");
        rotUD += mouseSensitivity * -Input.GetAxis("Mouse Y");
        
        cam.transform.localRotation = Quaternion.Euler(rotUD,0,0);
        body.transform.eulerAngles = new Vector3 (0,rotLR,0);

        if(Input.GetKey(KeyCode.W))
            body.transform.Translate( Vector3.forward*Time.deltaTime *walkSpeed );
        if(Input.GetKey(KeyCode.S))
            body.transform.Translate( Vector3.back   *Time.deltaTime *walkSpeed );
        if(Input.GetKey(KeyCode.A))
            body.transform.Translate( Vector3.left   *Time.deltaTime *walkSpeed );
        if(Input.GetKey(KeyCode.D))
            body.transform.Translate( Vector3.right  *Time.deltaTime *walkSpeed );
	}
}
