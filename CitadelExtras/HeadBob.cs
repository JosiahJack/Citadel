/*
WIP Headbobbing, weapon bobbing, and footsteps NOV 22.

IMPORTANT: In order for headbobbing to work, the Y value "0.84f" for the player MUST be removed in the appropriate script.
*/

using UnityEngine;

[System.Serializable]
public class HeadBob : MonoBehaviour
{
    
    public GameObject capsule = null;
    public Rigidbody rb = null;

    public GameObject Pipe;
    public float bobRate = 10.0f;
    public float bob = 0.02f;

    public float dHeight = 0.84f;
    public float stepRate = 0.5f;
    public float stepCoolDown;

    AudioSource audio;
    public AudioClip[] steps;
    
    float timer = 0;

    
    void Start()
    {
        steps = new AudioClip[4];
        capsule = GameObject.Find("PlayerCapsule");
        rb = capsule.GetComponent<Rigidbody>();
        audio = capsule.GetComponent<AudioSource>();
        Pipe = GameObject.Find("PipeTransformOffset");
    }

    
    void Update()
    {
        if(Mathf.Abs(rb.velocity.x) > 0.1f || Mathf.Abs(rb.velocity.z) > 0.1f )
        {
            //Player is moving
            timer += Time.deltaTime * bobRate;
            transform.localPosition = new Vector3(

                Mathf.Sin(timer) * bob, 
                dHeight + -(Mathf.Sin(timer)) * bob, 
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

        stepCoolDown -= Time.deltaTime;
        

/*
            This isn't done yet

        	if((Mathf.Abs(rb.velocity.x) > 0.1f || Mathf.Abs(rb.velocity.z) > 0.1f ) && stepCoolDown < 0f)
            {
            int r = Random.Range(1, 3);

            if (r == 1) {audio.clip = steps[0];}
            else if (r == 2) {audio.clip = steps[1];}
            else if (r == 3) {audio.clip = steps[2];}  
            else {audio.clip = steps[3];}     
			
            Debug.Log("Random is : " + r);
			audio.Play();
			stepCoolDown = stepRate;
		    }

        Debug.Log(timer);
        Debug.Log(transform.localPosition);
        Debug.Log(Time.deltaTime);
        */
    }
    
}

