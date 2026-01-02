using UnityEngine;
using TMPro;

public class ScoreBoardDisplay : MonoBehaviour
{
    public float valeurtotale;
    public float lastvaleur;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        valeurtotale = 0;
        lastvaleur = valeurtotale;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastvaleur != valeurtotale)
        {
            Debug.Log(valeurtotale);
            this.GetComponent<TMP_Text>().text = "Score: " + valeurtotale.ToString();
            lastvaleur = valeurtotale;
        }
    }
}
