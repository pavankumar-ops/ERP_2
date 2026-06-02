using ERP_Placement.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.ComponentModel.Design;
using System.Data;
using static System.Net.Mime.MediaTypeNames;



namespace ERP_Placement.DAL
{
    public class StudentDAL
    {
        private readonly string _conn;

        public StudentDAL(IConfiguration config)
        {
            _conn = config.GetConnectionString("DefaultConnection");
        }





        public int InsertStudentDetails(StudentRegistration s)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                SqlCommand cmd = new SqlCommand("SPStudentDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Name_Title", s.Name_Title ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FirstName", s.FirstName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MiddleName", s.MiddleName ??(object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LastName", s.LastName ??(object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Gender", s.Gender == null ? DBNull.Value : s.Gender);
                cmd.Parameters.AddWithValue("@DOB", s.DOB);
                cmd.Parameters.AddWithValue("@Email", s.Email ??(object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MobileNo", s.MobileNo ??(object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AlternateMobile", s.AlternateMobile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RegistrationDate", s.RegistrationDate);
                cmd.Parameters.AddWithValue("@Personal_Photo", s.Personal_Photo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Acadamic_Year", s.Acadamic_Year ??(object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Branch", s.Branch ??(object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Roll_No", s.Roll_No ??(object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Divison", s.Divison ??(object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Password", s.Password == null ? DBNull.Value : s.Password);

                cmd.Parameters.AddWithValue("@Approval_Status", "Not_Approved");

                // Education
                cmd.Parameters.AddWithValue("@SSC", s.SSC);
                cmd.Parameters.AddWithValue("@SSC_Passing_Year", s.SSC_Passing_Year ??(object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSC_Document_Path", s.SSC_Document_Path ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@HSC", s.HSC ??(object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HSC_Passing_Year", s.HSC_Passing_Year ??(object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HSC_Document_Path", s.HSC_Document_Path ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@Diploma_Branch", s.Diploma_Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Diploma_Percentages", s.Diploma_Percentages ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Diploma_Passing_Year", s.Diploma_Passing_Year ??(object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Diploma_Document_Path", s.Diploma_Document_Path ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@BTech_Branch", s.BTech_Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BTech_Passing_Year", s.BTech_Passing_Year ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BTech_Percentages", s.BTech_Percentages ??(object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BTech_Document_Path", s.BTech_Document_Path ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@Skills", s.Skills ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Resume", s.ResumePath ?? (object)DBNull.Value);
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


        public DataTable GetAllTest()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("SPPlacementCoordinator", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Flag", "GetAllTest");

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

        public DataTable GetStudDetails(string id)
        {
            DataTable dt = new DataTable();




            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"SELECT * FROM StudentDetails SD
INNER JOIN StudentEducation SE
ON SD.StudentId = SE.StudentId
WHERE SD.StudentId = @StudentId
            ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;
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
            {
                string query = @"
            SELECT StudentId, FirstName, Email
            FROM StudentDetails
            WHERE Email = @Email
              AND Password = @Password
              AND Approval_Status = 'Approved'";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = username;
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 20).Value = password;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            return dt;
        }


        public int InsertCompany(Placement_Coordinator_Model model)
        {
            using SqlConnection con = new SqlConnection(_conn);

            string query = @"
        INSERT INTO CompanyMaster
        (CompanyName, CompanyLogo, CompanyWebsite, CompanyDescription, RegisteredBy,HRName,HREmailId,HRMobNo,CompanyDomain)
        VALUES
        (@CompanyName, @CompanyLogo, @CompanyWebsite, @CompanyDescription, @RegisteredBy,@HRName,@HREmailId,@HRMobNo,@CompanyDomain );

        SELECT SCOPE_IDENTITY();";

            using SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@CompanyName", model.CompanyName);
            cmd.Parameters.AddWithValue("@CompanyLogo", model.CompanyLogo ?? "");
            cmd.Parameters.AddWithValue("@CompanyWebsite", model.CompanyWebsite ?? "");
            cmd.Parameters.AddWithValue("@CompanyDescription", model.CompanyDescription ?? "");
            cmd.Parameters.AddWithValue("@RegisteredBy", model.RegisteredBy);
            cmd.Parameters.AddWithValue("@HRName", model.HRName);
            cmd.Parameters.AddWithValue("@HREmailId", model.HRMail);
            cmd.Parameters.AddWithValue("@HRMobNo", model.HRMobNo);
            cmd.Parameters.AddWithValue("@CompanyDomain", model.CompanyDomain);

            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }


        public DataTable GetAllCompany()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("SPPlacementCoordinator", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Flag", "GetAllCompany");

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }



        public DataTable GetVaccany(string id)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("SPPlacementCoordinator", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Flag", "GetVaccany");
                    cmd.Parameters.AddWithValue("@CompanyId", id);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        public int UpdateInterviewStatus(int vacancyId)
        {
            int rows = 0;

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"UPDATE InterviewSchedule 
                         SET Status = @Status 
                         WHERE VacancyId = @VacancyId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@VacancyId", vacancyId);
                    cmd.Parameters.AddWithValue("@Status", "Applied");

                    con.Open();
                    rows = cmd.ExecuteNonQuery();
                }
            }

            return rows;
        }

        public DataTable GetInterviewList()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                //                string query = @"SELECT i.InterviewId,
                //       j.JobTitle,
                //       s.FirstName,
                //       s.LastName,
                //       i.Branch,
                //       i.InterviewType,
                //       i.InterviewDate,
                //       i.InterviewTime,
                //       i.LastDateToApply,
                //i.Status

                //FROM InterviewSchedule i
                //JOIN JobVacancy j ON j.VacancyId = i.VacancyId
                //JOIN StudentDetails s ON s.StudentId = i.StudentId where s.Approval_Status='Approved'";
                string query = @"
						SELECT 
    i.InterviewId,
    j.JobTitle,
s.FirstName,
Personal_Photo,
      s.LastName,
	c.CompanyId,
    c.CompanyName,
    c.CompanyLogo,
    c.CompanyDescription,
c.HREmailId,
	j.JobDescription,
    j.JobLocation,
    j.Salary,
    j.JobType,
    i.InterviewType,
    i.InterviewDate,
    i.InterviewTime,
    i.LastDateToApply,
	 i.StudentId,
	 j.VacancyId ,
    i.Status

FROM InterviewSchedule i

INNER JOIN JobVacancy j 
    ON j.VacancyId = i.VacancyId

INNER JOIN CompanyMaster c 
    ON c.CompanyId = j.CompanyId
JOIN StudentDetails s ON s.StudentId = i.StudentId

where i.Status = 'Assign' or i.Status = 'Applied'";




                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }

            return dt;
        }

        public DataTable GetVacancyDropdown()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = "SELECT VacancyId, JobTitle FROM JobVacancy";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }

            return dt;
        }


        public DataTable GetStudentDropdown()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = "SELECT * FROM StudentDetails";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }

            return dt;
        }






        public int SaveInterview(InterviewScheduleModel model)
        {
            int result = 0;

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"INSERT INTO InterviewSchedule
                        (VacancyId,StudentId,Branch,InterviewType,InterviewDate,InterviewTime,LastDateToApply,Status)
                        VALUES
                        (@VacancyId,@StudentId,@Branch,@InterviewType,@InterviewDate,@InterviewTime,@LastDateToApply,@Status)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@VacancyId", model.VacancyId);
                cmd.Parameters.AddWithValue("@StudentId", model.StudentId);
                cmd.Parameters.AddWithValue("@Branch", model.Branch);
                cmd.Parameters.AddWithValue("@InterviewType", model.InterviewType);
                cmd.Parameters.AddWithValue("@InterviewDate", model.InterviewDate);
                cmd.Parameters.AddWithValue("@InterviewTime", model.InterviewTime);
                cmd.Parameters.AddWithValue("@LastDateToApply", model.LastDateToApply);
                cmd.Parameters.AddWithValue("@Status", "Assign");

                con.Open();
                result = cmd.ExecuteNonQuery();
            }

            return result;
        }


        public DataTable GetStudentInterviews(string studentId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {

                string query = @"

						SELECT 
    i.InterviewId,
    j.JobTitle,
	c.CompanyId,
    c.CompanyName,
    c.CompanyLogo,
    c.CompanyDescription,
c.HREmailId,
	j.JobDescription,
    j.JobLocation,
    j.Salary,
    j.JobType,
    i.InterviewType,
    i.InterviewDate,
    i.InterviewTime,
    i.LastDateToApply,
	 i.StudentId,
	 j.VacancyId ,
    i.Status

FROM InterviewSchedule i

INNER JOIN JobVacancy j 
    ON j.VacancyId = i.VacancyId

INNER JOIN CompanyMaster c 
    ON c.CompanyId = j.CompanyId

WHERE i.StudentId = @StudentId and i.Status = 'Selected' ";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@StudentId", studentId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dt);
            }

            return dt;
        }
        public int SaveVacancy(JobVacancyModel model)
        {
            int result = 0;

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"INSERT INTO JobVacancy
                        (
                            CompanyId,
                            JobTitle,
                            JobDescription,
                            NoOfOpenings,
                            JobLocation,
                            Salary,
                            JobType,
                            ExperienceRequired,
                            Qualification,
                            SkillsRequired,
                            LastDate,
                            CreatedOn,
                            IsActive
                        )
                        VALUES
                        (
                            @CompanyId,
                            @JobTitle,
                            @JobDescription,
                            @NoOfOpenings,
                            @JobLocation,
                            @Salary,
                            @JobType,
                            @ExperienceRequired,
                            @Qualification,
                            @SkillsRequired,
                            @LastDate,
                            GETDATE(),
                            @IsActive
                        )";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CompanyId", model.CompanyId);
                    cmd.Parameters.AddWithValue("@JobTitle", model.JobTitle ?? "");
                    cmd.Parameters.AddWithValue("@JobDescription", model.JobDescription ?? "");
                    cmd.Parameters.AddWithValue("@NoOfOpenings", model.NoOfOpenings);
                    cmd.Parameters.AddWithValue("@JobLocation", model.JobLocation ?? "");
                    cmd.Parameters.AddWithValue("@Salary", model.Salary ?? 0);
                    cmd.Parameters.AddWithValue("@JobType", model.JobType ?? "");
                    cmd.Parameters.AddWithValue("@ExperienceRequired", model.ExperienceRequired ?? "");
                    cmd.Parameters.AddWithValue("@Qualification", model.Qualification ?? "");
                    cmd.Parameters.AddWithValue("@SkillsRequired", model.SkillsRequired ?? "");
                    cmd.Parameters.AddWithValue("@LastDate", model.LastDate);
                    cmd.Parameters.AddWithValue("@IsActive", model.IsActive);

