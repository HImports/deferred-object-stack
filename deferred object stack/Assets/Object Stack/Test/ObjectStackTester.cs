using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Click a UI button to create a new TransformStack for testing purposes. </summary>
public class ObjectStackTester : MonoBehaviour {

    // inspector-public:
    public Camera MainCamera;                           // cast som rays from the camera for fun
    public Material GreenMat;                           // marker objects turn green when active
    public InputField StackSizeField;                   // number of objects for created pool to instantiate
    public Toggle CubeToggle;                           // radio buttons to choose which objects to include
    public Toggle CylinderToggle;
    public Toggle IcosphereToggle;
    public Transform[] Cubes = new Transform[3];        // objects to populate said stacks with
    public Transform[] Cylinders = new Transform[3];
    public Transform[] Icospheres = new Transform[3];
    // local-private:
    Transform[] stackTransforms = new Transform[15];        // new transforms will be attached/positioned here
    TransformStack[] ObjectStacks = new TransformStack[15]; // in practice this ref is used (stack is not a component)
    int nextStackTransform = 0;                             // iterator for the next new stack marker

    float fountainRate = 0.1f;                              // spew stuff from stacks in a fountain on click
    float fountainNext = 0f;
    float time = 0f;
    int fountainStackNext = 0;

    /// <summary> Get refs to objects which serve as stack parents. </summary>
    private void Awake() {

        // get the parent objects
        int length = stackTransforms.Length - 1;
        for (int i = 0; i <= length; ++i) {
            stackTransforms[i] = transform.GetChild(i);
        }
        // start with a valid field entry...
        StackSizeField.text = "100";
    }

    /// <summary> Launch objects in a fountain when clicking. </summary>
    public void Update() {
        time = Time.time;

        if (time >= fountainNext) {
            fountainNext = time + fountainRate;

            if (Input.GetMouseButton(0)) {
                LaunchStackedObject();
            }
        }
    }

    /// <summary> Just the constructor for the sake of example brevity. </summary>
    public TransformStack StackConstructor(Transform[] StuffToPool, int HowMuchStuff, Transform ParentObject) {
        TransformStack newStack = new TransformStack(StuffToPool, HowMuchStuff, ParentObject);
        return newStack;       
    }

    /// <summary> Start a new ObjectStack attached to the next test transform. </summary>
    public void CreateNewStack() {

        // first get the lenght of the array necessary to hold our requested prefab mix
        int prefabsToPoolLength = 0;
        if(CubeToggle.isOn) {
            prefabsToPoolLength += Cubes.Length;
        }
        if(CylinderToggle.isOn) {
            prefabsToPoolLength += Cylinders.Length;
        }
        if (IcosphereToggle.isOn) {
            prefabsToPoolLength += Icospheres.Length;
        }
        // then copy those into one larger array
        Transform[] poolPrefabs = new Transform[prefabsToPoolLength];
        prefabsToPoolLength = 0; // will now act as next-iterator
        if(CubeToggle.isOn) {
            for(int i = Cubes.Length - 1; i >= 0; --i) {
                poolPrefabs[prefabsToPoolLength] = Cubes[i];
                ++prefabsToPoolLength;
            }
        }
        if(CylinderToggle.isOn) {
            for (int i = Cylinders.Length - 1; i >= 0; --i) {
                poolPrefabs[prefabsToPoolLength] = Cylinders[i];
                ++prefabsToPoolLength;
            }
        }
        if (IcosphereToggle.isOn) {
            for (int i = Cylinders.Length - 1; i >= 0; --i) {
                poolPrefabs[prefabsToPoolLength] = Icospheres[i];
                ++prefabsToPoolLength;
            }
        }
        // break and error if nothing was chosen to instantiate
        if(prefabsToPoolLength == 0) {
            Debug.Log("Pick something to instantiate.");
            return;
        }

        // arbitrary max 15 for this test scene is enough to get the point across
        if (nextStackTransform <= 14) {

            // get the parent transform and light it up green as illustration
            Transform t = stackTransforms[nextStackTransform];
            Renderer r = t.GetComponent<Renderer>();
            r.sharedMaterial = GreenMat;

            int newStackSize = int.Parse(StackSizeField.text);
            //TransformStack newStack = new TransformStack(poolPrefabs, newStackSize, t);
            TransformStack newStack = StackConstructor(poolPrefabs, newStackSize, t);
            ObjectStacks[nextStackTransform] = newStack;
            ++nextStackTransform;
        }
    }

    /// <summary> Launch objects from the stacks rapidly in a fountain. </summary>
    public void LaunchStackedObject() {

        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);     // lol Unity is fun
        RaycastHit hit;
        Transform t;
        Rigidbody r;
        Vector3 up = Vector3.up;
        if (Physics.Raycast(ray, out hit)) {

            // blobviously this is dumb: it's just an example for fun in 10 minutes cmon now
            if(ObjectStacks[fountainStackNext] != null) {

                // this could return a bool flag so you don't have to do a null check
                t = ObjectStacks[fountainStackNext].PopObject();
                if (t != null) {

                    // Originally, this script used C# generics. It can be adjusted to directly reference rigidbodies.
                    r = t.GetComponent<Rigidbody>();
                    t.position = hit.point + up;
                    Vector3 fountain = new Vector3(Random.Range(-6f, 6f), 12f, Random.Range(-6f, 6f));
                    r.AddForce(fountain, ForceMode.VelocityChange);
                }

                ++fountainStackNext;
                if(fountainStackNext >= nextStackTransform) {
                    fountainStackNext = 0;
                }
            }
        }
    }
}
