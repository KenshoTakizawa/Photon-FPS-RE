using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform viewPoint;

    public float mouseSensitivity = 1f;

    private Vector2 mouseInput;

    private float verticalIMouseInput;

    private Camera cam;

    private Vector3 moveDir;

    private Vector3 movement;

    private float activeMoveSpeed = 4f;

    public Vector3 jumpForce = new Vector3(0, 6, 0);

    public Transform groundCheckPoint;

    public LayerMask groundLayers;

    private Rigidbody rb;

    public float walkSpeed = 4f;

    public float runSpeed = 8f;

    private bool cursorLock = true;

    public List<Gun> guns = new List<Gun>();

    private int selectedGun = 0;

    private float shotTimer;

    [Tooltip("所持弾薬")]
    public int[] ammunition;

    [Tooltip("最高所持弾薬数")]
    public int[] maxAmmunition;

    [Tooltip("マガジン内の弾数")]
    public int[] ammoClip;

    [Tooltip("マガジンに入る最大の数")]
    public int[] maxAmmoClip;

    public float bulletImpactLifetime = 2f;

    //UIManager uIManager;

    private SpawnManager spawnManager;

    private void Awake()
    {
        //uIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        rb = GetComponent<Rigidbody>();

        UpdateCursorLock();

        transform.position = spawnManager.GetSpawnPoint().position;
    }

    // Update is called once per frame
    void Update()
    {
        PlayrRotate();

        PlayerMove();

        if (IsGround())
        {
            Run();

            Jump();
        }

        Aim();

        Fire();

        SwitchingGuns();

        UpdateCursorLock();
    }

    public void PlayrRotate()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X") * mouseSensitivity,
            Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

        transform.rotation = Quaternion.Euler
            (transform.eulerAngles.x,
            transform.eulerAngles.y + mouseInput.x,
            transform.eulerAngles.z);


        verticalIMouseInput += mouseInput.y;

        verticalIMouseInput = Mathf.Clamp(verticalIMouseInput, -60f, 60f);

        viewPoint.rotation = Quaternion.Euler
            (-verticalIMouseInput,
            viewPoint.transform.rotation.eulerAngles.y,
            viewPoint.transform.rotation.eulerAngles.z);
    }


    private void LateUpdate()
    {
        cam.transform.position = viewPoint.transform.position;
        cam.transform.rotation = viewPoint.transform.rotation;
    }

    public void PlayerMove()
    {
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0,
            Input.GetAxisRaw("Vertical"));

        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized;

        transform.position += movement * activeMoveSpeed * Time.deltaTime;
    }

    public void Jump()
    {
        if (IsGround() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(jumpForce, ForceMode.Impulse);
        }
    }

    public bool IsGround()
    {
        return Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.25f, groundLayers);
    }


    public void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            activeMoveSpeed = runSpeed;
        }
        else
        {
            activeMoveSpeed = walkSpeed;
        }
    }


    public void UpdateCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false;
        }
        else if (Input.GetMouseButton(0))
        {
            cursorLock = true;
        }

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void SwitchingGuns()

    {

        //Debug.Log(guns.Count);
        //Debug.Log(guns[1].gameObject);
        //Debug.Log(guns[2].gameObject);


        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            selectedGun++;

            if (selectedGun >= guns.Count)
            {
                selectedGun = 0;
            }

            switchGun();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;

            if (selectedGun < 0)
            {
                selectedGun = guns.Count - 1;
            }

            switchGun();
        }

        for (int i = 0; i < guns.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                selectedGun = i;

                switchGun();

            }
        }
    }


    void switchGun()
    {
        foreach (Gun gun in guns)
        {
            gun.gameObject.SetActive(false);

        }

        guns[selectedGun].gameObject.SetActive(true);
    }

    public void Aim()
    {
        if (Input.GetMouseButton(1))
        {
            cam.fieldOfView = Mathf.Lerp(
                cam.fieldOfView,
                guns[selectedGun].adsZoom,
                guns[selectedGun].adsSpeed * Time.deltaTime
            );
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(
                cam.fieldOfView,
                Mathf.Max(60f, guns[selectedGun].adsZoom),
                guns[selectedGun].adsSpeed * Time.deltaTime
            );
        }
    }

    public void Fire()
    {

        if (Input.GetMouseButton(0) && ammoClip[selectedGun] > 0 && Time.time > shotTimer)
        {
            FiringBullet();
        }

    }

    private void FiringBullet()
    {
        ammoClip[selectedGun]--;

        //Ray ray = cam.ViewportPointToRay(new Vector2(0.50000f, 0.5000000f));

        Ray ray = cam.ViewportPointToRay(new Vector2(0.50000f, 0.5000000f));

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.blue, 1f); //ここでRayに色をつける

        RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log("当たったオブジェクトは" + hit.collider.gameObject.name);

            Debug.Log(hit.point.z);
            Debug.Log(hit.point.y);

            GameObject bulletImpactObject = Instantiate(guns[selectedGun].bulletImpact, hit.point + (hit.normal * .002f), Quaternion.LookRotation(hit.normal, Vector3.up));

            Destroy(bulletImpactObject, bulletImpactLifetime);
        }


        shotTimer = Time.time + guns[selectedGun].shootInterval;
        ammoClip[selectedGun]--;

    }
}