                    con.Open();
                    result = cmd.ExecuteNonQuery();
                }
            }

            return result;
        }


        public int InsertTest(Placement_Coordinator_Model model)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand(@"
              INSERT INTO TestMaster (TestName, Branch, CreatedBy)
              VALUES (@TestName, @Branch, 1);
              SELECT SCOPE_IDENTITY();
          ", con, tran);

                    cmd.Parameters.AddWithValue("@TestName", model.TestName);
                    cmd.Parameters.AddWithValue("@Branch", model.Branch);

                    int testId = Convert.ToInt32(cmd.ExecuteScalar());

                    foreach (var q in model.QuestionList)
                    {
                        SqlCommand qCmd = new SqlCommand(@"
                  INSERT INTO TestQuestions
                  (TestId, Question, OptionA, OptionB, OptionC, OptionD, CorrectOption, QuestionNo)
                  VALUES
                  (@TestId, @Question, @OptionA, @OptionB, @OptionC, @OptionD, @CorrectOption, @QuestionNo)
              ", con, tran);

                        qCmd.Parameters.AddWithValue("@TestId", testId);
                        qCmd.Parameters.AddWithValue("@Question", q.Question);
                        qCmd.Parameters.AddWithValue("@OptionA", q.OptionA);
                        qCmd.Parameters.AddWithValue("@OptionB", q.OptionB);
                        qCmd.Parameters.AddWithValue("@OptionC", q.OptionC);
                        qCmd.Parameters.AddWithValue("@OptionD", q.OptionD);
                        qCmd.Parameters.AddWithValue("@CorrectOption", q.CorrectOption);
                        qCmd.Parameters.AddWithValue("@QuestionNo", q.QuestionNo);

                        qCmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                    return testId;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public DataTable GetUserByEmail(string email)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"
            SELECT *
            FROM StudentDetails
            WHERE Email = @Email
            AND Approval_Status = 'Approved'
        ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }


        public void SaveOTP(string email, string otp, DateTime expiryTime)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"
            DELETE FROM UserOTP 
            WHERE Email = @Email AND IsUsed = 0;

            INSERT INTO UserOTP (Email, OTP, ExpiryTime, IsUsed)
            VALUES (@Email, @OTP, @ExpiryTime, 0);
        ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@OTP", otp);
                    cmd.Parameters.AddWithValue("@ExpiryTime", expiryTime);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        public bool VerifyOTP(string email, string enteredOtp)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"
            SELECT COUNT(*)
            FROM UserOTP
            WHERE Email = @Email
            AND OTP = @OTP
            AND IsUsed = 0
            AND ExpiryTime >= GETDATE()
        ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@OTP", enteredOtp);

                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    con.Close();

                    return count > 0;
                }
            }
        }

        public bool UpdateStudentPassword(string email, string newPassword)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"
            UPDATE StudentDetails
            SET Password = @Password
            WHERE Email = @Email
        ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", newPassword);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    con.Close();

                    return rowsAffected > 0;
                }
            }
        }


        public List<Test> GetTestList()
        {
            List<Test> list = new List<Test>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = "SELECT TestId, TestName, Branch FROM TestMaster";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Test t = new Test();

                    t.TestId = Convert.ToInt32(dr["TestId"]);
                    t.TestName = dr["TestName"].ToString();
                    t.Branch = dr["Branch"].ToString();

                    list.Add(t);
                }
            }

            return list;
        }


        public List<Test> GetQuestionsByTestId(int testId)
        {
            List<Test> list = new List<Test>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = "SELECT * FROM TestQuestions WHERE TestId=@TestId ORDER BY QuestionNo";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@TestId", testId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Test q = new Test();

                    q.QuestionId = Convert.ToInt32(dr["QuestionId"]);
                    q.TestId = Convert.ToInt32(dr["TestId"]);
                    q.Question = dr["Question"].ToString();

                    q.OptionA = dr["OptionA"].ToString();
                    q.OptionB = dr["OptionB"].ToString();
                    q.OptionC = dr["OptionC"].ToString();
                    q.OptionD = dr["OptionD"].ToString();
                    q.CorrectOption = dr["CorrectOption"].ToString();
                    q.QuestionNo = Convert.ToInt32(dr["QuestionNo"]);

                    list.Add(q);
                }
            }

            return list;
        }



        public DataTable GetInterviewAppliedStudents()
        {
            SqlConnection con = new SqlConnection(_conn);

            string query = @"SELECT 
        i.InterviewId,
        j.JobTitle,
        c.CompanyId,
        c.CompanyName,
        c.CompanyLogo,
        c.CompanyDescription,
        c.HREmailId,
        j.JobDescription,
        sd.FirstName,
        sd.LastName,
        sd.Personal_Photo,
        sd.Branch,
        j.JobLocation,
        j.Salary,
        j.JobType,
        i.InterviewType,
        i.InterviewDate,
        i.InterviewTime,
        i.LastDateToApply,
        i.StudentId,
        j.VacancyId,
        sd.Email,
        i.Status
        FROM InterviewSchedule i
        INNER JOIN JobVacancy j ON j.VacancyId = i.VacancyId
        INNER JOIN CompanyMaster c ON c.CompanyId = j.CompanyId
        INNER JOIN StudentDetails sd ON i.StudentId = sd.StudentId
        WHERE (i.Status = 'Applied' OR i.Status = 'Selected')";

            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }




        public DataTable OfferletterName(string studid)
        {
            SqlConnection con = new SqlConnection(_conn);

            string query = @"SELECT 
        i.InterviewId,
        c.CompanyName,
        c.CompanyLogo,
        sd.FirstName,
        sd.LastName,
        sd.Email
    FROM InterviewSchedule i
    INNER JOIN JobVacancy j ON j.VacancyId = i.VacancyId
    INNER JOIN CompanyMaster c ON c.CompanyId = j.CompanyId
    INNER JOIN StudentDetails sd ON i.StudentId = sd.StudentId
    WHERE i.Status = 'Applied' AND sd.StudentId = @StudentId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@StudentId", studid);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

        public void SelectStudent(int interviewId)
        {
            SqlConnection con = new SqlConnection(_conn);

            string query = "UPDATE InterviewSchedule SET Status='Selected' WHERE InterviewId=@InterviewId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@InterviewId", interviewId);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }


        public void RejectStudent(int interviewId)
        {
            SqlConnection con = new SqlConnection(_conn);

            string query = "DELETE FROM InterviewSchedule WHERE InterviewId=@InterviewId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@InterviewId", interviewId);

            con.Open();
            cmd.ExecuteNonQuery();

            con.Close();
        }
        public void DeleteVacancy(int Id)
        {
            SqlConnection con = new SqlConnection(_conn);

            string query = "DELETE FROM JobVacancy WHERE VacancyId=@VacancyId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@VacancyId", Id);

            con.Open();
            cmd.ExecuteNonQuery();

            con.Close();
        }



        public void InsertOfferLetter(OfferLetterModel model)
        {
            SqlConnection con = new SqlConnection(_conn);

            string query = @"INSERT INTO OfferLetter
                        (StudentId,CompanyId,VacancyId,InterviewId,OfferLetterFile)
                        VALUES
                        (@StudentId,@CompanyId,@VacancyId,@InterviewId,@OfferLetterFile)";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@StudentId", model.StudentId);
            cmd.Parameters.AddWithValue("@CompanyId", model.CompanyId);
            cmd.Parameters.AddWithValue("@VacancyId", model.VacancyId);
            cmd.Parameters.AddWithValue("@InterviewId", model.InterviewId);
            cmd.Parameters.AddWithValue("@OfferLetterFile", model.OfferLetterFile);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public string GetOfferLetterPath(int interviewId)
        {
            string path = "";
            string query = @"SELECT OfferLetterFile 
                     FROM OfferLetter 
                     WHERE InterviewId = @InterviewId";

            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@InterviewId", interviewId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        path = result.ToString();
                    }
                }
            }
            return path;
        }

        public string ViewOfferLetter(int interviewId)
        {
            string offerPath = null;

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"SELECT TOP 1 OfferLetterFile 
                         FROM OfferLetter 
                         WHERE InterviewId = @InterviewId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@InterviewId", interviewId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        offerPath = result.ToString();
                }
            }

            return offerPath;
        }

        public int RejectStudentStudlist(int studentId)
        {
            int rowsAffected = 0;

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"DELETE FROM StudentEducation WHERE StudentId=@StudentId;
                         DELETE FROM StudentDetails WHERE StudentId=@StudentId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);

                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected;
        }


        public DataTable AssignComapnyStud(string studentId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {

                string query = @"

						SELECT 
    i.InterviewId,
    j.JobTitle,
	c.CompanyId,
    c.CompanyName,
    c.CompanyLogo,
    c.CompanyDescription,
c.HREmailId,
	j.JobDescription,
    j.JobLocation,
    j.Salary,
    j.JobType,
    i.InterviewType,
    i.InterviewDate,
    i.InterviewTime,
    i.LastDateToApply,
	 i.StudentId,
	 j.VacancyId ,
    i.Status

FROM InterviewSchedule i

INNER JOIN JobVacancy j 
    ON j.VacancyId = i.VacancyId

INNER JOIN CompanyMaster c 
    ON c.CompanyId = j.CompanyId

WHERE i.StudentId = @StudentId and i.Status = 'Assign'";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@StudentId", studentId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dt);
            }

            return dt;
        }


        public DataTable AppliedJobStud(string studentId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {

                string query = @"

						SELECT 
                            i.InterviewId,
                            j.JobTitle,
	                        c.CompanyId,
                            c.CompanyName,
                            c.CompanyLogo,
                            c.CompanyDescription,
                        c.HREmailId,
	                        j.JobDescription,
                            j.JobLocation,
                            j.Salary,
                            j.JobType,
                            i.InterviewType,
                            i.InterviewDate,
                            i.InterviewTime,
                            i.LastDateToApply,
	                         i.StudentId,
	                         j.VacancyId ,
                            i.Status

                        FROM InterviewSchedule i

                        INNER JOIN JobVacancy j 
                            ON j.VacancyId = i.VacancyId

                        INNER JOIN CompanyMaster c 
                            ON c.CompanyId = j.CompanyId

                        WHERE i.StudentId = @StudentId and i.Status = 'Applied'";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@StudentId", studentId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dt);
            }

            return dt;
        }



        public List<AppliedInterview> AppliedJobStudDB(string studentId)
        {
            var list = new List<AppliedInterview>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"
            SELECT TOP 2
                i.InterviewId,
                j.JobTitle,
                c.CompanyName,
                j.JobLocation,
                j.Salary,
                i.InterviewDate,
                i.InterviewTime
            FROM InterviewSchedule i
            INNER JOIN JobVacancy j ON j.VacancyId = i.VacancyId
            INNER JOIN CompanyMaster c ON c.CompanyId = j.CompanyId
            WHERE i.StudentId = @StudentId AND i.Status = 'Applied'
            ORDER BY i.InterviewDate ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new AppliedInterview
                            {
                                InterviewId = Convert.ToInt32(reader["InterviewId"]),
                                JobTitle = reader["JobTitle"].ToString(),
                                CompanyName = reader["CompanyName"].ToString(),
                                JobLocation = reader["JobLocation"].ToString(),
                                Salary = reader["Salary"].ToString(),
                                InterviewDate = Convert.ToDateTime(reader["InterviewDate"]),
                                InterviewTime = reader["InterviewTime"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }



        public List<AppliedInterview> AssignJobStudDB(string studentId)
        {
            var list = new List<AppliedInterview>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"
            SELECT TOP 2
                i.InterviewId,
                j.JobTitle,
                c.CompanyName,
                j.JobLocation,
                j.Salary,
                i.InterviewDate,
                i.InterviewTime
            FROM InterviewSchedule i
            INNER JOIN JobVacancy j ON j.VacancyId = i.VacancyId
            INNER JOIN CompanyMaster c ON c.CompanyId = j.CompanyId
            WHERE i.StudentId = @StudentId AND i.Status = 'Assign'
            ORDER BY i.InterviewDate ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new AppliedInterview
                            {
                                InterviewId = Convert.ToInt32(reader["InterviewId"]),
                                JobTitle = reader["JobTitle"].ToString(),
                                CompanyName = reader["CompanyName"].ToString(),
                                JobLocation = reader["JobLocation"].ToString(),
                                Salary = reader["Salary"].ToString(),
                                InterviewDate = Convert.ToDateTime(reader["InterviewDate"]),
                                InterviewTime = reader["InterviewTime"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }



        public List<AppliedInterview> OfferLetterDBStud(string studentId)
        {
            var list = new List<AppliedInterview>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"
            SELECT TOP 1
                i.InterviewId,
                j.JobTitle,
                c.CompanyName,
                j.JobLocation,
                j.Salary,
                i.InterviewDate,
                i.InterviewTime
            FROM InterviewSchedule i
            INNER JOIN JobVacancy j ON j.VacancyId = i.VacancyId
            INNER JOIN CompanyMaster c ON c.CompanyId = j.CompanyId
            WHERE i.StudentId = @StudentId AND i.Status = 'Selected'
            ORDER BY i.InterviewDate ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new AppliedInterview
                            {
                                InterviewId = Convert.ToInt32(reader["InterviewId"]),
                                JobTitle = reader["JobTitle"].ToString(),
                                CompanyName = reader["CompanyName"].ToString(),
                                JobLocation = reader["JobLocation"].ToString(),
                                Salary = reader["Salary"].ToString(),
                                InterviewDate = Convert.ToDateTime(reader["InterviewDate"]),
                                InterviewTime = reader["InterviewTime"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }




        public List<AppliedInterview> CountStudDB(string studentId)
        {
            var list = new List<AppliedInterview>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"
            SELECT 
                i.InterviewId,
                j.JobTitle,
                c.CompanyName,
                j.JobLocation,
                j.Salary,
                i.InterviewDate,
                i.InterviewTime,
i.Status

            FROM InterviewSchedule i
            INNER JOIN JobVacancy j ON j.VacancyId = i.VacancyId
            INNER JOIN CompanyMaster c ON c.CompanyId = j.CompanyId
            WHERE i.StudentId = @StudentId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new AppliedInterview
                            {
                                InterviewId = Convert.ToInt32(reader["InterviewId"]),
                                JobTitle = reader["JobTitle"].ToString(),
                                CompanyName = reader["CompanyName"].ToString(),
                                JobLocation = reader["JobLocation"].ToString(),
                                Salary = reader["Salary"].ToString(),
                                InterviewDate = Convert.ToDateTime(reader["InterviewDate"]),
                                InterviewTime = reader["InterviewTime"].ToString(),
                                Status = reader["Status"].ToString(),

                            });
                        }
                    }
                }
            }

            return list;
        }


        public List<RecentStudent> GetRecentStudents()
        {
            List<RecentStudent> list = new List<RecentStudent>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"
SELECT TOP 4 *
FROM StudentDetails
ORDER BY RegistrationDate DESC;";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new RecentStudent
                    {
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        Branch = dr["Branch"].ToString(),
                        Status = dr["Approval_Status"].ToString()
                    });
                }
            }

            return list;
        }



        public List<UpcomingInterview> GetUpcomingInterviews()
        {
            List<UpcomingInterview> list = new List<UpcomingInterview>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @" SELECT TOP 2
     i.InterviewId,
     j.JobTitle,
     c.CompanyName,
    
     i.InterviewDate,
     i.InterviewTime
 FROM InterviewSchedule i
 INNER JOIN JobVacancy j ON j.VacancyId = i.VacancyId
 INNER JOIN CompanyMaster c ON c.CompanyId = j.CompanyId
 WHERE  i.Status = 'Applied'
 ORDER BY i.InterviewDate ASC";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new UpcomingInterview
                    {
                        InterviewId = Convert.ToInt32(dr["InterviewId"]),
                        CompanyName = dr["CompanyName"].ToString(),
                        InterviewDate = Convert.ToDateTime(dr["InterviewDate"]),
                        InterviewTime = dr["InterviewTime"].ToString(),
                        Postion = dr["JobTitle"].ToString()
                    });
                }
            }

            return list;
        }



        public List<LatestJob> GetLatestJobs()
        {
            List<LatestJob> list = new List<LatestJob>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"
 SELECT TOP 3
                         c.CompanyName,
                         j.JobTitle AS Position,
                         j.Salary
                         FROM JobVacancy j
                         INNER JOIN CompanyMaster c 
                         ON c.CompanyId = j.CompanyId 
                         ORDER BY j.VacancyId DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new LatestJob
                    {
                        CompanyName = dr["CompanyName"].ToString(),
                        Position = dr["Position"].ToString(),
                        Salary = dr["Salary"].ToString()
                    });
                }
            }

            return list;
        }

        public PlacementActivity GetPlacementActivity()
        {
            PlacementActivity data = new PlacementActivity();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"
       SELECT 
        SUM(CASE WHEN Status IN ('Assign','Applied','Selected') THEN 1 ELSE 0 END) AS AssignedCount,
        SUM(CASE WHEN Status IN ('Applied','Selected') THEN 1 ELSE 0 END) AS AppliedCount,
        SUM(CASE WHEN Status = 'Selected' THEN 1 ELSE 0 END) AS SelectedCount
        FROM InterviewSchedule";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    data.AssignedCount = Convert.ToInt32(dr["AssignedCount"]);
                    data.AppliedCount = Convert.ToInt32(dr["AppliedCount"]);
                    data.SelectedCount = Convert.ToInt32(dr["SelectedCount"]);
                }
            }

            return data;
        }


        public List<RecentTest> GetRecentTests()
        {
            List<RecentTest> list = new List<RecentTest>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"SELECT TOP 3
                         TestId,
                         TestName,
                         Branch
                         FROM TestMaster
                         ORDER BY CreatedDate DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new RecentTest
                    {
                        TestId = Convert.ToInt32(dr["TestId"]),
                        TestName = dr["TestName"].ToString(),
                        Branch = dr["Branch"].ToString()
                    });
                }
            }

            return list;
        }


        public (int students, int companies, int interviews, int offers, int premium) GetDashboardCounts()
        {
            int students = 0;
            int companies = 0;
            int interviews = 0;
            int offers = 0;
            int premium = 0;

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"
            SELECT (SELECT COUNT(*) FROM StudentDetails) AS Students,
       (SELECT COUNT(*) FROM CompanyMaster) AS Companies,
       (SELECT COUNT(*) FROM InterviewSchedule WHERE  Status = 'Applied' or  Status = 'Assign'  ) AS Interviews,
       (SELECT COUNT(*) FROM InterviewSchedule WHERE  Status = 'Selected') AS Offers,
(SELECT COUNT(*) FROM Payments ) AS premium";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    students = Convert.ToInt32(dr["Students"]);
                    companies = Convert.ToInt32(dr["Companies"]);
                    interviews = Convert.ToInt32(dr["Interviews"]);
                    offers = Convert.ToInt32(dr["Offers"]);
                    premium = Convert.ToInt32(dr["premium"]);
                }
            }

            return (students, companies, interviews, offers, premium);
        }


        // GET SKILLS
        public List<SkillModel> GetSkills()
        {
            List<SkillModel> list = new List<SkillModel>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                SqlCommand cmd = new SqlCommand("SELECT SkillId,SkillName FROM Skills WHERE IsActive=1", con);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new SkillModel
                    {
                        SkillId = Convert.ToInt32(dr["SkillId"]),
                        SkillName = dr["SkillName"].ToString()
                    });
                }
            }

            return list;
        }

        // INSERT NEW SKILL
        public void AddSkill(string skill)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO Skills(SkillName) VALUES(@Skill)", con);

                cmd.Parameters.AddWithValue("@Skill", skill);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public DataTable GetTestQuestionsByTestId(int testId)
        {
            SqlConnection con = new SqlConnection(_conn);
            SqlCommand cmd = new SqlCommand("SELECT * FROM TestQuestions WHERE TestId=@TestId ORDER BY QuestionNo", con);

            cmd.Parameters.AddWithValue("@TestId", testId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            da.Fill(dt);

            return dt;
        }

        public void DeleteTest(int testId)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                con.Open();

                SqlTransaction tran = con.BeginTransaction();

                //try
                //{
                SqlCommand cmd1 = new SqlCommand(
                    "DELETE FROM TestQuestions WHERE TestId=@TestId",
                    con, tran);

                cmd1.Parameters.AddWithValue("@TestId", testId);
                cmd1.ExecuteNonQuery();


                SqlCommand cmd2 = new SqlCommand(
                    "DELETE FROM TestMaster WHERE TestId=@TestId",
                    con, tran);

                cmd2.Parameters.AddWithValue("@TestId", testId);
                cmd2.ExecuteNonQuery();

                tran.Commit();
                // }
                //catch
                //{
                //    tran.Rollback();
                //}
            }
        }

        public DataTable GetVacancyByCompany(int companyId)
        {
            SqlConnection con = new SqlConnection(_conn);
            SqlCommand cmd = new SqlCommand("select VacancyId,JobTitle from JobVacancy where CompanyId=@CompanyId", con);
            cmd.Parameters.AddWithValue("@CompanyId", companyId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }


        public StudentRegistration GetStudent(string studentId)
        {
            StudentRegistration model = new StudentRegistration();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                con.Open();

                // 🔹 BUSINESS LOGIC: Fetch both tables
                string query = @"
            SELECT S.*, E.*
            FROM StudentDetails S
            LEFT JOIN StudentEducation E ON S.StudentId = E.StudentId
            WHERE S.StudentId = @StudentId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StudentId", studentId);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    // ===== StudentDetails =====
                    model.StudentId = dr["StudentId"].ToString();
                    model.Name_Title = dr["Name_Title"].ToString();
                    model.FirstName = dr["FirstName"].ToString();
                    model.MiddleName = dr["MiddleName"].ToString();
                    model.LastName = dr["LastName"].ToString();
                    model.Gender = dr["Gender"].ToString();
                    model.DOB = Convert.ToDateTime(dr["DOB"]);
                    model.Email = dr["Email"].ToString();
                    model.MobileNo = dr["MobileNo"].ToString();
                    model.AlternateMobile = dr["AlternateMobile"].ToString();
                    model.Acadamic_Year = dr["Acadamic_Year"].ToString();
                    model.Roll_No = dr["Roll_No"].ToString();
                    model.Branch = dr["Branch"].ToString();
                    model.Divison = dr["Divison"].ToString();
                    model.Personal_Photo = dr["Personal_Photo"].ToString();

                    // ===== StudentEducation =====
                    model.SSC = dr["SSC"].ToString();
                    model.SSC_Passing_Year = dr["SSC_Passing_Year"].ToString();
                    model.SSC_Document_Path = dr["SSC_Document_Path"].ToString();


                    model.HSC = dr["HSC"].ToString();
                    model.HSC_Passing_Year = dr["HSC_Passing_Year"].ToString();
                    model.HSC_Document_Path = dr["HSC_Document_Path"].ToString();

                    model.Diploma_Percentages = dr["Diploma_Percentages"].ToString();
                    model.Diploma_Branch = dr["Diploma_Branch"].ToString();
                    model.Diploma_Passing_Year = dr["Diploma_Passing_Year"].ToString();
                    model.Diploma_Document_Path = dr["Diploma_Document_Path"].ToString();


                    model.BTech_Percentages = dr["BTech_Percentages"].ToString();
                    model.BTech_Branch = dr["BTech_Branch"].ToString();
                    model.BTech_Passing_Year = dr["BTech_Passing_Year"].ToString();
                    model.BTech_Document_Path = dr["BTech_Document_Path"].ToString();
                    model.Skills = dr["Skills"].ToString();
                    model.ResumePath = dr["Resume"].ToString();
                }

                dr.Close();
            }

            return model;
        }


        // =========================
        // UPDATE STUDENT (BUSINESS LOGIC + TRANSACTION)
        // =========================
        public bool UpdateStudent(StudentRegistration model)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                con.Open();

                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // ============================
                    // 🔹 STUDENT DETAILS
                    // ============================
                    string query1 = @"
            UPDATE StudentDetails SET
                Email = @Email,
                MobileNo = @MobileNo,
                AlternateMobile = @AlternateMobile,
                Acadamic_Year = @Acadamic_Year,
