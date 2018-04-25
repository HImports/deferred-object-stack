# deferred-object-stack
It's a stack you can push stuff onto, that fills itself over time, taking turns with others.

![https://i.imgur.com/KxVOBQl.gif](https://i.imgur.com/KxVOBQl.gif)

Try it:
https://github.com/flimflamgames/deferred-object-stack/raw/master/deferred%20object%20stack/Deferred%20Object%20Stack.unitypackage

This script is mostly uneccessary wasted overhead. I'm fond of it for nostalgic reasons: it marks an occasion upon which I found a reason to use knowledge I felt fading. I was depressed about that. It marks a decision not to listen to toxic people in my life and fight for my own talents instead.

It was in some iteration one of the first things I programmed for Unity. It uses an IEnumerator the way it does because I wanted to understand what Coroutines really were before using them. 

It was also originally generically-typed for Unity "Objects": I cut that rather than writing case-handlers for Rigibodies etc. You'd need the constructors to somehow differentiate for component types, instantiate provided prefabs, but then pull components for the stack refs. Not a huge thing, just exercise I don't need right now and as it was you'd break it passing it anything but GO or Transform.

Additionally, the ObjectStackFiller "expands and retracts" a fixed array as necessary. I was aware of LinQ and structures such as linked lists when I made this. Again, it's built this way as an intentional programming exercise.

This pooler has some purposeful quirks:

*	Designed to be constructed and populated during runtime gameplay.

*	Instantiates objects over time on a shared global interval.

*	All stacks take turns on that interval, creating a fixed constant rate of instantiation for the game.

*	Passed an array of objects, instantiate a random selection of a fixed size.

*	Immediate requests force instantiation. (asking it for stuff over-rides the interval)

*	Attaches new objects to provided parent.

*	Objects are not tracked by the pool. 

*	You can push objects back "onto it." (another script might return things using a tag. No searches, no additional data, no attachment between objects and pool.)
