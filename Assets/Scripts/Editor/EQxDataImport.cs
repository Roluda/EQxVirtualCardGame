using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace EQx.Game {
    public class EQxDataImport {
        [MenuItem("Assets/Create/EQxDataSet/FromJson")]
        public static void ImportCountryData() {
            string path = EditorUtility.OpenFilePanel("Select Save File", "", "");

            EQxDataSet asset = ScriptableObject.CreateInstance<EQxDataSet>();

            using (StreamReader stream = new StreamReader(path)) {
                string json = stream.ReadToEnd();
                asset.eqxCountryData = FromJson<EQxCountryData>("{\"items\":" + json + "}");
                //Debug.Log(FromJson<EQxCountryData>("{\"data\":" + json + "}").Length);
            }

            EditorUtility.FocusProjectWindow();
            AssetDatabase.CreateAsset(asset, "Assets/Resources/EQxDataSets/EQxData_New.asset");
            AssetDatabase.SaveAssets();
            Selection.activeObject = asset;
        }

        [Serializable]
        private class Wrapper<T> {
            public T[] items;
        }

        public static T[] FromJson<T>(string json) {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.items;
        }
    }
}