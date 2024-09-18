using UnityEngine;
using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Mono.Data.Sqlite;
using System.Linq;

namespace DataStorage
{
  
    #region Serialization
    /// <summary>
    /// A generic wrapper class used to serialize and deserialize a collection of data items in JSON format.
    /// This class wraps a list of items of type <typeparamref name="T"/> to facilitate JSON serialization
    /// and deserialization for collections of data.
    /// </summary>
    /// <typeparam name="T">The type of the data items contained within the wrapper.</typeparam>
    [Serializable]
    public class Wrapper<T>
    {
    /// <summary>
    /// Gets or sets the list of data items contained within the wrapper.
    /// </summary>
    /// <remarks>
    /// This list holds the items of type <typeparamref name="T"/> that are to be serialized or deserialized.
    /// </remarks>
    public List<T> data;
    }
    /// <summary>
    /// The JSONConverter class provides methods for serializing and deserializing data in JSON format. 
    /// It supports operations such as writing and reading float and T [type] data, retrieving raw JSON strings, 
    /// and overwriting objects with JSON data. It uses <see cref="JsonUtility"/> for serialization and deserialization.
    /// </summary>
    public class JSONConverter
    {
        private static JSONConverter _jsonConverter; 
        private static readonly object _lockObject = new(); // Lock object for thread safety
        private JSONConverter() {} // private Constructor to prevent instantiation
        public static JSONConverter jsonConverter 
        {
            get 
            { 
                lock(_lockObject)
                {
                    try
                    {
                        return _jsonConverter ??= new JSONConverter(); 
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Exception during JSONConverter instantiation: {ex.Message}");
                        throw; // Re-throw the exception after logging it
                    }
                }
            }
        }
          
        /// <summary>
        /// Constructs the file path for a given identifier by appending ".json" to the id.
        /// </summary>
        /// <param name="id">A string representing the unique identifier for the file.</param>
        /// <returns>A string representing the full path to the JSON file.</returns>
        private string filePath(string id)
        {
            return Path.Combine(Application.persistentDataPath, id + ".json");
        }
        /// <summary>
        /// Ensures that the specified file exists at the given file path.
        /// If the file does not exist, it throws a <see cref="FileNotFoundException"/>.
        /// </summary>
        /// <param name="_filePath">The full path of the file to check.</param>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the file at the specified path does not exist.
        /// </exception>
        private void EnsureFileExists(string _filePath)
        { 
            if (!File.Exists(_filePath))
            {
                Debug.LogError($"FileNotFoundException: The file {_filePath} does not exist.");
                throw new FileNotFoundException($"The file {_filePath} does not exist.");
            }
        }   
 
