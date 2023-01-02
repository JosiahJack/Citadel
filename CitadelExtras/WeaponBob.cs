using UnityEngine;

// clone of the Headbob Script, works for most weapons.
[System.Serializable]
public class WeaponBob : MonoBehaviour
{
    
    public Mesh mesh = null;
    public GameObject capsule = null;
    public Rigidbody rb = null;

    public GameObject Pipe;
    public float bobRate = 10.0f;
    public float bob = 0.02f;

    public float dHeight = 0.84f;
    public float stepRate = 0.5f;
    public float stepCoolDown;

    
    
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        mesh = this.transform.GetComponent<MeshFilter>().mesh;
        capsule = GameObject.Find("PlayerCapsule");
        rb = capsule.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(rb.velocity.x) > 0.1f || Mathf.Abs(rb.velocity.z) > 0.1f )
        {
            //Player is moving
            timer += Time.deltaTime * bobRate;
            transform.localPosition = new Vector3(

                Mathf.Sin(timer) * bob, 
                dHeight + Mathf.Cos(timer) * bob, 
                transform.localPosition.z

                );
            
        }
        else
        {
            //Idle
            timer = 0;
            transform.localPosition = new Vector3(
                Mathf.Lerp(transform.localPosition.x, 0f, Time.deltaTime * bobRate), 
                Mathf.Lerp(transform.localPosition.y, dHeight, Time.deltaTime * bobRate), 
                transform.localPosition.z);
        }

        

        Debug.Log(timer);
        Debug.Log(transform.localPosition);
        Debug.Log(Time.deltaTime);
    }
}