Personal_Photo = @Personal_Photo
            WHERE StudentId = @StudentId";

                    SqlCommand cmd1 = new SqlCommand(query1, con, tran);

                    cmd1.Parameters.AddWithValue("@Email", model.Email);
                    cmd1.Parameters.AddWithValue("@MobileNo", model.MobileNo);
                    cmd1.Parameters.AddWithValue("@AlternateMobile", model.AlternateMobile);
                    cmd1.Parameters.AddWithValue("@Acadamic_Year", model.Acadamic_Year);
                    cmd1.Parameters.AddWithValue("@StudentId", model.StudentId);
                    cmd1.Parameters.AddWithValue("@Personal_Photo", model.Personal_Photo);

                    cmd1.ExecuteNonQuery();

                    // ============================
                    // 🔹 EDUCATION DETAILS
                    // ============================
                    string query2 = @"
            UPDATE StudentEducation SET
                BTech_Percentages = @BTech_Percentages,
               
                BTech_Passing_Year = @BTech_Passing_Year,
                BTech_Document_Path = @BTech_Document_Path,
                Resume = @ResumePath,
                
                Skills = @Skills
            WHERE StudentId = @StudentId";

                    SqlCommand cmd2 = new SqlCommand(query2, con, tran);

                    cmd2.Parameters.AddWithValue("@StudentId", model.StudentId);
                    cmd2.Parameters.AddWithValue("@BTech_Percentages", model.BTech_Percentages);
                
                    cmd2.Parameters.AddWithValue("@BTech_Passing_Year", model.BTech_Passing_Year);
                    cmd2.Parameters.AddWithValue("@BTech_Document_Path", model.BTech_Document_Path);
                    cmd2.Parameters.AddWithValue("@ResumePath", model.ResumePath);
                    cmd2.Parameters.AddWithValue("@Skills", model.Skills);

                    cmd2.ExecuteNonQuery();

                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw; // better for debugging
                }
            }
        }



        // GET ALL EVENTS
        public List<PlacementEvents> GetAllEvents()
        {
            List<PlacementEvents> list = new List<PlacementEvents>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM PlacementEvents ORDER BY EventDate ASC", con);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new PlacementEvents
                    {
                        EventId = Convert.ToInt32(dr["EventId"]),
                        EventName = dr["EventName"].ToString(),
                        Branch = dr["Branch"].ToString(),
                        SpeakerName = dr["SpeakerName"].ToString(),
                        EventMode = dr["EventMode"].ToString(),
                        MeetingLink = dr["MeetingLink"].ToString(),
                        Venue = dr["Venue"].ToString(),
                        EventDate = Convert.ToDateTime(dr["EventDate"]),
                        EventTime = (TimeSpan)dr["EventTime"],
                        CreatedDate = Convert.ToDateTime(dr["CreatedDate"])
                    });
                }
            }

            return list;
        }

        // SAVE EVENT
        public void SaveEvent(PlacementEvents model)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"INSERT INTO PlacementEvents
                        (EventName, Branch, SpeakerName, EventMode, MeetingLink, Venue, EventDate, EventTime)
                        VALUES
                        (@EventName, @Branch, @SpeakerName, @EventMode, @MeetingLink, @Venue, @EventDate, @EventTime)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@EventName", model.EventName);
                cmd.Parameters.AddWithValue("@Branch", model.Branch);
                cmd.Parameters.AddWithValue("@SpeakerName", model.SpeakerName);
                cmd.Parameters.AddWithValue("@EventMode", model.EventMode);
                cmd.Parameters.AddWithValue("@MeetingLink", (object)model.MeetingLink ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Venue", (object)model.Venue ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EventDate", model.EventDate);
                cmd.Parameters.AddWithValue("@EventTime", model.EventTime);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // GET EVENT BY ID
        public PlacementEvents GetEventById(int id)
        {
            PlacementEvents model = new PlacementEvents();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM PlacementEvents WHERE EventId=@EventId", con);
                cmd.Parameters.AddWithValue("@EventId", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    model.EventId = Convert.ToInt32(dr["EventId"]);
                    model.EventName = dr["EventName"].ToString();
                    model.Branch = dr["Branch"].ToString();
                    model.SpeakerName = dr["SpeakerName"].ToString();
                    model.EventMode = dr["EventMode"].ToString();
                    model.MeetingLink = dr["MeetingLink"].ToString();
                    model.Venue = dr["Venue"].ToString();
                    model.EventDate = Convert.ToDateTime(dr["EventDate"]);
                    model.EventTime = (TimeSpan)dr["EventTime"];
                    model.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
                }
            }

            return model;
        }



        public void SavePayment(int studentId, string paymentId, decimal price)
        {
           
            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = "INSERT INTO Payments (StudentId, Amount, PaymentId, Status, PaymentDate) VALUES (@StudentId, @Amount, @PaymentId, @Status, GETDATE())";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                cmd.Parameters.AddWithValue("@Amount", price);
                cmd.Parameters.AddWithValue("@PaymentId", paymentId);
               // cmd.Parameters.AddWithValue("@Receipt", folderPath);
                cmd.Parameters.AddWithValue("@Status", "Success");

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        //public void SavePayment(string studentId, string paymentId, string folderPath)
        //{
           
        //    using (SqlConnection con = new SqlConnection(_conn))
        //    {
        //        string query = "INSERT INTO Payments (StudentId, Amount, PaymentId, Receipt,Status, PaymentDate) VALUES (@StudentId, @Amount, @PaymentId,Receipt, @Status, GETDATE())";

        //        SqlCommand cmd = new SqlCommand(query, con);
        //        cmd.Parameters.AddWithValue("@StudentId", studentId);
        //        cmd.Parameters.AddWithValue("@Amount", 299);
        //        cmd.Parameters.AddWithValue("@PaymentId", paymentId);
        //        cmd.Parameters.AddWithValue("@Receipt", folderPath);
        //        cmd.Parameters.AddWithValue("@Status", "Success");

        //        con.Open();
        //        cmd.ExecuteNonQuery();
        //    }
        //}

        // ✅ Make User Premium
        public void MakeUserPremium(int studentId)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = "UPDATE StudentDetails SET IsPremium = 1 WHERE StudentId = @StudentId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StudentId", studentId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ✅ Check Premium
        //public bool IsUserPremium(string studentId)
        //{
        //    using (SqlConnection con = new SqlConnection(_conn))
        //    {
        //        string query = "SELECT IsPremium FROM StudentDetails WHERE StudentId = @StudentId";

        //        SqlCommand cmd = new SqlCommand(query, con);
        //        cmd.Parameters.AddWithValue("@StudentId", studentId);

        //        con.Open();

        //        var result = cmd.ExecuteScalar();

        //        if (result != null && result != DBNull.Value)
        //        {
        //            return result.ToString() == "1";
        //        }

        //        return false;
        //    }
        //}
        public bool IsUserPremium(string studentId)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = "SELECT IsPremium FROM StudentDetails WHERE StudentId = @StudentId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StudentId", studentId);

                con.Open();

                var result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return result.ToString() == "1";
                }

                return false;
            }
        }


        public DataTable GetPaymentList()
        {
            SqlConnection con = new SqlConnection(_conn);

            SqlCommand cmd = new SqlCommand(@"
        SELECT 
            p.Id,
            p.StudentId,
            p.Amount,
            p.PaymentId,
            p.Status,
            p.PaymentDate,
            p.Receipt,

            s.FirstName + ' ' + s.MiddleName + ' ' + s.LastName AS StudentName,
            s.Personal_Photo

        FROM Payments p
        INNER JOIN StudentDetails s
            ON p.StudentId = s.StudentId


        ORDER BY p.Id DESC
    ", con);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            da.Fill(dt);

            return dt;
        }




        public DataTable GetLatestPaymentStudentDetails(string studentId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"
        SELECT TOP 1
            p.Id,
            p.StudentId,
            p.Amount,
            p.PaymentId,
            p.Status,
            p.PaymentDate,

            s.FirstName + ' ' + 
            ISNULL(s.MiddleName,'') + ' ' + 
            s.LastName AS StudentName,

            s.Email

        FROM Payments p

        INNER JOIN StudentDetails s
            ON p.StudentId = s.StudentId

        WHERE p.StudentId = @StudentId

        ORDER BY p.Id DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@StudentId", studentId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dt);
            }

            return dt;
        }


        public DataTable GetReceiptById(int Id)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"
     SELECT TOP 1
    Id,
    Receipt
