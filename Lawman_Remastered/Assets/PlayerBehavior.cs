using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Floats")]
    [SerializeField]
    float speed, distanceFromEnemy;

    [Space]

    [Header("Booleans")]
    [SerializeField]
    bool isFacingForward, isHoldingGun;

    [Space]
    [SerializeField]
    GameObject enemy;


    SpriteRenderer sr;
    ScenarioHandler scenario;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        scenario = FindObjectOfType<ScenarioHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        distanceFromEnemy = Vector3.Distance(transform.position,enemy.transform.position);

        var dist = new Yarn.Value(Mathf.RoundToInt(distanceFromEnemy));

        scenario.SetValue("$distance", dist);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isHoldingGun = true;
            var val = new Yarn.Value(true);
            scenario.SetValue("$isHoldingGun", val);

            /*   example of yarn variable mess that worked
             *   Debug.Log(yarnVar.GetValue("$didCry").AsBool);
                     if (Input.GetKeyDown(KeyCode.Space))
                         yarnVar.SetValue("$didCry", valueToSet);*/
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Move(Vector2.left);

            isFacingForward = false;
            sr.flipX = false;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            Move(Vector2.right);


            isFacingForward = true;
            sr.flipX = true;
        }
    }

    void Move(Vector2 dir)
    {
        transform.position += new Vector3(dir.x * speed, dir.y * speed, 0);
    }
}
