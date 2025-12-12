using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public float orbitDampening = 10f;
    public float cameraDistance = 10f;
    public float perspectiveZoomSpeed = 0.5f;


    private Transform cameraPivot;
    private Vector3   localRotation;

    private BigCube    bigCube;
    private GameObject firstHit;
    private Vector3    firstHitNormal;
    private Vector3    firstHitCenter;
    private GameObject secondHit;
    private Vector3    secondHitNormal;
    private Vector3    secondHitCenter;
    private float      offset;

    private readonly float rotationAngle = 90f;

    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;
    private Vector3 previousMousePosition;



    private void Start()
    {
        PlayerSettings.CameraDisable = true;
        PlayerSettings.CubeRotation = false;

        this.cameraPivot = this.transform.parent;

        bigCube = FindObjectOfType<BigCube>();
        offset  = PlayerSettings.CubeSize * 0.5f - 0.5f;
    }


    private void LateUpdate()
    {
        bool       gotInput      = false;
        Vector3    inputPosition = Vector3.zero;
        TouchPhase inputPhase    = TouchPhase.Ended;

        if (Input.GetMouseButtonDown(0))
        {
            if (!PlayerSettings.CubeRotation)
            {
                PlayerSettings.FaceRotation = true;
                Ray ray = Camera.main.ScreenPointToRay(inputPosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 100))
                {
                    firstHitNormal = hit.normal;
                    firstHitCenter = hit.transform.gameObject.GetComponent<Renderer>().bounds.center;
                    firstHit = hit.transform.parent.gameObject;
                }
            }
        }

    }   // void LateUpdate()


    private bool ConfirmWhichRotation(Vector3 normal, Vector3 tester, Vector3 move, char axis)
    {
        //Debug.Log("Normal: " + normal + " | tester: " + tester + " | move: " + move + " | axis: " + axis);
        Vector3 sum = normal + tester;

        if (axis == 'X')
        {
            sum = new Vector3(Mathf.Abs(move.x), Mathf.Abs(sum.y), Mathf.Abs(sum.z));
        }
        else if (axis == 'Y')
        {
            sum = new Vector3(Mathf.Abs(sum.x), Mathf.Abs(move.y), Mathf.Abs(sum.z));
        }
        else if (axis == 'Z')
        {
            sum = new Vector3(Mathf.Abs(sum.x), Mathf.Abs(sum.y), Mathf.Abs(move.z));
        }

        return sum == new Vector3(1, 1, 1);
    }


    private bool CheckForHitOnDifferentPlanes(Vector3 fromNormal, Vector3 fromCompare, Vector3 toNormal, Vector3 toCompare)
    {
        fromNormal = new Vector3(Mathf.Abs(fromNormal.x), Mathf.Abs(fromNormal.y), Mathf.Abs(fromNormal.z));
        toNormal   = new Vector3(Mathf.Abs(toNormal.x), Mathf.Abs(toNormal.y), Mathf.Abs(toNormal.z));

        //Debug.Log("From Vector: " + fromNormal + " | fromCompare: " + fromCompare);
        //Debug.Log("to Normal: " + toNormal + " | toComapre: " + toCompare);

        return (fromNormal == fromCompare && toNormal == toCompare);
    }


    private void DoTheRotation(Vector3 move)
    {
        if (firstHitNormal == secondHitNormal)
        {
            if (ConfirmWhichRotation(firstHitNormal, new Vector3(0, 0, 1), move, 'Y'))
            {
                StartCoroutine(bigCube.RotateAlongZ(firstHitNormal.x * move.y * rotationAngle, Mathf.RoundToInt(firstHit.transform.position.z + offset)));
            }
            else if (ConfirmWhichRotation(firstHitNormal, new Vector3(0, 1, 0), move, 'Z'))
            {
                StartCoroutine(bigCube.RotateAlongY(firstHitNormal.x * move.z * -rotationAngle, Mathf.RoundToInt(firstHit.transform.position.y + offset)));
                //Debug.Log("2nd: First hit " + firstHitNormal + " | move.y: " + move);
            }
            else if (ConfirmWhichRotation(firstHitNormal, new Vector3(0, 0, 1), move, 'X'))
            {
                StartCoroutine(bigCube.RotateAlongZ(firstHitNormal.y * move.x * -rotationAngle, Mathf.RoundToInt(firstHit.transform.position.z + offset)));
                //Debug.Log("3rd: First hit " + firstHitNormal + " | move.y: " + move);
            }
            else if (ConfirmWhichRotation(firstHitNormal, new Vector3(1, 0, 0), move, 'Z'))
            {
                StartCoroutine(bigCube.RotateAlongX(firstHitNormal.y * move.z * rotationAngle, Mathf.RoundToInt(firstHit.transform.position.x + offset)));
                //Debug.Log("4th: First hit " + firstHitNormal + " | move.y: " + move);
            }
            else if (ConfirmWhichRotation(firstHitNormal, new Vector3(0, 1, 0), move, 'X'))
            {
                StartCoroutine(bigCube.RotateAlongY(firstHitNormal.z * move.x * rotationAngle, Mathf.RoundToInt(firstHit.transform.position.y + offset)));
                //Debug.Log("5th: First hit " + firstHitNormal + " | move.y: " + move);
            }
            else if (ConfirmWhichRotation(firstHitNormal, new Vector3(1, 0, 0), move, 'Y'))
            {
                StartCoroutine(bigCube.RotateAlongX(firstHitNormal.z * move.y * -rotationAngle, Mathf.RoundToInt(firstHit.transform.position.x + offset)));
                //Debug.Log("6th: First hit " + firstHitNormal + " | move.y: " + move);
            }
        }
        else
        {
            if (CheckForHitOnDifferentPlanes(firstHitNormal, new Vector3(0, 0, 1), secondHitNormal, new Vector3(0, 1, 0)))
            {
                StartCoroutine(bigCube.RotateAlongX(firstHitNormal.z * secondHitNormal.y * -rotationAngle, Mathf.RoundToInt(firstHit.transform.position.x + offset)));
                //Debug.Log("1 ----");
            }
            else if (CheckForHitOnDifferentPlanes(firstHitNormal, new Vector3(0, 1, 0), secondHitNormal, new Vector3(0, 0, 1)))
            {
                StartCoroutine(bigCube.RotateAlongX(firstHitNormal.y * secondHitNormal.z * rotationAngle, Mathf.RoundToInt(firstHit.transform.position.x + offset)));
                //Debug.Log("2 ----");
            }
            else if (CheckForHitOnDifferentPlanes(firstHitNormal, new Vector3(0, 0, 1), secondHitNormal, new Vector3(1, 0, 0)))
            {
                StartCoroutine(bigCube.RotateAlongY(firstHitNormal.z * secondHitNormal.x * rotationAngle, Mathf.RoundToInt(firstHit.transform.position.y + offset)));
                //Debug.Log("3 ----");
            }
            else if (CheckForHitOnDifferentPlanes(firstHitNormal, new Vector3(1, 0, 0), secondHitNormal, new Vector3(0, 0, 1)))
            {
                StartCoroutine(bigCube.RotateAlongY(firstHitNormal.x * secondHitNormal.z * -rotationAngle, Mathf.RoundToInt(firstHit.transform.position.y + offset)));
                //Debug.Log("4 ----");
            }
            else if (CheckForHitOnDifferentPlanes(firstHitNormal, new Vector3(0, 1, 0), secondHitNormal, new Vector3(1, 0, 0)))
            {
                StartCoroutine(bigCube.RotateAlongZ(firstHitNormal.y * secondHitNormal.x * -rotationAngle, Mathf.RoundToInt(firstHit.transform.position.z + offset)));
                //Debug.Log("5 ----");
            }
            else if (CheckForHitOnDifferentPlanes(firstHitNormal, new Vector3(1, 0, 0), secondHitNormal, new Vector3(0, 1, 0)))
            {
                StartCoroutine(bigCube.RotateAlongZ(firstHitNormal.x * secondHitNormal.y * rotationAngle, Mathf.RoundToInt(firstHit.transform.position.z + offset)));
                //Debug.Log("6 ----");
            }
        }
    }


    public void ToggleCameraSettings()
    {
        PlayerSettings.CameraDisable = !PlayerSettings.CameraDisable;
    }


    void Update()
    {
        // Handle native touch events
        foreach (Touch touch in Input.touches)
        {
            HandleTouch(touch.fingerId, Camera.main.ScreenToWorldPoint(touch.position), touch.phase);
        }

        // Simulate touch events from mouse events
        if (Input.touchCount == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Began);
            }
            if (Input.GetMouseButton(0))
            {
                HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Moved);
            }
            if (Input.GetMouseButtonUp(0))
            {
                HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Ended);
            }
        }
    }

    private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase)
    {
        switch (touchPhase)
        {
            case TouchPhase.Began:
                break;

            case TouchPhase.Moved:
                break;

            case TouchPhase.Ended:
                break;
        }
    }
    
    
    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0)) // Left mouse button
    //    {
    //        SimulateTouch(Input.mousePosition, TouchPhase.Began);
    //    }
    //    else if (Input.GetMouseButton(0))
    //    {
    //        SimulateTouch(Input.mousePosition, TouchPhase.Moved);
    //    }
    //    else if (Input.GetMouseButtonUp(0))
    //    {
    //        SimulateTouch(Input.mousePosition, TouchPhase.Ended);
    //    }
    //}

    //void SimulateTouch(Vector3 position, TouchPhase phase)
    //{
    //    Debug.Log($"Simulated Touch at {position} with phase {phase}");
    //    // Add your custom logic here
    //}


}
