using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> An object pool that delays and spreads out instantiation...
/// <para>  * Is a last-in first-out stack. </para>
/// <para>  * Creates an array of size and instantiates Transform objects to populate it. </para>
/// <para>  * Pooled Transforms are stored starting at index 1. *ducks* </para>
/// <para>  * ObjectStack sends its FillStack IEnumerator to ObjectStackFiller to manage instantiation </para> </summary>
public class TransformStack {

    // local-private:
    private int top = 0;                        // index of next available object...
    private int size;                           // how many objects can the stack reference?
    private int initialized = 0;                // how many have been instantiated?
    private Transform[] stackPrefabs;           // the stack is randomly populated by these,
    private Transform stackParent;              // stacked objects are kept here in the heirarchy...
    private Transform[] objectStack;            // ...and listed for retrieval here
    // accessors:
    public int Size {
        get { return size; }
    }

    /// <summary> Create a new stack, attach new objects to a common parent. </summary>
    public TransformStack(Transform[] Objects, int Size, Transform ParentTransform) {
        objectStack = new Transform[Size + 1];
        stackPrefabs = Objects;
        size = Size;

        // one filler is shared (only when it's needed)
        if (ObjectStackFiller.Instance == null) {
            GameObject go = new GameObject("Object Stack Filler");
            go.AddComponent<ObjectStackFiller>();
        }

        // send an IEnumerator that populates this object pool
        ObjectStackFiller.Instance.AddFillTask(FillStackTask(size));
        stackParent = ParentTransform;
    }

    /// <summary> Run by ObjectStackFiller, take turns instantiating objects with other stacks. </summary>
    private IEnumerator FillStackTask(int Size) 
    {
        while (initialized < Size)
        {
            int r = Random.Range(0, stackPrefabs.Length);
            Transform newObject = Object.Instantiate(stackPrefabs[r], stackParent);
            objectStack[top + 1] = newObject;
            ++initialized;
            ++top;
            yield return null;
        }
    }

    /// <summary> Get GameObject from stack (instantiate immediately if still filling). </summary>
    public Transform PopObject() {

        if (top > 0) {
            Transform sentObject = objectStack[top];
            objectStack[top] = null;
            --top;
            return sentObject;
        }
        else {

            if (initialized < Size) {
                int r = Random.Range(0, stackPrefabs.Length);
                Transform newObject = Object.Instantiate(stackPrefabs[r], stackParent);
                objectStack[top + 1] = newObject;
                ++initialized;
                ++top;
                return newObject;
            }
#           if UNITY_EDITOR
            Debug.Log("(!) Attempt to pop from empty stack.");
#           endif
            return null;
        }
    }

    /// <summary> Push any GameObject back onto the stack. </summary>
    public void PushObject(Transform Object) {

        if (top < size) {
            ++top;
            objectStack[top] = Object;
        }
        else {
#           if UNITY_EDITOR
            Debug.Log("(!) Attempt to push to full stack.");
#           endif
        }
    }
}
