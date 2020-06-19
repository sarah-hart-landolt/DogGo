using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class WalkRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalkRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walk> GetAllWalks()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT 
                            w.Id, 
                            w.Date, 
                            w.WalkerId, 
                            w.DogId,
                            o.Name
                        FROM Walk w
                        JOIN Walker ker ON ker.Id = w.WalkerId
                        JOIN Dog d ON d.Id= w.DogId
                        JOIN Owner o ON o.id = dog.OwnerId
                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walk> Walks = new List<Walk>();
                    while (reader.Read())
                    {
                        Walk walk = new Walk
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId"))
                        };

                        Walks.Add(walk);
                    }

                    reader.Close();
                    return Walks;
                }


            }
        }


        public List<Walk> GetWalksByWalkerId(int WalkerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                 SELECT 
                            w.Id, 
                            w.Date, 
                            w.WalkerId,
                            w.Duration,
                            w.DogId,
                            o.[Name]
                        FROM Walks w
                        JOIN Walker ker ON ker.Id = w.WalkerId
                        JOIN Dog d ON d.Id= w.DogId
                        JOIN Owner o ON o.id = d.OwnerId
                WHERE WalkerId = @WalkerId
            ";

                    cmd.Parameters.AddWithValue("@WalkerId", WalkerId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walk> Walks = new List<Walk>();
                    while (reader.Read())
                    {
                        Walk walk = new Walk
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            Duration= (reader.GetInt32(reader.GetOrdinal("Duration")))/60,
                            Owner = new Owner
                            {
                                Name= reader.GetString(reader.GetOrdinal("Name")),
                            }
                        };

                        Walks.Add(walk);
                    }

                    reader.Close();
                    return Walks;
                }
            }
        }

        //        public Walk GetWalkById(int id)
        //        {
        //            using (SqlConnection conn = Connection)
        //            {
        //                conn.Open();
        //                using (SqlCommand cmd = conn.CreateCommand())
        //                {
        //                    cmd.CommandText = @"
        //                        SELECT 
        //                            o.Id, 
        //                            o.[Name], 
        //                            o.Email, 
        //                            o.Address, 
        //                            o.NeighborhoodId, 
        //                            o.Phone,
        //                            n.[Name] as NeighborhoodName
        //                        FROM Walk o
        //                        INNER JOIN Neighborhood n ON n.Id = o.NeighborhoodId
        //                        WHERE o.Id = @id;
        //                    ";

        //                    cmd.Parameters.AddWithValue("@id", id);

        //                    SqlDataReader reader = cmd.ExecuteReader();

        //                    if (reader.Read())
        //                    {
        //                        Walk Walk = new Walk
        //                        {
        //                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                            Name = reader.GetString(reader.GetOrdinal("Name")),
        //                            Email = reader.GetString(reader.GetOrdinal("Email")),
        //                            Address = reader.GetString(reader.GetOrdinal("Address")),
        //                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
        //                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
        //                            Neighborhood = new Neighborhood()
        //                            {
        //                                Id = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
        //                                Name = reader.GetString(reader.GetOrdinal("NeighborhoodName"))
        //                            }
        //                        };
        //                        reader.Close();
        //                        return Walk;
        //                    }

        //                    reader.Close();
        //                    return null;
        //                }
        //            }
        //        }

        //        public Walk GetWalkByEmail(string email)
        //        {
        //            using (SqlConnection conn = Connection)
        //            {
        //                conn.Open();

        //                using (SqlCommand cmd = conn.CreateCommand())
        //                {
        //                    cmd.CommandText = @"
        //                        SELECT Id, [Name], Email, Address, Phone, NeighborhoodId
        //                        FROM Walk
        //                        WHERE Email = @email";

        //                    cmd.Parameters.AddWithValue("@email", email);

        //                    SqlDataReader reader = cmd.ExecuteReader();

        //                    if (reader.Read())
        //                    {
        //                        Walk Walk = new Walk()
        //                        {
        //                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                            Name = reader.GetString(reader.GetOrdinal("Name")),
        //                            Email = reader.GetString(reader.GetOrdinal("Email")),
        //                            Address = reader.GetString(reader.GetOrdinal("Address")),
        //                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
        //                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
        //                        };

        //                        reader.Close();
        //                        return Walk;
        //                    }

        //                    reader.Close();
        //                    return null;
        //                }
        //            }
        //        }

        //        public void AddWalk(Walk Walk)
        //        {
        //            using (SqlConnection conn = Connection)
        //            {
        //                conn.Open();
        //                using (SqlCommand cmd = conn.CreateCommand())
        //                {
        //                    cmd.CommandText = @"
        //                    INSERT INTO Walk ([Name], Email, Phone, Address, NeighborhoodId)
        //                    OUTPUT INSERTED.ID
        //                    VA@model DogGo.Models.ViewModels.WalkFormViewModel
        //LUES (@name, @email, @phoneNumber, @address, @neighborhoodId);
        //                ";

        //                    cmd.Parameters.AddWithValue("@name", Walk.Name);
        //                    cmd.Parameters.AddWithValue("@email", Walk.Email);
        //                    cmd.Parameters.AddWithValue("@phoneNumber", Walk.Phone);

        //                    // Lets pretend Address is nullable
        //                    if (Walk.Address == null)
        //                    {
        //                        cmd.Parameters.AddWithValue("@address", DBNull.Value);
        //                    }
        //                    else
        //                    {
        //                        cmd.Parameters.AddWithValue("@address", Walk.Address);
        //                    }

        //                    cmd.Parameters.AddWithValue("@neighborhoodId", Walk.NeighborhoodId);

        //                    int id = (int)cmd.ExecuteScalar();

        //                    Walk.Id = id;
        //                }
        //            }
        //        }

        //        public void UpdateWalk(Walk Walk)
        //        {
        //            using (SqlConnection conn = Connection)
        //            {
        //                conn.Open();

        //                using (SqlCommand cmd = conn.CreateCommand())
        //                {
        //                    cmd.CommandText = @"
        //                            UPDATE Walk
        //                            SET 
        //                                [Name] = @name, 
        //                                Email = @email, 
        //                                Address = @address, 
        //                                Phone = @phone, 
        //                                NeighborhoodId = @neighborhoodId
        //                            WHERE Id = @id";

        //                    cmd.Parameters.AddWithValue("@name", Walk.Name);
        //                    cmd.Parameters.AddWithValue("@email", Walk.Email);
        //                    cmd.Parameters.AddWithValue("@address", Walk.Address);
        //                    cmd.Parameters.AddWithValue("@phone", Walk.Phone);
        //                    cmd.Parameters.AddWithValue("@neighborhoodId", Walk.NeighborhoodId);
        //                    cmd.Parameters.AddWithValue("@id", Walk.Id);

        //                    cmd.ExecuteNonQuery();
        //                }
        //            }
        //        }

        //        public void DeleteWalk(int WalkId)
        //        {
        //            using (SqlConnection conn = Connection)
        //            {
        //                conn.Open();

        //                using (SqlCommand cmd = conn.CreateCommand())
        //                {
        //                    cmd.CommandText = @"
        //                            DELETE FROM Walk
        //                            WHERE Id = @id
        //                        ";

        //                    cmd.Parameters.AddWithValue("@id", WalkId);

        //                    cmd.ExecuteNonQuery();
        //                }
        //            }
        //        }
        //}
    }

}
