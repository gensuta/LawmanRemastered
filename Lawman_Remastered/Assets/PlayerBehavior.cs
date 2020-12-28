using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] bool isHoldingGun;

    [SerializeField] GameObject enemy;

    [SerializeField] Animator anim, enemyAnim;

    ScenarioHandler scenario;

    public int warningDist;

    [Space]
    [Header("Node Names")]
    [SerializeField] string warningShot1;
    public string warningShot2, warningShot3, shotFloor, handOnGun, shotInFoot, blamOne,blamTwo, blamEnd,shotBcGun;

    [SerializeField] float timeTillShot, timeTillWarning = 1f;

    bool warnedAboutGun, didShootFloor, wasShot;

    [Header("Floats")]
    [SerializeField] float speed;
    public float distanceFromEnemy;

    [SerializeField] AudioClip shot, footStep,footStep2,missShot;
    float timeTillStep = 0.2f;

    [SerializeField] GameObject blamVisual;
    bool didLeft;


    // Start is called before the first frame update
    void Start()
    {
        scenario = FindObjectOfType<ScenarioHandler>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!scenario.gameOver)
        {
            distanceFromEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            var dist = new Yarn.Value(Mathf.RoundToInt(distanceFromEnemy));

            scenario.SetValue("$distance", dist);

            if (Input.GetKey(KeyCode.Space))
            {
                anim.Play("playerFeint");
                isHoldingGun = true;
            }
            else
            {
                isHoldingGun = false;
            }

            if(scenario.GetCurrentNode() == blamEnd)
            {
                scenario.saidBlam = false; // you survived the monologue
                scenario.startedMonologue = false; 
                blamVisual.SetActive(false);

            }
            if (scenario.startedMonologue)
            {
                blamVisual.SetActive(true);
                if (Input.GetKeyDown(KeyCode.B))
                {
                    if (!scenario.saidBlam)
                    {
                        scenario.StartNewNode(blamOne);
                        scenario.saidBlam = true;
                    }
                    else
                    {
                        scenario.StartNewNode(blamTwo);
                    }
                }
            }

            if (timeTillShot == 0)
            {
                if (scenario.isTyping() && !scenario.isOptionsVisible)
                {
                    if (scenario.GetCurrentNode() == scenario.startMonologue || scenario.GetCurrentNode() == scenario.startBanter)
                        enemyAnim.Play("enemyLaugh");
                    else if (scenario.GetCurrentNode() == warningShot1 || scenario.GetCurrentNode() == warningShot2)
                        enemyAnim.Play("enemyNervous1");
                    else if (scenario.GetCurrentNode() == warningShot3)
                        enemyAnim.Play("enemyNervous2");
                    else
                        enemyAnim.Play("enemyTalk");
                }
                else enemyAnim.Play("enemyIdle");

                if (!wasShot)
                {
                    if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)|| Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                    {
                        scenario.numPresses++;
                    }

                    if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                    {
                        anim.Play("playerShuffle");
                        Move(Vector2.left);
                    }
                    else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                    {
                        anim.Play("playerShuffle");
                        Move(Vector2.right);
                    }
                    else
                    {
                        if (!isHoldingGun)
                            anim.Play("playerIdle");
                    }
                }
            }

            if (timeTillWarning == 1f && !scenario.isTyping())
            {
                if (distanceFromEnemy < warningDist)
                {
                    if(scenario.GetCurrentNode() != warningShot1 && scenario.GetCurrentNode() != warningShot2 && scenario.GetCurrentNode() != warningShot3)
                    scenario.nodeBeforeWarning = scenario.GetCurrentNode();
                    if (isHoldingGun)
                    {
                        if (!warnedAboutGun)
                        {
                            scenario.StartNewNode(handOnGun);
                            warnedAboutGun = true;
                        }
                        else
                        {
                            if (distanceFromEnemy < 6f)
                                scenario.StartNewNode(shotInFoot);

                            else
                                scenario.StartNewNode(shotBcGun);

                            enemyAnim.Play("enemyNervousFire");
                            anim.Play("playerShot");
                            AudioSource.PlayClipAtPoint(missShot, transform.position, 0.5f);
                            timeTillShot = 2f;
                            wasShot = true;
                        }
                    }
                    else if (!didShootFloor)
                    {
                        switch (scenario.warnings)
                        {
                            case 0:
                                scenario.StartNewNode(warningShot1);
                                scenario.warnings++;
                                break;
                            case 1:
                                scenario.StartNewNode(warningShot2);
                                scenario.warnings++;
                                break;
                            case 2:
                                scenario.StartNewNode(warningShot3);
                                scenario.warnings++;
                                break;
                            case 3:
                                enemyAnim.Play("enemyNervousFire");
                                anim.Play("playerShot");
                                AudioSource.PlayClipAtPoint(missShot, transform.position, 0.5f);
                                timeTillShot = 2f;
                                scenario.warnings++;
                                wasShot = true;
                                break;
                        }
                    }
                }
                else
                {
                    if(scenario.warnings > 0 && !string.IsNullOrEmpty(scenario.nodeBeforeWarning) )
                    {
                        scenario.StartNewNode(scenario.nodeBeforeWarning);
                        //scenario.nodeBeforeWarning = "";
                    }
                   
                }
                timeTillWarning -= 0.2f;
            }

            if (!scenario.isRunning() && timeTillWarning < 1f && timeTillWarning > 0f)
            {
                timeTillWarning -= Time.deltaTime;
            }

            if (timeTillWarning <= 0f)
            {
                timeTillWarning = 1f;
            }

            if (timeTillShot > 0 && timeTillShot < 2.1f)
            {
                timeTillShot -= Time.deltaTime;
            }
            else if (timeTillShot < 0f)
            {
                if (scenario.warnings >= 3)
                {
                    if (distanceFromEnemy < 6f)
                    {
                        scenario.StartNewNode(shotInFoot);
                        wasShot = true;
                    }
                    else
                    {
                        didShootFloor = true;
                        scenario.StartNewNode(shotFloor);
                    }
                    timeTillShot = 0f;
                }
            }
        }
        else
        {
            if (scenario.saidBlam || scenario.saidYoMama)
            {
                enemyAnim.Play("enemyConfidentFire");
                anim.Play("playerShot");
                AudioSource.PlayClipAtPoint(shot, transform.position, 0.5f);
                wasShot = true;
            }
            else
                if (!scenario.isTyping())
                {
                    enemyAnim.Play("enemyIdle");
                }
        }
    }

    void Move(Vector2 dir)
    {
        transform.position += new Vector3(dir.x * speed, dir.y * speed, 0);
        if (timeTillStep > 0f) timeTillStep -= Time.deltaTime;
        else
        {
            if(didLeft)
                AudioSource.PlayClipAtPoint(footStep, transform.position, 0.5f);
            else
                AudioSource.PlayClipAtPoint(footStep2, transform.position, 0.5f);

            timeTillStep = 0.2f;
        }
        
    }
}