        /// <summary>
        /// Serializes a float value to JSON and writes it to a file at the path associated with the provided id.
        /// Thread Safe
        /// </summary>
        /// <param name="obj">The [type] value to serialize and save.</param>
        /// <param name="id">The unique identifier for the JSON file to be written.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown when the application doesn't have permission to write to the specified file.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs during the writing process (e.g., insufficient disk space).</exception>
        /// <exception cref="Exception">Catches any other unexpected errors that occur during the file writing process.</exception>
        public void writeData<T>(T obj, string id)
        {
            lock (_lockObject)  // Ensure thread safety
            {
            try
            {
                // Log the start of the write operation
                // Debug.Log($"Attempting to write data to file: {id}.json");
                // get filepath
                string _filePath = filePath(id);
                // Convert the object to a JSON string
                string _currentDataString = JsonUtility.ToJson(obj);
                // Log the full file path being written to
                //Debug.Log($"Writing data to path: {_filePath}");
                // Write the JSON data to the file
                using StreamWriter streamWriter = new(_filePath);
                streamWriter.Write(_currentDataString);
                // Log the successful write operation
                //Debug.Log($"Successfully wrote data to {_filePath}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.LogError($"UnauthorizedAccessException: Failed to write data to {id}.json. Check file permissions. Error: {ex.Message}");
            }
            catch (IOException ex)
            {
                Debug.LogError($"IOException: Failed to write data to {id}.json. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception: An unexpected error occurred while writing data to {id}.json. Error: {ex.Message}");
                throw; // Re-throw the exception if needed for higher-level handling
            }
            }
        }
        public void writeData<T>(IEnumerable<T> dataSet, string id)
        {
            lock (_lockObject)  // Ensure thread safety
            {
            try
            {
                // Log the start of the write operation
                //Debug.Log($"Attempting to write data to file: {id}.json");
                // get filepath
                string _filePath = filePath(id);
                // Convert the object to a JSON string
                var wrapper = new Wrapper<T> {data = dataSet.ToList()};
                string _currentDataString = JsonUtility.ToJson(wrapper);
                // Log the full file path being written to
                //Debug.Log($"Writing data to path: {_filePath}");
                // Write the JSON data to the file
                using StreamWriter streamWriter = new(_filePath);
                streamWriter.Write(_currentDataString);
                // Log the successful write operation
                //Debug.Log($"Successfully wrote data to {_filePath}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.LogError($"UnauthorizedAccessException: Failed to write data to {id}.json. Check file permissions. Error: {ex.Message}");
            }
            catch (IOException ex)
            {
                Debug.LogError($"IOException: Failed to write data to {id}.json. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception: An unexpected error occurred while writing data to {id}.json. Error: {ex.Message}");
                throw; // Re-throw the exception if needed for higher-level handling
            }
            }
        }
        /// <summary>
        /// Reads data from a JSON file, deserializes it into a float, and returns the serialized JSON string.
        /// Thread Safe
        /// </summary>
        /// <param name="id">The unique identifier for the JSON file to be read.</param>
        /// <returns>A JSON string representation of the deserialized float object.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the application doesn't have permission to read the file.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs during the reading process.</exception>
        /// <exception cref="Exception">Catches any other unexpected errors that occur during the file reading process.</exception> 
        public string readData(string id)
        {
            lock (_lockObject)  // Ensure thread safety
            {
            try
            {
                // Log the start of the read operation
                //Debug.Log($"Attempting to read data from file: {id}.json");
                // get filepath
                string _filePath = filePath(id);
                EnsureFileExists(_filePath);
                // Read the JSON data from the file
                using StreamReader streamReader = new(_filePath);
                string savedDataString = streamReader.ReadToEnd();
                //Debug.Log($"Successfully read data from {_filePath}");
                return savedDataString;
            }
             catch (FileNotFoundException ex)
            {
                Debug.LogError($"FileNotFoundException: Could not find the file {id}.json. Error: {ex.Message}");
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.LogError($"UnauthorizedAccessException: Access to the file {id}.json was denied. Error: {ex.Message}");
                throw;
            }
            catch (IOException ex)
            {
                Debug.LogError($"IOException: Failed to read data from {id}.json. Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception: An unexpected error occurred while reading data from {id}.json. Error: {ex.Message}");
                throw;
            }
            }
        }
        
