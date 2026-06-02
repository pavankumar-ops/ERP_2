namespace ERP_Placement.Models
{
    public class StudentRegistration
    {
        // -------------------- StudentDetails Table --------------------

        public string StudentId { get; set; }
        public string Name_Title { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string Gender { get; set; }
        public DateTime DOB { get; set; }

        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string AlternateMobile { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string Personal_Photo { get; set; }




        public string Acadamic_Year { get; set; }
        public string Roll_No { get; set; }
        public string Branch { get; set; }
        public string Divison { get; set; }
        public string Password { get; set; }






        // -------------------- StudentEducation Table --------------------

        // SSC
        public string SSC { get; set; }
        public string SSC_Passing_Year { get; set; }
        public string SSC_Document_Path { get; set; }

        // HSC
        public string HSC { get; set; }
        public string HSC_Passing_Year { get; set; }
        public string HSC_Document_Path { get; set; }

        // Diploma
        public string Diploma_Branch { get; set; }
        public string Diploma_Percentages { get; set; }
        public string Diploma_Passing_Year { get; set; }
        public string Diploma_Document_Path { get; set; }

        // BTech
        public string BTech_Branch { get; set; }
        public string BTech_Passing_Year { get; set; }
        public string BTech_Percentages { get; set; }
        public string BTech_Document_Path { get; set; }

        public string Skills { get; set; }
        public string ResumePath { get; set; }

        // Upload properties (not DB)
        public IFormFile SSC_File { get; set; }
        public IFormFile HSC_File { get; set; }
        public IFormFile Diploma_File { get; set; }
        public IFormFile BTech_File { get; set; }
        public IFormFile Resume { get; set; }
        public IFormFile Profile_photo { get; set; }


        // APPLIED JOB 

        public int InterviewId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string JobLocation { get; set; }
        public string Salary { get; set; }
        public DateTime InterviewDate { get; set; }
        public string InterviewTime { get; set; }




    }


    public class Test
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string Branch { get; set; }


        public int QuestionId { get; set; }

        //public int TestId { get; set; }

        public string Question { get; set; }

        public string OptionA { get; set; }

        public string OptionB { get; set; }

        public string OptionC { get; set; }

        public string OptionD { get; set; }

        public string CorrectOption { get; set; }

        public int QuestionNo { get; set; }

        public DateTime CreatedDate { get; set; }
        public string SelectedOption { get; set; }

    }


    public class AppliedInterview
    {
        public int InterviewId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string JobLocation { get; set; }
        public string Salary { get; set; }
        public DateTime InterviewDate { get; set; }
        public string InterviewTime { get; set; }
        public string Status { get; set; }
    }

    public class SkillModel
    {
        public int SkillId { get; set; }

        public string SkillName { get; set; }
    }


    public class PlacementEvents
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public string Branch { get; set; }

        public string SpeakerName { get; set; }

        public string EventMode { get; set; }     // Online / Offline

        public string MeetingLink { get; set; }

        public string Venue { get; set; }

        public DateTime EventDate { get; set; }

        public TimeSpan EventTime { get; set; }

        public DateTime CreatedDate { get; set; }
    }

}
