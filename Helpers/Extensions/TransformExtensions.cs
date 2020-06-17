using UnityEngine;

namespace Extensions
{

    public static class TransformExtensions
    {
        /// <summary>
        /// Lerps rectTransform to target position
        /// </summary>
        /// <param name="rectTransform">Rect transform of the object</param>
        /// <param name="targetPosition">End position of the Lerp</param>
        /// <param name="time">Time in seconds</param>
        /// <param name="owner">Class to run the lerp on</param>
        public static Coroutine LerpRectTransform(this RectTransform rectTransform, Vector3 targetPosition, float time, MonoBehaviour owner)
        {
            if (rectTransform == null) return null;

            if (owner != null)
            {
                return owner.StartCoroutine(ExtensionHelpers.LerpRectTransformPositions(rectTransform, targetPosition, time));
            } else
            {
                Debug.Log("Our Owner is null");
                return null;
            }
        }

        /// <summary>
        /// Lerps world space position to targeted position within speed (seconds)
        /// </summary>
        /// <param name="currentTransform">Transform with the starting position</param>
        /// <param name="targetPosition">End position of the lerp</param>
        /// <param name="time">Time in seconds</param>
        /// <param name="owner">Class to run the lerp on</param>
        /// <returns></returns>
        public static Coroutine LerpPosition(this Transform currentTransform, Vector3 targetPosition, float time, MonoBehaviour owner)
        {
            if (currentTransform == null) return null;

            if (owner != null)
            {
                return owner.StartCoroutine(ExtensionHelpers.LerpTransformPositions(currentTransform, targetPosition, time));
            } else
            {
                Debug.Log("Our Owner is null");
                return null;
            }
        }


        /// <summary>
        /// Lerps world space rotation to targeted rotation within speed (seconds)
        /// </summary>
        /// <param name="currentTransform">Transform with the starting rotation</param>
        /// <param name="targetRotation">End rotation of the lerp</param>
        /// <param name="time">Time in seconds</param>
        /// <param name="owner">Class to run the lerp on</param>
        /// <returns></returns>
        public static Coroutine LerpRotation(this Transform currentTransform, Vector3 targetRotation, float time, MonoBehaviour owner)
        {
            if (currentTransform == null) return null;

            if (owner != null)
            {
                return owner.StartCoroutine(ExtensionHelpers.LerpTransformRotations(currentTransform, targetRotation, time)); ;
            } else
            {
                Debug.Log("Our Owner is null");
                return null;
            }
        }

        /// <summary>
        /// Destroys all children of object
        /// </summary>
        /// <param name="obj">Object</param>
        public static void DestroyChildren(this Transform obj)
        {
            Transform[] children = obj.GetComponentsInChildren<Transform>();
            int cLength = children.Length;

            if(cLength < 1)
            {
                return;
            }
            Debug.Log(cLength);

            for (int i = 0; i < cLength; i++)
            {
                if (children[i] != obj)
                {
                    Object.Destroy(children[i].gameObject);
                }
            }
        }
    }
}
