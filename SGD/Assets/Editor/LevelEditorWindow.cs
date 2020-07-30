using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor
{
    public class LevelEditorWindow : ExtendedEditorWindow
    {
        private Vector2Int _selectedBlock = new Vector2Int(-1, -1);
        private List<Tuple<int, SerializedProperty>> _deleteList = new List<Tuple<int, SerializedProperty>>();
        private Vector2 _scrollPosDrawer;
        private Vector2 _scrollPosPool;
        private BlockData _blockData;
        private ReorderableList _blockPoolList;
        private SerializedProperty _selectedPoolBlock;
        private bool _overriden;

        private readonly string[] _arrows = {"↑", "→", "←", "↓"};
        
        public static void Open(LevelDataObject dataObject)
        {
            var window = GetWindow<LevelEditorWindow>("Level Editor");
            window.serializedObject = new SerializedObject(dataObject);
            window.minSize = new Vector2(350, 270);
            window.titleContent.image = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Gizmos/LevelEditorIcon.png", typeof(Texture2D));
        }

        private void OnGUI()
        {
            currentProperty = serializedObject.FindProperty("levels");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.MinWidth(90), GUILayout.ExpandHeight(true));
            _scrollPosDrawer = EditorGUILayout.BeginScrollView(_scrollPosDrawer);
            DrawerSidebar(currentProperty);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                if (currentProperty != null)
                {
                    ++currentProperty.arraySize;
                    var element = currentProperty.GetArrayElementAtIndex(currentProperty.arraySize - 1);
                    element.FindPropertyRelative("dimensions").vector2IntValue = Vector2Int.zero;
                    element.FindPropertyRelative("startPos").vector2IntValue = new Vector2Int(-1, -1);
                    element.FindPropertyRelative("endPos").vector2IntValue = new Vector2Int(-1, -1);
                    element.FindPropertyRelative("ordered").boolValue = false;
                    element.FindPropertyRelative("blockPool").ClearArray();
                    Apply();
                    if (currentProperty.arraySize > 0)
                    {
                        var direct =
                            (serializedObject.targetObject as LevelDataObject)?.levels[currentProperty.arraySize - 1];
                        if (direct != null)
                        {
                            direct.layout.Clear();
                            serializedObject.Update();
                        }
                    }
                }
            }
            if (GUILayout.Button("-"))
            {
                _deleteList.Add(new Tuple<int, SerializedProperty>(buttonIndex, currentProperty));
                buttonIndex = -1;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            if (selectedProperty != null)
            {
                DrawSelectedLevelPanel();
            }
            else
            {
                EditorGUILayout.LabelField($"Select an item from the list", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter});
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            // Garbage collector
            
            currentProperty = serializedObject.FindProperty("levels");
            foreach (var i in _deleteList)
            {
                if (i.Item2 == currentProperty)
                {
                    Select(i.Item2.GetArrayElementAtIndex(Mathf.Clamp(i.Item1 - 1, 0, int.MaxValue)), i.Item1 - 1);
                }

                i.Item2.DeleteArrayElementAtIndex(i.Item1);
            }
            _deleteList.Clear();
            
            Apply();
            serializedObject.Update();
        }
        private void DrawSelectedLevelPanel()
        {
            if (buttonIndex >= 0)
            {
                currentProperty = selectedProperty;

                EditorGUILayout.BeginHorizontal("box", GUILayout.Height(240), GUILayout.ExpandWidth(true));
                EditorGUILayout.BeginVertical(GUILayout.Width(200), GUILayout.ExpandHeight(true));
                DrawLevelGrid();
                DrawField("dimensions", true);
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                GUILayout.Space(20);
                DrawBlockDescription();
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                DrawLevelPoolPanel();
            }
        }
        private void DrawLevelPoolPanel()
        {
            if (_blockPoolList == null)
            {
                _blockPoolList = new ReorderableList(serializedObject,currentProperty.FindPropertyRelative("blockPool"), true, true, true, true);

                _blockPoolList.drawHeaderCallback = rect =>
                {
                    var gap = 35f;
                    var numColumns = 4;
                    var width = (rect.width - (numColumns - 1) * gap) / numColumns;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.width = width;
                    //
                    EditorGUI.LabelField(rect, "Name");
                    rect.x += rect.width + gap;
                    
                    EditorGUI.LabelField(rect, "Block data");
                    rect.x += rect.width + gap;
                    
                    EditorGUI.LabelField(rect, "Count");
                    rect.x += rect.width + gap;
                    
                    EditorGUI.LabelField(rect, "Chance");
                };
                
                _blockPoolList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    if (index < _blockPoolList.serializedProperty.arraySize && index >= 0)
                    {
                        var element = _blockPoolList.serializedProperty.GetArrayElementAtIndex(index);

                        var gap = 35f;
                        var numColumns = 4;
                        var width = (rect.width - (numColumns - 1) * gap) / numColumns;
                        rect.height = EditorGUIUtility.singleLineHeight;
                        rect.width = width;
                        //
                        EditorGUI.LabelField(rect, element.displayName.Replace("Element", "Block"));
                        rect.x += rect.width - EditorGUIUtility.singleLineHeight;

                        rect.width = EditorGUIUtility.singleLineHeight;
                        EditorGUI.DrawPreviewTexture(rect, GenerateThumbnail(index));
                        rect.width = width;
                        rect.x += gap;
                        
                        EditorGUI.PropertyField(rect, element.FindPropertyRelative("poolBlockData"),
                            GUIContent.none);
                        rect.x += rect.width + gap;

                        EditorGUI.PropertyField(rect, element.FindPropertyRelative("count"), GUIContent.none);
                        rect.x += rect.width + gap;

                        if (selectedProperty.FindPropertyRelative("ordered").boolValue) GUI.enabled = false;
                        EditorGUI.PropertyField(rect, element.FindPropertyRelative("chance"), GUIContent.none);
                        if (selectedProperty.FindPropertyRelative("ordered").boolValue) GUI.enabled = true;
                    }
                };

                _blockPoolList.onSelectCallback = list => _selectedPoolBlock = _blockPoolList.serializedProperty.GetArrayElementAtIndex(list.index);
                _blockPoolList.onAddCallback = list =>
                {
                    if (list.serializedProperty != null)
                    {
                        ++list.serializedProperty.arraySize;
                        list.index = list.serializedProperty.arraySize - 1;
                    }
                    else
                    {
                        var elementType = list.list.GetType().GetElementType();
                        if (elementType == typeof (string))
                            list.index = list.list.Add("");
                        else if (elementType != null && elementType.GetConstructor(Type.EmptyTypes) == null)
                            Debug.LogError("Cannot add element. Type " + elementType + " has no default constructor. Implement a default constructor or implement your own add behaviour.");
                        else if (list.list.GetType().GetGenericArguments()[0] != null)
                            list.index = list.list.Add(Activator.CreateInstance(list.list.GetType().GetGenericArguments()[0]));
                        else if (elementType != null)
                            list.index = list.list.Add(Activator.CreateInstance(elementType));
                        else
                            Debug.LogError("Cannot add element of type Null.");
                    }
                    
                    // FILL DEFAULTS
                    var element = _blockPoolList.serializedProperty.GetArrayElementAtIndex(list.index);
                    element.FindPropertyRelative("poolBlockData").objectReferenceValue = null;
                    element.FindPropertyRelative("count").intValue = 1;
                    element.FindPropertyRelative("chance").floatValue = 1f;
                    var oProp = element.FindPropertyRelative("overridePlacings");
                    if (oProp.isArray)
                    {
                        oProp.arraySize = 9;
                        for (var i = 0; i < 9; i++) oProp.GetArrayElementAtIndex(i).objectReferenceValue = null;
                    }
                };
                _blockPoolList.onRemoveCallback = list =>
                {
                    _selectedPoolBlock = null;
                    if (list.serializedProperty != null)
                    {
                        _deleteList.Add(new Tuple<int, SerializedProperty>(list.index, list.serializedProperty));
                        if (list.index < list.serializedProperty.arraySize - 2)
                            return;
                        list.index = list.serializedProperty.arraySize - 2;
                    }
                    else
                    {
                        list.list.RemoveAt(list.index);
                        if (list.index >= list.list.Count - 1)
                            list.index = list.list.Count - 1;
                    }
                    if (list.index >= 0) _selectedPoolBlock = _blockPoolList.serializedProperty.GetArrayElementAtIndex(list.index);
                };
            }

            _scrollPosPool = EditorGUILayout.BeginScrollView(_scrollPosPool, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, "box", GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Level block pool");
            EditorGUILayout.PropertyField(selectedProperty.FindPropertyRelative("ordered"));
            EditorGUILayout.EndHorizontal();
            _blockPoolList.DoLayoutList();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.Width(200), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (_selectedPoolBlock != null)
            {
                var a = _selectedPoolBlock.FindPropertyRelative("overridePlacings");
                EditorGUILayout.LabelField($"Editing placings for {_selectedPoolBlock.displayName.Replace("Element", "Block")}", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter});
                if (a != null)
                {
                    if (a.isArray && a.arraySize == 9)
                    {
                        if (_selectedPoolBlock.FindPropertyRelative("poolBlockData").objectReferenceValue !=
                            null)
                        {
                            for (var y = 0; y < 3; y++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.FlexibleSpace();
                                for (var x = 0; x < 3; x++)
                                {
                                    EditorGUILayout.PropertyField(a.GetArrayElementAtIndex(y * 3 + x), GUIContent.none,
                                        GUILayout.Width(40), GUILayout.Height(40));
                                }

                                GUILayout.FlexibleSpace();
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Selected block doesn't \n have any source",
                                new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter},
                                GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));
                        }
                    }
                    else
                    {
                        a.arraySize = 9;
                        serializedObject.Update();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No block selected", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter});
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        private Texture2D GenerateThumbnail(int index)
        {
            var element = ((LevelDataObject) serializedObject.targetObject).levels[buttonIndex].blockPool[index].overridePlacings;
            var tex = new Texture2D (3, 3, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;
            for (var i = 0; i < 9; i++)
            {
                if (element[i % 3 + Mathf.RoundToInt(i / 3) * 3] != null)
                    tex.SetPixel(i % 3, -(i / 3) + 2, Color.black);
            }
            tex.Apply();
            return tex;
        }
        
        private void DrawBlockDescription()
        {
            if (_selectedBlock != new Vector2Int(-1, -1))
            {
                void DrawData(LevelData levelObj, BlockData blockType, int selected)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginVertical();
                    
                    var sel = GUILayout.SelectionGrid(selected, new[] {"Start destination", "Finish destination"}, 2, "toggle");
                    switch (sel)
                    {
                        case 0:
                            selectedProperty.FindPropertyRelative("startPos")
                                .vector2IntValue = _selectedBlock;
                            if (selected == 1)
                            {
                                selectedProperty.FindPropertyRelative("endPos")
                                    .vector2IntValue = new Vector2Int(-1, -1);
                            }
                            break;
                        
                        case 1:
                            selectedProperty.FindPropertyRelative("endPos")
                                .vector2IntValue = _selectedBlock;
                            if (selected == 0)
                            {
                                selectedProperty.FindPropertyRelative("startPos")
                                    .vector2IntValue = new Vector2Int(-1, -1);
                            }
                            break;
                    }
                    Apply();

                    _blockData = EditorGUILayout.ObjectField("Source block:", blockType, typeof(BlockData), false) as BlockData;

                    if (levelObj != null)
                    {
                        if (_blockData != null)
                        {
                            if (!levelObj.layout.ContainsKey(_selectedBlock.x))
                                levelObj.layout.Add(_selectedBlock.x, new LayoutBlockDict());
                            if (!levelObj.layout[_selectedBlock.x].ContainsKey(_selectedBlock.y))
                                levelObj.layout[_selectedBlock.x].Add(_selectedBlock.y, new LayoutBlock());
                            if (levelObj.layout[_selectedBlock.x][_selectedBlock.y].layoutBlockData != _blockData)
                            {
                                levelObj.layout[_selectedBlock.x][_selectedBlock.y].layoutBlockData = _blockData;
                                levelObj.layout[_selectedBlock.x][_selectedBlock.y].overridePlacings =
                                    new GameObject[9];
                                levelObj.layout[_selectedBlock.x][_selectedBlock.y].facing =
                                    Rotation.NORTH;
                            }

                            serializedObject.Update();
                        }
                        else
                        {
                            if (levelObj.layout.ContainsKey(_selectedBlock.x))
                            {
                                if (levelObj.layout[_selectedBlock.x].ContainsKey(_selectedBlock.y))
                                {
                                    levelObj.layout[_selectedBlock.x].Remove(_selectedBlock.y);
                                    if (levelObj.layout[_selectedBlock.x].Count == 0)
                                    {
                                        levelObj.layout.Remove(_selectedBlock.x);
                                    }

                                    serializedObject.Update();
                                }
                            }
                        }


                        if (_blockData != null)
                        {
                            levelObj.layout[_selectedBlock.x][_selectedBlock.y].facing =
                                (Rotation) EditorGUILayout.EnumPopup(levelObj.layout[_selectedBlock.x][_selectedBlock.y]
                                    .facing);

                            if (GUILayout.Button(_overriden ? "Clear" : "Override"))
                            {
                                if (!_overriden)
                                {
                                    if (!levelObj.layout.ContainsKey(_selectedBlock.x))
                                        levelObj.layout[_selectedBlock.x] = new LayoutBlockDict();
                                    levelObj.layout[_selectedBlock.x][_selectedBlock.y].overridePlacings =
                                        (GameObject[]) _blockData.defaultPlacings.Clone();
                                    _overriden = true;
                                    serializedObject.Update();
                                }
                                else
                                {
                                    Clear();
                                }
                            }

                            void Clear()
                            {
                                levelObj.layout[_selectedBlock.x][_selectedBlock.y].overridePlacings =
                                    (GameObject[]) _blockData.defaultPlacings.Clone();
                                _overriden = false;
                                serializedObject.Update();
                            }

                            if (_overriden)
                            {
                                var placingData = levelObj.layout[_selectedBlock.x][_selectedBlock.y].overridePlacings;
                                EditorGUILayout.BeginVertical();
                                GUILayout.FlexibleSpace();
                                if (placingData != null)
                                {
                                    for (var y = 0; y < 3; y++)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.FlexibleSpace();
                                        for (var x = 0; x < 3; x++)
                                        {
                                            placingData[y * 3 + x] = (GameObject) EditorGUILayout.ObjectField(
                                                GUIContent.none,
                                                placingData[y * 3 + x], typeof(GameObject), false, GUILayout.Width(40),
                                                GUILayout.Height(40));
                                        }

                                        GUILayout.FlexibleSpace();
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }

                                levelObj.layout[_selectedBlock.x][_selectedBlock.y].overridePlacings = placingData;
                                serializedObject.Update();
                                GUILayout.FlexibleSpace();
                                EditorGUILayout.EndVertical();
                            }
                            else
                            {
                                if (levelObj.layout.ContainsKey(_selectedBlock.x))
                                {
                                    if (levelObj.layout[_selectedBlock.x].ContainsKey(_selectedBlock.y))
                                    {
                                        if (levelObj.layout[_selectedBlock.x][_selectedBlock.y].overridePlacings !=
                                            null)
                                        {
                                            if (!Enumerable.SequenceEqual(
                                                levelObj.layout[_selectedBlock.x][_selectedBlock.y].overridePlacings,
                                                _blockData.defaultPlacings))
                                                _overriden = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                    EditorGUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
                
                var obj = (serializedObject.targetObject as LevelDataObject)?.levels[buttonIndex];
                if (obj != null)
                {
                    EditorGUILayout.LabelField(
                        $"< Editing block with coordinates [{_selectedBlock.x}, {_selectedBlock.y}] >",
                        new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter});
                    var flag = _selectedBlock == obj.endPos ? 1 : (_selectedBlock == obj.startPos ? 0 : -1);

                    if (obj.layout.Count > 0 && obj.layout.TryGetValue(_selectedBlock.x, out var column))
                    {
                        if (column.TryGetValue(_selectedBlock.y, out var data))
                        {
                            DrawData(obj, data.layoutBlockData, flag);
                        }
                        else
                        {
                            DrawData(obj, _blockData, flag);
                        }
                    }
                    else
                    {
                        DrawData(obj, _blockData, flag);
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField($"Select block to edit");
            }
        }
        private void DrawLevelGrid()
        {
            var obj = (serializedObject.targetObject as LevelDataObject)?.levels[buttonIndex];
            if (obj != null && obj.dimensions.x != 0 && obj.dimensions.y != 0)
            {
                var size = 200 / Math.Max(obj.dimensions.x, obj.dimensions.y);
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical();
                for (var y = 0; y < obj.dimensions.y; y++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (var x = 0; x < obj.dimensions.x; x++)
                    {
                        var pos = new Vector2Int(x, y);
                        var style = new GUIStyle(GUI.skin.button);
                        var text = " ";
                        
                        GUI.backgroundColor = Color.white;
                        if (obj.layout.ContainsKey(x))
                        {
                            if (obj.layout[x].ContainsKey(y))
                                if (obj.layout[x][y] != null)
                                {
                                    GUI.backgroundColor = Color.gray;
                                    text = _arrows[(int) obj.layout[x][y].facing];
                                }
                        }
                        if (pos == obj.endPos)
                            GUI.backgroundColor = Color.red;
                        else if (pos == obj.startPos)
                            GUI.backgroundColor = Color.green;

                        if (GUILayout.Toggle(_selectedBlock == pos, text, style, GUILayout.Width(size), GUILayout.Height(size)))
                        {
                            if (_selectedBlock != pos)
                            {
                                _overriden = false;
                                _selectedBlock = pos;
                                _blockData = null;
                            }
                        }
                    }
                    
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            else
            {
                EditorGUILayout.LabelField($"Insufficient dimensions",
                    new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter}, GUILayout.ExpandWidth(true),
                    GUILayout.ExpandHeight(true));
            }
        
        }
        protected override void OnSelectionChange()
        {
            _blockPoolList = null;
            _selectedPoolBlock = null;
            _selectedBlock = new Vector2Int(-1,-1);
            _scrollPosPool = new Vector2(0, 0);
        }
    }
}
