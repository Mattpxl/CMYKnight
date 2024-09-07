using System;

namespace DataStorage {
public class PlayerData
{
   private SQLConverter _sqlConverter = new SQLConverter();
    private static String[] _valueTypes = new String[3]
            {
                "INT",
                "REAL", 
                "REAL",
            };
    private static String[] _valueNames = new String[3]
            {
                "Level",
                "Lives", 
                "Keys",
            };
    private static ValueTable  _playerData = new ValueTable
        (
            "PlayerData",
            _valueNames,
            _valueTypes
        );
    //primary access key
    private static TableEntry _playerTableID = new TableEntry
        (
            "PlayerData_id",
            "INT",
            1
        );
    private static TableEntry _playerLevelDefault = new TableEntry
        (
            "Level",
            "INT",
            1
        );
    private static TableEntry _playerLivesDefault = new TableEntry
        (
            "Lives",
            "REAL",
            3
        );
    private static TableEntry _playerKeysDefault = new TableEntry
        (
            "Keys",
            "REAL",
            0
        );
    private TableEntry[] _playerAccessDataSet = new TableEntry[4]
        {
            _playerTableID,
            _playerLevelDefault,
            _playerLivesDefault,
            _playerKeysDefault
        };
    
    public void initializeTable()
    {
        _sqlConverter.generateTable(_playerData);
    }
    public TableEntry loadPlayerValue(TableEntry entry)
    {
        TableEntry defaultValue = new TableEntry();
        foreach (TableEntry e in _playerAccessDataSet)
        {
            if (e._valueName == entry._valueName) defaultValue = e;
        }
        TableEntry value = new TableEntry();
        switch (entry._valueType)
        {
            case "TEXT":
                value = _sqlConverter.getValue(_playerData, entry, _playerTableID)._stringValue == null ? defaultValue : _sqlConverter.getValue(_playerData, entry, _playerTableID);
                break;
            case "REAL":
                value = _sqlConverter.getValue(_playerData, entry, _playerTableID)._floatValue == 0 ? defaultValue : _sqlConverter.getValue(_playerData, entry, _playerTableID);
                break;
            case "INT":
                value = _sqlConverter.getValue(_playerData, entry, _playerTableID)._intValue == 0 ? defaultValue : _sqlConverter.getValue(_playerData, entry, _playerTableID);
                break;
        }
        return value;
    }

    public void save(TableEntry[] _playerDataSet)
    {
        _sqlConverter.updateTable(_playerData, _playerDataSet, _playerTableID);
    }
}
}
