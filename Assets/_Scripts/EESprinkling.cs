using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UIElements;

public class EESprinkling : MonoBehaviour
{
    [Space]
    [SerializeField] Transform PlacingPosition;

    [Space]
    [SerializeField] GameObject prefabCircle;
    [SerializeField] float placingTimeInterval = 0.5f;
    [SerializeField] bool isColliderEnabledByDefault = false;
    [SerializeField] Color placingColor = Color.gray;

    private bool isPlacing = false;
    private float lastPlacedTime=0;


    private ArrayList freshlyPlaced;
    

    // Start is called before the first frame update
    void Start()
    {
        freshlyPlaced = new ArrayList();
        lastPlacedTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // If the stylus button is pressed or spacebar pressed
        if ( isPlacing || Input.GetKey(KeyCode.Space)) {
            Debug.Log(lastPlacedTime - Time.time);
            if (Time.time - lastPlacedTime > placingTimeInterval)
            {
                lastPlacedTime = Time.time;

                // Creating a circle
                GameObject circle = Instantiate(prefabCircle, PlacingPosition.position, Quaternion.identity);
                circle.transform.localScale = Vector3.one * UnityEngine.Random.Range(0.1f, 2f);
                circle.GetComponent<CircleCollider2D>().enabled = isColliderEnabledByDefault;
                circle.GetComponent<SpriteRenderer>().color = placingColor;

                // Adding the circle to the list of recently Placed circle
                freshlyPlaced.Add(circle);
            }
        }

        // If the stylus button and spacebar are released
        else if ( !isPlacing && !Input.GetKey(KeyCode.Space)) {
            if ( freshlyPlaced.Count > 0)
            {
                foreach (GameObject circle in freshlyPlaced)
                {
                    circle.GetComponent<CircleCollider2D>().enabled = true;
                    circle.GetComponent<SpriteRenderer>().color = Color.black;
                }
                freshlyPlaced.Clear();
            }
        }
        
    }


    public void startPlacing()
    {
        isPlacing = true;
    }

    public void stopPlacing()
    {
        isPlacing = false;
    }

}
