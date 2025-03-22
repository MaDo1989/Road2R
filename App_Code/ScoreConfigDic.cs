using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ScoreConfigDic
/// </summary>
public class ScoreConfigDic
{
    int id;
    string parameter;
    float minRangeValue;
    float maxRangeValue;
    float score;

    public ScoreConfigDic()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }

    public string Parameter
    {
        get
        {
            return parameter;
        }

        set
        {
            parameter = value;
        }
    }

    public float MinRangeValue
    {
        get
        {
            return minRangeValue;
        }

        set
        {
            minRangeValue = value;
        }
    }

    public float MaxRangeValue
    {
        get
        {
            return maxRangeValue;
        }

        set
        {
            maxRangeValue = value;
        }
    }

    public float Score
    {
        get
        {
            return score;
        }

        set
        {
            score = value;
        }
    }




    static public List<ScoreConfigDic> GetAllScoreConfigDictionary()
    {
        //return the all list from the DBservice -> sp = sp_getParamDic
        DBservice_Gilad db = new DBservice_Gilad();
        return db.GetAllConfigList();
      

    }

    static public bool updateScoreConfigDic(List<ScoreConfigDic> listToUpdate)
    {
        DBservice_Gilad db = new DBservice_Gilad();
        int sumOfres = 0;
        foreach (var item in listToUpdate)
        {
            sumOfres+= db.updateScoreConfigDic(item.id,item.score);
        }
        return sumOfres == listToUpdate.Count;
    }

    

    static public List<ScoreConfigDic> GetConfigByParameterName(string parameter,List<ScoreConfigDic> scoreConfigList)
    {
        List<ScoreConfigDic> listToReturn = new List<ScoreConfigDic>();
        foreach (var item in scoreConfigList)
        {
            if (item.Parameter == parameter)
            {
                listToReturn.Add(item);
            }
        }
        return listToReturn;
    }
}