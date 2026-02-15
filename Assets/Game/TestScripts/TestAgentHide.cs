using UnityEngine;

public class TestAgentHide : MonoBehaviour
{
    [SerializeField] Worker workerToTest;
    void Awake()
    {
        if(workerToTest == null)
        {
            Debug.LogError($"No Worker Component referenced in {GetType().Name}");// Logs this class as the object printing this message.
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))////GetKeyDown only triggers once when pressed.  GetKey triggers multiple frames
        {
            workerToTest.SetHide(true);
            // workerToTest.safety = 5f;
            Debug.Log($"Worker {workerToTest.GetType().Name} is hiding");
        }
        if(Input.GetKeyDown(KeyCode.N))//GetKeyDown only triggers once when pressed.  GetKey triggers multiple frames
        {
            workerToTest.SetHide(false);
            // workerToTest.safety = 15f;
            Debug.Log($"Worker {workerToTest.GetType().Name} is safe again");
        }
    }
}
