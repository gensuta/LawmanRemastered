using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] bool isHoldingGun;

    [SerializeField] GameObject enemy;


    SpriteRenderer sr;
    ScenarioHandler scenario;

    [SerializeField] int warningDist;

    [Space]
    [Header("Node Names")]
    [SerializeField] string warningShot1;
    public string warningShot2, warningShot3, shotFloor, handOnGun, shotInFoot, blamOne,blamEnd;

    float timeTillWarning = 1f;

    bool warnedAboutGun, didShootFloor;

    [Header("Floats")]
    [SerializeField] float speed;
    [SerializeField] float distanceFromEnemy;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
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
                isHoldingGun = true;
            }
            else
            {
                isHoldingGun = false;
            }

            if (Input.GetKeyDown(KeyCode.B) && scenario.startedMonologue)
            {
                if (!scenario.saidBlam)
                {
                    scenario.StartNewNode(blamOne);
                    scenario.saidBlam = true;
                }
                else
                {
                    scenario.StartNewNode(blamEnd);
                    scenario.gameOver = true;
                }
            }



            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Move(Vector2.left);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                Move(Vector2.right);
            }

            if (distanceFromEnemy < warningDist && timeTillWarning == 1f && !didShootFloor && scenario.isTyping())
            {
                if (isHoldingGun)
                {
                    if (!warnedAboutGun)
                    {
                        scenario.StartNewNode(handOnGun);
                        warnedAboutGun = true;
                    }
                    else
                    {
                        scenario.StartNewNode(shotFloor);
                        didShootFloor = true;
                    }
                }
                else
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
                            if (distanceFromEnemy < 1.5f)
                                scenario.StartNewNode(shotInFoot);
                            else
                                scenario.StartNewNode(shotFloor);
                            scenario.warnings++;
                            didShootFloor = true;
                            break;
                    }
                }
                timeTillWarning -= Time.deltaTime;
            }

            if (!scenario.isTyping() && timeTillWarning < 1f && timeTillWarning > 0f)
            {
                timeTillWarning -= Time.deltaTime;
            }

            if (timeTillWarning <= 0f)
            {
                timeTillWarning = 0f;
            }
        }
    }

    void Move(Vector2 dir)
    {
        transform.position += new Vector3(dir.x * speed, dir.y * speed, 0);
    }
}
