using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PeisPlatform.Helper
{
	public abstract class DbHelperMySQL
	{
		//连接数据库信息
		public static string connectionString = "server=localhost;port=3306;user=root;password='" + install.Form1.mysqlPassword + "'; database=mysql;CharSet=utf8;SslMode=none";
		public DbHelperMySQL()
		{
		}
		public static Boolean UserLogin(string name, string password)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				try
				{
					mySqlConnection.Open();
					MySqlCommand mySqlCommand = new MySqlCommand
					{
						Connection = mySqlConnection,
						CommandText = "select * from peis_userinfo where user_name = @name and password = @password"
					};
					mySqlCommand.Parameters.AddRange(
						new MySqlParameter[]
						{
						new MySqlParameter("@name",MySqlDbType.VarChar){Value=name },
						new MySqlParameter("@password",MySqlDbType.VarChar){Value=password },
						}
					);
					mySqlCommand.ExecuteNonQuery();
					DataSet dataSet = new DataSet();
					MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
					mySqlDataAdapter.Fill(dataSet, "ds");
					if (dataSet.Tables[0].Rows.Count > 0)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				catch (MySqlException ex)
				{
					mySqlConnection.Close();
					throw new Exception(ex.Message);
				}
			}
		}


		public static int GetMaxID(string FieldName, string TableName)
		{
			string sQLString = "select max(" + FieldName + ")+1 from " + TableName;
			object single = GetSingle(sQLString);
			if (single == null)
			{
				return 1;
			}
			return int.Parse(single.ToString());
		}

		public static bool Exists(string strSql)
		{
			object single = GetSingle(strSql);
			if (object.Equals(single, null) || object.Equals(single, DBNull.Value) || int.Parse(single.ToString()) == 0)
			{
				return false;
			}
			return true;
		}

		public static bool Exists(string strSql, params MySqlParameter[] cmdParms)
		{
			object single = GetSingle(strSql, cmdParms);
			if (object.Equals(single, null) || object.Equals(single, DBNull.Value) || int.Parse(single.ToString()) == 0)
			{
				return false;
			}
			return true;
		}

		public static int ExecuteSql(string SQLString)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection))
				{
					try
					{
						mySqlConnection.Open();
						return mySqlCommand.ExecuteNonQuery();
					}
					catch (MySqlException ex)
					{
						mySqlConnection.Close();
						throw ex;
					}
				}
			}
		}

		public static int ExecuteSqlByTime(string SQLString, int Times)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection))
				{
					try
					{
						mySqlConnection.Open();
						mySqlCommand.CommandTimeout = Times;
						return mySqlCommand.ExecuteNonQuery();
					}
					catch (MySqlException ex)
					{
						mySqlConnection.Close();
						throw ex;
					}
				}
			}
		}

		public static int ExecuteSqlTran(List<string> SQLStringList)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				mySqlConnection.Open();
				MySqlCommand mySqlCommand = new MySqlCommand();
				mySqlCommand.Connection = mySqlConnection;
				MySqlTransaction mySqlTransaction2 = mySqlCommand.Transaction = mySqlConnection.BeginTransaction();
				try
				{
					int num = 0;
					for (int i = 0; i < SQLStringList.Count; i++)
					{
						string text = SQLStringList[i];
						if (text.Trim().Length > 1)
						{
							mySqlCommand.CommandText = text;
							num += mySqlCommand.ExecuteNonQuery();
						}
					}
					mySqlTransaction2.Commit();
					return num;
				}
				catch
				{
					mySqlTransaction2.Rollback();
					return 0;
				}
			}
		}

		public static int ExecuteSql(string SQLString, string content)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection);
				MySqlParameter mySqlParameter = new MySqlParameter("@content", SqlDbType.NText);
				mySqlParameter.Value = content;
				mySqlCommand.Parameters.Add(mySqlParameter);
				try
				{
					mySqlConnection.Open();
					return mySqlCommand.ExecuteNonQuery();
				}
				catch (MySqlException ex)
				{
					throw ex;
				}
				finally
				{
					mySqlCommand.Dispose();
					mySqlConnection.Close();
				}
			}
		}

		public static object ExecuteSqlGet(string SQLString, string content)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection);
				MySqlParameter mySqlParameter = new MySqlParameter("@content", SqlDbType.NText);
				mySqlParameter.Value = content;
				mySqlCommand.Parameters.Add(mySqlParameter);
				try
				{
					mySqlConnection.Open();
					object obj = mySqlCommand.ExecuteScalar();
					if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
					{
						return null;
					}
					return obj;
				}
				catch (MySqlException ex)
				{
					throw ex;
				}
				finally
				{
					mySqlCommand.Dispose();
					mySqlConnection.Close();
				}
			}
		}

		public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				MySqlCommand mySqlCommand = new MySqlCommand(strSQL, mySqlConnection);
				MySqlParameter mySqlParameter = new MySqlParameter("@fs", SqlDbType.Image);
				mySqlParameter.Value = fs;
				mySqlCommand.Parameters.Add(mySqlParameter);
				try
				{
					mySqlConnection.Open();
					return mySqlCommand.ExecuteNonQuery();
				}
				catch (MySqlException ex)
				{
					throw ex;
				}
				finally
				{
					mySqlCommand.Dispose();
					mySqlConnection.Close();
				}
			}
		}

		public static object GetSingle(string SQLString)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection))
				{
					try
					{
						mySqlConnection.Open();
						object obj = mySqlCommand.ExecuteScalar();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							return null;
						}
						return obj;
					}
					catch (MySqlException ex)
					{
						mySqlConnection.Close();
						throw ex;
					}
				}
			}
		}

		public static object GetSingle(string SQLString, int Times)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection))
				{
					try
					{
						mySqlConnection.Open();
						mySqlCommand.CommandTimeout = Times;
						object obj = mySqlCommand.ExecuteScalar();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							return null;
						}
						return obj;
					}
					catch (MySqlException ex)
					{
						mySqlConnection.Close();
						throw ex;
					}
				}
			}
		}

		public static MySqlDataReader ExecuteReader(string strSQL)
		{
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			MySqlCommand mySqlCommand = new MySqlCommand(strSQL, mySqlConnection);
			try
			{
				mySqlConnection.Open();
				return mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
			}
			catch (MySqlException ex)
			{
				throw ex;
			}
		}

		public static DataSet Query(string SQLString)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				DataSet dataSet = new DataSet();
				try
				{
					mySqlConnection.Open();
					MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(SQLString, mySqlConnection);
					mySqlDataAdapter.Fill(dataSet, "ds");
				}
				catch (MySqlException ex)
				{
					throw new Exception(ex.Message);
				}
				return dataSet;
			}
		}

		public static DataSet Query(string SQLString, int Times)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				DataSet dataSet = new DataSet();
				try
				{
					mySqlConnection.Open();
					MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(SQLString, mySqlConnection);
					mySqlDataAdapter.SelectCommand.CommandTimeout = Times;
					mySqlDataAdapter.Fill(dataSet, "ds");
				}
				catch (MySqlException ex)
				{
					throw new Exception(ex.Message);
				}
				return dataSet;
			}
		}

		public static int ExecuteSql(string SQLString, params MySqlParameter[] cmdParms)
		{
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand())
				{
					try
					{
						PrepareCommand(mySqlCommand, conn, null, SQLString, cmdParms);
						int result = mySqlCommand.ExecuteNonQuery();
						mySqlCommand.Parameters.Clear();
						return result;
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
				}
			}
		}

		public static void ExecuteSqlTran(Hashtable SQLStringList)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				mySqlConnection.Open();
				using (MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction())
				{
					MySqlCommand mySqlCommand = new MySqlCommand();
					try
					{
						IDictionaryEnumerator enumerator = SQLStringList.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
								string cmdText = dictionaryEntry.Key.ToString();
								MySqlParameter[] cmdParms = (MySqlParameter[])dictionaryEntry.Value;
								PrepareCommand(mySqlCommand, mySqlConnection, mySqlTransaction, cmdText, cmdParms);
								int num = mySqlCommand.ExecuteNonQuery();
								mySqlCommand.Parameters.Clear();
							}
						}
						finally
						{
							IDisposable disposable = enumerator as IDisposable;
							disposable?.Dispose();
						}
						mySqlTransaction.Commit();
					}
					catch
					{
						mySqlTransaction.Rollback();
						throw;
					}
				}
			}
		}

		public static int ExecuteSqlTran(List<CommandInfo> cmdList)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				mySqlConnection.Open();
				using (MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction())
				{
					MySqlCommand mySqlCommand = new MySqlCommand();
					try
					{
						int num = 0;
						foreach (CommandInfo cmd in cmdList)
						{
							string commandText = cmd.CommandText;
							MySqlParameter[] cmdParms = (MySqlParameter[])cmd.Parameters;
							PrepareCommand(mySqlCommand, mySqlConnection, mySqlTransaction, commandText, cmdParms);
							if (cmd.EffentNextType == EffentNextType.WhenHaveContine || cmd.EffentNextType == EffentNextType.WhenNoHaveContine)
							{
								if (cmd.CommandText.ToLower().IndexOf("count(") == -1)
								{
									mySqlTransaction.Rollback();
									return 0;
								}
								object obj = mySqlCommand.ExecuteScalar();
								bool flag = false;
								if (obj == null && obj == DBNull.Value)
								{
									flag = false;
								}
								flag = (Convert.ToInt32(obj) > 0);
								if (cmd.EffentNextType == EffentNextType.WhenHaveContine && !flag)
								{
									mySqlTransaction.Rollback();
									return 0;
								}
								if (cmd.EffentNextType == EffentNextType.WhenNoHaveContine & flag)
								{
									mySqlTransaction.Rollback();
									return 0;
								}
							}
							else
							{
								int num2 = mySqlCommand.ExecuteNonQuery();
								num += num2;
								if (cmd.EffentNextType == EffentNextType.ExcuteEffectRows && num2 == 0)
								{
									mySqlTransaction.Rollback();
									return 0;
								}
								mySqlCommand.Parameters.Clear();
							}
						}
						mySqlTransaction.Commit();
						return num;
					}
					catch
					{
						mySqlTransaction.Rollback();
						throw;
					}
				}
			}
		}

		public static void ExecuteSqlTranWithIndentity(List<CommandInfo> SQLStringList)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				mySqlConnection.Open();
				using (MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction())
				{
					MySqlCommand mySqlCommand = new MySqlCommand();
					try
					{
						int num = 0;
						foreach (CommandInfo SQLString in SQLStringList)
						{
							string commandText = SQLString.CommandText;
							MySqlParameter[] array = (MySqlParameter[])SQLString.Parameters;
							MySqlParameter[] array2 = array;
							foreach (MySqlParameter mySqlParameter in array2)
							{
								if (mySqlParameter.Direction == ParameterDirection.InputOutput)
								{
									mySqlParameter.Value = num;
								}
							}
							PrepareCommand(mySqlCommand, mySqlConnection, mySqlTransaction, commandText, array);
							int num2 = mySqlCommand.ExecuteNonQuery();
							MySqlParameter[] array3 = array;
							foreach (MySqlParameter mySqlParameter2 in array3)
							{
								if (mySqlParameter2.Direction == ParameterDirection.Output)
								{
									num = Convert.ToInt32(mySqlParameter2.Value);
								}
							}
							mySqlCommand.Parameters.Clear();
						}
						mySqlTransaction.Commit();
					}
					catch
					{
						mySqlTransaction.Rollback();
						throw;
					}
				}
			}
		}

		public static void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				mySqlConnection.Open();
				using (MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction())
				{
					MySqlCommand mySqlCommand = new MySqlCommand();
					try
					{
						int num = 0;
						IDictionaryEnumerator enumerator = SQLStringList.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
								string cmdText = dictionaryEntry.Key.ToString();
								MySqlParameter[] array = (MySqlParameter[])dictionaryEntry.Value;
								MySqlParameter[] array2 = array;
								foreach (MySqlParameter mySqlParameter in array2)
								{
									if (mySqlParameter.Direction == ParameterDirection.InputOutput)
									{
										mySqlParameter.Value = num;
									}
								}
								PrepareCommand(mySqlCommand, mySqlConnection, mySqlTransaction, cmdText, array);
								int num2 = mySqlCommand.ExecuteNonQuery();
								MySqlParameter[] array3 = array;
								foreach (MySqlParameter mySqlParameter2 in array3)
								{
									if (mySqlParameter2.Direction == ParameterDirection.Output)
									{
										num = Convert.ToInt32(mySqlParameter2.Value);
									}
								}
								mySqlCommand.Parameters.Clear();
							}
						}
						finally
						{
							IDisposable disposable = enumerator as IDisposable;
							disposable?.Dispose();
						}
						mySqlTransaction.Commit();
					}
					catch
					{
						mySqlTransaction.Rollback();
						throw;
					}
				}
			}
		}

		public static object GetSingle(string SQLString, params MySqlParameter[] cmdParms)
		{
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand())
				{
					try
					{
						PrepareCommand(mySqlCommand, conn, null, SQLString, cmdParms);
						object obj = mySqlCommand.ExecuteScalar();
						mySqlCommand.Parameters.Clear();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							return null;
						}
						return obj;
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
				}
			}
		}

		public static MySqlDataReader ExecuteReader(string SQLString, params MySqlParameter[] cmdParms)
		{
			MySqlConnection conn = new MySqlConnection(connectionString);
			MySqlCommand mySqlCommand = new MySqlCommand();
			try
			{
				PrepareCommand(mySqlCommand, conn, null, SQLString, cmdParms);
				MySqlDataReader result = mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
				mySqlCommand.Parameters.Clear();
				return result;
			}
			catch (MySqlException ex)
			{
				throw ex;
			}
		}

		public static DataSet Query(string SQLString, params MySqlParameter[] cmdParms)
		{
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MySqlCommand mySqlCommand = new MySqlCommand();
				PrepareCommand(mySqlCommand, conn, null, SQLString, cmdParms);
				using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand))
				{
					DataSet dataSet = new DataSet();
					try
					{
						mySqlDataAdapter.Fill(dataSet, "ds");
						mySqlCommand.Parameters.Clear();
					}
					catch (MySqlException ex)
					{
						throw new Exception(ex.Message);
					}
					return dataSet;
				}
			}
		}

		private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
		{
			if (conn.State != ConnectionState.Open)
			{
				conn.Open();
			}
			cmd.Connection = conn;
			cmd.CommandText = cmdText;
			if (trans != null)
			{
				cmd.Transaction = trans;
			}
			cmd.CommandType = CommandType.Text;
			if (cmdParms != null)
			{
				foreach (MySqlParameter mySqlParameter in cmdParms)
				{
					if ((mySqlParameter.Direction == ParameterDirection.InputOutput || mySqlParameter.Direction == ParameterDirection.Input) && mySqlParameter.Value == null)
					{
						mySqlParameter.Value = DBNull.Value;
					}
					cmd.Parameters.Add(mySqlParameter);
				}
			}
		}
	}
}
