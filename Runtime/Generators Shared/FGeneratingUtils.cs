using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using FIMSpace.FEditor;
using UnityEditor;
#endif

namespace FIMSpace.FGenerating
{
    public static class FGeneratingUtils
    {

        #region Extensions


        public static Vector2 GetProgessPositionOverLines(List<Vector2Int> pathPoints, float progress)
        {
            float fullLength = 0f;
            for (int p = 0; p < pathPoints.Count - 1; p++)
                fullLength += Vector2.Distance(pathPoints[p], pathPoints[p + 1]);

            float progressLength = fullLength * progress;

            float checkProgr = 0f;
            for (int p = 0; p < pathPoints.Count - 1; p++)
            {
                float currProgr = checkProgr;
                checkProgr += Vector2.Distance(pathPoints[p], pathPoints[p + 1]);

                if (currProgr <= progressLength && checkProgr >= progressLength)
                {
                    float progr = Mathf.InverseLerp(currProgr, checkProgr, progressLength);
                    return Vector2.Lerp(pathPoints[p], pathPoints[p + 1], progr);
                }
            }

            return Vector2.zero;
        }

        public static Vector2 GetDirectionOver(List<Vector2Int> pathPoints, int startId, int endId)
        {
            if (endId < pathPoints.Count)
                return ((Vector2)pathPoints[startId + 1] - (Vector2)pathPoints[startId]).normalized;
            else
                return ((Vector2)pathPoints[startId] - (Vector2)pathPoints[startId - 1]).normalized;
        }

        public static Vector2 GetDirectionOverLines(List<Vector2Int> pathPoints, float progress)
        {
            float fullLength = 0f;
            for (int p = 0; p < pathPoints.Count - 1; p++)
                fullLength += Vector2.Distance(pathPoints[p], pathPoints[p + 1]);

            float progressLength = fullLength * progress;

            float checkProgr = 0f;
            for (int p = 0; p < pathPoints.Count - 1; p++)
            {
                float currProgr = checkProgr;
                checkProgr += Vector2.Distance(pathPoints[p], pathPoints[p + 1]);

                if (currProgr <= progressLength && checkProgr >= progressLength)
                {
                    return ((Vector2)pathPoints[p + 1] - (Vector2)pathPoints[p]).normalized;
                }
            }

            return Vector2.zero;
        }

        #endregion


        #region Vectors

        /// <summary> V2Int ToBound V3 </summary>
        public static Vector3 V2toV3Bound(this Vector2Int v, float y = 0f)
        {
            if (y == 0f) return new Vector3(v.x - 0.5f, y, v.y - 0.5f);
            return new Vector3(v.x, y, v.y);
        }

        public static Vector3 V2toV3(this Vector2Int v, float y = 0f)
        {
            return new Vector3(v.x, y, v.y);
        }

        public static Vector3 V2toV3(this Vector2 v, float y = 0f)
        {
            return new Vector3(v.x, y, v.y);
        }

        public static Vector2Int V2toV2Int(this Vector2 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }

        public static Vector3Int V2toV3Int(this Vector2Int v, int y = 0)
        {
            return new Vector3Int(v.x, y, v.y);
        }

        public static Vector2 V3toV2(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        /// <summary>
        /// Resetting local position, rotation, scale to zero on 1,1,1 (defaults)
        /// </summary>
        //public static void ResetCoords(this Transform t)
        //{
        //    t.localPosition = Vector3.zero;
        //    t.localRotation = Quaternion.identity;
        //    t.localScale = Vector3.one;
        //}

        public static Vector3 V3Divide(this Vector3 v, Vector3 divBy)
        {
            return new Vector3(
                divBy.x == 0 ? v.x : v.x / divBy.x, 
                divBy.y == 0 ? v.y : v.y / divBy.y, 
                divBy.z == 0 ? v.z : v.z / divBy.z);
        }


        public static Vector2Int V3toV2Int(this Vector3 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.z));
        }

        public static Vector3Int V3toV3Int(this Vector3 v)
        {
            return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
        }

