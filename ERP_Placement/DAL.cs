using ERP_Placement.Models;
using Microsoft.AspNetCore.Mvc;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Data;



namespace ERP_Placement.DAL
{
    public class StudentDAL
    {
        private readonly string _conn;

        public StudentDAL(IConfiguration config)
        {
            _conn = config.GetConnectionString("DefaultConnection");
        }

        //// =======================
        //// INSERT STUDENT DETAILS
        //// =======================
        //public int InsertStudentDetails(StudentRegistration s)
        //{
        //    using (SqlConnection con = new SqlConnection(_conn))
        //    {
        //        SqlCommand cmd = new SqlCommand("SPStudentDetails", con);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //      //  cmd.Parameters.AddWithValue("@StudentId", DBNull.Value);
        //        cmd.Parameters.AddWithValue("@StduentCode", s.StudentId ?? "STD001");
        //        cmd.Parameters.AddWithValue("@FirstName", s.FirstName);
        //        cmd.Parameters.AddWithValue("@MiddleName", s.MiddleName ?? "");
        //        cmd.Parameters.AddWithValue("@LastName", s.LastName);
        //        cmd.Parameters.AddWithValue("@Gender", s.Gender);
        //        cmd.Parameters.AddWithValue("@DOB", s.DOB);
        //        cmd.Parameters.AddWithValue("@Email", s.Email);
        //        cmd.Parameters.AddWithValue("@MobileNo", s.MobileNo);
        //        cmd.Parameters.AddWithValue("@AlternateMobile", s.AlternateMobile ?? "");
        //        cmd.Parameters.AddWithValue("@RegistrationDate", s.RegistrationDate);
        //        cmd.Parameters.AddWithValue("@Personal_Photo", s.Personal_Photo ?? "");
        //        cmd.Parameters.AddWithValue("@Acadamic_Year", s.Acadamic_Year);
        //        cmd.Parameters.AddWithValue("@Branch", s.Branch);
        //        cmd.Parameters.AddWithValue("@Roll_No", s.Roll_No);
        //        cmd.Parameters.AddWithValue("@Divison", s.Divison);

        //        cmd.Parameters.AddWithValue("@Flag", "Insert");

        //        con.Open();
        //        cmd.ExecuteNonQuery();
        //        con.Close();

        //        // Retrieve last inserted ID
        //        SqlCommand cmd2 = new SqlCommand("SELECT MAX(StudentId) FROM StudentDetails", con);
        //        con.Open();
        //        int studentId = Convert.ToInt32(cmd2.ExecuteScalar());
        //        con.Close();

        //        return studentId;
        //    }
        //}

        //// =======================
        //// INSERT STUDENT EDUCATION
        //// =======================
        //public void InsertStudentEducation(StudentRegistration s)
        //{
        //    using (SqlConnection con = new SqlConnection(_conn))
        //    {
        //        SqlCommand cmd = new SqlCommand("SPStudentEducation", con);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //      //  cmd.Parameters.AddWithValue("@StudentId", s.StudentId);
        //        cmd.Parameters.AddWithValue("@SSC", s.SSC ?? "");
        //        cmd.Parameters.AddWithValue("@SSC_Passing_Year", s.SSC_Passing_Year ?? "");
        //        cmd.Parameters.AddWithValue("@SSC_Document_Path", s.SSC_Document_Path ?? "");

        //        cmd.Parameters.AddWithValue("@HSC", s.HSC ?? "");
        //        cmd.Parameters.AddWithValue("@HSC_Passing_Year", s.HSC_Passing_Year ?? "");
        //        cmd.Parameters.AddWithValue("@HSC_Document_Path", s.HSC_Document_Path ?? "");

        //        cmd.Parameters.AddWithValue("@Diploma_Branch", s.Diploma_Branch ?? "");
        //        cmd.Parameters.AddWithValue("@Diploma_Percentages", s.Diploma_Percentages ?? "");
        //        cmd.Parameters.AddWithValue("@Diploma_Passing_Year", s.Diploma_Passing_Year ?? "");
        //        cmd.Parameters.AddWithValue("@Diploma_Document_Path", s.Diploma_Document_Path ?? "");

        //        cmd.Parameters.AddWithValue("@BTech_Branch", s.BTech_Branch ?? "");
        //        cmd.Parameters.AddWithValue("@BTech_Passing_Year", s.BTech_Passing_Year ?? "");
        //        cmd.Parameters.AddWithValue("@BTech_Percentages", s.BTech_Percentages ?? "");
        //        cmd.Parameters.AddWithValue("@BTech_Document_Path", s.BTech_Document_Path ?? "");

