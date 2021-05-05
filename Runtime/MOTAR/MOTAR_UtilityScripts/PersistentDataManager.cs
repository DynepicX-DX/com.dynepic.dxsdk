using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;

public class PersistentDataManager
{
    public static readonly PersistentDataManager Instance = new PersistentDataManager();

    private PersistentDataManager()
    {
    }

    static PersistentDataManager()
    {
    }

    public void Save(object data, string key)
    {
        var filepath = $"{Application.persistentDataPath}/{key}.dat";

        try
        {
            using (var file = File.Create(filepath))
            {
                new BinaryFormatter().Serialize(file, data);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public T Load<T>(string key)
    {
        var filepath = $"{Application.persistentDataPath}/{key}.dat";

        try
        {
            using (var file = File.Open(filepath, FileMode.Open))
            {
                return (T) new BinaryFormatter().Deserialize(file);
            }
        }
        catch (FileNotFoundException e)
        {
            return default(T);
        }
    }

    public void Clear(string key)
    {
        var filepath = $"{Application.persistentDataPath}/{key}.dat";
        try
        {
            File.Delete(filepath);
        }
        catch
        {
            
        }
    }
}