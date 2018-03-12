using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

    [HideInInspector]
    public GameObject associated;
    public int priority = 6;
    float speedx = 4f;//скорость поворота 
    float speedz = 0.1f;//скорость ходьбы
    bool live = true;
    Animator animator;//анимаотор
    CharacterController controller; //контроллер для ходьбы
    private bool inv_Open;

	bool isRunning = true;
	private float powerRegenTimer;

    private GameObject inv_Main;
    private GameObject m_Cam;
    public float pbarSlider;
    public float pbarStart;
    public float pbarCurrent;
    private PlayerHealth playerHealth;
    [HideInInspector]
    public float hspeed; //для хранения скорости мышки (для инвентаря)
    [HideInInspector]
    public float vspeed;

    // Use this for initialization
    void Start() {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        inv_Main = GameObject.FindGameObjectWithTag("inv_main");
        inv_Open = false;
        inv_Main.SetActive(false);
        m_Cam = GameObject.FindGameObjectWithTag("MainCamera");
        hspeed = m_Cam.GetComponent<ThirdPersonOrbitCamBasic>().horizontalAimingSpeed;
        vspeed = m_Cam.GetComponent<ThirdPersonOrbitCamBasic>().verticalAimingSpeed;
        playerHealth = GetComponent<PlayerHealth>();

    }
    private void Update()
    {
        if (live)
        {
            /*
		bool isRunning = Input.GetKey (KeyCode.LeftShift);
		if (isRunning) {
			playerHealth.currentPower = Mathf.Clamp (playerHealth.currentPower - (15 * Time.deltaTime), 0.0f, playerHealth.startPower);
			playerHealth.powerbar.value = playerHealth.currentPower;
			powerRegenTimer = 0f;
		} else if (playerHealth.currentPower<playerHealth.startPower){
			if (powerRegenTimer >= 3.0f) {
				playerHealth.currentPower = Mathf.Clamp (playerHealth.currentPower + (5 * Time.deltaTime), 0.0f, playerHealth.startPower);
				playerHealth.powerbar.value = playerHealth.currentPower;
			} else {
				powerRegenTimer += Time.deltaTime;
			}
		}
        */
            //Open inventory
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (inv_Open == false)
                {

                    inv_Main.SetActive(true);
                    inv_Open = true;
                    inventory_open(inv_Open);
                }
                else if (inv_Open == true)
                {

                    inv_Main.SetActive(false);
                    inv_Open = false;
                    inventory_open(inv_Open);
                }
                
            }
            if (Input.GetKeyDown(KeyCode.Mouse0) && !(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) && (inv_Open == false))
            {

                animator.SetTrigger("Attack");
                if (associated != null)
                {
                    if (Vector3.Distance(transform.position, associated.transform.position) < 3) associated.GetComponent<Behavior>().TakeDamage(gameObject, Random.Range(50, 100));

                }
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate() {
        if (live)
        {


            float h = Input.GetAxis("Horizontal");//перемещение курсора по горизонтали
            float v = Input.GetAxis("Vertical");//перемещение курсора по вертикали

            if (playerHealth.currentPower < 100)
            {
                playerHealth.currentPower += 2 * Time.deltaTime;
                playerHealth.powerbar.value = playerHealth.currentPower;
            }
            else if (playerHealth.currentPower > 100)
            {
                playerHealth.currentPower = 100;
                playerHealth.powerbar.value = playerHealth.currentPower;
            }
            //print (GetComponent<PlayerHealth> ().currentPower);
            //x - horiz, z - vert

            if (h != 0)
            {
                transform.Rotate(0f, h * speedx, 0f);//вращаем персонажа
            }

            if (v != 0) //если нажата клавиша ходьбы
            {
                Vector3 dir;
                dir = transform.TransformDirection(new Vector3(0f, -3f, v * speedz / 100));

                if (Input.GetKey(KeyCode.W))
                {
                    speedz = 5f;
                    animator.SetBool("Walk", true);//включаем анимацию ходьбы
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    speedz = 3f;
                    animator.SetBool("WalkBack", true);//включаем анимацию ходьбы			
                }
                // dir = transform.TransformDirection(new Vector3(0f, -3f, v * speedz));//высчитываем смещение вперед
                controller.Move(dir);//двигаем контроллер

            }
            else//если не нажата клавиша ходьбы
            {
                animator.SetBool("Walk", false);//выключаем ходьбу
                animator.SetBool("WalkBack", false);//выключаем ходьбу
            }
            Debug.Log(isRunning);
            if (Input.GetKey(KeyCode.W)&& Input.GetKey(KeyCode.LeftShift)&&isRunning)
            {                
                    playerHealth.currentPower -= 5 * Time.deltaTime;
                    playerHealth.powerbar.value = playerHealth.currentPower;
                    speedx = 3;
                    speedz = 20f;
                    animator.SetBool("Run", true);
            }
             else 
            {
                speedx = 4;
                speedz = 5f;
                animator.SetBool("Run", false);

            }
            if (playerHealth.currentPower < 1 && isRunning) StartCoroutine(NoRun());


            if (Input.GetKeyUp(KeyCode.LeftShift))
            {

                speedx = 4;
                speedz = 5f;
                animator.SetBool("Run", false);

            }
            
           
            if (Input.GetKeyDown(KeyCode.E) && associated != null)
            {
                if (associated.GetComponent<Behavior>().state == Behavior.State.dead)
                {
                    playerHealth.TakeFood(20);
                    //hp += 100;///когда кого-то съел
                    Destroy(associated);
                    associated = null;
                }
                else if (Vector3.Distance(gameObject.transform.position, associated.transform.position) > 30)
                {
                    associated.GetComponent<Behavior>().enemy = null;
                    associated = null;
                }

            }


          
        }
    }


    public void inventory_open(bool temp) {
        temp = inv_Open;
        if (temp == false) { //IF INVENTORY OPENED
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            m_Cam.GetComponent<ThirdPersonOrbitCamBasic>().horizontalAimingSpeed = hspeed;
            m_Cam.GetComponent<ThirdPersonOrbitCamBasic>().verticalAimingSpeed = vspeed;


        } else if (temp == true) { // IF INVENTORY CLOSED
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            m_Cam.GetComponent<ThirdPersonOrbitCamBasic>().horizontalAimingSpeed = 0f;
            m_Cam.GetComponent<ThirdPersonOrbitCamBasic>().verticalAimingSpeed = 0f;
        }
    }



    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (live&&hit.gameObject.name != "Terrain")
        {
            associated = hit.gameObject;
            hit.gameObject.GetComponent<Behavior>().GetEnemy(gameObject);
        }
    }

    public IEnumerator Dead()
    {
        Debug.Log("start death");
        animator.SetTrigger("Death");
        associated = null;
        live = false;
        yield return new WaitForSeconds(5f);
        Application.Quit();
    }

    public IEnumerator NoRun()
    {
        isRunning = false;
        speedx = 4;
        speedz = 5f;
        animator.SetBool("Run", false);
        yield return new WaitForSeconds(5f);
        isRunning = true;
    }



}
