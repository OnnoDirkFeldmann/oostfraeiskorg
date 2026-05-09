using System;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace oostfraeiskorg.Services;

public class TranslationCounterService
{
    private readonly string _dataFilePath;
    private readonly object _lock = new object();
    private TranslationCounterData _data;

    public TranslationCounterService()
    {
        // Store the counter file in the app's data directory
        _dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "translation_counter.json");
        LoadData();
    }

    private void LoadData()
    {
        lock (_lock)
        {
            if (File.Exists(_dataFilePath))
            {
                try
                {
                    string json = File.ReadAllText(_dataFilePath);
                    _data = JsonSerializer.Deserialize<TranslationCounterData>(json) ?? CreateNewData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading translation counter data: {ex.Message}");
                    _data = CreateNewData();
                }
            }
            else
            {
                _data = CreateNewData();
                SaveData();
            }
        }
    }

    private TranslationCounterData CreateNewData()
    {
        return new TranslationCounterData
        {
            Count = 0,
            StartDate = DateTime.UtcNow
        };
    }

    private void SaveData()
    {
        try
        {
            string json = JsonSerializer.Serialize(_data, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving translation counter data: {ex.Message}");
        }
    }

    public void IncrementCounter()
    {
        lock (_lock)
        {
            _data.Count++;
            SaveData();
        }
    }

    public int GetCount()
    {
        lock (_lock)
        {
            return _data.Count;
        }
    }

    public DateTime GetStartDate()
    {
        lock (_lock)
        {
            return _data.StartDate;
        }
    }

    private class TranslationCounterData
    {
        public int Count { get; set; }
        public DateTime StartDate { get; set; }
    }
}