FROM Payments
ORDER BY Id DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", Id);

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dt);
            }

            return dt;
        }
        public void UpdateReceiptPath(int studentId,string paymentId,string receiptPath)
        {
            using (SqlConnection con = new SqlConnection(_conn))
            {
                string query = @"

        UPDATE Payments

        SET Receipt = @Receipt

        WHERE StudentId = @StudentId
        AND PaymentId = @PaymentId

        ";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@StudentId", studentId);

                cmd.Parameters.AddWithValue("@PaymentId", paymentId);

                cmd.Parameters.AddWithValue("@Receipt", receiptPath);

                con.Open();

                cmd.ExecuteNonQuery();
            }
        }


        public DataTable GetStudentDetailsById(string studentId)
        {
            SqlConnection con = new SqlConnection(_conn);

            string query = @"SELECT 
                        FirstName + ' ' + LastName AS FullName,
                       Email
                     FROM StudentDetails
                     WHERE StudentId = @StudentId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@StudentId", studentId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

        public DataTable GetInterviewDetails(string studentId)
        {
            SqlConnection con = new SqlConnection(_conn);

            string query = @"SELECT TOP 1
                        Branch,
                        InterviewType,
                        InterviewDate,
                        InterviewTime,
                        Status
                     FROM InterviewSchedule
                     WHERE StudentId = @StudentId
                     ORDER BY InterviewId DESC";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@StudentId", studentId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }
    }
}




