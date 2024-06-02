#if UNITY_EDITOR

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FIMSpace.FEditor
{

    public static class FGizmosHandles
    {

        public static void DrawArrow( Vector3 position, Quaternion direction, float scale = 1f, float width = 5f, float stripeLength = 1f )
        {
            Vector3[] points = new Vector3[8];

            // Low base dots
            points[0] = new Vector3( -0.12f, 0f, 0f );
            points[1] = new Vector3( 0.12f, 0f, 0f );

            // Pre tip right triangle dot
            points[2] = new Vector3( 0.12f, 0f, 0.4f + 1 * stripeLength );
            // Tip right side
            points[3] = new Vector3( 0.4f, 0f, 0.32f + 1 * stripeLength );
            // Tip
            points[4] = new Vector3( 0.0f, 0f, 1f + 1 * stripeLength );
            // Tip left side
            points[5] = new Vector3( -0.4f, 0f, 0.32f + 1 * stripeLength );
            // Pre tip left triangle dot
            points[6] = new Vector3( -0.12f, 0f, 0.4f + 1 * stripeLength );
            points[7] = points[0];


            Matrix4x4 rotation = Matrix4x4.TRS( Vector3.zero, direction, Vector3.one * scale );

            for( int i = 0; i < points.Length; i++ )
            {
                points[i] = rotation.MultiplyPoint( points[i] );
                points[i] += position;
            }

            if( width <= 0f )
                Handles.DrawPolyLine( points );
            else
                Handles.DrawAAPolyLine( width, points );
        }

        public static void DrawBoneHandle( Vector3 from, Vector3 to, Vector3 forward, float fatness = 1f, float width = 1f, float arrowOffset = 1f, float lineWidth = 1f, float fillAlpha = 0f )
        {
            Vector3 dir = ( to - from );
            float ratio = dir.magnitude / 7f; ratio *= fatness;
            float baseRatio = ratio * 0.75f * arrowOffset;
            ratio *= width;
            Quaternion rot = ( dir == Vector3.zero ? rot = Quaternion.identity : rot = Quaternion.LookRotation( dir, forward ) );
            dir.Normalize();

            Vector3 p = from + dir * baseRatio;

            if( lineWidth <= 1f )
            {
                Handles.DrawLine( from, to );
                Handles.DrawLine( to, p + rot * Vector3.right * ratio );
                Handles.DrawLine( from, p + rot * Vector3.right * ratio );
                Handles.DrawLine( to, p - rot * Vector3.right * ratio );
                Handles.DrawLine( from, p - rot * Vector3.right * ratio );
            }
            else
            {
                Handles.DrawAAPolyLine( lineWidth, from, to );
                Handles.DrawAAPolyLine( lineWidth, to, p + rot * Vector3.right * ratio );
                Handles.DrawAAPolyLine( lineWidth, from, p + rot * Vector3.right * ratio );
                Handles.DrawAAPolyLine( lineWidth, to, p - rot * Vector3.right * ratio );
                Handles.DrawAAPolyLine( lineWidth, from, p - rot * Vector3.right * ratio );
            }

            if( fillAlpha > 0f )
            {
                Color preC = Handles.color;
                Handles.color = new Color( preC.r, preC.g, preC.b, fillAlpha * preC.a );
                Handles.DrawAAConvexPolygon( from, p + rot * Vector3.right * ratio, to, p - rot * Vector3.right * ratio, from );
                Handles.color = preC;
            }
        }

        public static void DrawBoneHandle( Vector3 from, Vector3 to, float fatness = 1f, bool faceCamera = false, float width = 1f, float arrowOffset = 1f, float lineWidth = 1f, float fillAlpha = 0f )
        {
            Vector3 forw = ( to - from ).normalized;

            if( faceCamera )
            {
                if( SceneView.lastActiveSceneView != null )
                    if( SceneView.lastActiveSceneView.camera )
                        forw = ( to - SceneView.lastActiveSceneView.camera.transform.position ).normalized;
            }

            DrawBoneHandle( from, to, forw, fatness, width, arrowOffset, lineWidth, fillAlpha );
        }

        public static void DrawRay( Vector3 pos, Vector3 dir )
        {
            Handles.DrawLine( pos, pos + dir );
        }

        public static void DrawDottedRay( Vector3 pos, Vector3 dir, float scale = 2f )
        {
            Handles.DrawDottedLine( pos, pos + dir, scale );
        }

        /// <summary>
        /// [To be executed in OnSceneGUI()]
        /// Drawing sphere handle in scene view with controll ability
        /// </summary>
        public static Vector3 DrawAndSetPositionForHandle( Vector3 position, Transform rootReference )
        {
            EditorGUI.BeginChangeCheck();

            Handles.color = Color.green;
            Quaternion rotation = ( UnityEditor.Tools.pivotRotation != UnityEditor.PivotRotation.Local ) ? Quaternion.identity : rootReference.rotation;

            float size = HandleUtility.GetHandleSize( position ) * 0.125f;
            Handles.SphereHandleCap( 0, position, rotation, size, UnityEngine.EventType.Repaint );
            Vector3 pos = Handles.PositionHandle( position, rotation );

            return pos;
        }

        /// <summary>
        /// [To be executed in OnSceneGUI()]
        /// Drawing sphere handle in scene view without option to controll it but clickable
        /// Returns true if mouse clicked on handle
        /// </summary>
        public static bool DrawSphereHandle( Vector3 position, string text = "" )
        {
            bool clicked = false;

            if( Event.current.button != 1 )
            {
                Handles.color = Color.white;

                float size = HandleUtility.GetHandleSize( position ) * 0.2f;

                if( Handles.Button( position, Quaternion.identity, size, size, Handles.SphereHandleCap ) )
                {
                    clicked = true;
                    InternalEditorUtility.RepaintAllViews();
                }

                Handles.BeginGUI();

                Vector2 labelSize = new Vector2( EditorGUIUtility.singleLineHeight * 2, EditorGUIUtility.singleLineHeight );
                Vector2 labelPos = HandleUtility.WorldToGUIPoint( position );

                labelPos.y -= labelSize.y / 2;
                labelPos.x -= labelSize.x / 2;

                GUILayout.BeginArea( new Rect( labelPos, labelSize ) );
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.black;
                style.alignment = UnityEngine.TextAnchor.MiddleCenter;
                GUILayout.Label( new GUIContent( text ), style );
                GUILayout.EndArea();

                Handles.EndGUI();

            }

            return clicked;
        }



        public static Quaternion RotationHandle( Quaternion rotation, Vector3 position, float size = 1f, bool worldScale = false )
        {
            float handleSize = size;
            if( worldScale ) handleSize = HandleUtility.GetHandleSize( position ) * size;

            Color color = Handles.color;
            Handles.color = Handles.xAxisColor;
            rotation = Handles.Disc( rotation, position, rotation * Vector3.right, handleSize, true, 1f );
            Handles.color = Handles.yAxisColor;
            rotation = Handles.Disc( rotation, position, rotation * Vector3.up, handleSize, true, 1f );
            Handles.color = Handles.zAxisColor;
            rotation = Handles.Disc( rotation, position, rotation * Vector3.forward, handleSize, true, 1f );
            Handles.color = Handles.centerColor;
            rotation = Handles.Disc( rotation, position, Camera.current.transform.forward, handleSize * 1.1f, false, 0f );
            rotation = Handles.FreeRotateHandle( rotation, position, handleSize );
            Handles.color = color;
            return rotation;
        }

        public static Vector3 ScaleHandle( Vector3 scale, Vector3 position, Quaternion rotation, float size, bool scaleAll = false, bool worldScale = false, bool drawX = true, bool drawY = true, bool drawZ = true )
        {
            float handleSize = size;
            if( worldScale ) handleSize = HandleUtility.GetHandleSize( position ) * size;
            float preScaleX = scale.x;

            if( !scaleAll )
            {
                if( drawX )
                {
                    Handles.color = Handles.xAxisColor;
                    scale.x = Handles.ScaleSlider( scale.x, position, rotation * Vector3.right, rotation, handleSize, 0.001f );
                }

                if( drawY )
                {
                    Handles.color = Handles.yAxisColor;
                    scale.y = Handles.ScaleSlider( scale.y, position, rotation * Vector3.up, rotation, handleSize, 0.001f );
                }

                if( drawZ )
                {
                    Handles.color = Handles.zAxisColor;
                    scale.z = Handles.ScaleSlider( scale.z, position, rotation * Vector3.forward, rotation, handleSize, 0.001f );
                }
            }

            Handles.color = Handles.centerColor;
            EditorGUI.BeginChangeCheck();
            float num1 = Handles.ScaleValueHandle( scale.x, position, rotation, handleSize, Handles.CubeHandleCap, 0.001f );

            if( EditorGUI.EndChangeCheck() )
            {
                float num2 = num1 / scale.x;
                scale.x = num1;
                scale.y *= num2;
                scale.z *= num2;
            }

            return scale;
        }

        public static Vector3 PositionHandle( Vector3 position, Quaternion rotation, float size = 1f, bool worldScale = false, bool freeHandle = true, bool colorize = true )
        {
            float handleSize = size;
            if( worldScale ) handleSize = HandleUtility.GetHandleSize( position ) * size;

            Color color = Handles.color;

            if( colorize ) Handles.color = Handles.xAxisColor;
            position = Handles.Slider( position, rotation * Vector3.right, handleSize, Handles.ArrowHandleCap, 0.001f );
            if( colorize ) Handles.color = Handles.yAxisColor;
            position = Handles.Slider( position, rotation * Vector3.up, handleSize, Handles.ArrowHandleCap, 0.001f );
            if( colorize ) Handles.color = Handles.zAxisColor;
            position = Handles.Slider( position, rotation * Vector3.forward, handleSize, Handles.ArrowHandleCap, 0.001f );

            if( freeHandle )
            {
                Handles.color = Handles.centerColor;
                position = Handles.FreeMoveHandle( position, rotation, handleSize * 0.15f, Vector3.one * 0.001f, Handles.RectangleHandleCap );
            }

            Handles.color = color;

            return position;
        }


    }

}

#endif