        /// <summary>
        /// Retrieves the raw JSON string from the file associated with the given identifier.
        /// </summary>
        /// <param name="id">The unique identifier for the JSON file to be read.</param>
        /// <returns>The raw JSON string from the file.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the application doesn't have permission to read the file.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs during the reading process.</exception>
        /// <exception cref="Exception">Catches any other unexpected errors that occur during the file reading process.</exception>
        public string getJSON(string id)
        {
            try
            {
                // Log the start of the JSON retrieval process
                //Debug.Log($"Attempting to get JSON data from file: {id}.json");
                // Construct the file path (assuming filePath is a custom method)
                string _filePath = filePath(id);
                EnsureFileExists(_filePath);
                // Log the file path being accessed
                //Debug.Log($"Retrieving data from path: {_filePath}");
                // Read the JSON data from the file
                using StreamReader streamReader = new StreamReader(_filePath);
                string savedDataString = streamReader.ReadToEnd();
                // Log the successful file read
                //Debug.Log($"Successfully read data from {_filePath}");

                // Return the original JSON string directly without converting it back and forth
                return savedDataString;
            }
            catch (FileNotFoundException ex)
            {
                Debug.LogError($"FileNotFoundException: Could not find the file {id}.json. Error: {ex.Message}");
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.LogError($"UnauthorizedAccessException: Access to the file {id}.json was denied. Error: {ex.Message}");
                throw;
            }
            catch (IOException ex)
            {
                Debug.LogError($"IOException: Failed to read data from {id}.json. Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception: An unexpected error occurred while retrieving data from {id}.json. Error: {ex.Message}");
                throw;
            }
        }
        /// <summary>
        /// Retrieves data from a JSON file, deserializes it into a [type]], and returns the value.
        /// Thread Safe
        /// </summary>
        /// <param name="id">The unique identifier for the JSON file to be read.</param>
        /// <returns>The deserialized [type] value from the JSON file.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the application doesn't have permission to read the file.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs during the reading process.</exception>
        /// <exception cref="Exception">Catches any other unexpected errors that occur during the file reading process.</exception>
        public T getDataObject<T>(string id)
        {
            lock (_lockObject)  // Ensure thread safety
            {
            try
            {
                // Log the start of the data retrieval process
                //Debug.Log($"Attempting to get data object from file: {id}.json");

                // Construct the file path (assuming filePath is a custom method)
                string _filePath = filePath(id);
                EnsureFileExists(_filePath);

                // Log the file path being accessed
                //Debug.Log($"Reading data from path: {_filePath}");
                // Read the JSON data from the file
                using StreamReader streamReader = new StreamReader(_filePath);
                string savedDataString = streamReader.ReadToEnd();

                // Deserialize the JSON string to a float value
                T obj = JsonUtility.FromJson<T>(savedDataString);

                // Log the successful read operation
                //Debug.Log($"Successfully retrieved data from {_filePath}");

                // Return the deserialized float value
                return obj;
            }
            catch (FileNotFoundException ex)
            {
                Debug.LogError($"FileNotFoundException: Could not find the file {id}.json. Error: {ex.Message}");
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
            Debug.LogError($"UnauthorizedAccessException: Access to the file {id}.json was denied. Error: {ex.Message}");
            throw;
            }
            catch (IOException ex)
            {
                Debug.LogError($"IOException: Failed to read data from {id}.json. Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception: An unexpected error occurred while retrieving data from {id}.json. Error: {ex.Message}");
                throw;
            }
            }
        }
        public List<T> getDataObjects<T>(string id)
        {
            lock (_lockObject)  // Ensure thread safety
            {
            try
            {
                // Log the start of the data retrieval process
                //Debug.Log($"Attempting to get data object from file: {id}.json");

                // Construct the file path (assuming filePath is a custom method)
                string _filePath = filePath(id);
                EnsureFileExists(_filePath);

                // Log the file path being accessed
                //Debug.Log($"Reading data from path: {_filePath}");
                // Read the JSON data from the file
                using StreamReader streamReader = new StreamReader(_filePath);
                string savedDataString = streamReader.ReadToEnd();
                var result = JsonUtility.FromJson<Wrapper<T>>(savedDataString);
                //Debug.Log($"Successfully read collection from {_filePath}");
                return result.data.ToList(); // Convert array back to List
            }
            catch (FileNotFoundException ex)
            {
                Debug.LogError($"FileNotFoundException: Could not find the file {id}.json. Error: {ex.Message}");
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
            Debug.LogError($"UnauthorizedAccessException: Access to the file {id}.json was denied. Error: {ex.Message}");
            throw;
            }
            catch (IOException ex)
            {
                Debug.LogError($"IOException: Failed to read data from {id}.json. Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception: An unexpected error occurred while retrieving data from {id}.json. Error: {ex.Message}");
                throw;
            }
            }
        }
        /// <summary>
        /// Overwrites the provided object with data from the given JSON string. 
        /// For class types, it uses <see cref="JsonUtility.FromJsonOverwrite"/>.
        /// For primitive types, it deserializes the JSON and assigns the value to the object.
        /// Thread Safe
        /// </summary>
        /// <typeparam name="T">The type of the object being overwritten. It can be a class or a primitive type.</typeparam>
        /// <param name="obj">A reference to the object to be overwritten or updated.</param>
        /// <param name="json">The JSON string containing the new data.</param>
        /// <exception cref="Exception">Catches and logs any unexpected errors during the overwrite process.</exception>
        public void overwriteDataObject<T>(ref T obj, string json)
        {
            lock(_lockObject)
            {
            try
            {
                // Log the operation start
                //Debug.Log("Attempting to overwrite data object with JSON data.");

                // Check if the object is a class (i.e., not a primitive)
                if (typeof(T).IsClass)
                {
                    // Use JsonUtility.FromJsonOverwrite for class types
                    JsonUtility.FromJsonOverwrite(json, obj);
                    //Debug.Log("Successfully overwrote the class object with new JSON data.");
                }
                else
                {
                    // For primitives, we need to deserialize the value and assign it to obj
                    obj = JsonUtility.FromJson<T>(json);
                    //Debug.Log($"Successfully assigned new value from JSON to primitive type: {obj}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception: An error occurred while overwriting the data object. Error: {ex.Message}");
                throw;
            }
            }
        }   
    /// <summary>
    /// Overwrites the provided set of objects with data from the given JSON strings. 
    /// For class types, it uses <see cref="JsonUtility.FromJsonOverwrite"/> to update the existing objects. 
    /// For primitive types, it deserializes each JSON string and assigns the new value to the corresponding object in the set.
    /// </summary>
    /// <typeparam name="T">The type of the objects being overwritten. It can be a class type or a primitive type.</typeparam>
    /// <param name="objects">A collection of references to the objects to be overwritten or updated. These should be passed by reference to allow modifications.</param>
    /// <param name="jsons">A collection of JSON strings containing the new data to be applied to the objects. Each string corresponds to an object in the set.</param>
    /// <exception cref="ArgumentException">Thrown when the number of JSON strings does not match the number of objects provided in the collection.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs during the overwrite process. Includes issues with the JSON string format or data.</exception>
    /// <remarks>
    /// This method uses a thread lock to ensure that the overwrite operation is thread-safe. 
    /// For class types, <see cref="JsonUtility.FromJsonOverwrite"/> is used to merge the new data into the existing objects.
    /// For primitive types, each JSON string is deserialized to update the value in the corresponding object.
    /// Ensure that the JSON strings are correctly formatted for the expected type of the objects, and that the number of JSON strings matches the number of objects.
    /// </remarks>
    public void overwriteDataObjects<T>(ICollection<T> objects, ICollection<string> jsons)
    {
    lock(_lockObject)
    {
        if (objects.Count != jsons.Count)
        {
            throw new ArgumentException("The number of JSON strings must match the number of objects.");
        }

        var objectEnumerator = objects.GetEnumerator();
        var jsonEnumerator = jsons.GetEnumerator();

        try
        {
            while (objectEnumerator.MoveNext() && jsonEnumerator.MoveNext())
            {
                var obj = objectEnumerator.Current;
                var json = jsonEnumerator.Current;

                // Log the operation start
                Debug.Log("Attempting to overwrite data object with JSON data.");

                // Check if the object is a class (i.e., not a primitive)
                if (typeof(T).IsClass)
                {
                    // Use JsonUtility.FromJsonOverwrite for class types
                    JsonUtility.FromJsonOverwrite(json, obj);
                    Debug.Log("Successfully overwrote the class object with new JSON data.");
                }
                else
                {
                    // For primitives, we need to deserialize the value and assign it to obj
                    obj = JsonUtility.FromJson<T>(json);
                    Debug.Log($"Successfully assigned new value from JSON to primitive type: {obj}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception: An error occurred while overwriting the data objects. Error: {ex.Message}");
            throw;
        }
    }
}

    }
    #endregion Serialization
   
    #region SQLite
          
    /// <summary>
    /// Contains Methods for creating and maintaining a SQLite data base
    /// </summary>
    #region Converter
        public class SQLConverter
        {
        // SINGLETON DEFINITION
        private static SQLConverter _sqlConverter;
        private static readonly object _lockObject = new();
        private SQLConverter() // private Constructor to prevent instantiation
        {
            EnableWAL();
        } 
        public static SQLConverter sqlConverter 
        {
            get 
            { 
                lock( _lockObject)
                {
                    try
                    {
                        return _sqlConverter ??= new SQLConverter();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Exception during SQLConverter instantiation: {ex.Message}");
                        throw; // Re-throw the exception after logging it
                    }
                }
            }
        }
        private static readonly string _dbTag = "URI=file:";
        private static readonly string _dbName = "CMYKDB";
        private static readonly string _extension = ".db";
        private static readonly string[] _directory = 
        {
            "Assets/",
            "Scripts/",
            "2DAPI/",
            "Data/"
        };
        private static readonly Dictionary<string, List<Type>> _supportedTypes = new()
        {
            { "TEXT", new List<Type> { typeof(string) } },
            { "INT", new List<Type> { typeof(int), typeof(bool) } },
            { "REAL", new List<Type> { typeof(float), typeof(double) } }
        };
        private static readonly string _path = $"{_dbTag}{string.Join("", _directory)}{_dbName}{_extension}";
        private void ExecuteNonQuery(string sqlCommand)
        {
            lock(_lockObject)
            {
                SqliteConnection connection = null;
                try
                {
                    connection = new(_path);
                    connection.Open();
                    using SqliteCommand command = new(sqlCommand, connection);
                    command.ExecuteNonQuery();
                }
                catch (SqliteException ex)
                {
                    Debug.LogError($"SQLite Exception: {ex.Message}");
                    throw new Exception($"Failed to ExecuteNonQuery -> {sqlCommand}", ex); // Pass exception details
                }
                catch (Exception ex)
                {
                    Debug.LogError($"General Exception: {ex.Message}");
                    throw; // Re-throw for upstream handling
                }
                finally
                {
                    connection?.Close(); // Ensure connection is always closed
                }
            }
        }
        private void ExecuteTransaction(List<string> sqlCommands)
        {
            SqliteConnection connection = null;
            try
            {
                connection = new(_path);
                connection.Open();
                using (SqliteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var command in sqlCommands)
                        {
                            using var cmd = new SqliteCommand(command, connection);
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
                connection.Close();

            }
            catch (SqliteException ex)
            {
             Debug.LogError($"SQLite Exception: {ex.Message}");
                throw new Exception($"Failed to ExecuteTransaction -> {sqlCommands}", ex); // Pass exception details
            }
            catch (Exception ex)
            {
                Debug.LogError($"General Exception: {ex.Message}");
                throw; // Re-throw for upstream handling
            }
            finally
            {
                connection?.Close(); // Ensure connection is always closed
            }
        }
        // Enable Write-Ahead Logging
        public void EnableWAL()
        {
            lock (_lockObject)
            {
            ExecuteNonQuery("PRAGMA journal_mode=WAL;");
            //Debug.Log("SLQConverter: Write-Ahead Logging enabled.");
            }
        }
        private object EntryType(TableEntry entry)
        {
            if (!ValidateEntry(entry))
            {
                Debug.LogError($"Invalid data type for {entry._valueName}. Got: {entry._value.GetType().Name}, expected: {string.Join(", ", _supportedTypes[entry._valueType])}");
                throw new ArgumentException($"Invalid data type for {entry._valueName}. Expected {string.Join(", ", _supportedTypes)}, but got {entry._value.GetType().Name}.");
            }
            else
            {
                return entry._valueType switch
                {
                    "TEXT" => $"'{entry._value}'",
                    "REAL" => entry._value,
                    "INT" => entry._value,
                    _ => throw new InvalidOperationException($"Unsupported value type: {entry._valueType}")
                };
            }
        }
        public bool ValidateEntry(TableEntry entry)
        {
            // Check if the value type exists in the dictionary
            if (_supportedTypes.ContainsKey(entry._valueType))
            {
                // Get the list of valid types for this SQLite type
                var validTypes = _supportedTypes[entry._valueType];

                // Check if the actual value type is in the list of valid types
                return validTypes.Contains(entry._value.GetType());
            }
            else
            {
                // If the SQLite type is not recognized, validation fails
                return false;
            }
        } 
        private string AddConditions(List<TableEntry> conditions)
        {   
            StringBuilder tableString = new();
            for(int i = 0; i < conditions.Count; i++)
            {
                tableString.AppendFormat("{0} = {1}", conditions[i]._valueName, EntryType(conditions[i]));
                if (i < conditions.Count - 1) tableString.Append(" AND ");
            }
            tableString.Append(";");
            return tableString.ToString();
        }
        public void generateTable(ValueTable table)
        {
            StringBuilder tableString = new();
            tableString.AppendFormat("CREATE TABLE IF NOT EXISTS {0} ({0}_id INTEGER PRIMARY KEY, ", table._name);

                int valueCount = table._valueNames.Count;
                for (int i = 0; i < valueCount; i++)
                {
                    tableString.AppendFormat("{0} {1}", table._valueNames[i], table._valueTypes[i]);
                    if(i < valueCount - 1) tableString.Append(", ");
                }
                tableString.Append(");");
            
            ExecuteNonQuery(tableString.ToString());
        }
        public void generateTable(List<ValueTable> tables)
        {
            foreach (ValueTable t in tables) generateTable(t);
        }
        public void addData(ValueTable table, List<TableEntry> entries)
        {
            if (entries == null || entries.Count == 0) throw new ArgumentException("Param: [List<TableEntry>] Entries must contain at least one element.");

            StringBuilder tableString = new();
            tableString.AppendFormat("INSERT INTO {0} (", table._name);
            // build names section of querie
            for(int i = 0; i < entries.Count; i++)
            {
                tableString.Append(entries[i]._valueName);
                if(i < entries.Count-1) tableString.Append(", ");
            }
            tableString.Append(") VALUES (");
            // build values section of querie
            for(int i = 0; i < entries.Count; i++)
            {
                tableString.Append(EntryType(entries[i]));
                if (i < entries.Count - 1) tableString.Append(", ");
            }
            tableString.Append(");");
            ExecuteTransaction(new()
            {
                tableString.ToString()
            });
        }

        public void updateTable(ValueTable table, List<TableEntry> entries, List<TableEntry> conditions)
        {
            if (entries == null || entries.Count == 0) throw new ArgumentException("Entries must contain at least one element.");
            if (conditions == null || conditions.Count == 0) throw new ArgumentException("Conditions must contain at least one element.");
            StringBuilder tableString = new();

            tableString.AppendFormat("UPDATE {0} SET ", table._name);
            for(int i = 0; i < entries.Count; i++)
            {
                tableString.AppendFormat("{0} = ", entries[i]._valueName);
                tableString.Append(EntryType(entries[i]));
                if (i < entries.Count - 1) tableString.Append(", ");
            }
            tableString.Append(" WHERE ");
            tableString.Append(AddConditions(conditions));
            ExecuteTransaction(new()
            {
                tableString.ToString()
            });
        }

        public void deleteData(ValueTable table, List<TableEntry> conditions)
        {
            if (conditions == null || conditions.Count == 0) throw new ArgumentException("Conditions must contain at least one element.");
            StringBuilder tableString = new();
            tableString.AppendFormat("DELETE FROM {0} WHERE ", table._name);
            tableString.Append(AddConditions(conditions));
            ExecuteTransaction(new()
            {
                tableString.ToString()
            });
        }
        public TableEntry getValue(ValueTable table, TableEntry entry, List<TableEntry> conditions)
        {
            TableEntry sendValue = null;
            StringBuilder tableString = new();
            tableString.AppendFormat("SELECT {0} FROM {1} WHERE ", entry._valueName, table._name);
            tableString.Append(AddConditions(conditions));
            using SqliteConnection connection = new(_path);
            connection.Open();
            using SqliteCommand command = new(tableString.ToString(), connection);
            using IDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                switch (entry._valueType)
                {
                    case "TEXT":
                        string s = dataReader.IsDBNull(dataReader.GetOrdinal(entry._valueName)) 
                        ? null 
                        : dataReader.GetString(dataReader.GetOrdinal(entry._valueName));
                        sendValue = new TableEntry(entry._valueName, entry._valueType, s);
                        break;
                    case "REAL":
                        float? n = dataReader.IsDBNull(dataReader.GetOrdinal(entry._valueName)) 
                        ? null 
                        : dataReader.GetFloat(dataReader.GetOrdinal(entry._valueName));
                        sendValue = new TableEntry(entry._valueName, entry._valueType, n);
                        break;
                    case "INT": // getBoolean
                        int? i = dataReader.IsDBNull(dataReader.GetOrdinal(entry._valueName)) 
                        ? null
                        : dataReader.GetInt32(dataReader.GetOrdinal(entry._valueName));
                        sendValue = new TableEntry(entry._valueName, entry._valueType, i);
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported value type: {entry._valueType}");
                }
            }
            connection.Close();
            return sendValue;

        }

        public List<TableEntry> getValues(ValueTable table, List<TableEntry> conditions)
        {
            List<TableEntry> sendValues = new();
            StringBuilder tableString = new();
            tableString.AppendFormat("SELECT {0} FROM {1} WHERE ", string.Join(", ", table._valueNames), table._name);
            tableString.Append(AddConditions(conditions));
    
            using SqliteConnection connection = new(_path);
            connection.Open();
            using SqliteCommand command = new(tableString.ToString(), connection);
            using IDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                for (int i = 0; i < table._valueNames.Count; i++)
                {
                    string columnName = table._valueNames[i];
                    string valueType = table._valueTypes[i];

                    TableEntry sendValue = valueType switch
                    {
                        "TEXT" => new TableEntry(columnName, valueType, dataReader.IsDBNull(i) ? null : dataReader.GetString(i)),
                        "REAL" => new TableEntry(columnName, valueType, dataReader.IsDBNull(i) ? (float?)null : dataReader.GetFloat(i)),
                        "INT" => new TableEntry(columnName, valueType, dataReader.IsDBNull(i) ? (int?)null : dataReader.GetInt32(i)),
                        _ => throw new InvalidOperationException($"Unsupported value type: {valueType}")
                    };
                    sendValues.Add(sendValue);
                }
            }
            connection.Close();
            return sendValues;
        }

        private List<TableEntry> getvaluesWithPagination(ValueTable table, List<TableEntry> conditions, int limit, int offset)
        {
            List<TableEntry> sendValues = new();
            StringBuilder tableString = new();
            tableString.AppendFormat("SELECT {0} FROM {1} WHERE ", string.Join(", ", table._valueNames), table._name);
            tableString.Append(AddConditions(conditions));
            tableString.AppendFormat(" LIMIT {0} OFFSET {1};", limit, offset);

            using SqliteConnection connection = new(_path);
            connection.Open();
            using SqliteCommand command = new(tableString.ToString(), connection);
            using IDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                for (int i = 0; i < table._valueNames.Count; i++)
                {
                    string columnName = table._valueNames[i];
                    string valueType = table._valueTypes[i];
                    TableEntry sendValue = valueType switch
                    {
                        "TEXT" => new TableEntry(columnName, valueType, dataReader.IsDBNull(i) ? null : dataReader.GetString(i)),
                        "REAL" => new TableEntry(columnName, valueType, dataReader.IsDBNull(i) ? (float?)null : dataReader.GetFloat(i)),
                        "INT" => new TableEntry(columnName, valueType, dataReader.IsDBNull(i) ? (int?)null : dataReader.GetInt32(i)),
                        _ => throw new InvalidOperationException($"Unsupported value type: {valueType}")
                    };
                    sendValues.Add(sendValue);
                }
            }
            connection.Close();
            return sendValues;
        }
        public List<TableEntry> getValuesWithJoin(ValueTable table1, ValueTable table2, string joinColumn)
        {
            List<TableEntry> sendValues = new();
            StringBuilder query = new();
            query.AppendFormat("SELECT * FROM {0} INNER JOIN {1} ON {0}.{2} = {1}.{2};", table1._name, table2._name, joinColumn);

            using SqliteConnection connection = new(_path);
            connection.Open();
            using SqliteCommand command = new(query.ToString(), connection);
            using IDataReader dataReader = command.ExecuteReader();

            // Read and process the joined table data...
            connection.Close();
            return sendValues;
        }   
        public void displayTable(ValueTable table)
        {
            string tableString = string.Format("SELECT * FROM {0}", table._name);
            using SqliteConnection connection = new(_path);
            connection.Open();
            SqliteCommand command = new(string.Format("{0}", tableString), connection);
            using IDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                for (int i = 0; i < table._valueNames.Count; i++) Debug.Log(string.Format("{0}: {1}", table._valueNames[i], dataReader.IsDBNull(i) ? "NULL" : dataReader.GetValue(i)));
            }
            connection.Close();
        }
        public bool CheckTableExists(string tableName)
        {
            using SqliteConnection connection = new SqliteConnection(_path);
            connection.Open();
            using SqliteCommand command = new SqliteCommand($"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}';", connection);
            using IDataReader reader = command.ExecuteReader();
            bool exists = reader.Read();
            connection.Close();
            return exists;
        }
    }
    #endregion Converter
    
    /// <summary>
    /// Contains methods for converting data types into SQLite data table entries 
    /// TableEntry > ValueTable > TableMap 
    /// </summary>
    #region Data Types

    public class ValueTable
    {
        public string _name {get; set;}
        public List<string> _valueNames {get; set;}
        public List<string> _valueTypes {get; set;}
        public ValueTable(string name, List<string> valueNames, List<string> valueTypes)
        {
            _name = name;
            _valueNames = valueNames ?? new List<string>();
            _valueTypes = valueTypes ?? new List<string>();
        }
    }

    public class TableEntry
    {
        public string _valueName  {get; set;}
        public string _valueType  {get; set;}
        public object _value {get; set;}
        public TableEntry(string valueName, string valueType, object value)
        {
            _valueName = valueName;
            _valueType = valueType;
            _value = value;
        }
    }

    public class TableMap
    {
        public string _mapName {get; set;}
        public List<ValueTable> _tables {get; set;}
        public TableMap(string mapName, List<ValueTable> tables )
        {
            _mapName = mapName;
            _tables = tables ?? new List<ValueTable>();
        }
    }
    #endregion Data Types
    
    #endregion SQLite
}
