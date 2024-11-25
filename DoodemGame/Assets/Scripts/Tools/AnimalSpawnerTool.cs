// using System.Linq;
// using Totems;
// using Unity.VisualScripting;
// using UnityEditor;
// using UnityEditor.EditorTools;
// using UnityEngine;
//
// namespace Tools
// {
//     [EditorTool("Totem Instantiate Tool", typeof(Totem))]
//     public class TotemSpawner: EditorTool
//     {
//         private GameObject _h;
//         private GameObject _b;
//         private GameObject _f;
//         public override void OnActivated()
//         {
//             _h = null;
//             _b = null;
//             _f = null;
//             base.OnActivated();
//         }
//
//         public override void OnToolGUI(EditorWindow window)
//         {
//             // if (window is not SceneView sceneView)
//                 // return;
//             Handles.BeginGUI();
//             InstantiateLayout();
//             Handles.EndGUI();
//             // base.OnToolGUI(window);
//         }
//
//         private void AssignPieceIfValid(out GameObject toAssign, GameObject fromAssign, string bodyType)
//         {
//             if (fromAssign)
//             {
//                 var tpiece = fromAssign.GetComponent<TotemPiece>();
//                 if (tpiece && tpiece.CompareTag(bodyType))
//                 {
//                     toAssign = fromAssign;
//                     return;
//                 }
//             }
//             toAssign = null;
//         }
//         
//         private void InstantiateLayout()
//         {
//             
//             using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
//             {
//                 using (new GUILayout.VerticalScope(EditorStyles.helpBox))
//                 {
//                               
//                     var aux = (GameObject)EditorGUILayout.ObjectField(label:"Head: ", _h, typeof(GameObject), false);
//                     AssignPieceIfValid(out _h, aux, "Head");
//                     aux =  (GameObject)EditorGUILayout.ObjectField(label:"Body: ", _b, typeof(GameObject), false);
//                     AssignPieceIfValid(out _b, aux, "Body");
//                     aux =  (GameObject)EditorGUILayout.ObjectField(label:"Feet: ", _f, typeof(GameObject), false);
//                     AssignPieceIfValid(out _f, aux, "Feet");
//                     var instantiate = GUILayout.Button("Instantiate");
//                     if (instantiate && _h && _b && _f)
//                     {
//                         target.GameObject().GetComponent<Totem>().CreateTotem(_h, _b, _f);
//                     }
//                 }
//             }
//         }
//     }
// }