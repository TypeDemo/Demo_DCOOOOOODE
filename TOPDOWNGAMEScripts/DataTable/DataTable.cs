using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

using CSVTable = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>;
using CacheTable = System.Collections.Generic.Dictionary<System.Type, System.Collections.Generic.Dictionary<int, object>>;
using System;

public class DataTable {

    static CacheTable cache = new CacheTable();

    public static T Get<T>(int id)
    {            //泛型
        var type = typeof(T);
        if (!cache.ContainsKey(type))
        {
            cache[type] = Load<T>("DataTable/" + type.Name);        //如果不存在该类型的表格，就从Resources/DataTable/文件名，加载
        }
      
        T data = (T)cache[type][id];
        return data;
    }

    private static Dictionary<int, object> Load<T>(string path)
    {
        Dictionary<int, object> datas = new Dictionary<int, object>();
        var textAsset = Resources.Load<TextAsset>(path);
        //var textAsse1t = r.Load<TextAsset>(path);

        var table = Parser(textAsset.text);     //解析文本内容

        FieldInfo[] fields = typeof(T).GetFields();
        foreach (var id in table.Keys)
        {
            var row = table[id];

            var obj = Activator.CreateInstance<T>();

            foreach (FieldInfo fi in fields)
            {
                fi.SetValue(obj, Convert.ChangeType(row[fi.Name], fi.FieldType));
            }
            datas[Convert.ToInt32(id)] = obj;
        }

        return datas;
    }

    private static CSVTable Parser(string content)
    {

        CSVTable result = new CSVTable();

        //按行读取内容
        string[] row = content.Replace("\r", "").Split(new char[] { '\n' });        // \n是换行，英文是New line，表示使光标到行首，\r是回车，英文是Carriage return，表示使光标下移一格

        //按","分割
        string[] columnHeads = row[0].Split(new char[] { ',' });


        for (int i = 1; i < row.Length; i++)
        {
            string[] line = row[i].Split(new char[] { ',' });
            var id = line[0];
            if (String.IsNullOrEmpty(id)) break;

            result[id] = new Dictionary<string, string>();
            for (int j = 0; j < line.Length; j++)
            {
                result[id][columnHeads[j]] = line[j];       //result[id]这行ID对应的所有的文字符键值对，表头的第J项为键，该行的第N项为值
            }
        }

        return result;
    }


}
