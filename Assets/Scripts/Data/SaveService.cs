using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveService
{
    const string PlayerPrefsKey = "records_data";
    const string CsvPath = "Data/records";
    List<Record> records;

    const int maxRecordsInLeadBoard = 10;

    public void Init()
    {
        LoadRecords();
    }

    void LoadRecords()
    {
        string json = PlayerPrefs.GetString(PlayerPrefsKey, null);

        if (!string.IsNullOrEmpty(json))
        {
            records = JsonUtility.FromJson<RecordWrapper>(json).records;
        }
        else
        {
            records = LoadFromCsv();
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
        records.Add(new Record(date, score));
        records.Sort();
        if (records.Count > maxRecordsInLeadBoard) records.RemoveAt(records.Count - 1);
        SaveRecords();
    }

    void SaveRecords()
    {
        RecordWrapper wrapper = new RecordWrapper { records = records };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
    }

    public List<Record> GetRecords() => records;

    public bool IsTopScore(int score)
    {
        if (records.Count < maxRecordsInLeadBoard) return true;
        
        List<Record> sorted = new List<Record>(records);
        sorted.Sort();
        
        return score >= sorted[sorted.Count - 1].score;
    }

    public int GetScorePosition(int score)
    {
        List<Record> sorted = new List<Record>(records);
        sorted.Sort();
        
        for (int i = 0; i < sorted.Count; i++)
        {
            if (sorted[i].score <= score)
            {
                return i;
            }
        }
        return -1;
    }

    // Вспомогательный класс для сериализации списка
    [System.Serializable]
    class RecordWrapper
    {
        public List<Record> records;
    }
}