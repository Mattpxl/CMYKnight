using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

namespace DataStorage
{
    /// <summary>
    /// JSON Serilization for Player Preferences (Game Settings)
    /// * IMPORTANT: This File requires the Serilization Package Located in Window > Package Manager > Unity Package Registry
    /// </summary>
    #region Serialization
    public class JSONConverter
    {
        //Application.persistentDataPath
        public static JSONConverter jsonConverter = new();
        public void writeData(float obj, string id)
        {
            string _currentDataString = JsonUtility.ToJson(obj);
            using StreamWriter streamWriter = new StreamWriter(Application.persistentDataPath + "/" + id + ".json");
            streamWriter.Write(_currentDataString);
        }
        public void readData(float obj, string id)
        {
            using StreamReader streamReader = new StreamReader(Application.persistentDataPath + "/" + id + ".json");
            string savedDataString = streamReader.ReadToEnd();
            obj = JsonUtility.FromJson<float>(savedDataString);
            id = JsonUtility.ToJson(obj);
        }
        public string getJSON(float id)
        {
            using StreamReader streamReader = new StreamReader(Application.persistentDataPath + "/" + id + ".json");
            string savedDataString = streamReader.ReadToEnd();
            float obj = JsonUtility.FromJson<float>(savedDataString);
            return JsonUtility.ToJson(obj);
        }
        public float getDataObject(string id)
        {
            using StreamReader streamReader = new StreamReader(Application.persistentDataPath + "/" + id + ".json");
            string savedDataString = streamReader.ReadToEnd();
            return JsonUtility.FromJson<float>(savedDataString);
        }

        public void overwriteDataObject(float obj, string json)
        {
            JsonUtility.FromJsonOverwrite(json, obj);
        }
    }

    #endregion Serialization
   
    /// <summary>
    /// C# to SQLite [variable to table entry] Converter 
    /// > Supports: INT, STRING, FLOAT (& BOOL)
    /// * IMPORTANT: This File requires SQLite Pre-compiled Binaries (https://www.sqlite.org/download.html) 
    /// * IMPORTANT: This File requires Mono.Data.Sqlite.dll 
    /// ** Files are to be placed in the Unity Assets Folder **
    /// </summary>
    #region SQLite
          