        //        cmd.Parameters.AddWithValue("@Skills", s.Skills ?? "");

        //        cmd.Parameters.AddWithValue("@Flag", "Insert");

        //        con.Open();
        //        cmd.ExecuteNonQuery();
        //        con.Close();
        //    }
        //}




        public int InsertStudentDetails(StudentRegistration s)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                SqlCommand cmd = new SqlCommand("SPStudentDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FirstName", s.FirstName);
                cmd.Parameters.AddWithValue("@MiddleName", s.MiddleName ?? "");
                cmd.Parameters.AddWithValue("@LastName", s.LastName);
                cmd.Parameters.AddWithValue("@Gender", s.Gender);
                cmd.Parameters.AddWithValue("@DOB", s.DOB);
                cmd.Parameters.AddWithValue("@Email", s.Email);
                cmd.Parameters.AddWithValue("@MobileNo", s.MobileNo);
                cmd.Parameters.AddWithValue("@AlternateMobile", s.AlternateMobile ?? "");
                cmd.Parameters.AddWithValue("@RegistrationDate", s.RegistrationDate);
                cmd.Parameters.AddWithValue("@Personal_Photo", s.Personal_Photo ?? "");
                cmd.Parameters.AddWithValue("@Acadamic_Year", s.Acadamic_Year);
                cmd.Parameters.AddWithValue("@Branch", s.Branch);
                cmd.Parameters.AddWithValue("@Roll_No", s.Roll_No);
                cmd.Parameters.AddWithValue("@Divison", s.Divison);
                cmd.Parameters.AddWithValue("@Approval_Status", "Not_Approved");

                // Education
                cmd.Parameters.AddWithValue("@SSC", s.SSC);
                cmd.Parameters.AddWithValue("@SSC_Passing_Year", s.SSC_Passing_Year);
                cmd.Parameters.AddWithValue("@SSC_Document_Path", s.SSC_Document_Path ?? "");

                cmd.Parameters.AddWithValue("@HSC", s.HSC);
                cmd.Parameters.AddWithValue("@HSC_Passing_Year", s.HSC_Passing_Year);
                cmd.Parameters.AddWithValue("@HSC_Document_Path", s.HSC_Document_Path ?? "");

                cmd.Parameters.AddWithValue("@Diploma_Branch", s.Diploma_Branch ?? "");
                cmd.Parameters.AddWithValue("@Diploma_Percentages", s.Diploma_Percentages);
                cmd.Parameters.AddWithValue("@Diploma_Passing_Year", s.Diploma_Passing_Year);
                cmd.Parameters.AddWithValue("@Diploma_Document_Path", s.Diploma_Document_Path ?? "");

                cmd.Parameters.AddWithValue("@BTech_Branch", s.BTech_Branch ?? "");
                cmd.Parameters.AddWithValue("@BTech_Passing_Year", s.BTech_Passing_Year);
                cmd.Parameters.AddWithValue("@BTech_Percentages", s.BTech_Percentages);
                cmd.Parameters.AddWithValue("@BTech_Document_Path", s.BTech_Document_Path ?? "");

                cmd.Parameters.AddWithValue("@Skills", s.Skills ?? "");
                cmd.Parameters.AddWithValue("@Flag", "Insert");

                // OUTPUT PARAM
                SqlParameter outputId = new SqlParameter("@StudentId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputId);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return Convert.ToInt32(outputId.Value);
            }
        }

        public DataTable GetPendingStudents()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                SqlCommand cmd = new SqlCommand("SPPlacementCoordinator", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Flag", "AllStudent");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }
        public DataTable GetAllStudents()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("SPPlacementCoordinator", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Flag", "GetAllStud");

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }



        public DataTable GetStudById(string id)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("SPPlacementCoordinator", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Flag", "GetbyStudId");
                    cmd.Parameters.AddWithValue("@StudentId", id);   
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }



        public void ApproveStudent(string id, string generatedPassword)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                SqlCommand cmd = new SqlCommand("SPPlacementCoordinator", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@StudentId", id);
                cmd.Parameters.AddWithValue("@Password", generatedPassword);
                cmd.Parameters.AddWithValue("@Approval_Status", "Approve");
                cmd.Parameters.AddWithValue("@Flag", "Approve");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }


        public DataTable GetApprovedStudent()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                SqlCommand cmd = new SqlCommand("SPPlacementCoordinator", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Flag", "NotApproved_Stud");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        public DataTable StudentLogin(string username, string password)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            using (SqlCommand cmd = new SqlCommand("SPStudentDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Flag", "StudentLogin");
                cmd.Parameters.AddWithValue("@Email", username);
                cmd.Parameters.AddWithValue("@Password", password);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }


    }
}



