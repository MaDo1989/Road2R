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

    //this method will return the accurate score for the percentage for example:
    // percentage = 0.5 and the range is 0.4-0.6 and the score of this range is 3 so his accurate score will be
    // 3 + (0.5-0.4)/(0.6-0.4) * (3-1) = 4 (the 1 is the score of range 0.2-0.4)
    public float GetAccurateScoring(float percentage,List<ScoreConfigDic> paramsList)
    {
        float AccurateScore = 0;
        float theScoreOfOneRowBefore = 0;
        foreach (var row in paramsList)
        {
            if (percentage> row.MinRangeValue && percentage<=row.MaxRangeValue)
            {
                AccurateScore = row.Score+ ((percentage-row.MinRangeValue)/(row.MaxRangeValue-row.MinRangeValue))*(row.Score-theScoreOfOneRowBefore);
                return AccurateScore;
            }
            theScoreOfOneRowBefore = row.Score;
        }
        return AccurateScore;
        
    }
}