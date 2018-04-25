using UnityEngine;
using System.Collections;

/// <summary> Fills new ObjectStacks over time, spreading out their instantiation...
/// <para>  * Enforce that all instantiating scripts must take turns creating new objects. </para>
/// <para>  * This version runs its own timer. </para></summary>
public class ObjectStackFiller : MonoBehaviour {
    public static ObjectStackFiller Instance;       // (it needs MonoBehaviour to call Instantiate)

    // local-private:
    private static IEnumerator[] fillTasks;         // an enumerated function which populates a stack, provided by the stack
    private static ushort nextStack = 0;            // the next stack that will instantiate an object
    private static float fillTimeFrequency = 1f;    // how often to instantiate one object on one stack
    private static float fillTimeNext = 0;
    private static float time;
    private bool hasTasks = false;
    // accessor:
    public static ushort SetFillFrequency {
        set { fillTimeFrequency = value; }
    }

    /// <summary> Claim singleton. </summary>
    private void Awake() {
        if (Instance == null) { Instance = this; }
        else { Debug.Log("(destroyed) ObjectStackFiller: Second instance of singleton should not exist."); Destroy(this); }
    }

    /// <summary> Instantiate objects on a timer. </summary>
    private void Update() {
        time = Time.time;

        if(hasTasks) {
            if (time >= fillTimeNext) {
                fillTimeNext = time + fillTimeFrequency;

                // run the function filling one stack, removing the task if it is complete
                if (fillTasks[nextStack].MoveNext() == false) {
                    RemoveFillTask(nextStack);
                }

                ++nextStack;
                // if we've reached the end of the array, roll over next stack to be filled
                if (nextStack >= fillTasks.Length) {
                    nextStack = 0;
                }
            }
        }
    }

    /// <summary> Add a stack to be filled, initialize or expand task array as necessary. </summary>
    public void AddFillTask(IEnumerator FillTask) {
        hasTasks = true;

        // fillTasks expands to the minimum required array size
        if (fillTasks == null) {

            fillTasks = new IEnumerator[1] { FillTask };
        }
        else {

            IEnumerator[] tasksExpand = new IEnumerator[fillTasks.Length + 1];
            for (int i = fillTasks.Length - 1; i >= 0; i--) {
                tasksExpand[i] = fillTasks[i];
            }

            tasksExpand[fillTasks.Length] = FillTask;
            fillTasks = tasksExpand;
        }
    }

    /// <summary> Remove a finished task, shrink tasks array, destroy if last task removed. </summary>
    public void RemoveFillTask(int IndexFilled) {

        // if the last task is removed, destroy this singleton
        if (fillTasks.Length == 1) {

            Destroy(transform.gameObject);
            hasTasks = false;
        }
        else {
            // make an array that's 1 smaller
            int fillTasksLength = fillTasks.Length;
            int retractLength = fillTasksLength - 1;
            IEnumerator[] tasksRetract = new IEnumerator[retractLength];

            // swap/move the finished task with the last task
            IEnumerator t = fillTasks[retractLength];
            fillTasks[retractLength] = fillTasks[IndexFilled];
            fillTasks[IndexFilled] = t;

            // copy the new shoter length forward, truncating the finished task
            for (int i = retractLength - 1; i >= 0; i--) {
                tasksRetract[i] = fillTasks[i];
            }

            fillTasks = tasksRetract;

            // don't forget... the iterator might have been on that last spot!
            if (nextStack >= retractLength) {
                nextStack = 0;
            }
        }
    }
}

// ~JayM 4-23-18
// I knew about LinQ List when I made it: I had not programmed in ages and used a fixed array as exercise.