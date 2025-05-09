using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class Look : MonoBehaviour
{
    [SerializeField] float SensitivityX = 2f;
    [SerializeField] float SensitivityY = 2f;
    
    [SerializeField] Transform Controller;
   

    float rotationX;
    float rotationY;

    float MouseX;
    float MouseY;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rotationX = Controller.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        MouseX = Input.GetAxisRaw("Mouse X")  * SensitivityX;
        MouseY = Input.GetAxisRaw("Mouse Y")  * SensitivityY;

        rotationX += MouseX;
        rotationY -= MouseY;

        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

      

        Controller.localRotation = Quaternion.Euler(Controller.rotation.x, rotationX, Controller.rotation.z); 
        transform.localRotation = Quaternion.Euler(rotationY, transform.rotation.y, transform.rotation.z); 
    }   

}