        public static Vector3Int V3toV3IntC(this Vector3 v)
        {
            return new Vector3Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z));
        }

        public static Vector3Int V3toV3IntF(this Vector3 v)
        {
            return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
        }

        public static Vector3 V3IntToV3(this Vector3Int v)
        {
            return new Vector3((float)(v.x), (float)(v.y), (float)(v.z));
        }

        public static Vector2Int V3IntToV2Int(this Vector3Int v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }

        /// <summary> Just to avoid errors in unity 2018.4 </summary>
        public static Vector3Int InverseV3Int(this Vector3Int v)
        {
            return new Vector3Int(-v.x, -v.y, -v.z);
        }

        public static int ToInt(this float v)
        {
            return Mathf.RoundToInt(v);
        }


        /// <summary>
        /// If direction is left/right then size is y * cellSize etc.
        /// </summary>
        public static Vector2 GetDirectionalSize(Vector2Int dir, int cellsSize)
        {
            if (cellsSize <= 1) return Vector2.one;
            if (dir.x != 0) return new Vector2(1, cellsSize);
            else return new Vector2(cellsSize, 1);
        }

        /// <summary>
        /// Getting   1,0  -1,0   0,1   0,-1
        /// </summary>
        public static Vector2Int GetFlatDirectionFrom(Vector2Int vect)
        {
            if (vect.x != 0) return new Vector2Int(vect.x, 0);
            else return new Vector2Int(0, vect.y);
        }


        public static Vector3Int GetRandomDirection()
        {
            int r = FGenerators.GetRandom(0, 5);
            if (r == 0) return new Vector3Int(1, 0, 0);
            else if (r == 1) return new Vector3Int(0, 0, 1);
            else if (r == 2) return new Vector3Int(-1, 0, 0);
            else return new Vector3Int(0, 0, -1);
        }


        /// <summary>
        /// Getting   1,0  -1,0   0,1   0,-1
        /// </summary>
        public static Vector2Int GetRotatedFlatDirectionFrom(Vector2Int vect)
        {
            if (vect.x != 0) return new Vector2Int(0, vect.x);
            else return new Vector2Int(vect.y, 0);
        }

        public static Vector3Int GetRotatedFlatDirectionFrom(Vector3Int vect)
        {
            if (vect.x != 0) return new Vector3Int(0, 0, vect.x);
            else return new Vector3Int(vect.z, 0, 0);
        }

        public static void TransferFromListToList<T>(List<T> from, List<T> to, bool checkForDuplicates = false) 
        {
            if (to == null) return;
            if (from == null) return;

            if (!checkForDuplicates)
            {
                for (int i = 0; i < from.Count; i++)
                    to.Add(from[i]);
            }
            else
            {
                for (int i = 0; i < from.Count; i++)
                    if (!to.Contains(from[i]))
                        to.Add(from[i]);
            }
        }

#if UNITY_EDITOR
        public static List<T> GetAllSelected<T>(T ignore) where T : MonoBehaviour
        {
            List<T> selected = new List<T>();

            for (int i = 0; i < UnityEditor.Selection.gameObjects.Length; i++)
            {
                T p = UnityEditor.Selection.gameObjects[i].GetComponent<T>();
                if (p != ignore) if (p) selected.Add(p);
            }

            return selected;
        }
