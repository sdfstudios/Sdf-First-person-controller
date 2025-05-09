
using UnityEngine;
using UnityEngine.UI;


public class FirstPersonController : MonoBehaviour
{
    [Header("Speeds")]
    //normal walking speed
    [SerializeField] float Speed = 4f;
    //Run speed
    [SerializeField] float RunSpeed = 8f;
    //Crouch Speed
    [SerializeField] float CrouchSpeed = 1.5f;
    [Header(" ")]

    //the speed of how the speed will change(for example, from walking to running)    
    [SerializeField] float SpeedChangeSpeed = 2f;
    //Speed now
    float TargetSpeed;
    
    [Header("Buttons")]
    //control buttons
    [SerializeField] KeyCode RunKeyCode = KeyCode.LeftShift;
    [SerializeField] KeyCode jumpKeyCode = KeyCode.Space;
    [SerializeField] KeyCode CrouchKeyCode = KeyCode.LeftControl;
   
   
    [Header("Jump")]

    [SerializeField] float JumpForce = 4f;
    [SerializeField] GroundCheck GroundCheck;
   
    [Header("Crouch")]
   
    [SerializeField] float CrouchForce = 0.1f;
    // how fast will the camera position change after crouch and after getting up  
    [SerializeField] float CrouchChangeCamPosSpeed = 5f;
    [SerializeField] Transform CamPosition;
    //checking what is above the player
    [SerializeField] AboveCheck AboveCheck;
    //The position that will be set after crouch (Set automatically. We recommend not to touch)
    Vector3 CrouchCamPosition;
    
    [Header("Sprint")]
    //if the player should not rest from running
    [SerializeField] bool UnlimitedSprint;
    //sprint return speed
    [SerializeField] float SprintReturnSpeed = 15f;
    //Sprint Loss Speed
    [SerializeField] float SprintLossSpeed = 20f;
    //at what point should sprint access be returned (the more it costs, the faster access to speed will return)    
    [Range(0f,100f)]
    [SerializeField] float ReturnTimeSprint = 55f;
    [SerializeField] float FovCam = 90f;


    bool CantRun;
    float SprintRemaining;
    
    [Header("Audio")]
    [SerializeField] AudioSource WalkAndRunAudioSource;
    [SerializeField] AudioSource CrouchAndJumpAudioSource;
    [Header("   ")]
    [SerializeField] AudioClip _Walk;
    [SerializeField] AudioClip _Run;
    [SerializeField] AudioClip _Jump;
    [SerializeField] AudioClip _Crouch;
    [SerializeField] AudioClip _CrouchWalk;
    [SerializeField] AudioClip _Landing;

    [Header("The player can")]
    [SerializeField] bool Run = true;
    [SerializeField] bool Jump = true;
    [SerializeField] bool Crouch = true;
   
