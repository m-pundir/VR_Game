using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDrawings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnableDrawingsWithDelay());
    }

    // Update is called once per frame
    IEnumerator EnableDrawingsWithDelay()
    {
        yield return new WaitForSeconds(1f);
        GameObject one = GameObject.Find("1");
        GameObject two = GameObject.Find("2");
        GameObject three = GameObject.Find("3");
        GameObject four = GameObject.Find("4");

        one.SetActive(true);
        two.SetActive(true);
        three.SetActive(true);
        four.SetActive(true);
    }
}