#endif

        public enum ERoundingMode
        {
            Floor, Round, Ceil
        }

        public static Vector3Int Round(Vector3 toRound, ERoundingMode mode)
        {
            if (mode == ERoundingMode.Floor) return toRound.V3toV3IntF();
            else if (mode == ERoundingMode.Round) return toRound.V3toV3Int();
            else return toRound.V3toV3IntC();
        }


        #endregion


        #region Core Utilities


        public static void CheckForNulls<T>(List<T> classes)
        {
            for (int i = classes.Count - 1; i >= 0; i--)
            {
                if (classes[i] == null) classes.RemoveAt(i);
            }
        }

        public static void CheckForNullsO<T>(List<T> objects) where T : UnityEngine.Object
        {
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                if (objects[i] == null)
                {
                    objects.RemoveAt(i);
                }
            }
        }

        public static void AdjustCount<T>(List<T> list, int targetCount, bool addNulls = false) where T : class, new()
        {
            if (list.Count == targetCount) return;

            if (list.Count < targetCount)
            {
                if (addNulls)
                {
                    while (list.Count < targetCount) list.Add(null);
                }
                else
                {
                    while (list.Count < targetCount) list.Add(new T());
                }
            }
            else
            {
                while (list.Count > targetCount) list.RemoveAt(list.Count - 1);
            }

            //if (list.Count < targetCount)
            //{
            //    for (int i = 0; i < targetCount - list.Count; i++)
            //    {
            //        if (addNulls)
            //            list.Add(null);
            //        else
            //            list.Add(new T());
            //    }
            //}
            //else
            //{
            //    for (int i = 0; i < list.Count - targetCount; i++)
            //    {
            //        list.RemoveAt(list.Count - 1);
            //    }
            //}
        }

        public static void AdjustUnityObjCount<T>(List<T> list, int targetCount, T defaultObj = null) where T : UnityEngine.Object
        {
            if (list.Count == targetCount) return;

            //if (list.Count < targetCount)
            //{
            //    for (int i = 0; i < targetCount - list.Count; i++)
            //    {
            //        if (defaultObj == null)
            //            list.Add(null);
            //        else
            //            list.Add(GameObject.Instantiate(defaultObj) );
            //    }
            //}
            //else
            //{
            //    for (int i = 0; i < list.Count - targetCount; i++)
            //    {
            //        list.RemoveAt(list.Count - 1);
            //    }
            //}

            if (list.Count < targetCount)
            {
                if (defaultObj == null)
                {
                    while (list.Count < targetCount) list.Add(null);
                }
                else
                {
                    while (list.Count < targetCount) list.Add(GameObject.Instantiate(defaultObj));
                }
            }
            else
            {
                while (list.Count > targetCount) list.RemoveAt(list.Count - 1);
            }
        }

        public static void AdjustStructsListCount<T>(List<T> list, int targetCount, T add) where T : struct
        {
            if (list.Count == targetCount) return;

            if (list.Count < targetCount)
            {
                while (list.Count < targetCount) list.Add(add);
            }
            else
            {
                while (list.Count > targetCount) list.RemoveAt(list.Count - 1);
            }

            //if (list.Count < targetCount)
            //{
            //    for (int i = 0; i < targetCount - list.Count; i++)
            //    {
            //        list.Add(add);
            //    }
            //}
            //else
            //{
            //    for (int i = 0; i < list.Count - targetCount; i++)
            //    {
            //        list.RemoveAt(list.Count - 1);
            //    }
            //}
        }

        #endregion


        #region Editor Textures and Icons

