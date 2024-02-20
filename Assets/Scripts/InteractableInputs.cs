using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace Interactables
{
	public class InteractableInputs : MonoBehaviour
	{
		public bool grab;
		public bool release;


#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED

        public void OnGrab(InputValue value)
        {
            GrabInput(value.isPressed);
        }

        public void OnRelease(InputValue value)
        {
            ReleaseInput(value.isPressed);
        }
#endif

        public void GrabInput(bool newGrabState)
		{
			grab = newGrabState;
		}

		public void ReleaseInput(bool newReleaseState)
		{
			release = newReleaseState;
		}
	}

}