    //Variables for storing data. We recommend not to touch
    [Header("Other")]
    [SerializeField] Image SprintBar;
    [SerializeField] Image SprintBarBG;
    //checking if the player is crouched
    bool inCrouching;
    //checking if the player is running
    bool inRunninng;
    //controller rigidbody
    Rigidbody rb;
    //velocity on which movement depends. all movement data is recorded here
    Vector2 Velocity;
    Camera cam;       
    //for sprint bar
    Color TransperentW;
    //for sprint bar BG
    Color TransperentB;
    //to check the desired color sprint bar
    Color SprintBarColor;
    //to change collider by crouch time
    CapsuleCollider Capsulecollider;
    //saves the default center of the collider in front of the crouch
    Vector3 CrounchCenterCapsuleCollider;
    //saves the default height of the collider before the crouch
    float HeightCapsule;
    //using for AboveCheck
    public static bool inCrouchingStatic;
    //saved at the beginning of the game to return the standard fov after the end of the sprint
    float StandardFovCam;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
        Capsulecollider = GetComponent<CapsuleCollider>();
        HeightCapsule = Capsulecollider.height;
        SprintBarColor = Color.white;
        StandardFovCam = cam.fieldOfView;


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Velocity = new Vector2(Input.GetAxis("Horizontal") * TargetSpeed, Input.GetAxis("Vertical") * TargetSpeed);
        rb.velocity = transform.rotation * new Vector3(Velocity.x, rb.velocity.y, Velocity.y);
    }

    private void Update()
    {
        inCrouching = inCrouchingStatic = Input.GetKey(CrouchKeyCode);        
        inRunninng = Input.GetKey(RunKeyCode);

        //Run and walk
        if (!inCrouching && !AboveCheck.above && Run)
        {

            if (inRunninng && !CantRun && Velocity != Vector2.zero)
            {
                if (TargetSpeed != RunSpeed)
                {
                    TargetSpeed = Mathf.Lerp(TargetSpeed, RunSpeed, SpeedChangeSpeed * Time.deltaTime);
                   

                }
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, FovCam, SpeedChangeSpeed * Time.deltaTime);

                if (!UnlimitedSprint)
                {
                    SprintRemaining -= SprintLossSpeed * Time.deltaTime;
                }
                 ChangeAudio(_Run);
            }
            else
            {
                if (TargetSpeed != Speed)
                {
                    TargetSpeed = Mathf.Lerp(TargetSpeed, Speed, SpeedChangeSpeed * Time.deltaTime);
                    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, StandardFovCam, SpeedChangeSpeed * Time.deltaTime);

                    ChangeAudio(_Walk);
                }
                

            }
        }

        if (!inRunninng | CantRun || inRunninng & inCrouching || inRunninng & Velocity == Vector2.zero)
        {
            SprintRemaining = Mathf.Clamp(SprintRemaining += SprintReturnSpeed * Time.deltaTime, SprintRemaining, 0);           
        }

        //jump
        if (Input.GetKeyDown(jumpKeyCode) && !inCrouching && GroundCheck.Inground && Jump)
        {
            rb.velocity = Vector3.up * JumpForce;
            CrouchAndJumpAudioSource.PlayOneShot(_Jump);
        }




        //Crouch
        if (Crouch)
        {
           if (Input.GetKeyDown(CrouchKeyCode))
           {
               CrounchCenterCapsuleCollider = new Vector3(Capsulecollider.center.x, Capsulecollider.center.y - CrouchForce / 2, Capsulecollider.center.z);
               CrouchCamPosition = new Vector3(CamPosition.localPosition.x, CamPosition.localPosition.y - CrouchForce, CamPosition.localPosition.z);
                ChangeAudio(_CrouchWalk);
                PlayCrouch();
         
           }
         
           if (Input.GetKey(CrouchKeyCode))
           {  
               Capsulecollider.center = CrounchCenterCapsuleCollider;
               Capsulecollider.height = HeightCapsule / 1.5f;
         
               cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, CrouchCamPosition, CrouchChangeCamPosSpeed * Time.deltaTime);
               if (TargetSpeed != CrouchSpeed)
                   TargetSpeed = Mathf.Lerp(TargetSpeed, CrouchSpeed, SpeedChangeSpeed * Time.deltaTime);
           }
           else
           {
               if (AboveCheck.above == false)
               {
                   Capsulecollider.center = Vector3.zero;
                   Capsulecollider.height = HeightCapsule;
                   cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, CamPosition.localPosition, CrouchChangeCamPosSpeed * Time.deltaTime);
                   if (!inRunninng) { ChangeAudio(_Walk);}
                }
           }
         
         
           if (Input.GetKeyUp(CrouchKeyCode))
           {
               if (CrouchAndJumpAudioSource.isPlaying)
               {
                   CrouchAndJumpAudioSource.Stop();
               }
               if (AboveCheck.above == false)
               {
                   PlayCrouch();
               }
                   
           }
        }  
        

        //Sprint
        SprintBar.transform.localPosition = new Vector3(Vector3.zero.x + SprintRemaining, Vector3.zero.y, Vector3.zero.z);


        if (SprintRemaining <= -100f)
        {
            CantRun = true;
            SprintBarColor = Color.red;
        }
        if (CantRun)
        {
            if (SprintRemaining >= -ReturnTimeSprint)
            {
                CantRun = false;
                SprintBarColor = Color.white;
            }
        }

        if (SprintRemaining != 0f)
        {
            SprintBar.color = Color.Lerp(SprintBar.color, SprintBarColor, 4f * Time.deltaTime);
            SprintBarBG.color = Color.Lerp(SprintBarBG.color, Color.black, 4f * Time.deltaTime);
        }
        else
        {
            SprintBar.color = Color.Lerp(SprintBar.color, TransperentW, 4f * Time.deltaTime);
            SprintBarBG.color = Color.Lerp(SprintBarBG.color, TransperentB, 4f * Time.deltaTime);
        }


        //Audio
        if (Velocity != Vector2.zero && GroundCheck.Inground)
        {
            if (!WalkAndRunAudioSource.isPlaying)
            {
                WalkAndRunAudioSource.Play();
            }
        }
        else
        {
            WalkAndRunAudioSource.Stop();
        }

    }

    void ChangeAudio(AudioClip a)
    {
        if (WalkAndRunAudioSource.clip != a)
        {

            WalkAndRunAudioSource.clip = a;
            WalkAndRunAudioSource.Stop();
        }
    }

    public void Playlanding() { CrouchAndJumpAudioSource.PlayOneShot(_Landing);}
    
       
    

    public void PlayCrouch()
    {
        if (CrouchAndJumpAudioSource.isPlaying)
        {
            CrouchAndJumpAudioSource.Stop();
        }
        CrouchAndJumpAudioSource.PlayOneShot(_Crouch);
    }

    private void OnEnable()
    {
        GroundCheck.Exittrigger += Playlanding;
        AboveCheck.PlayerGetUp += PlayCrouch;
    }

    private void OnDisable()
    {
        GroundCheck.Exittrigger -= Playlanding;
        AboveCheck.PlayerGetUp += PlayCrouch;
    }



}


