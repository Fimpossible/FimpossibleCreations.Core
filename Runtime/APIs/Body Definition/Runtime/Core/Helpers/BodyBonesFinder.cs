using UnityEngine;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Class with helper methods which are helpful when finding body bones of undefined characters
    /// </summary>
    public static class BodyBonesFinder 
    {

        public static Transform GetContinousChildTransform(Transform root)
        {
            Transform child = null;

            if ( root.childCount > 0)
            {
                if ( root.childCount == 1 ) return root.GetChild(0);

                // Get child which is continous, if all are continous, return parent of longest chain
                int deepest = 0;
                Transform deepestT = root.GetChild(0);
                
                for ( int i = 0; i < root.childCount; i++)
                {
                    int depth = CountChildChainLength(root.GetChild(i));
                    if ( depth > deepest)
                    {
                        deepest = depth;
                        deepestT = root.GetChild(i);
                    }
                }

                child = deepestT;
            }

            return child;
        }

        public static int CountChildChainLength(Transform root)
        {
            if ( root == null ) return 0;
            if (root.childCount == 0) return 0;

            Transform bottomChild = GetBottomMostChildTransform(root);
            return GetDepth(bottomChild, root);
        }




        #region Transforms Utils


        public static bool IsChildOf(Transform child, Transform parent)
        {
            Transform p = child;
            while (p != null)
            {
                if (p == parent) return true;
                p = p.parent;
            }

            return false;
        }

        public static Transform GetBottomMostChildTransform(Transform parent)
        {
            var allCh = parent.GetComponentsInChildren<Transform>(true);
            int lowest = 0;
            Transform lowestT = parent;

            for (int c = 0; c < allCh.Length; c++)
            {
                if (allCh[c] == parent) continue;

                Transform ch = allCh[c];
                int depth = 0;

                while (ch.parent != parent && ch.parent != null)
                {
                    depth += 1;
                    ch = ch.parent;
                }

                if (depth > lowest)
                {
                    lowest = depth;
                    lowestT = allCh[c];
                }
            }

            return lowestT;
        }


        public static int GetDepth(Transform t, Transform skelRootBone)
        {
            int depth = 0;
            if (t == skelRootBone) return 0;
            if (t == null) return 0;
            if (t.parent == null) return 0;

            while (t != null && t != skelRootBone)
            {
                t = t.parent;
                depth += 1;
            }

            return depth;
        }

        public static Transform GetParent(Transform start, int depth)
        {
            if ( start == null ) return null;
            Transform t = start.parent;

            for (int i = 0; i < depth; i++)
            {
                if (t == null) break;
                t = t.parent;
            }

            return t;
        }

        #endregion


        public static T FindComponentInAllChildren<T>(Transform transformToSearchIn) where T : Component
        {
            foreach (Transform childInDepth in transformToSearchIn.GetComponentsInChildren<Transform>())
            {
                T component = childInDepth.GetComponent<T>();
                if (component) return component;
            }

            return null;
        }

    }
}