using Unity.VisualScripting;
using UnityEngine;

public class TrashPlayer : MonoBehaviour
{
    private Rigidbody rigidbody;

    public TrashElectromagentic electromagenticField;
    public TrashWind windField;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.rigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //rigidbody.AddForce(new Vector3(1.0f, 0.0f, 0.0f));

        if (Input.GetKey(KeyCode.Q))
        {
            electromagenticField.gameObject.SetActive(true);
        }
        else
        {
            electromagenticField.gameObject.SetActive(false);
        }
        if (Input.GetKey(KeyCode.W))
        {
            windField.gameObject.SetActive(true);
        }
        else
        {
            windField.gameObject.SetActive(false);
        }


        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Translate(new Vector3(-2.0f, 0.0f, 0.0f) * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Translate(new Vector3(2.0f, 0.0f, 0.0f) * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rigidbody.AddForce(new Vector3(0.0f, 2.0f, 0.0f));
        }
    }
}