#if UNITY_EDITOR
        public static Texture2D Tex_Selector { get { if (__texSelctr != null) return __texSelctr; __texSelctr = Resources.Load<Texture2D>("SPR_MultiSelect"); return __texSelctr; } }
        private static Texture2D __texSelctr = null;

        public static Texture TEX_FolderDir { get { if (__Tex_folderdir != null) return __Tex_folderdir; __Tex_folderdir = EditorGUIUtility.IconContent("Folder Icon").image; return __Tex_folderdir; } }
        private static Texture __Tex_folderdir = null;

        public static Texture TEX_CellInstr { get { if (__texCellInstr != null) return __texCellInstr; __texCellInstr = Resources.Load<Texture2D>("SPR_CellCommand"); return __texCellInstr; } }
        private static Texture __texCellInstr = null;

        public static Texture TEX_Axes { get { if (__texAxes != null) return __texAxes; __texAxes = Resources.Load<Texture2D>("Fimp/Misc Icons/SPR_Axes"); return __texAxes; } }
        private static Texture __texAxes = null;
        public static Texture TEX_DragBars { get { if (__texdragbr != null) return __texdragbr; __texdragbr = Resources.Load<Texture2D>("Fimp/Misc Icons/SPR_DragBars"); return __texdragbr; } }
        private static Texture __texdragbr = null;

        public static Texture TEX_MenuIcon { get { if (__Tex_menuDir != null) return __Tex_menuDir; __Tex_menuDir = FGUIResources.Tex_MoreMenu; return __Tex_menuDir; } }
        private static Texture __Tex_menuDir = null;

        public static Texture TEX_IndentTree { get { if (__Tex_indtree != null) return __Tex_indtree; __Tex_indtree = Resources.Load<Texture2D>("Fimp/Misc Icons/SPR_IndentTreeLine"); return __Tex_indtree; } }
        private static Texture __Tex_indtree = null;
        public static Texture TEX_IndentTreeH { get { if (__Tex_indtreeh != null) return __Tex_indtreeh; __Tex_indtreeh = Resources.Load<Texture2D>("Fimp/Misc Icons/SPR_IndentTreeLineH"); return __Tex_indtreeh; } }
        private static Texture __Tex_indtreeh = null;
        public static Texture TEX_PrintIcon { get { if (__texPrnt != null) return __texPrnt; __texPrnt = Resources.Load<Texture2D>("SPR_PlanPrint"); return __texPrnt; } }
        private static Texture __texPrnt = null;
        public static Texture TEX_ModGraphIcon { get { if (__texMdGr != null) return __texMdGr; __texMdGr = Resources.Load<Texture2D>("SPR_ModNodeSmall"); return __texMdGr; } }
        private static Texture __texMdGr = null;
        public static Texture TEX_PGGIcon { get { if (__texPGGico != null) return __texPGGico; __texPGGico = Resources.Load<Texture2D>("SPR_PGGIcon"); return __texPGGico; } }
        private static Texture __texPGGico = null;
        public static Texture TEX_FieldIcon { get { if (__texFieldico != null) return __texFieldico; __texFieldico = Resources.Load<Texture2D>("SPR_FieldDesigner"); return __texFieldico; } }
        private static Texture __texFieldico = null;
        public static Texture TEX_Prepare { get { if (__texPrepr != null) return __texPrepr; __texPrepr = Resources.Load<Texture2D>("SPR_Prepare"); return __texPrepr; } }
        private static Texture __texPrepr = null;


        //public static GUIContent _PlannerIconOld { get { if (tex_plannerIconOld == null) tex_plannerIconOld = new GUIContent(Resources.Load<Texture2D>("SPR_PGG")); return tex_plannerIconOld; } }
        //private static GUIContent tex_plannerIconOld = null;

        public static Texture _Tex_ModPackSmall { get { if (__texModPackSml == null) __texModPackSml = (Resources.Load<Texture2D>("PR_ModPackSmall")); return __texModPackSml; } }
        private static Texture __texModPackSml = null;
        public static Texture _Tex_ModsSmall { get { if (__texModsSml == null) __texModsSml = (Resources.Load<Texture2D>("SPR_ModificationSmall")); return __texModsSml; } }
        private static Texture __texModsSml = null;
        public static Texture _Tex_Mod { get { if (__texMod == null) __texMod = (Resources.Load<Texture2D>("SPR_ModificationMid")); return __texMod; } }
        private static Texture __texMod = null;
        public static Texture _Tex_ModRule { get { if (__texModRle == null) __texModRle = (Resources.Load<Texture2D>("SPR_FieldRuleV2")); return __texModRle; } }
        private static Texture __texModRle = null;

        public static GUIContent _PlannerIcon { get { if (tex_plannerIcon == null) tex_plannerIcon = new GUIContent(Resources.Load<Texture2D>("PGG_PlannerSmall")); return tex_plannerIcon; } }
        private static GUIContent tex_plannerIcon = null;

        public static GUIContent _PlannerIconGray { get { if (tex_plannerIconGr == null) tex_plannerIconGr = new GUIContent(Resources.Load<Texture2D>("PGG_PlannerSmallGray")); return tex_plannerIconGr; } }
        private static GUIContent tex_plannerIconGr = null;

        private static GUIContent tex_cellIcon = null;
        public static GUIContent _CellIcon
        {
            get
            {
                if (tex_cellIcon == null) tex_cellIcon = new GUIContent(Resources.Load<Texture2D>("SPR_FieldCell"));
                return tex_cellIcon;
            }
        }

        public static void SetDarkerBacgroundOnLightSkin()
        {
            if (EditorGUIUtility.isProSkin == false)
            {
                Color preCol = GUI.color;
                GUI.color = new Color(0f, 0f, 0f, 1f);
                EditorGUILayout.BeginVertical(FGUIResources.FrameBoxStyle);
                GUI.color = preCol;
            }
        }

        public static void EndVerticalIfLightSkin()
        {
            if (EditorGUIUtility.isProSkin == false)
            {
                EditorGUILayout.EndVertical();
            }
        }

#endif

        #endregion


        #region Editor GUI walkarounds


#if UNITY_EDITOR

        private static int[] _editor_ignoredExceptionCodes = null;
        public static int[] _Editor_GetIgnoredExceptions()
        {
            if (_editor_ignoredExceptionCodes == null)
            {
                _editor_ignoredExceptionCodes = new int[]
                    {-2147024809, -2146233088 };
            }

            return _editor_ignoredExceptionCodes;
        }

        public static bool _Editor_IsExceptionIgnored(System.Exception exc)
        {
            if (exc == null) return true;

            var codes = _Editor_GetIgnoredExceptions();
            for (int i = 0; i < codes.Length; i++)
            {
                if (exc.HResult == codes[i]) return true;
            }

            return false;
        }

#endif

        #endregion

    }
}