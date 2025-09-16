using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public enum DriveType
    {
        FWD,
        RWD,
        AWD
    }

    public DriveType driveType;

    public enum BrakeType
    {
        FWB,
        RWB,
        AWB
    }

    public BrakeType brakeType;

    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private Rigidbody rb;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;

    public float addForce;
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    public List<Transform> wheelTransform;
    public List<WheelCollider> wheelColliders;

    public bool canAirContol;

    //for control in air
    public float pitchTorque = 5000.0f;
    public float yawTorque = 20000.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        if (driveType == DriveType.FWD)
        {
            wheelColliders[0].motorTorque = verticalInput * motorForce;
            wheelColliders[1].motorTorque = verticalInput * motorForce;
        }
        else if (driveType == DriveType.RWD)
        {
            wheelColliders[2].motorTorque = verticalInput * motorForce;
            wheelColliders[3].motorTorque = verticalInput * motorForce;
        }
        else
        {
            wheelColliders[0].motorTorque = verticalInput * motorForce;
            wheelColliders[1].motorTorque = verticalInput * motorForce;
            wheelColliders[2].motorTorque = verticalInput * motorForce;
            wheelColliders[3].motorTorque = verticalInput * motorForce;
        }

        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();

        //Helping car movement 
        if (wheelColliders[0].isGrounded)
        {
            rb.AddRelativeForce(Vector3.forward * addForce * verticalInput * Time.fixedDeltaTime, ForceMode.Impulse);
        }
        else
        {
            if (canAirContol)
            {
                ControlInAir();
            }
        }
    }

    void ControlInAir()
    {
        Vector3 castDownward = Vector3.down; //Looks locally down always
                                             //Debug.DrawRay(transform.position, castDownward * 1, Color.green);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, castDownward, out hit, 1))
        {

        }
        else
        {
            //0,0,0 (pitch, yaw, roll)
            Vector3 midAirControl = new Vector3(verticalInput * -pitchTorque, horizontalInput * yawTorque, 0);
            rb.AddRelativeTorque(midAirControl);
        }
    }

    private void ApplyBreaking()
    {
        if (brakeType == BrakeType.FWB)
        {
            wheelColliders[0].brakeTorque = currentbreakForce;
            wheelColliders[1].brakeTorque = currentbreakForce;
        }
        else if (brakeType == BrakeType.RWB)
        {
            wheelColliders[2].brakeTorque = currentbreakForce;
            wheelColliders[3].brakeTorque = currentbreakForce;
        }
        else
        {
            wheelColliders[0].brakeTorque = currentbreakForce;
            wheelColliders[1].brakeTorque = currentbreakForce;
            wheelColliders[2].brakeTorque = currentbreakForce;
            wheelColliders[3].brakeTorque = currentbreakForce;
        }
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;

        wheelColliders[0].steerAngle = currentSteerAngle;
        wheelColliders[1].steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(wheelColliders[0], wheelTransform[0]);
        UpdateSingleWheel(wheelColliders[1], wheelTransform[1]);
        UpdateSingleWheel(wheelColliders[2], wheelTransform[2]);
        UpdateSingleWheel(wheelColliders[3], wheelTransform[3]);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot
; wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
