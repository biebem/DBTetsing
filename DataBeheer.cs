﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Testing_DB;

namespace DBTets
{
    class DataBeheer
    {
        private string connectionString;
        public DataBeheer(string connectionString)
        {
            this.connectionString = connectionString;
        }
        private SqlConnection getConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            return connection;
        }
        public void VoegCursusToe(Cursus c)
        {
            SqlConnection connection = getConnection();
            string query = "INSERT INTO dbo.cursusSQL (cursusnaam) VALUES(@cursusnaam)"; // werkt altijd met parameters , niet veilig wegens sql injection 
            using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                try
                {

                    command.Parameters.Add(new SqlParameter("@cursusnaam", SqlDbType.NVarChar));
                    command.CommandText = query;
                    command.Parameters["@cursusnaam"].Value = c.cursusnaam;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }

            }
        }
        public Cursus GeefCursus(int id)
        {
            SqlConnection connection = getConnection();
            string query = "SELECT * FROM dbo.cursusSQL WHERE id=@id";
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                SqlParameter paramID = new SqlParameter();
                paramID.ParameterName = "@Id";
                paramID.DbType = System.Data.DbType.Int32;
                paramID.Value = id;
                command.Parameters.Add(paramID);
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    Cursus cursus = new Cursus((int)reader["id"], (string)reader["cursusnaam"]);
                    reader.Close();
                    return cursus;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    connection.Close();
                }

            }
        }
        public IEnumerable<Cursus> GeefCursussen()
        {
            SqlConnection connection = getConnection();
            IList<Cursus> lg = new List<Cursus>();
            string query = "SELECT * FROM dbo.cursusSQL";
            using (SqlCommand command = connection.CreateCommand()) 
            {
                command.CommandText = query;
                connection.Open();
                try
                {
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        int id = (int)dataReader["id"]; // geeft object terug die je dan typecast naar int kan niet via int.parse
                        string cursusnaam = dataReader.GetString(1);
                        lg.Add(new Cursus(id, cursusnaam));
                    }
                    dataReader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }
            return lg;
        }
        public Student GeefStudent(int id) 
        {
            SqlConnection connection = getConnection();
            string queryS = "SELECT¨* FROM dbo.studentSQL WHERE id=@id";
            string querySc = "SELECT * FROM [adresBeheer].[dbo].[cursusSQL] t1,[adresBeheer].[dbo].[student_cursusSQL] t2 " +
                "where t1.Id = t2.cursusid and t2.studentid = @id";
            using (SqlCommand command = connection.CreateCommand()) 
            {
                command.CommandText = queryS;
                SqlParameter paramId = new SqlParameter();
                paramId.ParameterName = "@Id";
                paramId.DbType = DbType.Int32;
                paramId.Value = id;
                command.Parameters.Add(paramId);
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int studentId = (int)reader["Id"];
                    string studentnaam = (string)reader["naam"];
                    int klasId = (int)reader["klasId"];
                    reader.Close();
                    Klas klas = GeefKlas(klasId);
                    Student student = new Student(studentId, studentnaam, klas);
                    command.CommandText = querySc;
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Cursus cursus = new Cursus(reader.GetInt32(0), reader.GetString(1));
                        student.VoegCursusToe(cursus);
                    }
                    reader.Close();
                    return student;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public Klas GeefKlas(int klasId)
        {
                SqlConnection connection = getConnection();
                string query = "SELECT * FROM dbo.klasSQL WHERE id=@id";
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    SqlParameter paramID = new SqlParameter();
                    paramID.ParameterName = "@Id";
                    paramID.DbType = System.Data.DbType.Int32;
                    paramID.Value = klasId;
                    command.Parameters.Add(paramID);
                    connection.Open();
                    try
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        Klas klas = new Klas((int)reader["id"], (string)reader["klasnaam"]);
                        reader.Close();
                        return klas;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
        }
        public void VoegKlasToe(Klas k) 
        {
            SqlConnection connection = getConnection();
            string query = "INSERT INTO dbo.klasSQL (klasnaam) VALUES(@klasnaam)"; // werkt altijd met parameters , niet veilig wegens sql injection 
            using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                try
                {

                    command.Parameters.Add(new SqlParameter("@klasnaam", SqlDbType.NVarChar));
                    command.CommandText = query;
                    command.Parameters["@klasnaam"].Value = k.klasnaam;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }

            }
        }

        public void UpdateCursus(Cursus c) 
        {
            SqlConnection connection = getConnection();
            Cursus cursusDB = GeefCursus(c.id);
            string query = "SELECT * FROM dbo.cursusSQL WHERE Id=@id";
            using (SqlDataAdapter adapter = new SqlDataAdapter()) 
            {
                try
                {
                    SqlParameter paramId = new SqlParameter();
                    paramId.ParameterName = "@Id";
                    paramId.DbType = DbType.Int32;
                    paramId.Value = c.id;
                    SqlCommandBuilder builder = new SqlCommandBuilder();
                    builder.DataAdapter = adapter;
                    adapter.SelectCommand = new SqlCommand();
                    adapter.SelectCommand.CommandText = query;
                    adapter.SelectCommand.Connection = connection;
                    adapter.SelectCommand.Parameters.Add(paramId);
                    adapter.UpdateCommand = builder.GetUpdateCommand();
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    table.Rows[0]["cursusnaam"] = c.cursusnaam;
                    adapter.Update(table);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally 
                {
                    connection.Close();
                }
            }
        }
        public void VerwijderCursussen(List<int> ids) 
        {
            string query = "SELECT * FROM dbo.cursuSQL";
            DataSet ds = new DataSet();
            SqlConnection connection = getConnection();
            using (SqlDataAdapter adapter = new SqlDataAdapter()) 
            {
                try
                {
                    SqlCommandBuilder builder = new SqlCommandBuilder();
                    builder.DataAdapter = adapter;
                    adapter.SelectCommand = new SqlCommand();
                    adapter.SelectCommand.CommandText = query;
                    adapter.SelectCommand.Connection = connection;
                    adapter.DeleteCommand = builder.GetDeleteCommand();
                    adapter.FillSchema(ds, SchemaType.Source, "cursus");
                    adapter.Fill(ds, "cursus");

                    foreach (int id in ids)
                    {
                        DataRow r = ds.Tables["cursus"].Rows.Find(id);
                        r.Delete();
                    }
                    adapter.Update(ds, "cursus");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally 
                {
                    connection.Close();
                }
            }
        }
        public void VoegStudentMetCursussenToe(Student s)
        {
            SqlConnection connection = getConnection();
            string queryS = "INSERT INTO dbo.studentSQL(naam,klasId) output INSERTED.ID VALUES(@naam,@klasId)";
            string querySC = "INSERT INTO dbo.student_cursusSQL(cursusId,studentId) VALUES(@cursusId,@studentId)";

            using (SqlCommand command1 = connection.CreateCommand())
            using (SqlCommand command2 = connection.CreateCommand())
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                command1.Transaction = transaction;
                command2.Transaction = transaction;
                try
                {
                    //student toevoegen
                    SqlParameter parNaam = new SqlParameter();
                    parNaam.ParameterName = "@naam";
                    parNaam.SqlDbType = SqlDbType.NVarChar;
                    command1.Parameters.Add(parNaam);
                    SqlParameter parKlas = new SqlParameter();
                    parKlas.ParameterName = "@klasId";
                    parKlas.DbType = DbType.Int32; //check
                    command1.Parameters.Add(parKlas);
                    command1.CommandText = queryS;
                    command1.Parameters["@naam"].Value = s.naam;
                    command1.Parameters["@klasId"].Value = s.klas.id;
                    //command1.ExecuteNonQuery();
                    int newID = (int)command1.ExecuteScalar();
                    //Cursussen toevoegen
                    command2.Parameters.Add(new SqlParameter("@cursusId", SqlDbType.Int));
                    command2.Parameters.Add(new SqlParameter("@studentId", SqlDbType.Int));

                    command2.CommandText = querySC;
                    command2.Parameters["@studentId"].Value = newID;

                    foreach (var cursus in s.cursussen)
                    {
                        command2.Parameters["@cursusId"].Value = cursus.id;
                        command2.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
