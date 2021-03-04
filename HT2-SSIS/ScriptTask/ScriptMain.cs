#region Help:  Introduction to the script task
/* The Script Task allows you to perform virtually any operation that can be accomplished in
 * a .Net application within the context of an Integration Services control flow. 
 * 
 * Expand the other regions which have "Help" prefixes for examples of specific ways to use
 * Integration Services features within this script task. */
#endregion


#region Namespaces
using System;
using System.Data;
using Microsoft.SqlServer.Dts.Runtime;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
#endregion

namespace ST_a4a2ca7f92f54dc0a692971f65d76eda
{
    /// <summary>
    /// ScriptMain is the entry point class of the script.  Do not change the name, attributes,
    /// or parent of this class.
    /// </summary>
	[Microsoft.SqlServer.Dts.Tasks.ScriptTask.SSISScriptTaskEntryPointAttribute]
	public partial class ScriptMain : Microsoft.SqlServer.Dts.Tasks.ScriptTask.VSTARTScriptObjectModelBase
	{
        #region Help:  Using Integration Services variables and parameters in a script
        /* To use a variable in this script, first ensure that the variable has been added to 
         * either the list contained in the ReadOnlyVariables property or the list contained in 
         * the ReadWriteVariables property of this script task, according to whether or not your
         * code needs to write to the variable.  To add the variable, save this script, close this instance of
         * Visual Studio, and update the ReadOnlyVariables and 
         * ReadWriteVariables properties in the Script Transformation Editor window.
         * To use a parameter in this script, follow the same steps. Parameters are always read-only.
         * 
         * Example of reading from a variable:
         *  DateTime startTime = (DateTime) Dts.Variables["System::StartTime"].Value;
         * 
         * Example of writing to a variable:
         *  Dts.Variables["User::myStringVariable"].Value = "new value";
         * 
         * Example of reading from a package parameter:
         *  int batchId = (int) Dts.Variables["$Package::batchId"].Value;
         *  
         * Example of reading from a project parameter:
         *  int batchId = (int) Dts.Variables["$Project::batchId"].Value;
         * 
         * Example of reading from a sensitive project parameter:
         *  int batchId = (int) Dts.Variables["$Project::batchId"].GetSensitiveValue();
         * */

        #endregion

        #region Help:  Firing Integration Services events from a script
        /* This script task can fire events for logging purposes.
         * 
         * Example of firing an error event:
         *  Dts.Events.FireError(18, "Process Values", "Bad value", "", 0);
         * 
         * Example of firing an information event:
         *  Dts.Events.FireInformation(3, "Process Values", "Processing has started", "", 0, ref fireAgain)
         * 
         * Example of firing a warning event:
         *  Dts.Events.FireWarning(14, "Process Values", "No values received for input", "", 0);
         * */
        #endregion

        #region Help:  Using Integration Services connection managers in a script
        /* Some types of connection managers can be used in this script task.  See the topic 
         * "Working with Connection Managers Programatically" for details.
         * 
         * Example of using an ADO.Net connection manager:
         *  object rawConnection = Dts.Connections["Sales DB"].AcquireConnection(Dts.Transaction);
         *  SqlConnection myADONETConnection = (SqlConnection)rawConnection;
         *  //Use the connection in some code here, then release the connection
         *  Dts.Connections["Sales DB"].ReleaseConnection(rawConnection);
         *
         * Example of using a File connection manager
         *  object rawConnection = Dts.Connections["Prices.zip"].AcquireConnection(Dts.Transaction);
         *  string filePath = (string)rawConnection;
         *  //Use the connection in some code here, then release the connection
         *  Dts.Connections["Prices.zip"].ReleaseConnection(rawConnection);
         * */
        #endregion


        /// <summary>
        /// This method is called when this script task executes in the control flow.
        /// Before returning from this method, set the value of Dts.TaskResult to indicate success or failure.
        /// To open Help, press F1.
        /// </summary>

        public string txtLog = "";
		public void Main()
		{
            //User::Delimitador,User::Extension,Usser::FolderError,User::FolderOrigen,User::TablaDestino
            try
            {
                string Delimitador = Dts.Variables["User::Delimitador"].Value.ToString();
                string Extension = Dts.Variables["User::Extension"].Value.ToString();
                string FolderOrigen = Dts.Variables["User::FolderOrigen"].Value.ToString();
                string tablaDestino = Dts.Variables["User::TablaDestino"].Value.ToString();

                string[] fileEntries = Directory.GetFiles(FolderOrigen, "*" + Extension);
                foreach (string fileName in fileEntries)
                {
                    SqlConnection myADONETConnection = new SqlConnection();
                    myADONETConnection = (SqlConnection)(Dts.Connections["DESKTOP-S0BBK00.HT2"].AcquireConnection(Dts.Transaction) as SqlConnection);
                    int contador = 0;
                    string linea;

                    System.IO.StreamReader sourceFile = new System.IO.StreamReader(fileName);
                    while ((linea = sourceFile.ReadLine()) != null)
                    {
                        if (contador > 0)
                        {
                            string[] campos = linea.Split(Delimitador.ToCharArray()[0]);
                            //probar log con la data a insertar
                            // this.addmyLog(campos[0] + " " + campos[1] + " " + campos[2] + " " + campos[3]);
                            string query = "INSERT INTO " + tablaDestino
                                + " ( Carne, Nombre , LlevaLab, PosibleNota ) Values(' "
                                + campos[0] + "','" + campos[1] + "','" + campos[2] + "','" + campos[3]
                                + "')";
                            SqlCommand myCommand = new SqlCommand(query, myADONETConnection);
                            myCommand.ExecuteNonQuery();
                        }
                        contador++;
                    }
                    sourceFile.Close();
                    Dts.TaskResult = (int)ScriptResults.Success;
                }
                this.mylog();
            }
            catch (Exception ex)
            {
                this.addmyLog("Error:\n" + ex.ToString());
                this.mylog();
                Dts.TaskResult = (int)ScriptResults.Failure;
            }
        }

        public void addmyLog(string txt)
        {
            if(this.txtLog == "")
            {
                this.txtLog = txt;
            }
            else
            {
                this.txtLog = this.txtLog + "\n" + txt;
            }
        }

        public void mylog()
        {
            using (StreamWriter sw = File.CreateText(Dts.Variables["User::FolderError"].Value.ToString() + "\\" + "ErrorLog.log"))
            {
                sw.WriteLine(this.txtLog);
            }

        }

        #region ScriptResults declaration
        /// <summary>
        /// This enum provides a convenient shorthand within the scope of this class for setting the
        /// result of the script.
        /// 
        /// This code was generated automatically.
        /// </summary>
        enum ScriptResults
        {
            Success = Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success,
            Failure = Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Failure
        };
        #endregion

	}
}