using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System;
using System.Collections.Generic;

public class DBManager : Singleton<DBManager>
{
	public string[] fileName;
    public string tableName = "ItemList";
    public string tableName2 = "QuestList";

    public Dictionary<int, ItemInpo> itemInpos = new Dictionary<int, ItemInpo>();
    public Dictionary<int, QuestList> questList = new Dictionary<int, QuestList>();

    private void Start()
    {
        DBCreate();
        DBRead();
    }

    public void ReDB()
    {
        Debug.Log("ReDB");
        itemInpos.Clear();
        questList.Clear();
        DBRead();
    }

    private void DBCreate()
	{
        string filePath = "";

        for (int i = 0; i < 2; i++)
        {
            filePath = Application.dataPath + fileName[i];
            Debug.Log(fileName[i]);

            if (!File.Exists(filePath))
            {
                File.Copy(Application.streamingAssetsPath + fileName[i], filePath);
            }
        }
    }

	public string GetDBFilePath(int count)
    {
		string str = string.Empty;
        if(count == 0)
		    str = "URI=file:" + Application.dataPath + fileName[0];
        else
            str = "URI=file:" + Application.dataPath + fileName[1];
        return str;
    }

    public void DBConnectionCheck()
    {
        try
        {
            IDbConnection dbConnection = new SqliteConnection(GetDBFilePath(0));
            dbConnection.Open();
            
            if(dbConnection.State == ConnectionState.Open)
            {
                Debug.Log("성공");
            }
            else
            {
                Debug.Log("실패");
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    public void DBRead()
    {
        string query = "";

        for (int i = 0; i < 2; i++)
        {

            if (i == 0)
                query = "Select * From " + tableName;
            else if (i == 1)
                query = "Select * From " + tableName2;

            IDbConnection dbConnection = new SqliteConnection(GetDBFilePath(i));
            dbConnection.Open();
            IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = query;
            IDataReader dataReader = dbCommand.ExecuteReader();

            while (dataReader.Read())
            {
                if (i == 0)
                {
                    itemInpos.Add(dataReader.GetInt32(0), new ItemInpo(dataReader.GetString(1), (Item.ItemType)dataReader.GetInt32(2), 
                        (Item.ItemClass)dataReader.GetInt32(3), dataReader.GetInt32(4),(Item.ItemKinds)dataReader.GetInt32(5), 
                        dataReader.GetString(6), dataReader.GetString(7), dataReader.GetInt32(0), dataReader.GetInt32(8)));
                }
                else if(i == 1)
                {
                    questList.Add(dataReader.GetInt32(0), new QuestList(dataReader.GetInt32(0),dataReader.GetString(1), dataReader.GetInt32(2), 
                        dataReader.GetString(3), dataReader.GetString(4),dataReader.GetString(5),dataReader.GetString(6),dataReader.GetString(7), 
                        dataReader.GetString(8), dataReader.GetString(9),dataReader.GetString(10), dataReader.GetString(11), dataReader.GetString(12),
                        dataReader.GetInt32(13), dataReader.GetInt32(14), dataReader.GetInt32(15),dataReader.GetInt32(16) ,dataReader.GetInt32(17),
                        dataReader.GetInt32(18), dataReader.GetString(19)));
                }
            }

            dataReader.Dispose();
            dataReader = null;
            dbCommand = null;
            dbConnection.Close();
            dbConnection = null;
        }
    }

}
