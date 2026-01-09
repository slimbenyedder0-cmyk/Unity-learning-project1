using UnityEngine;
using System.Collections;

public class snakebody : MonoBehaviour
{
    private bool headfollow;
    public GameObject spawnattach;
    public Vector3 Trajectory;
    public bool ledebug;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (headfollow == false)
        {
            StartCoroutine(Headfollow());
        }
    }

    public IEnumerator Headfollow()
    {
        yield return null;
        headfollow = true;
        yield return new WaitForSeconds(0.25f);
        headfollow = false;
        if (Vector3.Distance(transform.position, spawnattach.transform.position) > 0.01f)
        {
            Trajectory = spawnattach.transform.position - transform.position;
            this.GetComponent<Rigidbody>().linearVelocity = Trajectory.normalized * 12;
            //transform.position = Vector3.MoveTowards(transform.position, spawnattach.transform.position, 10f * Time.deltaTime);
        }
    }
}