    /// <summary>
        /// Contains Methods for creating and maintaining a SQLite data base
        /// </summary>
    #region Converter
        public class SQLConverter
        {
        public static SQLConverter sqlConverter;
        private static string _dbTag = "URI=file:";
        private static string _dbName = "CMYKDB";
        private static string _extension = ".db";
        private static string[] _directory = new string[4]
        {
            "Assets/",
            "Scripts/",
            "2DAPI/",
            "Data/"
        };
        private static string _path = string.Format
            (
                "{0}{1}{2}{3}{4}{5}{6}",
                _dbTag,
                _directory[0],
                _directory[1],
                _directory[2],
                _directory[3],
                _dbName,
                _extension
            );
        public void generateTable(ValueTable table)
        {
            string tableString = string.Format("CREATE TABLE IF NOT EXISTS {0} ({1}_id INTEGER PRIMARY KEY, ", table._name, table._name);
            if (table._valueNames.Length == 1) tableString += string.Format("{0} {1} NOT NULL);", table._valueNames[0], table._valueTypes[0]);
            else
            {
                tableString += string.Format("{0} {1}, ", table._valueNames[0], table._valueTypes[0]);
                if (table._valueNames.Length > 2)
                {
                    for (int i = 1; i <= table._valueNames.Length - 2; i++)
                    {
                        tableString += string.Format("{0} {1}, ", table._valueNames[i], table._valueTypes[i]);
                    }
                }
                tableString += string.Format("{0} {1});", table._valueNames[table._valueNames.Length - 1], table._valueTypes[table._valueTypes.Length - 1]);
            }
            using SqliteConnection connection = new SqliteConnection(_path);
            connection.Open();
            SqliteCommand command = new SqliteCommand(string.Format("{0}", tableString), connection);
            var r = command.ExecuteNonQuery();
            connection.Close();
        }
        public void generateTable(ValueTable[] tables)
        {
            foreach (ValueTable t in tables)
            {
                generateTable(t);
            }
        }
        public void addData(ValueTable table, TableEntry entry)
        {
            string tableString = string.Format("INSERT INTO {0} ({1}) VALUES (", table._name, entry._valueName);
            switch (entry._valueType)
            {
                case "TEXT":
                    tableString += string.Format("'{0}');", entry._stringValue);
                    break;
                case "REAL":
                    tableString += string.Format("{0});", entry._floatValue);
                    break;
                case "INT":
                    tableString += string.Format("{0});", entry._intValue);
                    break;
                default:
                    break;
            }
            using SqliteConnection connection = new SqliteConnection(_path);
            SqliteCommand command = new SqliteCommand(string.Format("BEGIN; {0} COMMIT;", tableString), connection);
            command.Prepare();
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
        public void addData(ValueTable table, TableEntry[] entries)
        {
            string tableString = string.Format("INSERT INTO {0} (", table._name);

            for (int i = 0; i < entries.Length - 2; i++)
            {
                tableString += string.Format("{0}, ", entries[i]._valueName);
            }
            tableString += string.Format("{0}) VALUES (", entries[entries.Length - 1]._valueName);
            for (int i = 0; i < entries.Length - 2; i++)
            {
                switch (entries[i]._valueType)
                {
                    case "TEXT":
                        tableString += string.Format("'{0}', ", entries[i]._stringValue);
                        break;
                    case "REAL":
                        tableString += string.Format("{0}, ", entries[i]._floatValue);
                        break;
                    case "INT":
                        tableString += string.Format("{0}, ", entries[i]._intValue);
                        break;
                    default:
                        break;
                }
            }
            switch (entries[entries.Length - 1]._valueType)
            {
                case "TEXT":
                    tableString += string.Format("'{0}');", entries[entries.Length - 1]._stringValue);
                    break;
                case "REAL":
                    tableString += string.Format("{0});", entries[entries.Length - 1]._floatValue);
                    break;
                case "INT":
                    tableString += string.Format("{0});", entries[entries.Length - 1]._intValue);
                    break;
                default:
                    break;
            }
            using SqliteConnection connection = new SqliteConnection(_path);
            SqliteCommand command = new SqliteCommand(string.Format("BEGIN; {0} COMMIT;", tableString), connection);
            command.Prepare();
            connection.Open();
            command.ExecuteReader();
            connection.Close();
        }

        public void updateTable(ValueTable table, TableEntry entry, TableEntry condition)
        {
            string tableString = string.Format("UPDATE {0} SET {1} = ", table._name, entry._valueName);
            switch (entry._valueType)
            {
                case "TEXT":
                    tableString += string.Format("'{0}';", entry._stringValue);
                    break;
                case "REAL":
                    tableString += string.Format("{0}", entry._floatValue);
                    break;
                case "INT":
                    tableString += string.Format("{0}", entry._intValue);
                    break;
            }
            tableString += string.Format(" WHERE {0} = {1};", condition._valueName, condition._intValue);
            using SqliteConnection connection = new SqliteConnection(_path);
            SqliteCommand command = new SqliteCommand(string.Format("BEGIN; {0} COMMIT;", tableString), connection);
            command.Prepare();
            connection.Open();
            command.ExecuteReader();
            connection.Close();
        }
        public void updateTable(ValueTable table, TableEntry[] entries, TableEntry condition)
        {
            // WHERE table._name + "_id", get value table._name + "_id,
            string tableString = string.Format("UPDATE {0} SET ", table._name);
            for (int i = 0; i < entries.Length - 2; i++)
            {
                switch (entries[i]._valueType)
                {
                    case "TEXT":
                        tableString += string.Format("{0} = '{1}', ", entries[i]._valueName, entries[i]._stringValue);
                        break;
                    case "REAL":
                        tableString += string.Format("{0} = {1}, ", entries[i]._valueName, entries[i]._floatValue);
                        break;
                    case "INT":
                        tableString += string.Format("{0} = {1}, ", entries[i]._valueName, entries[i]._intValue);
                        break;
                }
            }
            switch (entries[entries.Length - 1]._valueType)
            {
                case "TEXT":
                    tableString += string.Format("{0} = '{1}' ", entries[entries.Length - 1]._valueName, entries[entries.Length - 1]._stringValue);
                    break;
                case "REAL":
                    tableString += string.Format("{0} = {1} ", entries[entries.Length - 1]._valueName, entries[entries.Length - 1]._floatValue);
                    break;
                case "INT":
                    tableString += string.Format("{0} = {1} ", entries[entries.Length - 1]._valueName, entries[entries.Length - 1]._intValue);
                    break;
            }
            tableString += string.Format("WHERE {0} = {1};", condition._valueName, condition._intValue);
            using SqliteConnection connection = new SqliteConnection(_path);
            SqliteCommand command = new SqliteCommand(string.Format("BEGIN; {0} COMMIT;", tableString), connection);
            command.Prepare();
            connection.Open();
            command.ExecuteReader();
            connection.Close();
        }
        public void deleteData(ValueTable table, TableEntry condition)
        {
            // delete where primary key equals condition
            string tableString = string.Format("DELETE FROM {0} WHERE {1} = {2};", table._name, condition._valueName, condition._intValue);
            using SqliteConnection connection = new SqliteConnection(_path);
            SqliteCommand command = new SqliteCommand(string.Format("BEGIN; {0} COMMIT;", tableString), connection);
            command.Prepare();
            connection.Open();
            command.ExecuteReader();
            connection.Close();
        }

        public void deleteData(ValueTable table, TableEntry[] conditions)
        {
            // WHERE table._name + "_id", get value table._name + "_id,
            string tableString = string.Format("DELETE FROM {0} WHERE ", table._name);
            for (int i = 0; i < conditions.Length - 2; i++)
            {
                tableString += string.Format("{0} = {1}, ", conditions[i]._valueName, conditions[i]._intValue);
            }
            tableString += string.Format("{0} = {1};", conditions[conditions.Length - 1]._valueName, conditions[conditions.Length - 1]._intValue);
            using SqliteConnection connection = new SqliteConnection(_path);
            SqliteCommand command = new SqliteCommand(string.Format("BEGIN; {0} COMMIT;", tableString), connection);
            command.Prepare();
            connection.Open();
            command.ExecuteReader();
            connection.Close();
        }
        public float totalCount(ValueTable table)
        {
            float count = 0;
            string tableString = string.Format("SELECT * FROM {0};", table._name);
            using SqliteConnection connection = new SqliteConnection(_path);
            SqliteCommand command = new SqliteCommand(string.Format("{0}", tableString), connection);
            connection.Open();
            using IDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read()) count += 1;
            connection.Close();
            return count;
        }
        public float entryCount(ValueTable table, TableEntry entry)
        {
            float count = 0;
            string tableString = string.Format("SELECT * FROM {0} WHERE {1} = ", table._name, entry._valueName);
            switch (entry._valueType)
            {
                case "TEXT":
                    tableString += string.Format("'{0}';", entry._stringValue);
                    break;
                case "REAL":
                    tableString += string.Format("{0};", entry._floatValue);
                    break;
                case "INT":
                    tableString += string.Format("{0};", entry._intValue);
                    break;
                default:
                    return 0;
            }
            using SqliteConnection connection = new SqliteConnection(_path);
            connection.Open();
            SqliteCommand command = new SqliteCommand(string.Format("{0}", tableString), connection);
            using IDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read()) count += 1;
            connection.Close();
            return count;
        }
        public TableEntry getValue(ValueTable table, TableEntry entry, TableEntry condition)
        {
            // get primary keys
            TableEntry sendValue = new TableEntry();
            string tableString = string.Format("SELECT {0} FROM {1} WHERE {2} = {3};", entry._valueName, table._name, condition._valueName, condition._intValue);
            using SqliteConnection connection = new SqliteConnection(_path);
            connection.Open();
            SqliteCommand command = new SqliteCommand(tableString, connection);
            using IDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                switch (entry._valueType)
                {
                    case "TEXT":
                        string s = dataReader.IsDBNull(dataReader.GetOrdinal(entry._valueName)) == true ? null : dataReader.GetValue(dataReader.GetOrdinal(entry._valueName)).ToString();
                        sendValue = new TableEntry(entry._valueName, entry._valueType, s);
                        break;
                    case "REAL":
                        float n = dataReader.IsDBNull(dataReader.GetOrdinal(entry._valueName)) == true ? 0 : dataReader.GetFloat(dataReader.GetOrdinal(entry._valueName));
                        sendValue = new TableEntry(entry._valueName, entry._valueType, n);
                        break;
                    case "INT": // getBoolean
                        int i = dataReader.IsDBNull(dataReader.GetOrdinal(entry._valueName)) == true ? 0 : (int)dataReader.GetFloat(dataReader.GetOrdinal(entry._valueName));
                        sendValue = new TableEntry(entry._valueName, entry._valueType, i);
                        break;
                }
            }
            connection.Close();
            return sendValue;

        }
        /* public TableEntry getValue(ValueTable table, TableEntry entry) // get primary key value
         {
             // get primary keys
             TableEntry sendValue = new TableEntry();
             string tableString = string.Format("SELECT {0} FROM {1};", entry._valueName, table._name);
             connection.Open();
             SqliteCommand command = new SqliteCommand(tableString, connection);
             IDataReader dataReader = command.ExecuteReader();
             while (dataReader.Read())
             {
                 switch (entry._valueType)
                 {
                     case "TEXT":
                         string s = dataReader.IsDBNull(dataReader.GetOrdinal(entry._valueName)) == true ? "NULL" : dataReader.GetValue(dataReader.GetOrdinal(entry._valueName)).ToString();
                         sendValue = new TableEntry(entry._valueName, entry._valueType, s);
                         break;
                     case "REAL":
                         float n = dataReader.IsDBNull(dataReader.GetOrdinal(entry._valueName)) == true ? 0 : dataReader.GetFloat(dataReader.GetOrdinal(entry._valueName));
                         sendValue = new TableEntry(entry._valueName, entry._valueType, n);
                         break;
                     case "INT": // getBoolean
                         int i = dataReader.IsDBNull(dataReader.GetOrdinal(entry._valueName)) == true ? 0 : (int)dataReader.GetFloat(dataReader.GetOrdinal(entry._valueName));
                         sendValue = new TableEntry(entry._valueName, entry._valueType, i);
                         break;
                 }
             }
             connection.Close();
             return sendValue;

         }*/

        public void displayTable(ValueTable table)
        {
            string tableString = string.Format("SELECT * FROM {0}", table._name);
            using SqliteConnection connection = new SqliteConnection(_path);
            connection.Open();
            SqliteCommand command = new SqliteCommand(string.Format("{0}", tableString), connection);
            using IDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                for (int i = 0; i < table._valueNames.Length - 1; i++)
                    Debug.Log(string.Format("{0}:{1}", table._valueNames[i], dataReader.GetValue(i + 1)));
            }
            connection.Close();
        }
        // restore previous backup through JSON Serialization of DB file on quit. if data currupt (check IDatatReader error flags) display load back-up message - from local JSON file
    }
   
    #endregion Converter
    
    /// <summary>
    /// Contains methods for converting data types into SQLite data table entries 
    /// TableEntry > ValueTable > TableMap 
    /// </summary>
    #region Data Types
    public class ValueTable
    {
        public string _name;
        public string[] _valueNames;
        public string[] _valueTypes;
        public TableEntry _tableEntry;
        public TableEntry[] _tableEntries;
        public ValueTable(string name, string[] valueNames, string[] valueTypes)
        {
            _name = name;
            _valueNames = valueNames;
            _valueTypes = valueTypes;
        }
        public ValueTable(string name, TableEntry enrty)
        {
            _name = name;
            _tableEntry = enrty;
        }
        public ValueTable(string name, TableEntry[] enrties)
        {
            _name = name;
            _tableEntries = enrties;
        }
    }

    public class TableEntry
    {
        public string _valueName;
        public string _valueType;
        public string _stringValue;
        public float _floatValue;
        public int _intValue;
        public Nullable<int> _nullValue;

        public TableEntry()
        {
            _valueName = "";
            _valueType = "";
            _stringValue = "";
            _floatValue = 0;
            _intValue = 0;
        }
        public TableEntry(string valueName, string valueType, string stringValue)
        {
            _valueName = valueName;
            _valueType = valueType;
            _stringValue = stringValue;
        }
        public TableEntry(string valueName, string valueType, float floatValue)
        {
            _valueName = valueName;
            _valueType = valueType;
            _floatValue = floatValue;
        }
        public TableEntry(string valueName, string valueType, int intValue)
        {
            _valueName = valueName;
            _valueType = valueType;
            _intValue = intValue;
        }
        public TableEntry(string valueName, string valueType, int? nullValue)
        {
            _valueName = valueName;
            _valueType = valueType;
            _nullValue = nullValue;
        }
    }

    public class TableMap
    {
        public string _mapName;
        public ValueTable[] _tables;

        public TableMap
        (
            string mapName,
            ValueTable[] tables
        )
        {
            _mapName = mapName;
            _tables = tables;
        }
    }
    #endregion Data Types
    
    #endregion SQLite
}
