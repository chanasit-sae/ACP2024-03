using UnityEngine.Animations;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public GameObject sword;
    private bool isSlashing;
    private int comboState = 0;
    private int maxComboState = 3;
    public float comboTime;
    public float currComboTime;
    public bool isDoingCombo;
    public string playerAni;

    AnimatorStateInfo animationState;

    [SerializeField] public int attackDamage;
    [SerializeField] private Animator swordAni;
    [SerializeField] private Transform player;
    [SerializeField] private Transform swordSprite;
    [SerializeField] private Transform swordEffect;
    [SerializeField] public UIManager UIManager;

    public float hitRadius;

    private Vector2 attackDirection = Vector2.right;
    private Vector3 startOffset = new Vector3(0, 0.3f, 0.125f);

    private class Direction {
        public bool up;
        public bool down;
        public bool left;
        public bool right;
    }

    Direction direction = new Direction();

    void Start()
    {
        Application.targetFrameRate = 120;
        sword.GetComponent<Collider>().enabled = false;
        sword.transform.position = player.position + startOffset;
        swordSprite.GetComponent<SpriteRenderer>().enabled = false;
        swordEffect.GetComponent<SpriteRenderer>().enabled = false;
        playerAni = "attack";
    }
    
    void Update()
    {
        getDirection();

            if (isDoingCombo)
            {
                currComboTime -= Time.deltaTime;
                if (currComboTime <= 0f) ResetCombo();
            }

            if (Input.GetKeyDown(KeyCode.J) && !UIManager.isPaused)
            {
                animationState = swordAni.GetCurrentAnimatorStateInfo(0);

                if (animationState.IsName("DefaultState"))
                {
                    if (!isDoingCombo) attack("combo1");
                    else if (comboState == 1) attack("combo2");
                }
            }
        
    }

    void attack(string combo) 
    {
        swordSprite.GetComponent<SpriteRenderer>().enabled = true;
        //bool noDirectionInput = !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D);
        //just in case I want to switch to manual direction hit

        Collider[] hitColliders = Physics.OverlapSphere(player.position, hitRadius);
        Collider closestEnemy = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            for (int j = i; j < hitColliders.Length; j++)
            {
                if (hitColliders[j].CompareTag("Enemy") || hitColliders[j].CompareTag("Dashing enemy"))
                {
                    float distance = Vector3.Distance(player.position, hitColliders[j].transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = hitColliders[j];
                    }
                }
            }
        }

        if (closestEnemy != null)
        {
            Vector3 directionToEnemy = closestEnemy.transform.position - player.position;
            directionToEnemy.y = 0;
            float angle = Mathf.Atan2(directionToEnemy.z, directionToEnemy.x) * Mathf.Rad2Deg;/*Find angle
                                                                                               * I'm bad at math.So this one is on gpt.*/
            sword.transform.rotation = Quaternion.Euler(0, -angle, 0);
        }
        else
        {
            if ((direction.up || direction.left || direction.right) && !direction.down)
            {
                if (direction.up && direction.right) sword.transform.rotation = Quaternion.Euler(0, 315, 0);
                else if (direction.up && direction.left) sword.transform.rotation = Quaternion.Euler(0, 225, 0);
                else if (direction.up) sword.transform.rotation = Quaternion.Euler(0, 270, 0);
                else if (direction.left) sword.transform.rotation = Quaternion.Euler(0, 180, 0);
                else sword.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (direction.down || direction.left || direction.right)
            {
                if (direction.down && direction.right) sword.transform.rotation = Quaternion.Euler(0, 45, 0);
                else if (direction.down && direction.left) sword.transform.rotation = Quaternion.Euler(0, 135, 0);
                else if (direction.down) sword.transform.rotation = Quaternion.Euler(0, 90, 0);
                else if (direction.left) sword.transform.rotation = Quaternion.Euler(0, 180, 0);
                else sword.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        float swordYRotation = sword.transform.rotation.eulerAngles.y;

        if (swordYRotation >= 135 && swordYRotation <= 225) playerAni = "attack_left";
        else if(swordYRotation > 45 && swordYRotation < 135) playerAni = "attack_front";//add up and down duay
        else if (swordYRotation > 225 && swordYRotation < 315) playerAni = "attack_back";
        else playerAni = "attack";

        SoundManager.Instance.PlaySound3D("hit", transform.position);
        swordAni.SetTrigger(combo); //can configure the trigger in animator tab
        //addCombo(); //***********uncomment this for combo system*************
        sword.transform.position = player.position + startOffset;

    }

    void addCombo() {
        comboState++;
        if (comboState > maxComboState)
        {
            comboState = 0;
            return;
        }
        currComboTime = comboTime;
        isDoingCombo = true;
    }

    void ResetCombo() { 
        comboState = 0;
        currComboTime = 0f;
        isDoingCombo = false;
    }

    private void EnableHitbox()
    {
        sword.GetComponent<Collider>().enabled = true;
    }

    private void DisableHitbox()
    {
        sword.GetComponent<Collider>().enabled = false;
    }

    private void EnableRenderer() {
        swordSprite.GetComponent<SpriteRenderer>().enabled = true;
        swordEffect.GetComponent<SpriteRenderer>().enabled = true;
    }
    private void DisableRenderer()
    {
        swordSprite.GetComponent<SpriteRenderer>().enabled = false;
        swordEffect.GetComponent<SpriteRenderer>().enabled = false;

    }
    void OnTriggerEnter(Collider other)
    {
        // Check if the object hit has the "Enemy" tag
        if (other.CompareTag("Enemy") || other.CompareTag("Dashing enemy"))
        {
            enemy_health enemyHealth = other.GetComponent<enemy_health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);  // Deal damage
            }
        }
    }

    void getDirection()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            direction.up = true;
            direction.down = false;
            if (!Input.GetKey(KeyCode.A)) direction.left = false;
            if (!Input.GetKey(KeyCode.D)) direction.right = false;

        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            direction.down = true;
            direction.up = false;
            if (!Input.GetKey(KeyCode.A)) direction.left = false;
            if (!Input.GetKey(KeyCode.D)) direction.right = false;

        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            direction.left = true;
            direction.right = false;
            if(!Input.GetKey(KeyCode.W)) direction.up = false;
            if(!Input.GetKey(KeyCode.S)) direction.down = false;

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            direction.right = true;
            direction.left = false;
            if (!Input.GetKey(KeyCode.W)) direction.up = false;
            if (!Input.GetKey(KeyCode.S)) direction.down = false;

        }
    }

}
