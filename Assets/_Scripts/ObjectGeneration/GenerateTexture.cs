using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GenerateTexture : MonoBehaviour
{
    public Texture2D Stippled;
    public GameObject point;

    private GameObject newPoint;
    // Start is called before the first frame update
    void Start()
    {
        point.SetActive(false);
        if (Stippled != null)
        {
            float x = 0f;
            float y = 0f;

            float divideI = Stippled.height/this.GetComponent<Renderer>().bounds.size.y;
            float divideJ = Stippled.width/this.GetComponent<Renderer>().bounds.size.x;

            for (int i = 0; i < Stippled.width; i++)
            {
                if (i <= Stippled.height / 2)
                {
                    x = -1.5f + (float)i/(divideI);
                }
                else
                {
                    x = 1.5f - (float)i / (divideI);
                }
                for (int j = 0; j < Stippled.height; j++)
                {
                    if (j <= Stippled.width / 2)
                    {
                        y = -1.5f + (float)j / (divideJ);
                    }
                    else
                    {
                        y = 1.5f - (float)j / (divideJ);
                    }

                    Color color = Stippled.GetPixel(i, j);

                   if (color.r <= 0.8)
                   {
                        if (i < Stippled.height / 2 && j < Stippled.width / 2)
                        {
                            newPoint = Instantiate(point, new Vector3(transform.position.x - x, transform.position.y - y, transform.position.z), transform.rotation);
                        }
                        else if (i >= Stippled.height / 2 && j < Stippled.width / 2)
                        {
                            newPoint = Instantiate(point, new Vector3(transform.position.x + x, transform.position.y - y, transform.position.z), transform.rotation);

                        }
                        else if (i < Stippled.height / 2 && j >= Stippled.width / 2)
                        {
                            newPoint = Instantiate(point, new Vector3(transform.position.x - x, transform.position.y + y, transform.position.z), transform.rotation);

                        }
                        else
                        {
                            newPoint = Instantiate(point, new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z), transform.rotation);
                        }
                        newPoint.SetActive(true);
                        //Color pointColor = newPoint.GetComponent<Material>().color;
                        //pointColor.r = 0;
                    }
                    //Debug.Log("This is X: " + x + "  " + "This is Y: " + y);
                }
            }
        }
        else
        {
            Debug.Log("The texture is null!");
        }
    }
        
}