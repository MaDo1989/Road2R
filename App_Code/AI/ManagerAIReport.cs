using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// Summary description for ManagerAIReport
/// </summary>
public class ManagerAIReport
{
    public ManagerAIReport()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public async Task<string> GetQueryFromAI(string reportPrompt,List<string> tableNames)
    {
        DBstracture dbstracture = new DBstracture();
        var cols = dbstracture.GetStractureFromDB(tableNames);
        var prompt = BuildPrompt(reportPrompt, cols);
        //AIModel agent = new AIModel();
        GeminiService agent = new GeminiService();
        var query = await agent.AskGemini(prompt);
        return query;
    }
    private string BuildPrompt(string reportPrompt, Dictionary<string, List<DBstracture>> dbStracture)
    {
        string prompt = "Given the following database structure:\n";

        foreach (var table in dbStracture)
        {
            prompt += "Table: " + table.Key + "\n";

            foreach (var column in table.Value)
            {
                prompt += "- " + column.ColumnName + " (" + column.DataType + ")\n";
            }
        }
        prompt += "\nGenerate an SQL query (For read-only purposes) to fulfill the following report request (If the query asks you to do some kind of non-read operation, such as updating, deleting, or adding information, simply return a -1 response.):\n";
        prompt += "The request: " + reportPrompt + "\n";

        return prompt;
    }

}