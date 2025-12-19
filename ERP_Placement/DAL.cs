using ERP_Placement.Models;
using System.Data;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;



namespace ERP_Placement.DAL
{
    public class StudentDAL
    {
        private readonly string _conn;

        public StudentDAL(IConfiguration config)
        {
            _conn = config.GetConnectionString("DefaultConnection");
        }

        // =======================
        // INSERT STUDENT DETAILS
        // =======================
        public int InsertStudentDetails(StudentRegistration s)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                SqlCommand cmd = new SqlCommand("SPStudentDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@StudentId", DBNull.Value);
                cmd.Parameters.AddWithValue("@StduentCode", s.StudentId ?? "STD001");
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

                cmd.Parameters.AddWithValue("@Flag", "Insert");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                // Retrieve last inserted ID
                SqlCommand cmd2 = new SqlCommand("SELECT MAX(StudentId) FROM StudentDetails", con);
                con.Open();
                int studentId = Convert.ToInt32(cmd2.ExecuteScalar());
                con.Close();

                return studentId;
            }
        }

        // =======================
        // INSERT STUDENT EDUCATION
        // =======================
        public void InsertStudentEducation(StudentRegistration s)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                SqlCommand cmd = new SqlCommand("SPStudentEducation", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@StudentId", s.StudentId);
                cmd.Parameters.AddWithValue("@SSC", s.SSC ?? "");
                cmd.Parameters.AddWithValue("@SSC_Passing_Year", s.SSC_Passing_Year ?? "");
                cmd.Parameters.AddWithValue("@SSC_Document_Path", s.SSC_Document_Path ?? "");

                cmd.Parameters.AddWithValue("@HSC", s.HSC ?? "");
                cmd.Parameters.AddWithValue("@HSC_Passing_Year", s.HSC_Passing_Year ?? "");
                cmd.Parameters.AddWithValue("@HSC_Document_Path", s.HSC_Document_Path ?? "");

                cmd.Parameters.AddWithValue("@Diploma_Branch", s.Diploma_Branch ?? "");
                cmd.Parameters.AddWithValue("@Diploma_Percentages", s.Diploma_Percentages ?? "");
                cmd.Parameters.AddWithValue("@Diploma_Passing_Year", s.Diploma_Passing_Year ?? "");
                cmd.Parameters.AddWithValue("@Diploma_Document_Path", s.Diploma_Document_Path ?? "");

                cmd.Parameters.AddWithValue("@BTech_Branch", s.BTech_Branch ?? "");
                cmd.Parameters.AddWithValue("@BTech_Passing_Year", s.BTech_Passing_Year ?? "");
                cmd.Parameters.AddWithValue("@BTech_Percentages", s.BTech_Percentages ?? "");
                cmd.Parameters.AddWithValue("@BTech_Document_Path", s.BTech_Document_Path ?? "");

                cmd.Parameters.AddWithValue("@Skills", s.Skills ?? "");

                cmd.Parameters.AddWithValue("@Flag", "Insert");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
}
