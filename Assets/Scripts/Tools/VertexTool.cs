using Assets.Scripts.Components;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools
{
    public class VertexToolWindow : EditorWindow
    {
        private static readonly Color BgDark = new Color(0.13f, 0.13f, 0.13f);
        private static readonly Color BgMid = new Color(0.18f, 0.18f, 0.18f);
        private static readonly Color Accent = new Color(0.29f, 0.56f, 1.00f);
        private static readonly Color AccentHover = new Color(0.38f, 0.64f, 1.00f);
        private static readonly Color AccentActive = new Color(0.20f, 0.45f, 0.90f);
        private static readonly Color VertexAColor = new Color(1.00f, 0.60f, 0.15f);
        private static readonly Color VertexBColor = new Color(0.80f, 0.30f, 1.00f);
        private static readonly Color VertexSelectedColor = new Color(0.30f, 0.85f, 1.00f);
        private static readonly Color MidpointColor = new Color(0.30f, 1.00f, 0.50f);
        private static readonly Color TextPrimary = new Color(0.92f, 0.92f, 0.92f);
        private static readonly Color TextSecondary = new Color(0.55f, 0.55f, 0.55f);
        private static readonly Color Separator = new Color(0.26f, 0.26f, 0.26f);
        private static readonly Color ActiveGreen = new Color(0.20f, 0.80f, 0.40f);

        private bool _toolActive;
        private PrimitiveObject _targetA;
        private MeshFilter _meshFilterA;
        private Vector3[] _verticesA;
        private int _vertexA = -1;
        private PrimitiveObject _targetB;
        private MeshFilter _meshFilterB;
        private Vector3[] _verticesB;
        private int _vertexB = -1;
        private GUIStyle _styleTitle;
        private GUIStyle _styleSubtitle;
        private GUIStyle _styleLabel;
        private GUIStyle _styleMuted;
        private GUIStyle _styleBtnPrimary;
        private GUIStyle _styleBtnToggleOff;
        private GUIStyle _styleBtnToggleOn;
        private GUIStyle _styleVertexBadge;
        private GUIStyle _styleVertexBadgeSelected;
        private bool _stylesReady;

        [MenuItem("Thaumiel/Vertex Tool")]
        public static void Open()
        {
            VertexToolWindow w = GetWindow<VertexToolWindow>(false, "Vertex Tool");
            w.minSize = new Vector2(290f, 520f);
            w.Show();
        }

        private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            Deactivate();
        }

        private void SetTargetA(PrimitiveObject primitive)
        {
            _vertexA = -1;
            _targetA = primitive;
            _meshFilterA = null;
            _verticesA = null;

            if (primitive == null) return;
            _meshFilterA = primitive.GetComponent<MeshFilter>();
            if (_meshFilterA != null && _meshFilterA.sharedMesh != null)
                _verticesA = _meshFilterA.sharedMesh.vertices;
        }

        private void SetTargetB(PrimitiveObject primitive)
        {
            _vertexB = -1;
            _targetB = primitive;
            _meshFilterB = null;
            _verticesB = null;

            if (primitive == null) return;
            _meshFilterB = primitive.GetComponent<MeshFilter>();
            if (_meshFilterB != null && _meshFilterB.sharedMesh != null)
                _verticesB = _meshFilterB.sharedMesh.vertices;
        }

        private void Deactivate()
        {
            _toolActive = false;
            _vertexA = -1;
            _vertexB = -1;
            SceneView.RepaintAll();
        }

        private void ClearSelection()
        {
            _vertexA = -1;
            _vertexB = -1;
            SceneView.RepaintAll();
            Repaint();
        }

        private void ScaleToMeetVertex()
        {
            if (_vertexA < 0 || _vertexB < 0 || _targetA == null || _targetB == null)
                return;

            Transform tA = _targetA.transform;
            Vector3 wA = tA.TransformPoint(_verticesA[_vertexA]);
            Vector3 wB = _targetB.transform.TransformPoint(_verticesB[_vertexB]);
            Vector3 mid = (wA + wB) * 0.5f;

            Vector3 vALocal = _verticesA[_vertexA];
            Vector3 localTarget = Quaternion.Inverse(tA.rotation) * (mid - tA.position);

            Vector3 newScale = tA.localScale;
            if (Mathf.Abs(vALocal.x) > 0.0001f) newScale.x = localTarget.x / vALocal.x;
            if (Mathf.Abs(vALocal.y) > 0.0001f) newScale.y = localTarget.y / vALocal.y;
            if (Mathf.Abs(vALocal.z) > 0.0001f) newScale.z = localTarget.z / vALocal.z;

            Undo.RecordObject(tA, "Scale Vertex to Meet");
            tA.localScale = newScale;

            _vertexA = -1;
            _vertexB = -1;

            SceneView.RepaintAll();
            Repaint();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!_toolActive) return;

            int ctrlId = GUIUtility.GetControlID(FocusType.Passive);
            if (Event.current.type == EventType.Layout)
                HandleUtility.AddDefaultControl(ctrlId);

            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

            DrawObjectVertices(_targetA, _verticesA, ref _vertexA, VertexAColor);
            DrawObjectVertices(_targetB, _verticesB, ref _vertexB, VertexBColor);

            if (_vertexA >= 0 && _vertexB >= 0 && _targetA != null && _targetB != null)
            {
                Vector3 wA = _targetA.transform.TransformPoint(_verticesA[_vertexA]);
                Vector3 wB = _targetB.transform.TransformPoint(_verticesB[_vertexB]);
                Vector3 mid = (wA + wB) * 0.5f;

                Handles.color = new Color(VertexSelectedColor.r, VertexSelectedColor.g, VertexSelectedColor.b, 0.5f);
                Handles.DrawDottedLine(wA, wB, 5f);

                Handles.color = MidpointColor;
                float midSize = HandleUtility.GetHandleSize(mid) * 0.07f;
                Handles.DotHandleCap(0, mid, Quaternion.identity, midSize, EventType.Repaint);
                Handles.Label(mid + Vector3.up * midSize * 2f, "midpoint",
                    new GUIStyle { normal = { textColor = MidpointColor }, fontSize = 10 });
            }

            sceneView.Repaint();
        }

        private void DrawObjectVertices(PrimitiveObject target, Vector3[] vertices, ref int selected, Color baseColor)
        {
            if (target == null || vertices == null) return;

            Transform t = target.transform;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 world = t.TransformPoint(vertices[i]);
                float size = HandleUtility.GetHandleSize(world) * 0.05f;

                Handles.color = (i == selected) ? VertexSelectedColor : baseColor;

                if (Handles.Button(world, Quaternion.identity, size, size * 2f, Handles.DotHandleCap))
                {
                    selected = (selected == i) ? -1 : i;
                    Repaint();
                }
            }
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawHeader();
            DrawSep();
            DrawObjectASection();
            DrawSep();
            DrawObjectBSection();
            DrawSep();
            DrawSelectionPreview();
            DrawSep();
            DrawActionsSection();
            GUILayout.FlexibleSpace();
            DrawFooter();
        }

        private void DrawHeader()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, 54), BgDark);
            GUILayout.Space(10);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(14);
                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Label("Vertex Tool", _styleTitle);
                    GUILayout.Label("Scale a primitive so its vertex edge meets another", _styleMuted);
                }
                GUILayout.FlexibleSpace();
                string toggleLabel = _toolActive ? "● ACTIVE" : "○ INACTIVE";
                GUIStyle toggleStyle = _toolActive ? _styleBtnToggleOn : _styleBtnToggleOff;
                if (GUILayout.Button(toggleLabel, toggleStyle, GUILayout.Height(22)))
                {
                    _toolActive = !_toolActive;
                    if (!_toolActive) ClearSelection();
                    SceneView.RepaintAll();
                }
                GUILayout.Space(12);
            }
            GUILayout.Space(10);
        }

        private void DrawObjectASection()
        {
            GUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(14);
                GUI.color = VertexAColor;
                GUILayout.Label("■", GUILayout.Width(14));
                GUI.color = Color.white;
                GUILayout.Label("OBJECT A  —  scaled to meet", _styleSubtitle);
            }
            GUILayout.Space(6);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(14);
                EditorGUI.BeginChangeCheck();
                PrimitiveObject next = (PrimitiveObject)EditorGUILayout.ObjectField(
                    _targetA, typeof(PrimitiveObject), allowSceneObjects: true);
                if (EditorGUI.EndChangeCheck()) SetTargetA(next);
                GUILayout.Space(14);
            }
            DrawMeshInfo(_targetA, _meshFilterA, _verticesA, _vertexA);
            GUILayout.Space(8);
        }

        private void DrawObjectBSection()
        {
            GUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(14);
                GUI.color = VertexBColor;
                GUILayout.Label("■", GUILayout.Width(14));
                GUI.color = Color.white;
                GUILayout.Label("OBJECT B  —  reference, not modified", _styleSubtitle);
            }
            GUILayout.Space(6);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(14);
                EditorGUI.BeginChangeCheck();
                PrimitiveObject next = (PrimitiveObject)EditorGUILayout.ObjectField(
                    _targetB, typeof(PrimitiveObject), allowSceneObjects: true);
                if (EditorGUI.EndChangeCheck()) SetTargetB(next);
                GUILayout.Space(14);
            }
            DrawMeshInfo(_targetB, _meshFilterB, _verticesB, _vertexB);
            GUILayout.Space(8);
        }

        private void DrawMeshInfo(PrimitiveObject target, MeshFilter filter, Vector3[] verts, int selected)
        {
            if (target != null && filter == null)
            {
                GUILayout.Space(4);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(14);
                    EditorGUILayout.HelpBox("Target has no MeshFilter.", MessageType.Warning);
                    GUILayout.Space(14);
                }
                return;
            }

            if (verts == null) return;

            GUILayout.Space(4);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(14);
                string label = selected >= 0
                    ? $"{verts.Length} vertices  ·  selected #{selected}  {FormatVec(verts[selected])}"
                    : $"{verts.Length} vertices  ·  none selected";
                GUILayout.Label(label, _styleMuted);
            }
        }

        private void DrawSelectionPreview()
        {
            GUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(14);
                GUILayout.Label("SELECTION", _styleSubtitle);
                GUILayout.FlexibleSpace();
                using (new EditorGUI.DisabledScope(_vertexA < 0 && _vertexB < 0))
                {
                    if (GUILayout.Button("Clear", EditorStyles.miniButton))
                        ClearSelection();
                }
                GUILayout.Space(14);
            }
            GUILayout.Space(8);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(14);
                DrawVertexSlot("Vertex A  (scaled)", _vertexA, _verticesA, VertexAColor);
                GUILayout.Space(8);
                DrawVertexSlot("Vertex B  (target)", _vertexB, _verticesB, VertexBColor);
                GUILayout.Space(14);
            }

            if (_vertexA >= 0 && _vertexB >= 0 && _targetA != null && _targetB != null)
            {
                Vector3 wA = _targetA.transform.TransformPoint(_verticesA[_vertexA]);
                Vector3 wB = _targetB.transform.TransformPoint(_verticesB[_vertexB]);
                Vector3 mid = (wA + wB) * 0.5f;

                GUILayout.Space(6);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(14);
                    GUILayout.Label($"Midpoint  →  {FormatVec(mid)}", new GUIStyle(_styleMuted)
                    {
                        normal = { textColor = MidpointColor }
                    });
                }
            }

            GUILayout.Space(8);
        }

        private void DrawVertexSlot(string label, int index, Vector3[] vertices, Color accent)
        {
            bool hasValue = index >= 0 && vertices != null;
            GUIStyle style = hasValue ? _styleVertexBadgeSelected : _styleVertexBadge;

            using (new EditorGUILayout.VerticalScope(style, GUILayout.ExpandWidth(true)))
            {
                GUILayout.Space(4);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(8);
                    GUILayout.Label(label, new GUIStyle(_styleMuted) { fontSize = 9 });
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(8);
                    if (hasValue)
                    {
                        GUILayout.Label($"#{index}", new GUIStyle(_styleLabel)
                        {
                            normal = { textColor = accent },
                            fontStyle = FontStyle.Bold
                        });
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(FormatVec(vertices[index]), _styleMuted);
                        GUILayout.Space(4);
                    }
                    else
                    {
                        GUILayout.Label("—  not selected", _styleMuted);
                    }
                    GUILayout.Space(8);
                }
                GUILayout.Space(4);
            }
        }

        private void DrawActionsSection()
        {
            GUILayout.Space(10);

            bool canScale = _toolActive
                && _vertexA >= 0 && _vertexB >= 0
                && _targetA != null && _targetB != null;

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(14);
                using (new EditorGUI.DisabledScope(!canScale))
                {
                    if (GUILayout.Button("Scale A to Meet B", _styleBtnPrimary, GUILayout.Height(36)))
                        ScaleToMeetVertex();
                }
                GUILayout.Space(14);
            }

            GUILayout.Space(6);

            string hint = null;
            if (!_toolActive) hint = "Activate the tool to begin picking vertices.";
            else if (_targetA == null) hint = "Assign Object A — the primitive to be scaled.";
            else if (_targetB == null) hint = "Assign Object B — the reference primitive.";
            else if (_vertexA < 0 && _vertexB < 0) hint = "Click an orange dot on Object A in the Scene view.";
            else if (_vertexA >= 0 && _vertexB < 0) hint = "Now click a purple dot on Object B.";
            else if (_vertexA < 0) hint = "Now click an orange dot on Object A.";

            if (hint != null)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(14);
                    GUILayout.Label(hint, _styleMuted);
                    GUILayout.Space(14);
                }
            }

            GUILayout.Space(10);
        }

        private void DrawFooter()
        {
            DrawSep();
            EditorGUI.DrawRect(new Rect(0, position.height - 28, position.width, 28), BgDark);
            GUILayout.Space(6);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(14);
                GUILayout.Label("Orange = Object A  ·  Purple = Object B", new GUIStyle(_styleMuted) { fontSize = 10 });
            }
        }

        private static string FormatVec(Vector3 v) => $"({v.x:F2}, {v.y:F2}, {v.z:F2})";

        private void DrawSep()
        {
            Rect r = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(r, Separator);
        }

        private void EnsureStyles()
        {
            if (_stylesReady) return;

            _styleTitle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = TextPrimary }
            };

            _styleSubtitle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 9,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.45f, 0.45f, 0.45f) },
                margin = new RectOffset(0, 0, 0, 0)
            };

            _styleLabel = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                normal = { textColor = TextPrimary }
            };

            _styleMuted = new GUIStyle(EditorStyles.label)
            {
                fontSize = 11,
                wordWrap = true,
                normal = { textColor = TextSecondary }
            };

            _styleBtnPrimary = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white, background = MakeTex(Accent) },
                hover = { textColor = Color.white, background = MakeTex(AccentHover) },
                active = { textColor = Color.white, background = MakeTex(AccentActive) },
                fixedHeight = 36
            };

            _styleBtnToggleOff = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 9,
                fontStyle = FontStyle.Bold,
                normal = { textColor = TextSecondary, background = MakeTex(BgMid) },
                padding = new RectOffset(8, 8, 3, 3)
            };

            _styleBtnToggleOn = new GUIStyle(_styleBtnToggleOff)
            {
                normal = { textColor = ActiveGreen, background = MakeTex(new Color(0.15f, 0.28f, 0.18f)) }
            };

            _styleVertexBadge = new GUIStyle
            {
                normal = { background = MakeTex(BgMid) },
                padding = new RectOffset(0, 0, 2, 2),
                margin = new RectOffset(0, 0, 0, 0)
            };

            _styleVertexBadgeSelected = new GUIStyle(_styleVertexBadge)
            {
                normal = { background = MakeTex(new Color(0.18f, 0.28f, 0.40f)) }
            };

            _stylesReady = true;
        }

        private static Texture2D MakeTex(Color col)
        {
            Texture2D t = new Texture2D(1, 1);
            t.SetPixel(0, 0, col);
            t.Apply();
            return t;
        }
    }
}