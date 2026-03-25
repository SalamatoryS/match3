using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveService
{
    const string PlayerPrefsKey = "records_data";
    const string CsvPath = "Data/records";
    List<Record> _records;

    public void Init()
    {
        LoadRecords();
    }

    void LoadRecords()
    {
        string json = PlayerPrefs.GetString(PlayerPrefsKey, null);

        if (!string.IsNullOrEmpty(json))
        {
            _records = JsonUtility.FromJson<RecordWrapper>(json).records;
        }
        else
        {
            _records = LoadFromCsv();
            SaveRecords();
        }
    }

    List<Record> LoadFromCsv()
    {
        List<Record> loadedRecords = new List<Record>();
        TextAsset csvFile = Resources.Load<TextAsset>(CsvPath);

        if (csvFile != null)
        {
            string[] lines = csvFile.text.Split('\n');
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;
                string[] parts = lines[i].Split(';');
                if (parts.Length >= 2)
                {
                    loadedRecords.Add(new Record(parts[0].Trim(), int.Parse(parts[1].Trim())));
                }
            }
        }
        return loadedRecords;
    }

    public void SaveRecord(string date, int score)
    {
        _records.Add(new Record(date, score));
        _records.Sort();
        if (_records.Count > 10) _records.RemoveAt(_records.Count - 1);
        SaveRecords();
    }

    void SaveRecords()
    {
        RecordWrapper wrapper = new RecordWrapper { records = _records };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
    }

    public List<Record> GetRecords() => _records;

    // Вспомогательный класс для сериализации списка
    [System.Serializable]
    class RecordWrapper
    {
        public List<Record> records;
    }
}